// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheResult.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    /// <summary>The cache result.</summary>
    public class CacheResult
    {
        #region Constructors and Destructors

        /// <summary>Prevents a default instance of the <see cref="CacheResult"/> class from being created.</summary>
        private CacheResult()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the original relative path.</summary>
        public string RelativeContentPath { get; private set; }

        /// <summary>Gets the alternate relative path.</summary>
        public string RelativeHashedContentPath { get; private set; }

        /// <summary>Gets the cached file path.</summary>
        public string CachedFilePath { get; private set; }

        /// <summary>Gets the file category.</summary>
        public string FileCategory { get; private set; }

        /// <summary>Gets the content hash.</summary>
        public string ContentHash { get; private set; }

        /// <summary>Gets a value indicating whether end result.</summary>
        public bool EndResult { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>The from result file.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cacheCategory">The cache category.</param>
        /// <param name="endResult">The end result.</param>
        /// <param name="fileCategory">The file category.</param>
        /// <param name="contentItem">The result file.</param>
        /// <returns>The <see cref="CacheResult"/>.</returns>
        public static CacheResult FromContentFile(IWebGreaseContext context, string cacheCategory, bool endResult, string fileCategory, ContentItem contentItem)
        {
            return new CacheResult
                        {
                           EndResult = endResult,
                           FileCategory = fileCategory,
                           CachedFilePath = context.Cache.StoreInCache(cacheCategory, contentItem),
                           ContentHash = contentItem.GetContentHash(context),
                           RelativeContentPath = contentItem.RelativeContentPath,
                           RelativeHashedContentPath = contentItem.RelativeHashedContentPath,
                        };
        }

        #endregion
    }
}