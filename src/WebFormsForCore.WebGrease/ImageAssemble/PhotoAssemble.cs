// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotoAssemble.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   This class assembles JPEG images
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.ImageAssemble
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;

    /// <summary>This class assembles JPEG images</summary>
    internal class PhotoAssemble : ImageAssembleBase
    {
        #region Private fields

        /// <summary>
        /// Default value for JPEG Quality
        /// </summary>
        private const long DefaultJpegQuality = 100L;

        #endregion

        /// <summary>Initializes a new instance of the <see cref="PhotoAssemble"/> class.</summary>
        /// <param name="context">The context.</param>
        public PhotoAssemble(IWebGreaseContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets default extension for Image type.
        /// </summary>
        internal override string DefaultExtension
        {
            get
            {
                return ".jpg";
            }
        }

        /// <summary>
        /// Gets image type
        /// </summary>
        internal override ImageType Type
        {
            get
            {
                return ImageType.Photo;
            }
        }

        /// <summary>
        /// Gets image type
        /// </summary>
        protected override ImageFormat Format
        {
            get
            {
                return ImageFormat.Jpeg;
            }
        }

        /// <summary>Optimizes the image quality for JPEG files while saving it.</summary>
        /// <param name="newImage">Image to be saved.</param>
        protected override void SaveImage(Bitmap newImage)
        {
            const string MimeType = "image/jpeg";
            ImageCodecInfo encoder;

            // Get ImageCodecInfo for JPEG Image
            var encoders = ImageCodecInfo.GetImageEncoders();
            var enc = encoders.Where(e => e.MimeType == MimeType);

            // Get the first and if it doesn't exist then exception is expected
            encoder = enc.First();

            var qualityEncoder = Encoder.Quality;

            // Quality of JPEG compression passed as long via Hardcoded value
            using (var ratio = new EncoderParameter(qualityEncoder, DefaultJpegQuality))
            {
                // Add the quality paramete to the list
                using (var codecParams = new EncoderParameters(1))
                {
                    codecParams.Param[0] = ratio;
                    newImage.Save(this.AssembleFileName, encoder, codecParams);
                }
            }
        }
    }
}
