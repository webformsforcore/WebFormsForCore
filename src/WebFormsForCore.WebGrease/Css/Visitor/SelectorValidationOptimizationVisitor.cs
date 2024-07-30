// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectorValidationOptimizationVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation for validating or removing the banned selectors in CSS.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    using Ast;
    using Ast.MediaQuery;
    using Extensions;

    using WebGrease.Css.Ast.Selectors;

    /// <summary>Provides the implementation for validating or removing the banned selectors in CSS.</summary>
    public sealed class SelectorValidationOptimizationVisitor : NodeVisitor
    {
        /// <summary>
        /// The list of selectors to scan
        /// </summary>
        private readonly HashSet<string> selectorsToValidateOrRemove;

        /// <summary>
        /// The flag which indicates if the partial match should be performed or the full match
        /// </summary>
        private readonly bool shouldMatchExactly;

        /// <summary>
        /// The flag which indicates if the validation/exception should be thrown or optimize the css
        /// after removing the selector
        /// </summary>
        private readonly bool validate;

        /// <summary>Initializes a new instance of the SelectorValidationOptimizationVisitor class.</summary>
        /// <param name="selectorsToValidateOrRemove">The list of selectors to validate or remove</param>
        /// <param name="shouldMatchExactly">The flag which indicates if the partial match should be performed or the full match</param>
        /// <param name="validate">The flag which indicates if the validation/exception should be thrown or optimize the css after removing the selector</param>
        public SelectorValidationOptimizationVisitor(HashSet<string> selectorsToValidateOrRemove, bool shouldMatchExactly, bool validate)
        {
            this.validate = validate;
            this.shouldMatchExactly = shouldMatchExactly;
            this.selectorsToValidateOrRemove = selectorsToValidateOrRemove ?? new HashSet<string>();
        }

        /// <summary>The <see cref="StyleSheetNode"/> visit implementation</summary>
        /// <param name="styleSheet">The styleSheet AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
        {
            if (styleSheet == null)
            {
                throw new ArgumentNullException("styleSheet");
            }

            var updatedStyleSheetRules = new List<StyleSheetRuleNode>();
            styleSheet.StyleSheetRules.ForEach(
                ruleSetMediaPageNode =>
                {
                    var updatedRuleSetMediaPageNode = (StyleSheetRuleNode)ruleSetMediaPageNode.Accept(this);

                    // If there is a request for optimization, the ruleset will be null.
                    // Only add the non-null rulesets here.
                    if (updatedRuleSetMediaPageNode != null)
                    {
                        updatedStyleSheetRules.Add(updatedRuleSetMediaPageNode);
                    }
                });

            return new StyleSheetNode(styleSheet.CharSetString, styleSheet.Imports, styleSheet.Namespaces, updatedStyleSheetRules.AsReadOnly());
        }

        /// <summary>The <see cref="RulesetNode"/> visit implementation</summary>
        /// <param name="rulesetNode">The ruleset AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
        {
            // Here are few possible scenarios as how client code will utilize this visitor:
            // 1. Request to raise the exception for hacks. It would do a match on selectors and if match is found
            // exception will be thrown.
            // 2. Remove the banned selectors It would do a match on the selectors and if a match is found,
            // the ruleset will be returned null so that it can be removed from transformed Ast.
            var rulesetSelector = rulesetNode.PrintSelector();
            var hack = string.Empty;
            var match = false;

            if (this.shouldMatchExactly)
            {
                // Find the match in list (lower/upper case not considered since lowercase is already a standard)
                match = this.selectorsToValidateOrRemove.Contains(rulesetSelector);
                hack = rulesetSelector;
            }
            else
            {
                foreach (var selectorToValidateOrRemove in this.selectorsToValidateOrRemove)
                {
                    if (!rulesetSelector.Contains(selectorToValidateOrRemove))
                    {
                        continue;
                    }

                    // Match is found
                    match = true;
                    hack = selectorToValidateOrRemove;
                    break;
                }
            }

            // If a match is found, take a decision whether to validate/optimize/both
            if (match)
            {
                // It is just required to validate
                if (this.validate)
                {
                    throw new BuildWorkflowException(string.Format(CultureInfo.CurrentUICulture, CssStrings.CssSelectorHackError, hack));
                }

                // If the ruleset has multiple selectors, we need to check if we need to remove all of them or just some.
                if (rulesetNode.SelectorsGroupNode.SelectorNodes.Count > 1)
                {
                    // Get the selector nodes that do not match the banned selectors
                    var selectorNodes = rulesetNode.SelectorsGroupNode.SelectorNodes
                        .Where(sn =>
                            !this.selectorsToValidateOrRemove.Any(sr =>
                                sn.MinifyPrint().Contains(sr))).ToList();

                    // If we still have selectors remaining, we create a new rulesetnode, with the remaining selectors, and return it as the current node.
                    if (selectorNodes.Any())
                    {
                        return new RulesetNode(
                            new SelectorsGroupNode(
                                new ReadOnlyCollection<SelectorNode>(selectorNodes)), 
                            rulesetNode.Declarations,
                            rulesetNode.ImportantComments);
                    }
                }

                // Otherwise it is an optimization to remove the ruleset
                return null;
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

            var updatedRulesetNodes = new List<RulesetNode>();
            var updatePageNodes = new List<PageNode>();

            mediaNode.Rulesets.ForEach(rulesetNode =>
            {
                var updatedRulesetNode = (RulesetNode)rulesetNode.Accept(this);

                if (updatedRulesetNode != null)
                {
                    updatedRulesetNodes.Add(updatedRulesetNode);
                }
            });

            mediaNode.PageNodes.ForEach(page =>
                {
                    var updatedPageNode = (PageNode)page.Accept(this);
                    if (updatedPageNode != null)
                    {
                        updatePageNodes.Add(updatedPageNode);
                    }
                }
            );
            
            return new MediaNode(mediaNode.MediaQueries, updatedRulesetNodes.AsReadOnly(), updatePageNodes.AsReadOnly());
        }
    }
}