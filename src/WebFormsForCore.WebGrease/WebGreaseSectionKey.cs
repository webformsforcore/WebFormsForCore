// ---------------------------------------------------------------------
// <copyright file="WebGreaseSectionKey.cs" company="Microsoft">
//    Copyright Microsoft Corporation, all rights reserved
// </copyright>
// ---------------------------------------------------------------------
namespace WebGrease
{
    using System.Collections.Generic;
    using System.Linq;

    using WebGrease.Configuration;
    using WebGrease.Extensions;

    /// <summary>The web grease section key.</summary>
    public class WebGreaseSectionKey
    {
        /// <summary>
        /// The cache section file version key.
        /// This number is used to create the unique hash for each cache file. 
        /// Upping this number will basically invalidate any of the existingcache files users of webgrease have on their box. 
        /// Whenever we change caching logic/structure we should change/up this value.
        /// </summary>
        private const string CacheSectionFileVersionKey = "1.0.11";

        /// <summary>The delimiter.</summary>
        private const string Delimiter = "|";

        /// <summary>Initializes a new instance of the <see cref="WebGreaseSectionKey"/> class.</summary>
        /// <param name="context">The context.</param>
        /// <param name="category">The id.</param>
        /// <param name="cacheVarByContentItem">The cache var by content item.</param>
        /// <param name="cacheVarBySetting">The cache var by setting.</param>
        /// <param name="cacheVarByFileSet">The cache var by file set.</param>
        /// <param name="uniqueKey">The unique Key.</param>
        public WebGreaseSectionKey(IWebGreaseContext context, string category, ContentItem cacheVarByContentItem, object cacheVarBySetting, IFileSet cacheVarByFileSet, string uniqueKey = null)
        {
            this.Category = category;
            this.Value = uniqueKey;
            if (string.IsNullOrWhiteSpace(uniqueKey))
            {
                var varyByFiles = new List<CacheVaryByFile>();
                var varyBySettings = new List<string>();

                if (cacheVarByContentItem != null)
                {
                    varyByFiles.Add(CacheVaryByFile.FromFile(context, cacheVarByContentItem));
                    varyBySettings.Add(cacheVarByContentItem.ResourcePivotKeys.ToJson());
                }

                if (cacheVarByFileSet != null)
                {
                    varyBySettings.Add(cacheVarByFileSet.ToJson());
                }

                if (context.Configuration.Overrides != null)
                {
                    varyBySettings.Add(context.Configuration.Overrides.UniqueKey);
                }

                varyBySettings.Add(cacheVarBySetting.ToJson(true));

                this.Value = CacheSectionFileVersionKey + Delimiter + category + Delimiter + string.Join(Delimiter, varyByFiles.Select(vbf => vbf.Hash).Concat(varyBySettings));
            }
        }

        /// <summary>Gets the category.</summary>
        public string Category { get; private set; }

        /// <summary>Gets the unique key.</summary>
        public string Value { get; private set; }
    }
}