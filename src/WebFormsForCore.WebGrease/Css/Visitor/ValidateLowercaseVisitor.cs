// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidateLowercaseVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation for validating the lower case in CSS.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using System.Globalization;
    using Ast;
    using Ast.MediaQuery;
    using Extensions;

    using WebGrease.Activities;

    /// <summary>Provides the implementation for validating the lower case in CSS.</summary>
    public sealed class ValidateLowercaseVisitor : NodeVisitor
    {
        /// <summary>The <see cref="StyleSheetNode"/> visit implementation</summary>
        /// <param name="styleSheet">The styleSheet AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
        {
            if (styleSheet == null)
            {
                throw new ArgumentNullException("styleSheet");
            }

            // Note - "@charset" string cannot be verified here as it is analyzed during the parsing.
            // No need to wrap the exception context here since it would be the beginning of Css.
            ValidateForLowerCase(styleSheet.CharSetString);

            // Note - "@import" string cannot be verified here as it is analyzed during the parsing.
            // No need to wrap the exception context here since it would be the beginning of Css.
            styleSheet.Imports.ForEach(importNode => ValidateForLowerCase(importNode.MinifyPrint()));

            // Visit the ruleset, media or page nodes
            styleSheet.StyleSheetRules.ForEach(styleSheetRule => styleSheetRule.Accept(this));

            return styleSheet;
        }

        /// <summary>The <see cref="RulesetNode"/> visit implementation</summary>
        /// <param name="rulesetNode">The ruleset AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
        {
            if (rulesetNode == null)
            {
                throw new ArgumentNullException("rulesetNode");
            }

            try
            {
                // Validate the selectors for lower case
                rulesetNode.SelectorsGroupNode.SelectorNodes.ForEach(selectorNode => ValidateForLowerCase(selectorNode.MinifyPrint()));

                // Visit declarations
                rulesetNode.Declarations.ForEach(declarationNode => declarationNode.Accept(this));
            }
            catch (BuildWorkflowException exception)
            {
                throw new WorkflowException(string.Format(CultureInfo.CurrentUICulture, CssStrings.CssLowercaseValidationParentNodeError, rulesetNode.PrettyPrint()), exception);
            }

            return rulesetNode;
        }

        /// <summary>The <see cref="MediaNode"/> visit implementation</summary>
        /// <param name="mediaNode">The media AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaNode(MediaNode mediaNode)
        {
            if (mediaNode == null)
            {
                throw new ArgumentNullException("mediaNode");
            }

            // Note - "@media" string cannot be verified here as it is analyzed during the parsing.
            // The print visitors convert it to lower case "@media" after printing.
            try
            {
                // Validate the mediums for lower case
                mediaNode.MediaQueries.ForEach(mediaQuery => ValidateForLowerCase(mediaQuery.MinifyPrint()));

                // Visit rulesets
                mediaNode.Rulesets.ForEach(rulesetNode => rulesetNode.Accept(this));
            }
            catch (BuildWorkflowException exception)
            {
                throw new WorkflowException(string.Format(CultureInfo.CurrentUICulture, CssStrings.CssLowercaseValidationParentNodeError, mediaNode.PrettyPrint()), exception);
            }

            return mediaNode;
        }

        /// <summary>The <see cref="PageNode"/> visit implementation</summary>
        /// <param name="pageNode">The page AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitPageNode(PageNode pageNode)
        {
            if (pageNode == null)
            {
                throw new ArgumentNullException("pageNode");
            }

            // Note - "@page" string cannot be verified here as it is analyzed during the parsing.
            // The print visitors convert it to lower case "@page" after printing.
            try
            {
                // Validate the pseudo nodes on page for lower case
                ValidateForLowerCase(pageNode.PseudoPage);

                // Visit declarations
                pageNode.Declarations.ForEach(declarationNode => declarationNode.Accept(this));
            }
            catch (BuildWorkflowException exception)
            {
                throw new WorkflowException(string.Format(CultureInfo.CurrentUICulture, CssStrings.CssLowercaseValidationParentNodeError, pageNode.PrettyPrint()), exception);
            }

            return pageNode;
        }

        /// <summary>The <see cref="DeclarationNode"/> visit implementation</summary>
        /// <param name="declarationNode">The declaration AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
        {
            if (declarationNode == null)
            {
                throw new ArgumentNullException("declarationNode");
            }

            // Validate the property for lower case
            ValidateForLowerCase(declarationNode.Property);
            
            return declarationNode;
        }

        /// <summary>Validate the selector for the lower case</summary>
        /// <param name="textToValidate">The artifact to validate</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "By design")]
        private static void ValidateForLowerCase(string textToValidate)
        {
            textToValidate = ResourcesResolver.LocalizationResourceKeyRegex.Replace(textToValidate, string.Empty);

            // remove all tokens before validate lowercase.
            if (string.IsNullOrWhiteSpace(textToValidate))
            {
                return;
            }

            // Throw an exception if the lower case does not match the original string
            // By design, we would catch/throw only on last caller to avoid nested contexts and multiple unwinds.
            if (string.CompareOrdinal(textToValidate, textToValidate.ToLower(CultureInfo.InvariantCulture)) != 0)
            {
                throw new BuildWorkflowException(string.Format(CultureInfo.InvariantCulture, CssStrings.CssLowercaseValidationError, textToValidate));
            }
        }
    }
}
