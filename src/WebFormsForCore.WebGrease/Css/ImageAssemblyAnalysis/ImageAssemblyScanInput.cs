// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssemblyScanInput.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The image sprite scan input.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;

    /// <summary>The image sprite scan input.</summary>
    public sealed class ImageAssemblyScanInput
    {
        /// <summary>Initializes a new instance of the <see cref="ImageAssemblyScanInput"/> class.</summary>
        /// <param name="bucketName">The bucket name say Lazy Load.</param>
        /// <param name="imagesInBucket">The images in bucket.</param>
        public ImageAssemblyScanInput(string bucketName, ReadOnlyCollection<string> imagesInBucket)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(bucketName));
            Contract.Requires(imagesInBucket != null);

            this.BucketName = bucketName;
            this.ImagesInBucket = imagesInBucket;
        }

        /// <summary>Gets the image assembly bucket name.</summary>
        public string BucketName { get; private set; }

        /// <summary>Gets the list of images in bucket.</summary>
        public ReadOnlyCollection<string> ImagesInBucket { get; private set; }
    }
}
