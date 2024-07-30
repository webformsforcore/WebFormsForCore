// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssemblyAnalysis.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents the css analysis node generated from css which is analyzed
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
    using Ast;

    using WebGrease.ImageAssemble;

    /// <summary>The failure reason enumeration</summary>
    public enum FailureReason
    {
        /// <summary>The position criteria is not matched</summary>
        IncorrectPosition,

        /// <summary>The background size is set to non default value.</summary>
        BackgroundSizeIsSetToNonDefaultValue,

        /// <summary>The invalid dpi.</summary>
        InvalidDpi,

        /// <summary>The background repeat invalid.</summary>
        BackgroundRepeatInvalid,

        /// <summary>Has multiple url's which are unspoorted by webgrease.</summary>
        MultipleUrls, 

        /// <summary>The no-repeat criteria is not found</summary>
        NoRepeat, 

        /// <summary>The url is not found</summary>
        NoUrl, 

        /// <summary>The url is configured to ignore</summary>
        IgnoreUrl,

        /// <summary>Ignored using -wg-spriting: ignore.</summary>
        SpritingIgnore
    }

    /// <summary>Represents the image log generated from css which is analyzed</summary>
    internal class ImageAssemblyAnalysis
    {
        /// <summary>Gets or sets the failure reason for node not being considered for image analysis</summary>
        internal FailureReason? FailureReason { get; set; }

        /// <summary>Gets or sets the Ast node to print for the context</summary>
        internal AstNode AstNode { get; set; }

        /// <summary>Gets or sets the image.</summary>
        internal string Image { get; set; }

        /// <summary>Gets or sets the image type.</summary>
        internal ImageType? ImageType { get; set; }

        /// <summary>Gets or sets the sprited image.</summary>
        internal string SpritedImage { get; set; }
    }
}
