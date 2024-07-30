// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheSection.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System.Collections.Generic;
    using System.IO;

    using WebGrease.Configuration;

    /// <summary>The CacheSection interface.</summary>
    public interface ICacheSection
    {
        /// <summary>Gets the parent.</summary>
        ICacheSection Parent { get; }

        /// <summary>Gets the unique key.</summary>
        string UniqueKey { get; }

        /// <summary>Ends the section.</summary>
        void EndSection();

        /// <summary>If all the cache files are valid and all results could be restored from content.</summary>
        /// <returns>If it can be restored from content.</returns>
        bool CanBeRestoredFromCache();

        /// <summary>Adds an end result file from a result file.</summary>
        /// <param name="contentItem">The result file.</param>
        /// <param name="id">The category.</param>
        /// <param name="isEndResult">If the result is an endresult</param>
        void AddResult(ContentItem contentItem, string id, bool isEndResult = false);

        /// <summary>Add a source dependency from a file.</summary>
        /// <param name="file">The file.</param>
        void AddSourceDependency(string file);

        /// <summary>Adds a source dependency from a directory.</summary>
        /// <param name="directory">The directory.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchOption">The search option.</param>
        void AddSourceDependency(string directory, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly);

        /// <summary>Add a source dependency from an input spec.</summary>
        /// <param name="inputSpec">The input spec.</param>
        void AddSourceDependency(InputSpec inputSpec);

        /// <summary>Stores the section.</summary>
        void Save();

        /// <summary>Determiones if all the end results are there and valid, and can therefor be skipped for processing.</summary>
        /// <returns>If it can be skipped.</returns>
        bool CanBeSkipped();

        /// <summary>Gets the cached content item.</summary>
        /// <param name="fileCategory">The file category.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        ContentItem GetCachedContentItem(string fileCategory);

        /// <summary>Gets the cached content items.</summary>
        /// <param name="fileCategory">The file category.</param>
        /// <param name="endResultOnly">If it should return end results only.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        IEnumerable<ContentItem> GetCachedContentItems(string fileCategory, bool endResultOnly = false);

        /// <summary>The get cache data.</summary>
        /// <param name="id">The id.</param>
        /// <typeparam name="T">The typeof object</typeparam>
        /// <returns>The <see cref="T"/>.</returns>
        T GetCacheData<T>(string id) where T : new();

        /// <summary>The set cache data.</summary>
        /// <param name="id">The id.</param>
        /// <param name="obj">The data object.</param>
        /// <typeparam name="T">The typeof object</typeparam>
        void SetCacheData<T>(string id, T obj) where T : new();

        /// <summary>Gets the cached content item.</summary>
        /// <param name="fileCategory">The file category.</param>
        /// <param name="relativeDestinationFile">The relative Destination File.</param>
        /// <param name="relativeHashedDestinationFile">The relative hashed Destination File.</param>
        /// <param name="contentPivots">The content Pivots.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        ContentItem GetCachedContentItem(string fileCategory, string relativeDestinationFile, string relativeHashedDestinationFile = null, IEnumerable<ResourcePivotKey> contentPivots = null);

        /// <summary>The load.</summary>
        void Load();
    }
}