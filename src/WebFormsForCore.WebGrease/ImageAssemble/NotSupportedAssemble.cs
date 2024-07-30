// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotSupportedAssemble.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   This class assembles JPEG images
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.ImageAssemble
{
    using System.Collections.Generic;
    using System.Drawing.Imaging;

    /// <summary>This class assembles JPEG images</summary>
    internal class NotSupportedAssemble : ImageAssembleBase
    {
        /// <summary>Initializes a new instance of the <see cref="NotSupportedAssemble"/> class.</summary>
        /// <param name="context">The context.</param>
        public NotSupportedAssemble(IWebGreaseContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets image type
        /// </summary>
        internal override ImageType Type
        {
            get
            {
                return ImageType.NotSupported;
            }
        }

        /// <summary>
        /// Gets default extension for Image type.
        /// </summary>
        internal override string DefaultExtension
        {
            get
            {
                return ".bmp";
            }
        }

        /// <summary>
        /// Gets image type
        /// </summary>
        protected override ImageFormat Format
        {
            get
            {
                return ImageFormat.Bmp;
            }
        }

        /// <summary>For image types that are not supported, write a separate entry in the image map
        /// since we are not assembling these images.</summary>
        /// <param name="inputImages">The input Images.</param>
        /// <returns>The <see cref="bool"/>. True if something was assembled, false if not.</returns>
        internal override bool Assemble(List<BitmapContainer> inputImages)
        {
            foreach (var entry in inputImages)
            {
                this.ImageXmlMap.AppendToXml(entry.InputImage.AbsoluteImagePath, "Not supported");
            }

            return false;
        }
    }
}
