// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssemblyUpdateVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation for ImageAssembly update visitor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Ast;
    using Ast.MediaQuery;
    using Extensions;
    using ImageAssemble;

    using WebGrease.Css.ImageAssemblyAnalysis;
    using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
    using WebGrease.Css.ImageAssemblyAnalysis.PropertyModel;
    using WebGrease.Extensions;

    using ImageAssembleException = WebGrease.Css.ImageAssemblyAnalysis.ImageAssembleException;

    /// <summary>Provides the implementation for ImageAssembly update visitor</summary>
    public class ImageAssemblyUpdateVisitor : NodeVisitor
    {
        /// <summary>
        /// The output unit
        /// </summary>
        private readonly string outputUnit;

        /// <summary>
        /// The output unit factor from 1.
        /// </summary>
        private readonly double outputUnitFactor;

        /// <summary>
        /// The css path
        /// </summary>
        private readonly string cssPath;

        /// <summary>
        /// The input images computed from log file
        /// </summary>
        private readonly IEnumerable<AssembledImage> inputImages;

        /// <summary>
        /// The default DPI to use for the entire stylesheet
        /// </summary>
        private readonly float dpi;

        /// <summary>
        /// The source directory to determine relative from image to css.
        /// </summary>
        private readonly string destinationDirectory;

        /// <summary>The prepend to destination.</summary>
        private readonly string prependToDestination;

        /// <summary>The available source images.</summary>
        private readonly IDictionary<string, string> availableSourceImages;

        private readonly string missingImage;

        /// <summary>Initializes a new instance of the ImageAssemblyUpdateVisitor class</summary>
        /// <param name="cssPath">The css file path which would be used to configure the image path</param>
        /// <param name="imageLogs">The log path which contains the image map after spriting</param>
        /// <param name="dpi">The dpi.</param>
        /// <param name="outputUnit">The output unit </param>
        /// <param name="outputUnitFactor">The output unit factor. </param>
        /// <param name="destinationDirectory">The destination directory</param>
        /// <param name="prependToDestination">Prepend the the relative output path.</param>
        /// <param name="availableSourceImages">The available Source Images.</param>
        /// <param name="missingImage">The missing Image.</param>
        internal ImageAssemblyUpdateVisitor(string cssPath, IEnumerable<ImageLog> imageLogs, float dpi = 1f, string outputUnit = ImageAssembleConstants.Px, double outputUnitFactor = 1, string destinationDirectory = null, string prependToDestination = null, IDictionary<string, string> availableSourceImages = null, string missingImage = null)
        {
            Contract.Requires(imageLogs != null);

            // Output unit and factor
            this.outputUnit = outputUnit;
            this.outputUnitFactor = outputUnitFactor;
            this.destinationDirectory = destinationDirectory;
            this.prependToDestination = prependToDestination;

            this.availableSourceImages = availableSourceImages;
            this.missingImage = missingImage;

            // default dpi is 1
            this.dpi = dpi;

            // Normalize css path
            this.cssPath = cssPath.GetFullPathWithLowercase();

            try
            {
                // Load all input images.
                this.inputImages = imageLogs.SelectMany(i => i.InputImages);
            }
            catch (Exception exception)
            {
                throw new ImageAssembleException(string.Format(CultureInfo.CurrentUICulture, CssStrings.InnerExceptionFile, string.Join(CssConstants.Comma.ToString(CultureInfo.InvariantCulture), imageLogs)), exception);
            }
        }

        /// <summary>The <see cref="StyleSheetNode"/> visit implementation</summary>
        /// <param name="styleSheet">The styleSheet AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
        {
            var updatedStyleSheetRuleNodes = new List<StyleSheetRuleNode>();
            styleSheet.StyleSheetRules.ForEach(styleSheetRuleNode => updatedStyleSheetRuleNodes.Add((StyleSheetRuleNode)styleSheetRuleNode.Accept(this)));
            return new StyleSheetNode(styleSheet.CharSetString, styleSheet.Imports, styleSheet.Namespaces, updatedStyleSheetRuleNodes.AsReadOnly());
        }

        /// <summary>The <see cref="RulesetNode"/> visit implementation</summary>
        /// <param name="rulesetNode">The ruleset AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
        {
            return new RulesetNode(rulesetNode.SelectorsGroupNode, this.UpdateDeclarations(rulesetNode.Declarations, rulesetNode), rulesetNode.ImportantComments);
        }

        /// <summary>The <see cref="MediaNode"/> visit implementation</summary>
        /// <param name="mediaNode">The media AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaNode(MediaNode mediaNode)
        {
            var updatedRulesets = new List<RulesetNode>();
            var updatedPageNodes = new List<PageNode>();
            mediaNode.Rulesets.ForEach(rulesetNode => updatedRulesets.Add((RulesetNode)rulesetNode.Accept(this)));
            mediaNode.PageNodes.ForEach(pageNode => updatedPageNodes.Add((PageNode)pageNode.Accept(this)));

            return new MediaNode(mediaNode.MediaQueries, updatedRulesets.AsReadOnly(), updatedPageNodes.AsReadOnly());
        }

        /// <summary>The <see cref="PageNode"/> visit implementation</summary>
        /// <param name="pageNode">The page AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitPageNode(PageNode pageNode)
        {
            return new PageNode(pageNode.PseudoPage, this.UpdateDeclarations(pageNode.Declarations, pageNode));
        }

        /// <summary>Update the declarations </summary>
        /// <param name="declarationNodes">The list of declaration nodes</param>
        /// <param name="originalDeclarationNode">The original declaration node</param>
        /// <param name="updatedDeclarationNode">The updated declaration node</param>
        private static void UpdateDeclarations(IList<DeclarationNode> declarationNodes, DeclarationNode originalDeclarationNode, DeclarationNode updatedDeclarationNode)
        {
            declarationNodes[declarationNodes.IndexOf(originalDeclarationNode)] = updatedDeclarationNode;
        }

        /// <summary>Gets the position string.</summary>
        /// <param name="value">The value.</param>
        /// <param name="source">The source.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetPositionString(float? value, Source? source)
        {
            if (source != null)
            {
                switch (source.Value)
                {
                    // these source values ignore any value (there shouldn't be one)
                    case Source.Left:
                        return "left";
                    case Source.Right:
                        return "right";
                    case Source.Top:
                        return "top";
                    case Source.Bottom:
                        return "bottom";
                    case Source.Center:
                        return "center";

                    // these source values should have a value and a specific units
                    case Source.Percentage:
                        return string.Format(CultureInfo.InvariantCulture, "{0}%", value.GetValueOrDefault());
                    case Source.Px:
                        return string.Format(CultureInfo.InvariantCulture, "{0}px", value.GetValueOrDefault());
                    case Source.Rem:
                        return string.Format(CultureInfo.InvariantCulture, "{0}rem", value.GetValueOrDefault());
                    case Source.Em:
                        return string.Format(CultureInfo.InvariantCulture, "{0}em", value.GetValueOrDefault());

                    // this source has no units, so there better be a value
                    case Source.NoUnits:
                        return value == null ? string.Empty : value.Value.ToString(CultureInfo.InvariantCulture);

                    case Source.Unknown:
                    default:
                        // unknown source - format it as best we can
                        return (value != null ? value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty)
                            + source.Value.ToString();
                }
            }

            // source is null -- just use the value
            // default to "center" if both the source and the value are null.
            // (we know we should use center because if both x and y weren't specified, we won't be called.)
            return value == null ? "center" : value.Value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Create a declaration comment that indicates the original position of the background image from the source
        /// </summary>
        /// <param name="xPosition">x position</param>
        /// <param name="xSource">x position source</param>
        /// <param name="yPosition">y position</param>
        /// <param name="ySource">y position source</param>
        /// <returns>new declaration node</returns>
        private static DeclarationNode CreateDebugOriginalPositionComment(float? xPosition, Source? xSource, float? yPosition, Source? ySource)
        {
            var xExists = xPosition != null || xSource != null;
            var yExists = yPosition != null || ySource != null;
            if (!xExists && !yExists)
            {
                // neither exists, defaults are zero
                return CreateDebugDeclarationComment("-wg-original-position", "0 0");
            }

            // at least one -- x or y -- exists. So if one doesn't exist, it will default to center
            return CreateDebugDeclarationComment("-wg-original-position", GetPositionString(xPosition, xSource) + " " + GetPositionString(yPosition, ySource));
        }

        /// <summary>
        /// Create a declaration comment that indicates the absolute pixel position of the source image in the generated sprite
        /// </summary>
        /// <param name="xPixels">x position in pixels</param>
        /// <param name="yPixels">y position in pixels</param>
        /// <returns>new declaration node</returns>
        private static DeclarationNode CreateDebugSpritePositionComment(int? xPixels, int? yPixels)
        {
            return CreateDebugDeclarationComment("-wg-sprite-position", Math.Abs(xPixels.GetValueOrDefault()) + "px " + Math.Abs(yPixels.GetValueOrDefault()) + "px");
        }

        /// <summary>
        /// Create a declaration comment that indicates the DPI being used for the sprite calculations
        /// </summary>
        /// <param name="dpi">The dpi</param>
        /// <returns>new declaration node</returns>
        private static DeclarationNode CreateDpiComment(double dpi)
        {
            return CreateDebugDeclarationComment("-wg-background-dpi", dpi.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Create a debug declaration comment: /* property: propertyValue; */
        /// </summary>
        /// <param name="propertyName">name of the property</param>
        /// <param name="propertyValue">value of the property</param>
        /// <returns>new declaration node</returns>
        private static DeclarationNode CreateDebugDeclarationComment(string propertyName, string propertyValue)
        {
            return new DeclarationNode("/* " + propertyName, new ExprNode(new TermNode(string.Empty, null, propertyValue + "; */", null, null, null), null, null), string.Empty, null);
        }

        /// <summary>Updates the list of declarations with the updated value from
        /// image assembly log.
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
        /// <param name="declarationNodes">The list of declarations</param>
        /// <param name="parent">The parent AST node</param>
        /// <returns>The list of updated declarations</returns>
        private ReadOnlyCollection<DeclarationNode> UpdateDeclarations(ReadOnlyCollection<DeclarationNode> declarationNodes, AstNode parent)
        {
            try
            {
                // Populate the nodes from AST extension
                Background backgroundNode;
                BackgroundImage backgroundImageNode;
                BackgroundPosition backgroundPositionNode;
                DeclarationNode backgroundSizeNode;

                // There is no background node found in set of declarations, return without any change
                if (!declarationNodes.TryGetBackgroundDeclaration(parent, out backgroundNode, out backgroundImageNode, out backgroundPositionNode, out backgroundSizeNode, null, null, null, this.outputUnit, this.outputUnitFactor))
                {
                    // No change, return the original collection
                    return declarationNodes;
                }

                // At this point, there should be atleast one "background" or "background-image" node found.
                // In addition, there can be an optional "background-position" node
                // Initialize a cloned set of declarations (The original AST collection is immutable by design)
                var updatedDeclarations = new List<DeclarationNode>(declarationNodes);

                if (this.dpi != 1f)
                {
                    // not a normal-DPI (1x) -- output a comment at the top stating what our DPI really is
                    updatedDeclarations.Insert(0, CreateDpiComment(this.dpi));
                }

                // Empty object to be discovered from log
                AssembledImage assembledImage;

                if (backgroundNode != null)
                {
                    // Short hand declaration found:
                    // #selector
                    // {
                    // background: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif) no-repeat -10px -200px;
                    // }
                    // Query the log file and see if there is an assembled node for 
                    // the background image
                    if (!this.TryGetAssembledImage(backgroundNode.Url, backgroundNode.BackgroundPosition, out assembledImage))
                    {
                        return declarationNodes;
                    }

                    // add some comments that we can use to help debugging
                    updatedDeclarations.Insert(0, CreateDebugOriginalPositionComment(backgroundNode.BackgroundPosition.X, backgroundNode.BackgroundPosition.XSource, backgroundNode.BackgroundPosition.Y, backgroundNode.BackgroundPosition.YSource));
                    updatedDeclarations.Insert(0, CreateDebugSpritePositionComment(assembledImage.X, assembledImage.Y));

                    // Update the declaration node with new values in AST
                    var updatedDeclaration = backgroundNode.UpdateBackgroundNode(assembledImage.RelativeOutputFilePath, assembledImage.X, assembledImage.Y, this.dpi);

                    // Update the declaration list
                    UpdateDeclarations(updatedDeclarations, backgroundNode.DeclarationAstNode, updatedDeclaration);

                    this.SetBackgroundSize(updatedDeclarations, backgroundSizeNode, this.dpi, assembledImage);
                }
                else if (backgroundImageNode != null)
                {
                    // Long declaration found for background-image:
                    // #selector
                    // {
                    // background-image: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif);
                    // }
                    // Query the log file and see if there is an assembled node for 
                    // the background image
                    if (!this.TryGetAssembledImage(backgroundImageNode.Url, backgroundPositionNode, out assembledImage))
                    {
                        // Return without update with a group of declarations
                        return declarationNodes;
                    }

                    // Update the declaration node with new values in AST
                    var updatedDeclaration = backgroundImageNode.UpdateBackgroundImageNode(assembledImage.RelativeOutputFilePath);

                    // Update the list of declarations
                    UpdateDeclarations(updatedDeclarations, backgroundImageNode.DeclarationNode, updatedDeclaration);

                    if (backgroundPositionNode != null)
                    {
                        // Long declaration found for background-position:
                        // #selector
                        // {
                        // background-position: -10px  -200px;
                        // }
                        // Update the declaration node with new values in AST

                        // add some comments that can help with debugging
                        updatedDeclarations.Insert(0, CreateDebugOriginalPositionComment(backgroundPositionNode.X, backgroundPositionNode.XSource, backgroundPositionNode.Y, backgroundPositionNode.YSource));
                        updatedDeclarations.Insert(0, CreateDebugSpritePositionComment(assembledImage.X, assembledImage.Y));

                        updatedDeclaration = backgroundPositionNode.UpdateBackgroundPositionNode(assembledImage.X, assembledImage.Y, this.dpi);

                        // Update the list of declarations
                        UpdateDeclarations(updatedDeclarations, backgroundPositionNode.DeclarationNode, updatedDeclaration);
                    }
                    else
                    {
                        // If there is no declaration found for "background-position",
                        // Create a new declaration node in AST for "background-position" declaration
                        var newDeclaration = BackgroundPosition.CreateNewDeclaration(assembledImage.X, assembledImage.Y, this.dpi, this.outputUnit, this.outputUnitFactor);

                        // add a comment to help with debugging
                        updatedDeclarations.Insert(0, CreateDebugSpritePositionComment(assembledImage.X, assembledImage.Y));

                        if (newDeclaration != null)
                        {
                            updatedDeclarations.Add(newDeclaration);
                        }
                    }

                    this.SetBackgroundSize(updatedDeclarations, backgroundSizeNode, this.dpi, assembledImage);
                }

                return updatedDeclarations.AsReadOnly();
            }
            catch (Exception exception)
            {
                throw new ImageAssembleException(string.Format(CultureInfo.CurrentUICulture, CssStrings.InnerExceptionSelector, parent.PrettyPrint()), exception);
            }
        }

        /// <summary>
        /// Sets the background-size priority node if the dpi does not equal 1, also replace an existing one if it exists.
        /// </summary>
        /// <param name="updatedDeclarations">The updated declarations.</param>
        /// <param name="backgroundSizeNode">The node containing the possible existing background size.</param>
        /// <param name="dpiFactor">The background dpi.</param>
        /// <param name="assembledImage">The assembled image.</param>
        private void SetBackgroundSize(List<DeclarationNode> updatedDeclarations, DeclarationNode backgroundSizeNode, float dpiFactor, AssembledImage assembledImage)
        {
            if (backgroundSizeNode != null)
            {
                updatedDeclarations.Remove(backgroundSizeNode);
            }

            // if the DPI factor is not 1.0, then we need to output a properly-scaled background-size property, 
            // regardless of the the output units we want (if we don't specify an output unit, pixels is used).
            // but if we specify an output unit, we need to output the background-size property even if the DPI is 1.0,
            // in order to properly handle the browser default font-size change functionality.
            if ((dpiFactor != 1f || this.outputUnit != null)
                && assembledImage.SpriteHeight.HasValue && assembledImage.SpriteWidth.HasValue)
            {
                updatedDeclarations.AddRange(this.CreateBackgroundSizeNode(assembledImage, dpiFactor));
            }
        }

        /// <summary>
        /// Returns the background size node with the correctly adjusted values for dpi and output units.
        /// </summary>
        /// <param name="assembledImage">The assembled image.</param>
        /// <param name="dpiFactor">The background dpi.</param>
        /// <returns>The background-size node.</returns>
        private IEnumerable<DeclarationNode> CreateBackgroundSizeNode(AssembledImage assembledImage, float dpiFactor)
        {
            var calcWidth = (float?)Math.Round((assembledImage.SpriteWidth ?? 0f) * this.outputUnitFactor / dpiFactor, 3);
            var calcHeight = (float?)Math.Round((assembledImage.SpriteHeight ?? 0f) * this.outputUnitFactor / dpiFactor, 3);

            var widthTermNode = new TermNode(
                calcWidth.UnaryOperator(),
                calcWidth.CssUnitValue(this.outputUnit),
                null,
                null,
                null,
                null);

            var heightTermNode = new TermNode(
                calcHeight.UnaryOperator(),
                calcHeight.CssUnitValue(this.outputUnit),
                null,
                null,
                null,
                null);

            var termWithOperatorNodes = new List<TermWithOperatorNode>
                {
                    new TermWithOperatorNode(ImageAssembleConstants.SingleSpace, heightTermNode)
                };

            var newBackgroundSizeNode =
                new DeclarationNode(
                    ImageAssembleConstants.BackgroundSize,
                    new ExprNode(
                        widthTermNode,
                        termWithOperatorNodes.ToSafeReadOnlyCollection(), null),
                        null,
                        null);

            return new[] 
            { 
                // add a comment to help with sprite debugging
                CreateDebugDeclarationComment("-wg-background-size-params", " (sprite size: " + assembledImage.SpriteWidth + "px " + assembledImage.SpriteHeight + "px) (output unit factor: " + this.outputUnitFactor + ") (dpi: " + dpiFactor + ") (imageposition:" + assembledImage.ImagePosition + ")"),
                newBackgroundSizeNode
            };
        }

        /// <summary>Gets the assembled image information from the dictionary</summary>
        /// <param name="parsedImagePath">The parsed url</param>
        /// <param name="backgroundPosition">The background position node</param>
        /// <param name="assembledImage">The assembled image path</param>
        /// <returns>The assembled image object</returns>
        private bool TryGetAssembledImage(string parsedImagePath, BackgroundPosition backgroundPosition, out AssembledImage assembledImage)
        {
            if (this.availableSourceImages == null && string.IsNullOrWhiteSpace(this.cssPath))
            {
                throw new BuildWorkflowException("Need either images or css path to be able to set a valid image file.");
            }

            assembledImage = null;

            if (this.inputImages == null)
            {
                return false;
            }

            // TODO - Spec. issue: What are the supported path formats supported in CSS?
            // For optimization, should the relative paths be enforced?

            // Get the full path of parsed image file (convert from ../../ to absolute path)
            if (parsedImagePath.StartsWith("hash://", StringComparison.OrdinalIgnoreCase))
            {
                parsedImagePath = parsedImagePath.Substring(7);
            }

            if (this.availableSourceImages != null)
            {
                if (!this.availableSourceImages.TryGetValue(parsedImagePath.NormalizeUrl(), out parsedImagePath))
                {
                    if (!string.IsNullOrWhiteSpace(this.missingImage))
                    {
                        parsedImagePath = this.availableSourceImages.TryGetValue(this.missingImage);
                    }
                }
            }
            else
            {
                parsedImagePath = parsedImagePath.MakeAbsoluteTo(this.cssPath);
            }

            var imagePosition = ImagePosition.Left;
            if (backgroundPosition != null)
            {
                imagePosition = backgroundPosition.GetImagePositionInVerticalSprite();
            }

            // Try to locate the input image in the list
            assembledImage = this.inputImages.FirstOrDefault(inputImage => inputImage.ImagePosition == imagePosition && inputImage.OriginalFilePath.Equals(parsedImagePath, StringComparison.OrdinalIgnoreCase));

            if (assembledImage != null &&
                assembledImage.OutputFilePath != null)
            {
                assembledImage.RelativeOutputFilePath =
                    !string.IsNullOrWhiteSpace(this.destinationDirectory)
                        ? Path.Combine(this.prependToDestination, assembledImage.OutputFilePath.MakeRelativeToDirectory(this.destinationDirectory).Replace('\\', '/'))
                        : assembledImage.OutputFilePath.MakeRelativeTo(this.cssPath);

                if (!string.IsNullOrWhiteSpace(assembledImage.RelativeOutputFilePath))
                {
                    assembledImage.RelativeOutputFilePath = assembledImage.RelativeOutputFilePath.Replace('\\', '/');
                }

                return true;
            }

            return false;
        }
    }
}