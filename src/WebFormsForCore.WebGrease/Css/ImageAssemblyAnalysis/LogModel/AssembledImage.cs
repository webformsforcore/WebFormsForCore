// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssembledImage.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents the Input Elment in log file
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml.Linq;
    using Extensions;
    using ImageAssemble;

    using WebGrease.Extensions;

    /// <summary>Represents the Input Elment in log file</summary>
    internal class AssembledImage
    {
        /// <summary>Initializes a new instance of the <see cref="AssembledImage"/> class.</summary>
        internal AssembledImage() { }

        /// <summary>Initializes a new instance of the AssembledImage class</summary>
        /// <param name="element">The element having the list of inputs</param>
        /// <param name="spriteWidth">The width of the sprite containing the image.</param>
        /// <param name="spriteHeight">The height of the sprite containing the image.</param>
        internal AssembledImage(XContainer element, int? spriteWidth, int? spriteHeight)
        {
            this.SpriteWidth = spriteWidth;
            this.SpriteHeight = spriteHeight;
            if (element != null)
            {
                element.Elements().ForEach(this.ParseElement);
            }
        }

        /// <summary>
        /// Gets the relative output file path
        /// </summary>
        internal int? SpriteWidth { get; private set; }

        /// <summary>
        /// Gets the relative output file path
        /// </summary>
        internal int? SpriteHeight { get; private set; }

        /// <summary>
        /// Gets or sets the relative output file path
        /// </summary>
        internal string RelativeOutputFilePath { get; set; }

        /// <summary>
        /// Gets or sets the output file path
        /// </summary>
        internal string OutputFilePath { get; set; }

        /// <summary>
        /// Gets or sets the original file path
        /// </summary>
        internal string OriginalFilePath { get; set; }

        /// <summary>
        /// Gets the x coordinate
        /// </summary>
        internal int? X { get; private set; }

        /// <summary>
        /// Gets the y coodrinate
        /// </summary>
        internal int? Y { get; private set; }

        /// <summary>
        /// Gets the position in sprite
        /// </summary>
        internal ImagePosition? ImagePosition { get; private set; }

        /// <summary>Loads the value from the element into the int values</summary>
        /// <param name="element">The element with the value</param>
        /// <returns>The int value</returns>
        private static int LoadDimension(XElement element)
        {
            int value;
            if (int.TryParse(element.Value, out value))
            {
                return value;
            }

            throw new ImageAssembleException(string.Format(CultureInfo.CurrentUICulture, CssStrings.InvalidDimensionsError, element.Name));
        }

        /// <summary>The parse element.</summary>
        /// <param name="childElement">The child element.</param>
        private void ParseElement(XElement childElement)
        {
            var elementName = childElement.Name.ToString();

            switch (elementName)
            {
                case ImageAssembleConstants.OriginalfileElementName:
                    this.OriginalFilePath = childElement.Value.GetFullPathWithLowercase();
                    break;
                case ImageAssembleConstants.XCoordinateElementName:
                    this.X = LoadDimension(childElement);
                    break;
                case ImageAssembleConstants.YCoordinateElementName:
                    this.Y = LoadDimension(childElement);
                    break;
                case ImageAssembleConstants.PositionInSpriteElementName:
                    this.ImagePosition = (ImagePosition)Enum.Parse(typeof(ImagePosition), childElement.Value);
                    break;
            }
        }
    }
}