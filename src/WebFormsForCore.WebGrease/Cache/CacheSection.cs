// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheSection.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using WebGrease.Configuration;
    using WebGrease.Extensions;

    /// <summary>The cache section.</summary>
    public class CacheSection : ICacheSection
    {
        #region Fields

        /// <summary>The cache results.</summary>
        private readonly List<CacheResult> cacheResults = new List<CacheResult>();

        /// <summary>The source dependencies.</summary>
        private readonly IDictionary<string, CacheSourceDependency> sourceDependencies = new Dictionary<string, CacheSourceDependency>(StringComparer.OrdinalIgnoreCase);

        /// <summary>The child cache sections.</summary>
        private Lazy<List<CacheSection>> childCacheSections = new Lazy<List<CacheSection>>(() => new List<CacheSection>(), true);

        /// <summary>The cache category.</summary>
        private string cacheCategory;

        /// <summary>The cached section.</summary>
        private ReadOnlyCacheSection cachedSection;

        /// <summary>The context.</summary>
        private IWebGreaseContext context;

        /// <summary>If it is dirty (has changes not saved).</summary>
        private bool isUnsaved = true;

        /// <summary>The parent.</summary>
        private CacheSection parent;

        /// <summary>The absolute path.</summary>
        private string absolutePath;

        #endregion

        #region Constructors and Destructors

        /// <summary>Prevents a default instance of the <see cref="CacheSection"/> class from being created.</summary>
        private CacheSection()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the parent.</summary>
        public ICacheSection Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>Gets the unique key.</summary>
        public string UniqueKey { get; private set; }

        #endregion

        #region Properties

        /// <summary>The child cache sections.</summary>
        private List<CacheSection> ChildCacheSections
        {
            get
            {
                return this.childCacheSections.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Begins the section, create a new section and returns it.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cacheCategory">The cache category.</param>
        /// <param name="uniqueKey">The unique Key.</param>
        /// <param name="parentCacheSection">The parent cache section.</param>
        /// <param name="autoLoad">The auto Load.</param>
        /// <returns>The <see cref="CacheSection"/>.</returns>
        public static CacheSection Begin(IWebGreaseContext context, string cacheCategory, string uniqueKey, ICacheSection parentCacheSection = null, bool autoLoad = true)
        {
            var cacheSection = new CacheSection
            {
                parent = parentCacheSection as CacheSection,
                cacheCategory = cacheCategory,
                context = context,
                UniqueKey = uniqueKey
            };

            cacheSection.absolutePath = context.Cache.GetAbsoluteCacheFilePath(cacheSection.cacheCategory, context.GetValueHash(cacheSection.UniqueKey) + ".cache.json");

            if (cacheSection.parent != null)
            {
                cacheSection.parent.AddChildCacheSection(cacheSection);
            }

            EnsureCachePath(context, cacheCategory);

            if (autoLoad)
            {
                cacheSection.Load();
            }

            return cacheSection;
        }

        /// <summary>The load.</summary>
        public void Load()
        {
            try
            {
                var fileName = new FileInfo(this.absolutePath);
                if (fileName.Exists)
                {
                    this.cachedSection = ReadOnlyCacheSection.Load(fileName.FullName, this.context);
                    this.isUnsaved = this.cachedSection != null;
                }
            }
            catch (PathTooLongException ex)
            {
                throw new BuildWorkflowException("Path to long: {0}".InvariantFormat(this.absolutePath), ex);
            }
        }

        /// <summary>The get cache data.</summary>
        /// <param name="id">The id.</param>
        /// <typeparam name="T">The typeof object</typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        public T GetCacheData<T>(string id) where T : new()
        {
            if (this.cachedSection != null)
            {
                var cacheDataContentItem = this.GetCachedContentItems(id).FirstOrDefault();
                if (cacheDataContentItem != null && !string.IsNullOrWhiteSpace(cacheDataContentItem.Content))
                {
                    return cacheDataContentItem.Content.FromJson<T>(true);
                }
            }

            return new T();
        }

        /// <summary>The set cache data.</summary>
        /// <param name="id">The id.</param>
        /// <param name="obj">The obj.</param>
        /// <typeparam name="T">The typeof object</typeparam>
        public void SetCacheData<T>(string id, T obj) where T : new()
        {
            var json = obj.ToJson(true);
            this.AddResult(ContentItem.FromContent(json), id, false);
        }

        /// <summary>Adds an end result file from a result file.</summary>
        /// <param name="contentItem">The result file.</param>
        /// <param name="id">The category.</param>
        /// <param name="isEndResult">If the result is an endresult.</param>
        public void AddResult(ContentItem contentItem, string id, bool isEndResult)
        {
            this.isUnsaved = true;
            Safe.Lock(
                this.cacheResults, 
                Safe.MaxLockTimeout, 
                () => this.cacheResults.Add(CacheResult.FromContentFile(this.context, this.cacheCategory, isEndResult, id, contentItem)));
        }

        /// <summary>Add a source dependency from a file.</summary>
        /// <param name="file">The file.</param>
        public void AddSourceDependency(string file)
        {
            if (!File.Exists(file))
            {
                throw new BuildWorkflowException("Cannot add a source dependency that does not exists on disk: {0}".InvariantFormat(file));
            }

            this.AddSourceDependency(new InputSpec { Path = file });
        }

        /// <summary>Adds a source dependency from a directory.</summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        public void AddSourceDependency(string directory, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!Directory.Exists(directory))
            {
                throw new BuildWorkflowException("Cannot add a source dependency that does not exists on disk: {0}".InvariantFormat(directory));
            }

            this.AddSourceDependency(new InputSpec { Path = directory, SearchPattern = searchPattern, SearchOption = searchOption });
        }

        /// <summary>Add a source dependency from an input spec.</summary>
        /// <param name="inputSpec">The input spec.</param>
        public void AddSourceDependency(InputSpec inputSpec)
        {
            this.isUnsaved = true;
            var key = inputSpec.ToJson(true);
            Safe.UniqueKeyLock(
                key, 
                Safe.MaxLockTimeout, 
                () =>
                {
                    if (!this.sourceDependencies.ContainsKey(key))
                    {
                        this.sourceDependencies.Add(
                            key,
                            CacheSourceDependency.Create(
                                this.context,
                                new InputSpec
                                    {
                                        IsOptional = inputSpec.IsOptional,
                                        Path = inputSpec.Path,
                                        SearchOption = inputSpec.SearchOption,
                                        SearchPattern = inputSpec.SearchPattern
                                    }));
                    }
                });
        }

        /// <summary>If all the cache files are valid and all results could be restored from content.</summary>
        /// <returns>If it can be restored from content.</returns>
        public bool CanBeRestoredFromCache()
        {
            return this.cachedSection != null 
                && this.cachedSection.CanBeRestoredFromCache();
        }

        /// <summary>Determiones if all the end results are there and valid, and can therefor be skipped for processing.</summary>
        /// <returns>If it can be skipped.</returns>
        public bool CanBeSkipped()
        {
            return this.cachedSection != null 
                && this.cachedSection.CanBeSkipped();
        }

        /// <summary>Ends the section.</summary>
        public void EndSection()
        {
            this.context.Cache.EndSection(this);
            this.Dispose();
        }

        /// <summary>Gets the cache results for the category recursively.</summary>
        /// <param name="fileCategory">The category.</param>
        /// <param name="endResultOnly">If it should return end results only.</param>
        /// <returns>The cache results for the category.</returns>
        public IEnumerable<CacheResult> GetCacheResults(string fileCategory = null, bool endResultOnly = false)
        {
            return this.cachedSection.GetCacheResults(fileCategory, endResultOnly);
        }

        /// <summary>Gets the cached content item.</summary>
        /// <param name="fileCategory">The file category.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public ContentItem GetCachedContentItem(string fileCategory)
        {
            var cacheResult = this.GetCacheResults(fileCategory).FirstOrDefault();
            return 
                cacheResult != null
                ? ContentItem.FromCacheResult(cacheResult)
                : null;
        }

        /// <summary>Gets the cached content item.</summary>
        /// <param name="fileCategory">The file category.</param>
        /// <param name="relativeDestinationFile">The relative Destination File.</param>
        /// <param name="relativeHashedDestinationFile">The relative hashed Destination File.</param>
        /// <param name="contentPivots">The content Pivots.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public ContentItem GetCachedContentItem(string fileCategory, string relativeDestinationFile, string relativeHashedDestinationFile = null, IEnumerable<ResourcePivotKey> contentPivots = null)
        {
            return ContentItem.FromCacheResult(this.GetCacheResults(fileCategory).FirstOrDefault(), relativeDestinationFile, relativeHashedDestinationFile, contentPivots != null ? contentPivots.ToArray() : null);
        }

        /// <summary>Gets the cached content items.</summary>
        /// <param name="fileCategory">The file category.</param>
        /// <param name="endResultOnly">If it should return end results only.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public IEnumerable<ContentItem> GetCachedContentItems(string fileCategory, bool endResultOnly = false)
        {
            return this.GetCacheResults(fileCategory, endResultOnly).Select(crf => ContentItem.FromCacheResult(crf));
        }

        /// <summary>Saves the cache section to the cache folder.</summary>
        public void Save()
        {
            if (this.isUnsaved)
            {
                this.isUnsaved = false;

                var path = new FileInfo(this.absolutePath);
                if (path.Directory != null && !path.Directory.Exists)
                {
                    path.Directory.Create();
                }

                File.WriteAllText(this.absolutePath, ToReadOnlyCacheSectionJson(this));
                this.Touch();
            }
        }

        #endregion

        #region Methods

        /// <summary>The ensure cache path.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cacheCategory">The cache category.</param>
        private static void EnsureCachePath(IWebGreaseContext context, string cacheCategory)
        {
            var cachePath = context.Cache.GetAbsoluteCacheFilePath(cacheCategory, string.Empty);
            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }
        }

        /// <summary>Get a unique json string for the cache section.</summary>
        /// <param name="cacheSection">The Cache Section</param>
        /// <returns>The json string.</returns>
        private static string ToReadOnlyCacheSectionJson(CacheSection cacheSection)
        {
            return
                new
                {
                    sourceDependencies = cacheSection.sourceDependencies.Values,
                    cacheSection.cacheResults,
                    children = cacheSection.ChildCacheSections.Select(ccs => ccs.absolutePath),
                    cacheSection.absolutePath
                }.ToJson();
        }

        /// <summary>The add child cache section.</summary>
        /// <param name="cacheSection">The cache section.</param>
        private void AddChildCacheSection(CacheSection cacheSection)
        {
            Safe.Lock(this.ChildCacheSections, () => this.ChildCacheSections.Add(cacheSection));
        }

        /// <summary>The dispose method, cleans up references and objects.</summary>
        private void Dispose()
        {
            if (this.cachedSection != null)
            {
                this.cachedSection.Dispose();
            }

            this.context = null;
            this.parent = null;
            this.sourceDependencies.Clear();
            this.ChildCacheSections.Clear();
            this.childCacheSections = null;
            this.cacheResults.Clear();
        }

        /// <summary>The touch all files.</summary>
        private void Touch()
        {
            this.context.Touch(this.absolutePath);
            this.cacheResults.ForEach(cr => this.context.Touch(cr.CachedFilePath));
        }

        #endregion
    }
}