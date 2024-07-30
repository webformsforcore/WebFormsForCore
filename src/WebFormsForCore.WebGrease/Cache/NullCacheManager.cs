// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullCacheManager.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Generic;

    /// <summary>The null cache manager.</summary>
    internal class NullCacheManager : ICacheManager
    {
        #region Static Fields

        /// <summary>The empty cache section.</summary>
        internal static readonly ICacheSection EmptyCacheSection = new NullCacheSection();

        /// <summary>The empty read only cache sections.</summary>
        private static readonly Dictionary<string, ReadOnlyCacheSection> EmptyReadOnlyCacheSections = new Dictionary<string, ReadOnlyCacheSection>();

        #endregion

        #region Public Properties

        /// <summary>Gets the current cache section.</summary>
        public ICacheSection CurrentCacheSection
        {
            get
            {
                return EmptyCacheSection;
            }
        }

        /// <summary>Gets the loaded cache sections.</summary>
        public IDictionary<string, ReadOnlyCacheSection> LoadedCacheSections
        {
            get
            {
                return EmptyReadOnlyCacheSections;
            }
        }

        /// <summary>Gets the root cache path for this caching session.</summary>
        public string RootPath
        {
            get
            {
                return null;
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
            return EmptyCacheSection;
        }

        /// <summary>Cleans up all the cache files that we don't need anymore.</summary>
        public void CleanUp()
        {
        }

        /// <summary>Ends the cache section.</summary>
        /// <param name="cacheSection">The cache section.</param>
        public void EndSection(ICacheSection cacheSection)
        {
        }

        /// <summary>Gets absolute cache file path.</summary>
        /// <param name="category">The category.</param>
        /// <param name="fileName">The relative cache file name.</param>
        /// <returns>The absolute cache file path.</returns>
        public string GetAbsoluteCacheFilePath(string category, string fileName)
        {
            return null;
        }

        /// <summary>Sets the current context.</summary>
        /// <param name="newContext">The current context.</param>
        public void SetContext(IWebGreaseContext newContext)
        {
        }

        /// <summary>Stores the content file in cache.</summary>
        /// <param name="cacheCategory">The cache category.</param>
        /// <param name="contentItem">The content file.</param>
        /// <returns>The cache file path.</returns>
        public string StoreInCache(string cacheCategory, ContentItem contentItem)
        {
            return null;
        }

        /// <summary>The locked file cache action.</summary>
        /// <param name="lockFileContent">The lock file content.</param>
        /// <param name="action">The action.</param>
        public void LockedFileCacheAction(string lockFileContent, Action action)
        {
            action();
        }

        #endregion
    }
}