// ----------------------------------------------------------------------------------------------------
// <copyright file="ContentItem.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System.Collections.Generic;
    using System.IO;

    using WebGrease.Configuration;

    /// <summary>
    /// The content item is the class that is used for all the intermediate states of the webgrease processing pipeline.
    /// It is an abstraction for files on disk/in mrmory/in cache, and allows the seperate parts of the pipeline to not have to care about where the content is.
    /// TODO: Will in a future release allow the addition of sourceMap property to add source maps.
    /// </summary>
    public class ContentItem
    {
        /// <summary>The content hash.</summary>
        private string contentHash;

        /// <summary>The content of the filoe on disk, used to store the conten after reading for reuse.</summary>
        private string content;

        #region Constructors and Destructors

        /// <summary>Prevents a default instance of the <see cref="ContentItem"/> class from being created.</summary>
        private ContentItem()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the origin relative source path.</summary>
        public string RelativeContentPath { get; private set; }

        /// <summary>Gets the locale.</summary>
        public IEnumerable<ResourcePivotKey> ResourcePivotKeys { get; private set; }

        /// <summary>Gets the alternate relative path.</summary>
        public string RelativeHashedContentPath { get; private set; }

        /// <summary>Gets the content of the file.</summary>
        /// <value>The file content.</value>
        public string Content
        {
            get
            {
                return this.ContentItemType == ContentItemType.Path
                    ? this.ContentFromDisk()
                    : this.ContentValue;
            }
        }

        /// <summary>Gets a value indicating whether is from disk.</summary>
        public bool IsFromDisk
        {
            get
            {
                return this.ContentItemType == ContentItemType.Path;
            }
        }

        /// <summary>Gets the absolute disk path.</summary>
        public string AbsoluteDiskPath
        {
            get
            {
                return this.IsFromDisk ? this.AbsoluteContentPath : null;
            }
        }

        /// <summary>Gets the content.</summary>
        private string ContentValue { get; set; }

        /// <summary>Gets the absolute destination path.</summary>
        private string AbsoluteContentPath { get; set; }

        /// <summary>Gets the content type.</summary>
        private ContentItemType ContentItemType { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Creates a content item from a cache result.</summary>
        /// <param name="cacheResult">The cache result.</param>
        /// <param name="resourcePivotKeys">The pivots.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public static ContentItem FromCacheResult(CacheResult cacheResult, params ResourcePivotKey[] resourcePivotKeys)
        {
            return new ContentItem
                       {
                           ContentItemType = ContentItemType.Path,
                           AbsoluteContentPath = cacheResult.CachedFilePath,
                           RelativeContentPath = cacheResult.RelativeContentPath,
                           RelativeHashedContentPath = cacheResult.RelativeHashedContentPath,
                           ResourcePivotKeys = resourcePivotKeys,
                       };
        }

        /// <summary>Creates a content item from a cache result.</summary>
        /// <param name="cacheResult">The cache result.</param>
        /// <param name="relativeContentPath">The relative Content Path.</param>
        /// <param name="relativeHashedContentPath">The relative Hashed Content Path.</param>
        /// <param name="resourcePivotKeys">The resource Pivot Keys.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public static ContentItem FromCacheResult(CacheResult cacheResult, string relativeContentPath = null, string relativeHashedContentPath = null, params ResourcePivotKey[] resourcePivotKeys)
        {
            return new ContentItem
                       {
                           ContentItemType = ContentItemType.Path,
                           AbsoluteContentPath = cacheResult.CachedFilePath,
                           RelativeContentPath = relativeContentPath ?? cacheResult.RelativeContentPath,
                           RelativeHashedContentPath = relativeHashedContentPath ?? cacheResult.RelativeHashedContentPath,
                           ResourcePivotKeys = resourcePivotKeys,
                       };
        }

        /// <summary>Creates a content item from a file.</summary>
        /// <param name="absoluteContentPath">The path.</param>
        /// <param name="relativeContentPath">The original Relative Path.</param>
        /// <param name="relativeHashedContentPath">The alternate Relative Path.</param>
        /// <param name="resourcePivotKeys">The resource Pivot Keys.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public static ContentItem FromFile(string absoluteContentPath, string relativeContentPath = null, string relativeHashedContentPath = null, params ResourcePivotKey[] resourcePivotKeys)
        {
            return new ContentItem
                         {
                             ContentItemType = ContentItemType.Path,
                             AbsoluteContentPath = absoluteContentPath,
                             RelativeContentPath = relativeContentPath ?? absoluteContentPath,
                             RelativeHashedContentPath = relativeHashedContentPath,
                             ResourcePivotKeys = resourcePivotKeys,
                         };
        }

        /// <summary>Creates a content item from another content item and changes the relativeHashedPath.</summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="relativeContentPath">The relative Content Path.</param>
        /// <param name="relativeHashedContentPath">The alternate Relative Path.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public static ContentItem FromContentItem(ContentItem contentItem, string relativeContentPath = null, string relativeHashedContentPath = null)
        {
            return new ContentItem
                       {
                           RelativeHashedContentPath = relativeHashedContentPath ?? contentItem.RelativeHashedContentPath,
                           RelativeContentPath = relativeContentPath ?? contentItem.RelativeContentPath,

                           AbsoluteContentPath = contentItem.AbsoluteContentPath,
                           ContentItemType = contentItem.ContentItemType,
                           ContentValue = contentItem.ContentValue,
                           ResourcePivotKeys = contentItem.ResourcePivotKeys,

                           contentHash = contentItem.contentHash
                       };
        }

        /// <summary>The from content.</summary>
        /// <param name="content">The content.</param>
        /// <param name="resourcePivotKeys">The resource Pivot Keys.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public static ContentItem FromContent(string content, params ResourcePivotKey[] resourcePivotKeys)
        {
            return new ContentItem
            {
                ContentItemType = ContentItemType.Value,
                ContentValue = content,
                ResourcePivotKeys = resourcePivotKeys,
            };
        }

        /// <summary>The from content.</summary>
        /// <param name="content">The content.</param>
        /// <param name="relativeContentPath">The original Relative Path.</param>
        /// <param name="relativeHashedContentPath">The alternate Relative Path.</param>
        /// <param name="resourcePivotKeys">The resource Pivot Keys.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public static ContentItem FromContent(string content, string relativeContentPath, string relativeHashedContentPath = null, params ResourcePivotKey[] resourcePivotKeys)
        {
            return new ContentItem
            {
                ContentItemType = ContentItemType.Value,
                ContentValue = content,
                ResourcePivotKeys = resourcePivotKeys,
                RelativeContentPath = relativeContentPath,
                RelativeHashedContentPath = relativeHashedContentPath,
            };
        }

        /// <summary>Creates a content item from a string value and a previous contentItem.</summary>
        /// <summary>The from content.</summary>
        /// <param name="content">The content.</param>
        /// <param name="contentItem">The content item.</param>
        /// <param name="resourcePivotKeys">The resource Pivot Keys.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        public static ContentItem FromContent(string content, ContentItem contentItem, params ResourcePivotKey[] resourcePivotKeys)
        {
            return new ContentItem
            {
                ContentItemType = ContentItemType.Value,
                ContentValue = content,
                RelativeContentPath = contentItem.RelativeContentPath,
                RelativeHashedContentPath = contentItem.RelativeHashedContentPath,
                ResourcePivotKeys = resourcePivotKeys ?? contentItem.ResourcePivotKeys
            };
        }

        #endregion

        /// <summary>Gets the md5 hash for the current content item.</summary>
        /// <param name="context">The context.</param>
        /// <returns>The md5 hash as a string.</returns>
        internal string GetContentHash(IWebGreaseContext context)
        {
            return this.contentHash ??
                (this.contentHash = ContentItemType == ContentItemType.Value
                    ? context.GetValueHash(this.Content)
                    : context.GetFileHash(this.AbsoluteContentPath));
        }

        /// <summary>Writes the content to the relative hashed path using the provided destination directory as a root.</summary>
        /// <summary>The the current content to the hashed file path.</summary>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <param name="overwrite">if it should overwrite if it already exists.</param>
        internal void WriteToRelativeHashedPath(string destinationDirectory, bool overwrite = false)
        {
            this.WriteTo(Path.Combine(destinationDirectory ?? string.Empty, this.RelativeHashedContentPath), overwrite);
        }

        /// <summary>Writes the content to the relative content path using the provided destination directory as a root.</summary>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <param name="overwrite">The overwrite.</param>
        internal void WriteToContentPath(string destinationDirectory, bool overwrite = false)
        {
            this.WriteTo(Path.Combine(destinationDirectory ?? string.Empty, this.RelativeContentPath), overwrite);
        }

        /// <summary>Save to disk.</summary>
        /// <param name="fullPath">The full path</param>
        /// <param name="overwrite">The overwrite.</param>
        internal void WriteTo(string fullPath, bool overwrite = false)
        {
            var absolutePath = new FileInfo(fullPath);
            Safe.FileLock(absolutePath, () =>
            {
                if (!absolutePath.Exists || overwrite)
                {
                    if (absolutePath.Directory != null && !absolutePath.Directory.Exists)
                    {
                        absolutePath.Directory.Create();
                    }

                    if (this.ContentItemType == ContentItemType.Path)
                    {
                                File.Copy(this.AbsoluteContentPath, absolutePath.FullName, overwrite);
                    }
                    else
                    {
                        File.WriteAllText(absolutePath.FullName, this.Content);
                    }
                }
            });
        }

        /// <summary>Reads the content from disk, uses File.ReadAll, caches the result in a private field.</summary>
        /// <returns>The content of the file as a string..</returns>
        private string ContentFromDisk()
        {
            return this.content ?? (this.content = File.ReadAllText(this.AbsoluteContentPath));
        }
    }
}