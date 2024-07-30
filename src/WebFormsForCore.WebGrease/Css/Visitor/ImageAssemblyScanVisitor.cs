// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssemblyScanVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation for ImageAssembly log visitor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Ast;
    using Ast.MediaQuery;
    using Extensions;
    using ImageAssemble;
    using ImageAssemblyAnalysis;
    using ImageAssemblyAnalysis.LogModel;
    using ImageAssemblyAnalysis.PropertyModel;

    using WebGrease.Extensions;

    using ImageAssembleException = ImageAssemblyAnalysis.ImageAssembleException;
    using WebGrease.Activities;

    /// <summary>Provides the implementation for ImageAssembly log visitor</summary>
    public sealed class ImageAssemblyScanVisitor : NodeVisitor
    {
        /// <summary>
        /// The value used to determine if the visitor should ignore images that have a non-default background-size value set.
        /// </summary>
        private readonly bool _ignoreImagesWithNonDefaultBackgroundSize;

        /// <summary>
        /// Output unit to use for image spriting (default will be "px")
        /// </summary>
        private readonly string outputUnit;

        /// <summary>
        /// Output unit factor to use for image spriting (default is 1)
        /// </summary>
        private readonly double outputUnitFactor;

        /// <summary>
        /// The css path
        /// </summary>
        private readonly string _cssPath;

        /// <summary>The _missing image url.</summary>
        private readonly string _missingImage;

        /// <summary>
        /// The default image asssembly scan output.
        /// </summary>
        private readonly ImageAssemblyScanOutput _defaultImageAssemblyScanOutput = new ImageAssemblyScanOutput();

        /// <summary>
        /// The list of image references populated from AST for background images
        /// declarations which does not meet the criteria of image assembly
        /// </summary>
        private readonly ImageAssemblyAnalysisLog _imageAssemblyAnalysisLog = new ImageAssemblyAnalysisLog();

        /// <summary>
        /// The list of image assembly outputs
        /// </summary>
        private readonly IList<ImageAssemblyScanOutput> _imageAssemblyScanOutputs = new List<ImageAssemblyScanOutput>();

        /// <summary>
        /// The list of image references which should be ignored
        /// while scanning the AST
        /// </summary>
        private readonly HashSet<string> _imageReferencesToIgnore = new HashSet<string>();

        /// <summary>
        /// The root output folder for the images.
        /// </summary>
        private readonly IDictionary<string, string> _availableImageSources;

        /// <summary>
        /// The list of image references populated from AST for background images
        /// declarations which does not meet the criteria of image assembly
        /// </summary>
        private readonly HashSet<string> _imagesCriteriaFailedReferences = new HashSet<string>();

        /// <summary>The image not found throw error.</summary>
        private bool imageNotFoundThrowError;

        internal IWebGreaseContext Context { get; set; }

        /// <summary>
        /// Gets the Default Image Assembly Scan Output.
        /// </summary>
        internal ImageAssemblyScanOutput DefaultImageAssemblyScanOutput
        {
            get { return this._defaultImageAssemblyScanOutput; }
        }

        /// <summary>
        /// Gets the Image Assembly Scan Outputs.
        /// </summary>
        internal IList<ImageAssemblyScanOutput> ImageAssemblyScanOutputs
        {
            get { return this._imageAssemblyScanOutputs; }
        }

        /// <summary>Initializes a new instance of the ImageAssemblyScanVisitor class</summary>
        /// <param name="cssPath">The css file path which would be used to configure the image path</param>
        /// <param name="imageReferencesToIgnore">The list of image references to ignore</param>
        /// <param name="ignoreImagesWithNonDefaultBackgroundSize">The value used to determine if the visitor should ignore images that have a non-default background-size value set. </param>
        /// <param name="outputUnit">The output unit.</param>
        /// <param name="outputUnitFactor">The output unit factor.</param>
        /// <param name="availableImageSources"></param>
        /// <param name="missingImage">The missing Image Url.</param>
        public ImageAssemblyScanVisitor(string cssPath, IEnumerable<string> imageReferencesToIgnore, bool ignoreImagesWithNonDefaultBackgroundSize = false, string outputUnit = ImageAssembleConstants.Px, double outputUnitFactor = 1d, IDictionary<string, string> availableImageSources = null, string missingImage = null, bool imageNotFoundThrowError = false)
        {
            Contract.Requires(availableImageSources != null || !string.IsNullOrWhiteSpace(cssPath));

            this._missingImage = missingImage;
            this.imageNotFoundThrowError = imageNotFoundThrowError;

            // Set the image output root.
            this._availableImageSources = availableImageSources;

            // Add the scan outputs
            this._imageAssemblyScanOutputs.Add(_defaultImageAssemblyScanOutput);

            // Normalize css path
            this._cssPath = cssPath.GetFullPathWithLowercase();

            // Determine if the visitor should ignore images that have a non-default background-size value set.
            this._ignoreImagesWithNonDefaultBackgroundSize = ignoreImagesWithNonDefaultBackgroundSize;
            this.outputUnit = outputUnit;
            this.outputUnitFactor = outputUnitFactor;

            if (imageReferencesToIgnore != null)
            {
                // Normalize the image references paths to ignore
                imageReferencesToIgnore.ForEach(imageReferenceToIgnore => this._imageReferencesToIgnore.Add(imageReferenceToIgnore.NormalizeUrl()));
            }
        }

        /// <summary>Gets the Image Assembly Analysis Log.</summary>
        public ImageAssemblyAnalysisLog ImageAssemblyAnalysisLog
        {
            get { return _imageAssemblyAnalysisLog; }
        }

        /// <summary>The <see cref="StyleSheetNode"/> visit implementation</summary>
        /// <param name="styleSheet">The styleSheet AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
        {
            _imagesCriteriaFailedReferences.Clear();
            styleSheet.StyleSheetRules.ForEach(styleSheetRuleNode => styleSheetRuleNode.Accept(this));
            return styleSheet;
        }

        /// <summary>The <see cref="RulesetNode"/> visit implementation</summary>
        /// <param name="rulesetNode">The ruleset AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
        {
            this.VisitBackgroundDeclarationNode(rulesetNode.Declarations, rulesetNode);
            return rulesetNode;
        }

        /// <summary>The <see cref="MediaNode"/> visit implementation</summary>
        /// <param name="mediaNode">The media AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaNode(MediaNode mediaNode)
        {
            mediaNode.Rulesets.ForEach(rulesetNode => rulesetNode.Accept(this));
            mediaNode.PageNodes.ForEach(pageNode => pageNode.Accept(this));
            return mediaNode;
        }

        /// <summary>The <see cref="PageNode"/> visit implementation</summary>
        /// <param name="pageNode">The page AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitPageNode(PageNode pageNode)
        {
            this.VisitBackgroundDeclarationNode(pageNode.Declarations, pageNode);
            return pageNode;
        }

        /// <summary>The <see cref="TermWithOperatorNode"/> visit implementation</summary>
        /// <param name="termWithOperatorNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode)
        {
            termWithOperatorNode.TermNode.Accept(this);

            return termWithOperatorNode;
        }

        /// <summary>Visits the background declaration node
        /// Example Css with shorthand declaration:
        /// #selector
        /// {
        ///   background: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif) no-repeat -10px -200px;
        /// }
        /// Example Css with long declarations:
        /// #selector
        /// {
        ///   background-repeat: no-repeat;
        ///   background-position: -10px  -200px;
        ///   background-image: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif);
        /// }</summary>
        /// <param name="declarations">The list of declarations</param>
        /// <param name="parent">The parent AST node</param>
        private void VisitBackgroundDeclarationNode(IEnumerable<DeclarationNode> declarations, AstNode parent)
        {
            try
            {
                // There should be 0 or 1 declaration nodes in a set of declarations which
                // should be either "background" or "background-image". Both shorthand and
                // specific declaration are not allowed in a set of declarations.
                Background backgroundNode;
                BackgroundImage backgroundImageNode;
                BackgroundPosition backgroundPositionNode;
                DeclarationNode backgroundSize;

                var imagesCriteriaFailedUrls = new List<string>();

                if (!declarations.TryGetBackgroundDeclaration(// For image path/logging etc
                    parent, // For printing the Pretty Print Node for logging
                    out backgroundNode,
                    out backgroundImageNode,
                    out backgroundPositionNode,
                    out backgroundSize,
                    imagesCriteriaFailedUrls, // Images which don't pass the spriting criteria
                    _imageReferencesToIgnore, // Images which should not be considered for spriting
                    _imageAssemblyAnalysisLog,
                    this.outputUnit,
                    this.outputUnitFactor,
                    _ignoreImagesWithNonDefaultBackgroundSize))
                {
                    // Store the list of failed urls
                    imagesCriteriaFailedUrls.ForEach(imagesCriteriaFailedUrl =>
                    {
                        var url = imagesCriteriaFailedUrl.NormalizeUrl().MakeAbsoluteTo(_cssPath);

                        // Throw an exception if image has passed the criteria in past and now
                        // now fails the criteria
                        if (_imageAssemblyScanOutputs.Any(imageAssemblyScanOutput => imageAssemblyScanOutput.ImageReferencesToAssemble.Where(imageReference => imageReference.AbsoluteImagePath == url).Any()))
                        {
                            throw new ImageAssembleException(string.Format(CultureInfo.CurrentUICulture, CssStrings.DuplicateImageReferenceWithDifferentRulesError, url));
                        }

                        _imagesCriteriaFailedReferences.Add(url);
                    });

                    return;
                }

                if (backgroundNode != null)
                {
                    // Short hand declaration found:
                    // #selector
                    // {
                    // background: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif) no-repeat -10px -200px;
                    // }
                    // Add the url and target image position to found list
                    this.AddImageReference(backgroundNode.Url, backgroundNode.BackgroundPosition);
                }
                else if (backgroundImageNode != null && backgroundPositionNode != null)
                {
                    // Long declaration found for background-image:
                    // #selector
                    // {
                    // background-image: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif);
                    // background-position: -10px -200px;
                    // }
                    // Add the url and target image position to found list
                    this.AddImageReference(backgroundImageNode.Url, backgroundPositionNode);
                }
            }
            catch (Exception exception)
            {
                throw new ImageAssembleException(string.Format(CultureInfo.CurrentUICulture, CssStrings.InnerExceptionSelector, parent.PrettyPrint()), exception);
            }
        }

        /// <summary>Adds the url in list which would be passed to the
        /// image assembly tool for concatenation</summary>
        /// <param name="url">The url for sprite candidate image</param>
        /// <param name="backgroundPosition">THe background position</param>
        private void AddImageReference(string url, BackgroundPosition backgroundPosition)
        {
            var relativeUrl = url.NormalizeUrl();
            var originalUrl = url;

            // as long as there is token.
            if (ResourcesResolver.LocalizationResourceKeyRegex.IsMatch(relativeUrl))
            {
                // ignore tokenized urls. this is having assumption that build time tokenization must be done before spriting.
                return;
            }

            url = this.GetAbsoluteImagePath(relativeUrl);

            // No need to report the url if it is present in ignore list
            if (this._imageReferencesToIgnore.Contains(relativeUrl) || this._imageReferencesToIgnore.Contains(Path.GetDirectoryName(relativeUrl) + "\\*"))
            {
                return;
            }

            if (this._imagesCriteriaFailedReferences.Any(ir => ir.Equals(url, StringComparison.OrdinalIgnoreCase)))
            {
                // Throw an exception if image has failed the criteria in past and now
                // now passes the criteria
                throw new ImageAssembleException(string.Format(CultureInfo.CurrentUICulture, CssStrings.DuplicateImageReferenceWithDifferentRulesError, url));
            }

            var imagePosition = backgroundPosition.GetImagePositionInVerticalSprite();
            var added = false;

            ////
            //// Try adding the image in the non default scan output
            ////
            for (var count = 1; count < this._imageAssemblyScanOutputs.Count; count++)
            {
                var imageAssemblyScanOutput = this._imageAssemblyScanOutputs[count];
                if (!imageAssemblyScanOutput.ImageAssemblyScanInput.ImagesInBucket.Contains(url))
                {
                    continue;
                }

                // Make sure that image don't exist already in list
                if (imageAssemblyScanOutput.ImageReferencesToAssemble.Any(inputImage => inputImage.AbsoluteImagePath == url && inputImage.Position == imagePosition))
                {
                    continue;
                }

                imageAssemblyScanOutput.ImageReferencesToAssemble.Add(new InputImage { AbsoluteImagePath = url, Position = imagePosition, OriginalImagePath = originalUrl });
                added = true;
            }

            if (added)
            {
                return;
            }

            ////
            //// Add the image in the default scan output
            ////
            if (this._defaultImageAssemblyScanOutput.ImageReferencesToAssemble.Any(inputImage => inputImage.AbsoluteImagePath == url && inputImage.Position == imagePosition))
            {
                return;
            }

            this._defaultImageAssemblyScanOutput.ImageReferencesToAssemble.Add(new InputImage { AbsoluteImagePath = url, Position = imagePosition, OriginalImagePath = originalUrl });
        }

        /// <summary>Gets the absolute image path.</summary>
        /// <param name="relativeUrl">The relative url.</param>
        /// <returns>The absolute image path.</returns>
        private string GetAbsoluteImagePath(string relativeUrl)
        {
            string url;

            if (this._availableImageSources != null)
            {
                var sourceFile = this._availableImageSources.ContainsKey(relativeUrl) ? this._availableImageSources[relativeUrl] : null;

                url = sourceFile;
            }
            else
            {
                url = relativeUrl.MakeAbsoluteTo(this._cssPath);
            }

            if (string.IsNullOrWhiteSpace(url) || !File.Exists(url))
            {
                var isMissingImageUrl = relativeUrl.Equals(this._missingImage);
                if (!string.IsNullOrWhiteSpace(this._missingImage) && !isMissingImageUrl)
                {
                    url = this.GetAbsoluteImagePath(this._missingImage);
                }
                else
                {
                    if (this.imageNotFoundThrowError)
                    {
                        throw new FileNotFoundException("Could not find the image file:" + relativeUrl + " (" + url + ")", url ?? string.Empty);
                    }
                }
            }

            return url;
        }
    }
}
