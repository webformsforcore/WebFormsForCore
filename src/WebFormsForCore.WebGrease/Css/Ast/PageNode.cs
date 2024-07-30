// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PageNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   page:
//   PAGE_SYM S* pseudo_page? S*
//   LBRACE S* declaration [ ';' S* declaration ]* '}' S*
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>page:
    /// PAGE_SYM S* pseudo_page? S*
    /// LBRACE S* declaration [ ';' S* declaration ]* '}' S*</summary>
    public sealed class PageNode : StyleSheetRuleNode
    {
        /// <summary>Initializes a new instance of the PageNode class</summary>
        /// <param name="pseudoPage">PseudoNode page</param>
        /// <param name="declarations">Declarations Dictionary</param>
        public PageNode(string pseudoPage, ReadOnlyCollection<DeclarationNode> declarations)
        {
            Contract.Requires(declarations != null && declarations.Count > 0);

            this.PseudoPage = pseudoPage;
            this.Declarations = declarations;
        }

        /// <summary>
        /// Gets PseudoNode page name
        /// </summary>
        /// <value>PseudoNode page name</value>
        public string PseudoPage { get; private set; }

        /// <summary>
        /// Gets Declarations dictionary
        /// </summary>
        /// <value>DeclarationNode dictionary</value>
        public ReadOnlyCollection<DeclarationNode> Declarations { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitPageNode(this);
        }
    }
}
