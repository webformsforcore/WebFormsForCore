// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheManager.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Generic;

    /// <summary>The CacheManager interface.</summary>
    public interface ICacheManager
    {
        #region Public Properties

        /// <summary>Gets the current cache section.</summary>
        ICacheSection CurrentCacheSection { get; }

        /// <summary>Gets the loaded cache sections.</summary>
        IDictionary<string, ReadOnlyCacheSection> LoadedCacheSections { get; }

        /// <summary>Gets the root cache path for this caching session.</summary>
        string RootPath { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Begins a new cache section.</summary>
        /// <param name="webGreaseSectionKey">The web Grease Section Key.</param>
        /// <param name="autoLoad">The auto Load.</param>
        /// <returns>The <see cref="ICacheSection"/>.</returns>
        ICacheSection BeginSection(WebGreaseSectionKey webGreaseSectionKey, bool autoLoad = true);

        /// <summary>Cleans up all the cache files that we don't need anymore.</summary>
        void CleanUp();

        /// <summary>Ends the cache section.</summary>
        /// <param name="cacheSection">The cache section.</param>
        void EndSection(ICacheSection cacheSection);

        /// <summary>Gets absolute cache file path.</summary>
        /// <param name="category">The category.</param>
        /// <param name="fileName">The relative cache file name.</param>
        /// <returns>The absolute cache file path.</returns>
        string GetAbsoluteCacheFilePath(string category, string fileName);

        /// <summary>Sets the current context.</summary>
        /// <param name="newContext">The current context.</param>
        void SetContext(IWebGreaseContext newContext);

        /// <summary>Stores the content file in cache.</summary>
        /// <param name="cacheCategory">The cache category.</param>
        /// <param name="contentItem">The content file.</param>
        /// <returns>The cache file path.</returns>
        string StoreInCache(string cacheCategory, ContentItem contentItem);

        /// <summary>The locked file cache action.</summary>
        /// <param name="lockFileContent">The lock file content.</param>
        /// <param name="action">The action.</param>
        void LockedFileCacheAction(string lockFileContent, Action action);

        #endregion
    }
}