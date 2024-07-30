// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssembleConstants.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation for ImageAssembly constants
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis
{
    /// <summary>Provides the implementation for ImageAssembly constants</summary>
    internal static class ImageAssembleConstants
    {
        /// <summary>
        /// Background Image Declaration property name
        /// </summary>
        public const string BackgroundImage = "background-image";

        /// <summary>
        /// Background Repeat Declaration property name
        /// </summary>
        public const string BackgroundRepeat = "background-repeat";

        /// <summary>
        /// Background Position Declaration property name
        /// </summary>
        public const string BackgroundPosition = "background-position";

        /// <summary>
        /// Background Size property name
        /// </summary>
        public const string BackgroundSize = "background-size";

        /// <summary>
        /// WebGrease background dpi property name, use to determine the factor of the positioning and the background size.
        /// </summary>
        public const string WebGreaseBackgroundDpi = "-wg-background-dpi";

        /// <summary>
        /// Background Declaration property name
        /// </summary>
        public const string Background = "background";

        /// <summary>
        /// Background repeat
        /// </summary>
        public const string Repeat = "repeat";

        /// <summary>
        /// Background no repeat
        /// </summary>
        public const string NoRepeat = "no-repeat";

        /// <summary>
        /// Background repeat x
        /// </summary>
        public const string RepeatX = "repeat-x";

        /// <summary>
        /// Background repeat y
        /// </summary>
        public const string RepeatY = "repeat-y";

        /// <summary>
        /// Background position left
        /// </summary>
        public const string Left = "left";

        /// <summary>
        /// Background position right
        /// </summary>
        public const string Right = "right";

        /// <summary>
        /// Background position center
        /// </summary>
        public const string Center = "center";

        /// <summary>
        /// Background position top
        /// </summary>
        public const string Top = "top";

        /// <summary>
        /// Background position bottom
        /// </summary>
        public const string Bottom = "bottom";

        /// <summary>
        /// Measurement units in Px
        /// </summary>
        public const string Px = "px";

        /// <summary>
        /// Measurement units in Rem
        /// </summary>
        public const string Rem = "rem";

        /// <summary>
        /// Measurement units in Em
        /// </summary>
        public const string Em = "em";

        /// <summary>
        /// Measurement units in %
        /// </summary>
        public const string Percentage = "%";

        /// <summary>
        /// Subtract Unary Operator
        /// </summary>
        public const string Subtract = "-";

        /// <summary>
        /// Single Space
        /// </summary>
        public const string SingleSpace = " ";

        /// <summary>
        /// The up folder
        /// </summary>
        public const string UpFolder = "..";

        /// <summary>
        /// Zero string
        /// </summary>
        public const string Zero = "0";

        /// <summary>
        /// Px format string
        /// </summary>
        public const string PxFormat = "{0}" + Px;

        /// <summary>
        /// Px format string
        /// </summary>
        public const string RemFormat = "{0}" + Rem;

        /// <summary>
        /// Px format string
        /// </summary>
        public const string EmFormat = "{0}" + Em;

        /// <summary>
        /// Log original file element name
        /// </summary>
        public const string OriginalfileElementName = "originalfile";

        /// <summary>
        /// Log width element name
        /// </summary>
        public const string WidthElementName = "width";

        /// <summary>
        /// Log height element name
        /// </summary>
        public const string HeightElementName = "height";

        /// <summary>
        /// Log X coordinate elment name
        /// </summary>
        public const string XCoordinateElementName = "xposition";

        /// <summary>
        /// Log Y coordinate element name
        /// </summary>
        public const string YCoordinateElementName = "yposition";

        /// <summary>
        /// Position in sprite element name
        /// </summary>
        public const string PositionInSpriteElementName = "positioninsprite";

        /// <summary>
        /// Log input element name
        /// </summary>
        public const string InputElementName = "input";

        /// <summary>
        /// Log output element name
        /// </summary>
        public const string OutputElementName = "output";

        /// <summary>
        /// Log file attribute name
        /// </summary>
        public const string FileAttributeName = "file";

        /// <summary>
        /// The url term
        /// </summary>
        public const string UrlTerm = "url({0})";
    }
}