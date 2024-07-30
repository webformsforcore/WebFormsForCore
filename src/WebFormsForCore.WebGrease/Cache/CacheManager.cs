// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheManager.cs" company="Microsoft">
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

    /// <summary>The cache manager.</summary>
    public class CacheManager : ICacheManager
    {
        #region Fields

        /// <summary>The lock file name.</summary>
        internal const string LockFileName = "webgrease.caching.lock";

        /// <summary>The first.</summary>
        private static readonly IList<string> First = new List<string>();

        /// <summary>The loaded cache sections.</summary>
        private readonly IDictionary<string, ReadOnlyCacheSection> loadedCacheSections = new Dictionary<string, ReadOnlyCacheSection>(StringComparer.OrdinalIgnoreCase);

        /// <summary>The cache root path.</summary>
        private readonly string cacheRootPath;

        /// <summary>The context.</summary>
        private IWebGreaseContext context;

        /// <summary>The current cache section.</summary>
        private ICacheSection currentCacheSection;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="CacheManager"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="parentCacheSection">The parent Cache Section.</param>
        public CacheManager(WebGreaseConfiguration configuration, LogManager logManager, ICacheSection parentCacheSection)
        {
            this.currentCacheSection = parentCacheSection;

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (logManager == null)
            {
                throw new ArgumentNullException("logManager");
            }

            var cacheRoot = configuration.CacheRootPath.AsNullIfWhiteSpace() ?? "_webgrease.cache";

            if (!Path.IsPathRooted(cacheRoot))
            {
                cacheRoot = Path.Combine(configuration.SourceDirectory, cacheRoot);
            }

            this.cacheRootPath = GetCacheUniquePath(cacheRoot, configuration.CacheUniqueKey);

            if (!Directory.Exists(this.cacheRootPath))
            {
                Directory.CreateDirectory(this.cacheRootPath);
            }

            Safe.Lock(
                First,
                () =>
                {
                    // Only log this once when multi threading.
                    if (First.Contains(this.cacheRootPath))
                    {
                        First.Add(this.cacheRootPath);
                        logManager.Information("Cache enabled using cache path: {0}".InvariantFormat(this.cacheRootPath));
                    }
                });
        }

        /// <summary>The locked file cache action.</summary>
        /// <param name="lockFileContent">The lock file content.</param>
        /// <param name="action">The action.</param>
        public void LockedFileCacheAction(string lockFileContent, Action action)
        {
            var cacheLockFile = Path.Combine(this.RootPath, LockFileName);
            var success = Safe.WriteToFileStream(
                cacheLockFile,
                fs =>
                    {
                        var sr = new StreamWriter(fs);
                        sr.Write(lockFileContent);
                        sr.Flush();
                        action();
                        sr.Write("\r\nDone");
                        sr.Flush();
                    });

            if (!success)
            {
                throw new BuildWorkflowException("Could not get a unique lock on cache lock file: {0}".InvariantFormat(cacheLockFile));
            }
        }

        /// <summary>The get cache unique path.</summary>
        /// <param name="cacheRoot">The cache root.</param>
        /// <param name="cacheUniqueKey">The cache Unique Key.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetCacheUniquePath(string cacheRoot, string cacheUniqueKey)
        {
            string cachePath = null;
            var uniqueKey = cacheUniqueKey ?? string.Empty;
            var mapFilePath = Path.Combine(cacheRoot, "cachefoldermap.txt");
            var success = Safe.WriteToFileStream(
                    mapFilePath,
                    fs =>
                    {
                        var items = new Dictionary<string, string>();
                        var sr = new StreamReader(fs);
                        items = sr.ReadToEnd().FromJson<Dictionary<string, string>>() ?? items;

                        string uniqueValue;
                        if (!items.TryGetValue(uniqueKey, out uniqueValue))
                        {
                            var i = 0;
                            do
                            {
                                uniqueValue = "CF{0}".InvariantFormat(++i);
                            }
                            while (items.ContainsValue(uniqueValue));

                            items.Add(uniqueKey, uniqueValue);
                            fs.Seek(0, SeekOrigin.Begin);
                            var sw = new StreamWriter(fs);
                            var newContent = items.ToJson();
                            sw.Write(newContent);
                            sw.Flush();
                        }

                        cachePath = Path.Combine(cacheRoot, uniqueValue);
                    });

            if (!success)
            {
                throw new BuildWorkflowException("Could not get a unique lock on: {0}".InvariantFormat(mapFilePath));
            }

            if (string.IsNullOrWhiteSpace(cachePath) || cachePath == cacheRoot)
            {
                throw new BuildWorkflowException("Could not find a valid cache folder in: {0}".InvariantFormat(cacheRoot));
            }

            return cachePath;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the current cache section.</summary>
        public ICacheSection CurrentCacheSection
        {
            get
            {
                return this.currentCacheSection;
            }
        }

        /// <summary>Gets the loaded cache sections.</summary>
        public IDictionary<string, ReadOnlyCacheSection> LoadedCacheSections
        {
            get
            {
                return this.loadedCacheSections;
            }
        }

        /// <summary>Gets the root cache path for this caching session.</summary>
        public string RootPath
        {
            get
            {
                return this.cacheRootPath;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Begins a new cache section.</summary>
        /// <param name="webGreaseSectionKey">The web Grease Section Key.</param>
        /// <param name="autoLoad">The auto Load.</param>
        /// <returns>The <see cref="ICacheSection"/>.</returns>
        public ICacheSection BeginSection(WebGreaseSectionKey webGreaseSectionKey, bool autoLoad = true)
        {
            return this.currentCacheSection = CacheSection.Begin(
                this.context,
                webGreaseSectionKey.Category,
                webGreaseSectionKey.Value,
                this.CurrentCacheSection,
                autoLoad);
        }

        /// <summary>Cleans up all the cache files that we don't need anymore.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all by design.")]
        public void CleanUp()
        {
            var startTime = this.context.SessionStartTime.UtcDateTime;
            if (this.context.Configuration.CacheTimeout.TotalSeconds > 0)
            {
                var expireTime = startTime - this.context.Configuration.CacheTimeout;
                var allFiles = Directory.GetFiles(this.cacheRootPath, "*.*", SearchOption.AllDirectories);
                var filesToDelete = allFiles.Where(f => File.GetLastWriteTimeUtc(f) < expireTime);
                foreach (var fileToDelete in filesToDelete)
                {
                    try
                    {
                        File.Delete(fileToDelete);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /// <summary>Ends the cache section.</summary>
        /// <param name="cacheSection">The cache section.</param>
        public void EndSection(ICacheSection cacheSection)
        {
#if DEBUG
            if (this.CurrentCacheSection != cacheSection)
            {
                throw new BuildWorkflowException("Something unexpected went wrong with the caching logic.");
            }
#endif
            if (this.CurrentCacheSection == cacheSection)
            {
                this.currentCacheSection = cacheSection.Parent;
            }
        }

        /// <summary>Gets absolute cache file path.</summary>
        /// <param name="category">The category.</param>
        /// <param name="fileName">The relative cache file name.</param>
        /// <returns>The absolute cache file path.</returns>
        public string GetAbsoluteCacheFilePath(string category, string fileName)
        {
            return Path.Combine(this.cacheRootPath, category, fileName);
        }

        /// <summary>Sets the current context.</summary>
        /// <param name="newContext">The current context.</param>
        public void SetContext(IWebGreaseContext newContext)
        {
            this.context = newContext;
        }

        /// <summary>Stores the content file in cache.</summary>
        /// <param name="cacheCategory">The cache category.</param>
        /// <param name="contentItem">The content file.</param>
        /// <returns>The cache file path.</returns>
        public string StoreInCache(string cacheCategory, ContentItem contentItem)
        {
            // Get the unique hash id for the file.
            var uniqueId = contentItem.GetContentHash(this.context);

            // Get the file extension, fallback to .txt
            var extension = Path.GetExtension(contentItem.RelativeContentPath) ?? ".txt";

            // Get the absolute cache file path
            var absoluteCacheFilePath = this.GetAbsoluteCacheFilePath(cacheCategory, uniqueId + extension);

            contentItem.WriteTo(absoluteCacheFilePath);

            return absoluteCacheFilePath;
        }

        #endregion
    }
}