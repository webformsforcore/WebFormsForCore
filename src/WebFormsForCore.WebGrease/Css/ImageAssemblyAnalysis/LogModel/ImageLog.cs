// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageLog.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents the image log generated from image assembler tool
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;
    using Extensions;

    using WebGrease.Extensions;

    /// <summary>Represents the image log generated from image assembler tool</summary>
    internal class ImageLog
    {
        /// <summary>Initializes a new instance of the <see cref="ImageLog"/> class.</summary>
        internal ImageLog()
        {
            // List input file object
            this.InputImages = new List<AssembledImage>();
        }

        /// <summary>Initializes a new instance of the <see cref="ImageLog"/> class.</summary>
        /// <param name="imageMapDocument">The image map.</param>
        internal ImageLog(XDocument imageMapDocument)
            : this()
        {
            if (imageMapDocument == null)
            {
                throw new ArgumentNullException("imageMapDocument");
            }

            if (imageMapDocument.Root != null)
            {
                // For each output child element
                imageMapDocument.Root
                    .Elements(ImageAssembleConstants.OutputElementName)
                    .ForEach(this.ProcessOutputElement);
            }
        }

        /// <summary>
        /// Gets the list of output images
        /// </summary>
        internal List<AssembledImage> InputImages { get; private set; }

        /// <summary>The process output elements.</summary>
        /// <param name="outputElement">The output element.</param>
        private void ProcessOutputElement(XElement outputElement)
        {
            // Get the total sprite width and height.
            var spriteWidth = (int?)outputElement.Attribute("width");
            var spriteHeight = (int?)outputElement.Attribute("height");

            var fileAttribute = outputElement.Attribute(ImageAssembleConstants.FileAttributeName);

            // This is a case of images ignored by image assembler
            if (fileAttribute == null)
            {
                return;
            }

            // Output image file path
            var outputFilePath = fileAttribute.Value;

            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                // This is a case of images ignored by image assembler
                return;
            }

            // TODO - Spec. issue: Shall we support http:// paths?
            outputFilePath = outputFilePath.GetFullPathWithLowercase();

            // Validate the output file
            if (!File.Exists(outputFilePath))
            {
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentUICulture, CssStrings.FileNotFoundError, outputFilePath));
            }

            outputElement.Descendants(ImageAssembleConstants.InputElementName).ForEach(
                inputElement => this.ProcessInputElement(inputElement, spriteWidth, spriteHeight, outputFilePath));
        }

        /// <summary>The process input elements.</summary>
        /// <param name="inputElement">The input element.</param>
        /// <param name="spriteWidth">The sprite width.</param>
        /// <param name="spriteHeight">The sprite height.</param>
        /// <param name="outputFilePath">The output file path.</param>
        private void ProcessInputElement(XElement inputElement, int? spriteWidth, int? spriteHeight, string outputFilePath)
        {
            // Add the input object to dictionary
            this.InputImages.Add(
                new AssembledImage(
                    inputElement, 
                    spriteWidth, 
                    spriteHeight)
                    {
                        OutputFilePath = outputFilePath
                    });
        }
    }
}