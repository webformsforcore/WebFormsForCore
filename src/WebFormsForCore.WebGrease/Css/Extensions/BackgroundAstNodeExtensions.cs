// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundAstNodeExtensions.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   BackgroundAstNodeExtensions Class - Provides the extension on AstNode types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ast;
    using ImageAssemblyAnalysis;
    using ImageAssemblyAnalysis.LogModel;
    using ImageAssemblyAnalysis.PropertyModel;

    /// <summary>BackgroundAstNodeExtensions Class - Provides the extension on AstNode types</summary>
    public static class BackgroundAstNodeExtensions
    {
        /// <summary>Finds the background declaration which satisfies these rules:
        /// 1. "background" or "background-repeat" declaration is present
        /// 2. The declarations have "no-repeat" configured
        /// 3. The position should have only px units or no units or top/left
        /// 4. Both the long and short-hand declarations are not simultaneously used.
        /// <para>
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
        /// }
        /// The CSS 2.1 grammar for a declaration is:
        /// declaration
        ///  : property ':' S* expr prio?
        ///  ;
        /// property
        /// : IDENT S*
        /// ;
        /// expr
        ///  : term [ operator? term ]*
        ///  ;
        /// term
        ///  : unary_operator?
        ///    [ NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* |
        ///      TIME S* | FREQ S* ]
        ///  | STRING S* | IDENT S* | URI S* | hexcolor | function
        ///  ;
        /// function
        ///  : FUNCTION S* expr ')' S*
        ///  ;</para>
        /// </summary>
        /// <param name="declarationAstNodes">The list of declarations</param>
        /// <param name="parentAstNode">The parent of list of declarations</param>
        /// <param name="backgroundNode">The out "backgound" node</param>
        /// <param name="backgroundImageNode">The out "background-image" node</param>
        /// <param name="backgroundPositionNode">The out "background-position" node</param>
        /// <param name="backgroundSize">The background size </param>
        /// <param name="imageReferencesInInvalidDeclarations">The list of urls which are valid but could not pass the other conditions in a list of declarations</param>
        /// <param name="imageReferencesToIgnore">The urls which should be igoned while scan</param>
        /// <param name="imageAssemblyAnalysisLog">The logging object</param>
        /// <param name="outputUnit">The output unit</param>
        /// <param name="outputUnitFactor">The output unit factor.</param>
        /// <param name="ignoreImagesWithNonDefaultBackgroundSize">Determines whether to ignore images that have a non-default background image set.</param>
        /// <returns>The declaration node which matches the criteria</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Needs refactoring in a later release.")]
        internal static bool TryGetBackgroundDeclaration(this IEnumerable<DeclarationNode> declarationAstNodes, AstNode parentAstNode, out Background backgroundNode, out BackgroundImage backgroundImageNode, out BackgroundPosition backgroundPositionNode, out DeclarationNode backgroundSize, List<string> imageReferencesInInvalidDeclarations, HashSet<string> imageReferencesToIgnore, ImageAssemblyAnalysisLog imageAssemblyAnalysisLog, string outputUnit, double outputUnitFactor, bool ignoreImagesWithNonDefaultBackgroundSize = false)
        {
            // Initialize the nodes to null
            backgroundNode = null;
            backgroundImageNode = null;
            backgroundPositionNode = null;
            backgroundSize = null;

            // With CSS3 multiple urls can be present in a single rule, this is not yet supported
            // background: url(flower.png), url(ball.png), url(grass.png) no-repeat;
            if (BackgroundImage.HasMultipleUrls(parentAstNode.MinifyPrint()))
            {
                imageAssemblyAnalysisLog.SafeAdd(parentAstNode, null, FailureReason.MultipleUrls);
                return false;
            }

            // This will check if the selector hash -wg-spriting: ignore. If it does we will ignore the sprite.
            var webGreaseSpritingProperty = declarationAstNodes.FirstOrDefault(d => d.Property == "-wg-spriting");
            if (webGreaseSpritingProperty != null && webGreaseSpritingProperty.ExprNode.TermNode.StringBasedValue == "ignore")
            {
                imageAssemblyAnalysisLog.SafeAdd(parentAstNode, null, FailureReason.SpritingIgnore);
                return false;
            }

            // The list of declarations should not have the duplicate declaration
            // properties. Validate and get the dictionary of declarations.
            var declarationProperties = declarationAstNodes.LoadDeclarationPropertiesDictionary();

            // The selector design is inefficient if the shorthand notation and long name is defined
            // in a scope of ruleset declaration, media ruleset declaration or page declaration. This
            // would be a great feature to add to Optimization visitor but would require a full table
            // scanning which is not yet implemented.
            // Per MSN CSS standards, the practice of defining the shorthand notation and long name
            // in scope of same selector is not allowed.
            DeclarationNode declarationAstNode;

            if (declarationProperties.TryGetValue(ImageAssembleConstants.Background, out declarationAstNode))
            {
                // There should not be any short and long notation simultaneosuly used in these set of declarations.
                // For example: Such a list of declarations end up in inefficient CSS.
                // #selector
                // {
                // background:url(foo.gif);
                // background-image:url(../../i/D5/DF5D9B4EFD5CFF9122942A67A1EEC5.gif);
                // background-position:500px 500px;
                // background-repeat:no-repeat
                // }
                // By design, we are not computing cascade here.
                // TODO: RTUIT: Add support to override some of these with extra values depending on the order, or use optimization to do this when merging styles.
                if (declarationProperties.ContainsKey(ImageAssembleConstants.BackgroundRepeat) ||
                    declarationProperties.ContainsKey(ImageAssembleConstants.BackgroundImage) ||
                    declarationProperties.ContainsKey(ImageAssembleConstants.BackgroundPosition))
                {
                    throw new ImageAssembleException(CssStrings.DuplicateBackgroundFormatError);
                }

                // Load the model for the "background" declaration
                var parsedBackground = new Background(declarationAstNode, outputUnit, outputUnitFactor);

                ////
                //// The url should be present
                ////
                bool shouldIgnore;
                if (!parsedBackground.BackgroundImage.VerifyBackgroundUrl(parentAstNode, imageReferencesToIgnore, imageAssemblyAnalysisLog, out shouldIgnore) ||
                    shouldIgnore)
                {
                    return false;
                }

                ////
                //// The "no-repeat" term should be explicitly configured on the "background" declaration
                ////
                if (!parsedBackground.BackgroundRepeat.VerifyBackgroundNoRepeat())
                {
                    imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackground.Url, FailureReason.BackgroundRepeatInvalid);
                    UpdateFailedUrlsList(parsedBackground.Url, imageReferencesInInvalidDeclarations);
                    return false;
                }

                ////
                //// The background position should only be empty, x = any value and y = 0, top or px
                ////
                if (!parsedBackground.BackgroundPosition.IsVerticalSpriteCandidate())
                {
                    imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackground.Url, FailureReason.IncorrectPosition);
                    UpdateFailedUrlsList(parsedBackground.Url, imageReferencesInInvalidDeclarations);
                    return false;
                }

                //// Try to get the background size, returns false if we want to ignore images with background sizes and the background size is set to a non-default value.
                if (!TryGetBackgroundSize(ignoreImagesWithNonDefaultBackgroundSize, declarationProperties, out backgroundSize))
                {
                    imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackground.Url, FailureReason.BackgroundSizeIsSetToNonDefaultValue);
                    UpdateFailedUrlsList(parsedBackground.Url, imageReferencesInInvalidDeclarations);
                    return false;
                }

                backgroundNode = parsedBackground;
                imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackground.Url);

                //// SUCCESS - This is the candidate!
                return true;
            }

            // Now there should be declaration for "background-image" or "background-repeat" or both
            if (declarationProperties.TryGetValue(ImageAssembleConstants.BackgroundImage, out declarationAstNode))
            {
                // Load the property model for the "background-image" declaration
                var parsedBackgroundImage = new BackgroundImage(declarationAstNode);

                ////
                //// The url should be present
                ////
                bool shouldIgnore;
                if (!parsedBackgroundImage.VerifyBackgroundUrl(parentAstNode, imageReferencesToIgnore, imageAssemblyAnalysisLog, out shouldIgnore) ||
                    shouldIgnore)
                {
                    return false;
                }

                ////
                //// There is a "background-repeat" declaration found
                ////
                DeclarationNode backgroundRepeat;
                if (!declarationProperties.TryGetValue(ImageAssembleConstants.BackgroundRepeat, out backgroundRepeat))
                {
                    imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackgroundImage.Url, FailureReason.NoRepeat);
                    UpdateFailedUrlsList(parsedBackgroundImage.Url, imageReferencesInInvalidDeclarations);
                    return false;
                }

                ////
                //// Now make sure that "background-repeat" is "no-repeat"
                ////
                if (!new BackgroundRepeat(backgroundRepeat).VerifyBackgroundNoRepeat())
                {
                    imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackgroundImage.Url, FailureReason.BackgroundRepeatInvalid);
                    UpdateFailedUrlsList(parsedBackgroundImage.Url, imageReferencesInInvalidDeclarations);
                    return false;
                }

                //// Try to get the background size, returns false if we want to ignore images with background sizes and the background size is set to a non-default value.
                if (!TryGetBackgroundSize(ignoreImagesWithNonDefaultBackgroundSize, declarationProperties, out backgroundSize))
                {
                    imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackgroundImage.Url, FailureReason.BackgroundSizeIsSetToNonDefaultValue);
                    UpdateFailedUrlsList(parsedBackgroundImage.Url, imageReferencesInInvalidDeclarations);
                    return false;
                }

                ////
                //// The background position should only be empty, x = any value and y = 0, top or px
                ////
                DeclarationNode backgroundPosition;
                if (declarationProperties.TryGetValue(ImageAssembleConstants.BackgroundPosition, out backgroundPosition))
                {
                    // Now if there is a "background-position" declaration (optional), lets make 
                    // it should only be empty, px or left/top or right/px
                    var parsedBackgroundPosition = new BackgroundPosition(backgroundPosition, outputUnit, outputUnitFactor);

                    if (!parsedBackgroundPosition.IsVerticalSpriteCandidate())
                    {
                        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackgroundImage.Url, FailureReason.IncorrectPosition);
                        UpdateFailedUrlsList(parsedBackgroundImage.Url, imageReferencesInInvalidDeclarations);
                        return false;
                    }

                    backgroundImageNode = parsedBackgroundImage;
                    backgroundPositionNode = parsedBackgroundPosition;
                    imageAssemblyAnalysisLog.SafeAdd(parentAstNode, parsedBackgroundImage.Url);

                    //// SUCCESS - This is the candidate!
                    return true;
                }

                backgroundImageNode = parsedBackgroundImage;
                imageAssemblyAnalysisLog.SafeAdd(parentAstNode, backgroundImageNode.Url);

                //// SUCCESS - This is the candidate!
                return true;
            }

            return false;
        }

        /// <summary>The add to analysis log.</summary>
        /// <param name="imageAssemblyAnalysisLog">The image assembly analysis log.</param>
        /// <param name="parentAstNode">The parent ast node.</param>
        /// <param name="image">The image.</param>
        /// <param name="failureReason">The failure reason.</param>
        internal static void SafeAdd(this ImageAssemblyAnalysisLog imageAssemblyAnalysisLog, AstNode parentAstNode, string image = null, FailureReason? failureReason = null)
        {
            if (imageAssemblyAnalysisLog != null)
            {
                imageAssemblyAnalysisLog.Add(new ImageAssemblyAnalysis { AstNode = parentAstNode, Image = image, FailureReason = failureReason });
            }
        }

        /// <summary>The enumerable for declaration node</summary>
        /// <param name="declarationNode">The declaration node</param>
        /// <returns>The term node</returns>
        internal static IEnumerable<TermWithOperatorNode> DeclarationEnumerator(this DeclarationNode declarationNode)
        {
            if (declarationNode == null)
            {
                yield break;
            }

            // The primary term
            yield return new TermWithOperatorNode(ImageAssembleConstants.SingleSpace, declarationNode.ExprNode.TermNode);

            // The children term with operators
            foreach (var termWithOperatorNode in declarationNode.ExprNode.TermsWithOperators)
            {
                yield return termWithOperatorNode;
            }
        }

        /// <summary>Creates a copy of term node</summary>
        /// <param name="termNode">The original term node</param>
        /// <returns>The new term node</returns>
        internal static TermNode CopyTerm(this TermNode termNode)
        {
            return termNode == null ? null : new TermNode(termNode.UnaryOperator, termNode.NumberBasedValue, termNode.StringBasedValue, termNode.Hexcolor, termNode.FunctionNode, termNode.ImportantComments);
        }

        /// <summary>Creates a declaration node from list of term with operator nodes</summary>
        /// <param name="declarationNode">The original declaration node</param>
        /// <param name="termWithOperatorNodes">The list of term with operator nodes</param>
        /// <returns>The new declaration node</returns>
        internal static DeclarationNode CreateDeclarationNode(this DeclarationNode declarationNode, List<TermWithOperatorNode> termWithOperatorNodes)
        {
            if (declarationNode == null || termWithOperatorNodes == null || termWithOperatorNodes.Count <= 0)
            {
                return declarationNode;
            }

            var primaryTerm = termWithOperatorNodes[0].TermNode;
            termWithOperatorNodes.RemoveAt(0);

            return new DeclarationNode(declarationNode.Property, new ExprNode(primaryTerm, termWithOperatorNodes.AsReadOnly(), null), declarationNode.Prio, declarationNode.ImportantComments);
        }

        /// <summary>
        /// Get the background-size declaration for this rule, if there is one
        /// </summary>
        /// <param name="ignoreImagesWithNonDefaultBackgroundSize">if true, ignore background-size declarations where the size value is auto</param>
        /// <param name="declarationProperties">collection of declarations</param>
        /// <param name="backgroundSize">background-size declaration to return</param>
        /// <returns>true if successful; false otherwise (or if ignored)</returns>
        private static bool TryGetBackgroundSize(bool ignoreImagesWithNonDefaultBackgroundSize, IDictionary<string, DeclarationNode> declarationProperties, out DeclarationNode backgroundSize)
        {
            ////
            //// The background size should only be empty, auto or auto auto.
            //// Only ignores them when IgnoreWhenBackgroundSizeIsSet setting is set to true.
            ////
            if (declarationProperties.TryGetValue(ImageAssembleConstants.BackgroundSize, out backgroundSize))
            {
                if (ignoreImagesWithNonDefaultBackgroundSize)
                {
                    var sizeValue = backgroundSize.ExprNode.MinifyPrint();
                    if (!sizeValue.Equals("auto") && !sizeValue.Equals("auto auto"))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>This method updates the collection of urls which are deemed to be valid but the
        /// overall declaration does not satisfy the requirement for image assembly.
        /// For example: The following url is valid but overall declaration does not
        /// meet the requirements of image assembly since the position is given
        /// in the relative units.
        /// #selector
        /// {
        ///   background: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif) no-repeat 1em 2em;
        /// }</summary>
        /// <param name="parsedUrl">The parsed url</param>
        /// <param name="imagesCriteriaFailedUrls">The list of failed urls</param>
        private static void UpdateFailedUrlsList(string parsedUrl, ICollection<string> imagesCriteriaFailedUrls)
        {
            if (imagesCriteriaFailedUrls != null && !string.IsNullOrWhiteSpace(parsedUrl))
            {
                imagesCriteriaFailedUrls.Add(parsedUrl);
            }
        }

        /// <summary>Validates the duplicate property names in a given list of declarations
        /// This is to make sure that the declarations are in good state before
        /// doing any further processing.
        /// With in a set of declarations, a duplicate property is not allowed by design. 
        /// For Example: The selector below is sloppy CSS. The image assembly tool
        /// by design is not intended to compute the cascade.
        /// #selector
        /// {
        ///   background: url(abc.gif);
        ///   background: url(def.gif);
        /// }</summary>
        /// <param name="declarationNodes">The list of declaration nodes</param>
        /// <returns>The key value pairs of property name and the declaration nodes</returns>
        private static Dictionary<string, DeclarationNode> LoadDeclarationPropertiesDictionary(this IEnumerable<DeclarationNode> declarationNodes)
        {
            // Lower case is not considered since it should be handler by a separate visitor
            var declarationPropertyNames = new Dictionary<string, List<DeclarationNode>>(StringComparer.OrdinalIgnoreCase);
            declarationNodes.ForEach(declarationNode =>
            {
                List<DeclarationNode> otherProperties;
                var propertyName = declarationNode.Property;
                if (!declarationPropertyNames.TryGetValue(propertyName, out otherProperties))
                {
                    declarationPropertyNames[propertyName] = otherProperties = new List<DeclarationNode>();
                }

                otherProperties.Add(declarationNode);
            });

            return declarationPropertyNames.ToDictionary(d => d.Key, d => d.Value.LastOrDefault());
        }
    }
}
