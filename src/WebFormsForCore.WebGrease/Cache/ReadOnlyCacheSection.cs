// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyCacheSection.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using WebGrease.Css.Extensions;
    using WebGrease.Extensions;

    public class ReadOnlyCacheSection
    {
        /// <summary>The load lock.</summary>
        private static readonly object LoadLock = new object();

        private readonly string absolutePath;

        private IWebGreaseContext context;

        private IEnumerable<CacheSourceDependency> sourceDependencies;

        private IEnumerable<CacheResult> cacheResults;

        private IEnumerable<string> childCacheSectionFiles;

        private IEnumerable<ReadOnlyCacheSection> childCacheSections;

        private bool disposed;

        private int referenceCount;

        private ReadOnlyCacheSection(string jsonString, IWebGreaseContext context)
        {
            this.context = context;

            var json = JObject.Parse(jsonString);

            this.sourceDependencies = json["sourceDependencies"].ToString().FromJson<IEnumerable<CacheSourceDependency>>(true);
            this.cacheResults = json["cacheResults"].ToString().FromJson<IEnumerable<CacheResult>>(true);

            this.childCacheSectionFiles = json["children"].AsEnumerable().Select(f => (string)f);

            this.absolutePath = (string)json["absolutePath"];
        }

        /// <summary>The child cache sections.</summary>
        private IEnumerable<ReadOnlyCacheSection> ChildCacheSections
        {
            get
            {
                return this.childCacheSections ?? (this.childCacheSections = this.childCacheSectionFiles.Select(childCacheSectionFile => Load(childCacheSectionFile, this.context)).ToArray());
            }
        }

        /// <summary>The load.</summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="CacheSection"/>.</returns>
        internal static ReadOnlyCacheSection Load(string fullPath, IWebGreaseContext context)
        {
            if (!File.Exists(fullPath))
            {
                return null;
            }

            return Safe.Lock(LoadLock, () =>
            {
                ReadOnlyCacheSection cacheSection;
                if (!context.Cache.LoadedCacheSections.TryGetValue(fullPath, out cacheSection))
                {
                    cacheSection = new ReadOnlyCacheSection(File.ReadAllText(fullPath), context);
                    context.Cache.LoadedCacheSections.Add(fullPath, cacheSection);
                }

                cacheSection.referenceCount++;
                return cacheSection;
            });
        }

        internal IEnumerable<CacheResult> GetCacheResults(string fileCategory = null, bool endResultOnly = false)
        {
            return this.cacheResults
                       .Where(cr => (!endResultOnly || cr.EndResult) && (fileCategory == null || cr.FileCategory == fileCategory))
                       .Concat(this.ChildCacheSections.SelectMany(css => css.GetCacheResults(fileCategory, endResultOnly)));
        }

        internal void Dispose()
        {
            if (this.disposed)
            {
                throw new BuildWorkflowException("Cannot dispose an object twice.");
            }

            if (Unload(this.context, this.absolutePath))
            {
                this.disposed = true;
                if (this.childCacheSections != null)
                {
                    this.childCacheSections.Where(ccs => ccs != null).ForEach(ccs => ccs.Dispose());
                }

                this.context = null;
                this.cacheResults = null;
                this.sourceDependencies = null;
                this.childCacheSections = null;
                this.childCacheSectionFiles = null;
            }
        }

        internal bool CanBeRestoredFromCache()
        {
            var allChildCacheSections = new[] { this }.Concat(this.SafeAllRecursiveChildSections());
            foreach (var childCacheSection in allChildCacheSections)
            {
                if (childCacheSection == null)
                {
                    return false;
                }

                if (childCacheSection.cacheResults.Any(cr => cr == null || !File.Exists(cr.CachedFilePath)))
                {
                    return false;
                }

                if (childCacheSection.HasChangedSourceDependencies())
                {
                    return false;
                }
            }

            return true;
        }

        internal bool CanBeSkipped()
        {
            var allChildCacheSections = new[] { this }.Concat(this.SafeAllRecursiveChildSections());
            var skippedChildCacheSections = new List<ReadOnlyCacheSection>();
            bool hasEndResults = false;
            foreach (var childCacheSection in allChildCacheSections)
            {
                if (childCacheSection == null)
                {
                    return false;
                }

                var endCacheResults = childCacheSection.cacheResults.Where(cr => cr.EndResult).ToArray();
                if (endCacheResults.Any())
                {
                    hasEndResults = true;
                    if (endCacheResults.Any(cr => HasCachedEndResultThatChanged(this.context, cr)))
                    {
                        return false;
                    }
                }

                if (childCacheSection.sourceDependencies.Any(sd => sd == null || sd.HasChanged(this.context)))
                {
                    return false;
                }

                skippedChildCacheSections.Add(childCacheSection);
            }

            if (hasEndResults)
            {
                skippedChildCacheSections.ForEach(scc => scc.Touch());
                this.Touch();
            }

            return hasEndResults;
        }

        /// <summary>The unload.</summary>
        /// <param name="context">the context</param>
        /// <param name="fullPath">The full path.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private static bool Unload(IWebGreaseContext context, string fullPath)
        {
            return Safe.Lock(LoadLock, () =>
            {
                ReadOnlyCacheSection cacheSection;
                if (context.Cache.LoadedCacheSections.TryGetValue(fullPath, out cacheSection))
                {
                    cacheSection.referenceCount--;
                    if (cacheSection.referenceCount == 0)
                    {
                        context.Cache.LoadedCacheSections.Remove(fullPath);
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        private static bool HasCachedEndResultThatChanged(IWebGreaseContext context, CacheResult r)
        {
            if (r == null)
            {
                return true;
            }

            var absoluteEndResultPath = Path.Combine(context.Configuration.DestinationDirectory, r.RelativeHashedContentPath ?? r.RelativeContentPath);
            return !File.Exists(absoluteEndResultPath) || !r.ContentHash.Equals(context.GetFileHash(absoluteEndResultPath));
        }

        /// <summary>The touch all files.</summary>
        private void Touch()
        {
            this.context.Touch(this.absolutePath);
            this.cacheResults.ForEach(cr => this.context.Touch(cr.CachedFilePath));
        }

        private bool HasChangedSourceDependencies()
        {
            return this.sourceDependencies.Any(sd => sd == null || sd.HasChanged(this.context));
        }

        private IEnumerable<ReadOnlyCacheSection> SafeAllRecursiveChildSections()
        {
            return this.ChildCacheSections.Concat(
                this.ChildCacheSections.SelectMany(css =>
                                                   css != null
                                                       ? css.SafeAllRecursiveChildSections()
                                                       : null));
        }
    }
}