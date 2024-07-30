// -----------------------------------------------------------------------
// <copyright file="DocumentQueryNode.cs" company="Microsoft">
// Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Supports @document rules from http://www.w3.org/TR/css3-conditional/#at-document.
//   DOCUMENT_SYM S* document_match_function S* CURLY_BEGIN ruleset* CURLY_END
// </summary>
// -----------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>
    /// Supports @document rules.
    /// </summary>
    public sealed class DocumentQueryNode : StyleSheetRuleNode
    {
        /// <summary>
        /// Initializes a new instance of the DocumentQueryNode class
        /// </summary>
        /// <param name="matchFunctionName">Name of the function used for determining whether the rulesets should match the current document.</param>
        /// <param name="documentSymbol">Document @ rule name</param>
        /// <param name="rulesets">rulesets to apply when matched</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "rulesets", Justification = "Spelled correctly")]
        public DocumentQueryNode(string matchFunctionName, string documentSymbol, ReadOnlyCollection<RulesetNode> rulesets)
        {
            Contract.Requires(!string.IsNullOrEmpty(matchFunctionName));
            Contract.Requires(!string.IsNullOrWhiteSpace(documentSymbol));
            Contract.Requires(rulesets != null && rulesets.Count > 0);
            this.Rulesets = rulesets;
            this.MatchFunctionName = matchFunctionName;
            this.DocumentSymbol = documentSymbol;
        }

        /// <summary>
        /// Gets the function used for the document matching.
        /// </summary>
        public string MatchFunctionName { get; private set; }

        /// <summary>
        /// Gets the document @rule used for this, e.g. @document or @-moz-document
        /// </summary>
        public string DocumentSymbol { get; private set; }

        /// <summary>
        /// Gets Rules sets
        /// </summary>
        /// <value>Rules List</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rulesets")]
        public ReadOnlyCollection<RulesetNode> Rulesets { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitDocumentQueryNode(this);
        }
    }
}
