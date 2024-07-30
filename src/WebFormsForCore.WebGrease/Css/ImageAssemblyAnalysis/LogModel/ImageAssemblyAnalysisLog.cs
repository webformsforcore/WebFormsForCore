// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssemblyAnalysisLog.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents the css analysis log generated from css which is analyzed
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis.LogModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using WebGrease.Css.Extensions;
    using WebGrease.ImageAssemble;

    /// <summary>Represents the image log generated from css which is analyzed</summary>
    public class ImageAssemblyAnalysisLog
    {
        /// <summary>The message for no px</summary>
        private const string PxMessage = "No declaration with absolute vertical position found.";

        /// <summary>The message for no url</summary>
        private const string NoUrlMessage = "No declaration with background url.";

        /// <summary>The message for no repeat</summary>
        private const string NoRepeatMessage = "No declaration with background 'no-repeat'.";

        /// <summary>The message for ignore url</summary>
        private const string IgnoreUrlMessage = "The image url is configured to ignore in locale resx file.";

        /// <summary>The invalid dpi message.</summary>
        private const string InvalidDpiMessage = "-wg-dpi was set but was invalid.";

        private const string SpritingIgnoredMessage = "-wg-spriting: ignore.";

        /// <summary>The multiple urls message.</summary>
        private const string MultipleUrlsMessage = "Multiple url's in a single background are not supported by webgrease at this time.";

        /// <summary>The background repeat invalid message.</summary>
        private const string BackgroundRepeatInvalidMessage = "Background-repeat value was invalid (only no-repeat allows spriting)";

        /// <summary>The log nodes which maintains the list of criteria</summary>
        private readonly List<ImageAssemblyAnalysis> logNodes = new List<ImageAssemblyAnalysis>();

        /// <summary>Gets the failed sprites.</summary>
        internal IEnumerable<ImageAssemblyAnalysis> FailedSprites
        {
            get
            {
                return this.logNodes.Where(ln =>
                    ln.FailureReason != null
                    && ln.FailureReason != FailureReason.NoUrl
                    && ln.FailureReason != FailureReason.IgnoreUrl
                    && ln.FailureReason != FailureReason.SpritingIgnore);
            }
        }

        /// <summary>Gets the message for the failure.</summary>
        /// <param name="analysis">The image assembly analysis object.</param>
        /// <returns>The failue message.</returns>
        internal static string GetFailureMessage(ImageAssemblyAnalysis analysis)
        {
            switch (analysis.FailureReason)
            {
                case FailureReason.IncorrectPosition:
                    return PxMessage;
                case FailureReason.NoUrl:
                    return NoUrlMessage;
                case FailureReason.NoRepeat:
                    return NoRepeatMessage;
                case FailureReason.IgnoreUrl:
                    return IgnoreUrlMessage;
                case FailureReason.SpritingIgnore:
                    return SpritingIgnoredMessage;
                case FailureReason.BackgroundSizeIsSetToNonDefaultValue:
                    return IgnoreUrlMessage;
                case FailureReason.InvalidDpi:
                    return InvalidDpiMessage;
                case FailureReason.MultipleUrls:
                    return MultipleUrlsMessage;
                case FailureReason.BackgroundRepeatInvalid:
                    return BackgroundRepeatInvalidMessage;
                case null:
                    return "No failure";
                default:
                    return "Unknown failure reason";
            }
        }

        /// <summary>Provides the interface to add the log node</summary>
        /// <param name="logNode">The log node</param>
        internal void Add(ImageAssemblyAnalysis logNode)
        {
            if (logNode != null)
            {
                this.logNodes.Add(logNode);
            }
        }

        /// <summary>The set image type.</summary>
        /// <param name="imageType">The image type.</param>
        /// <param name="imagePath">The image path.</param>
        /// <param name="spritedImage">The sprited Image.</param>
        internal void UpdateSpritedImage(ImageType imageType, string imagePath, string spritedImage)
        {
            this.logNodes.Where(ln =>
                {
                    var originalImage = ln.Image;
                    if (originalImage == null)
                    {
                        return false;
                    }

                    return originalImage.Equals(imagePath, StringComparison.OrdinalIgnoreCase);
                }).ForEach(i =>
                    {
                        i.ImageType = imageType;
                        i.SpritedImage = spritedImage;
                    });
        }

        /// <summary>The save.</summary>
        /// <param name="path">The path.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Anonymouse objects needs them in the same method.")]
        internal void Save(string path)
        {
            if (!this.logNodes.Any())
            {
                return;
            }

            var sprited = this.logNodes.Where(ln => ln.FailureReason == null);
            var unsprited = this.logNodes.Where(ln => ln.FailureReason != null);
            var unspritedWithUrl = unsprited.Where(ln => ln.FailureReason != FailureReason.NoUrl);
            var unspritedIgnored = unspritedWithUrl.Where(ln => ln.FailureReason == FailureReason.IgnoreUrl || ln.FailureReason == FailureReason.SpritingIgnore);
            var unspritedFailed = unspritedWithUrl.Where(ln => ln.FailureReason != FailureReason.IgnoreUrl && ln.FailureReason != FailureReason.SpritingIgnore);

            new XElement(
                "SpritingLog",
                new XElement("Failed", unspritedFailed.OrderBy(i => i.FailureReason).Select(LogNodeToXElement)),
                new XElement("Ignored", unspritedIgnored.OrderBy(i => i.FailureReason).Select(LogNodeToXElement)),
                sprited.GroupBy(ln => new { ln.SpritedImage, ln.ImageType }).Select(logNode =>
                    {
                        var spritedElement = new XElement("Sprited", logNode.Select(LogNodeToXElement));

                        if (logNode.Key.SpritedImage != null)
                        {
                            spritedElement.Add(new XAttribute("SpritedImage", logNode.Key.SpritedImage));
                        }

                        if (logNode.Key.ImageType != null)
                        {
                            spritedElement.Add(new XAttribute("ImageType", logNode.Key.ImageType));
                        }

                        return spritedElement;
                    }))
                .Save(path);
        }

        /// <summary>Creates an xelement for a log node.</summary>
        /// <param name="logNode">The log node.</param>
        /// <returns>The <see cref="XElement"/>.</returns>
        private static XElement LogNodeToXElement(ImageAssemblyAnalysis logNode)
        {
            var logNodeElement = new XElement("SpriteItem", Environment.NewLine + logNode.AstNode.PrettyPrint() + "\t");
            if (logNode.FailureReason != null)
            {
                logNodeElement.Add(new XAttribute("FailureReason", logNode.FailureReason));
                logNodeElement.Add(new XAttribute("FailureMessage", GetFailureMessage(logNode)));
            }

            if (logNode.Image != null)
            {
                logNodeElement.Add(new XAttribute("Image", logNode.Image));
            }

            return logNodeElement;
        }
    }
}