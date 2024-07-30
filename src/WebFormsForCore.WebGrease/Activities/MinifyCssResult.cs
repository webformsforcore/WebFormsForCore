// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinifyCssResult.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Activities
{
    using System.Collections.Generic;

    /// <summary>The minify css result.</summary>
    internal class MinifyCssResult
    {
        /// <summary>Initializes a new instance of the <see cref="MinifyCssResult"/> class.</summary>
        /// <param name="css">The css.</param>
        /// <param name="spritedImages">The sprited images.</param>
        /// <param name="hashedImages">The hashed images.</param>
        public MinifyCssResult(IEnumerable<ContentItem> css, IEnumerable<ContentItem> spritedImages, IEnumerable<ContentItem> hashedImages)
        {
            this.Css = css;
            this.SpritedImages = spritedImages;
            this.HashedImages = hashedImages;
        }

        /// <summary>Gets the css.</summary>
        internal IEnumerable<ContentItem> Css { get; private set; }

        /// <summary>Gets the sprited images.</summary>
        internal IEnumerable<ContentItem> SpritedImages { get; private set; }

        /// <summary>Gets the hashed images.</summary>
        internal IEnumerable<ContentItem> HashedImages { get; private set; }
    }
}