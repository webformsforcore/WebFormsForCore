// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectorsGroupNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The selectors group node.
//   selectors_group
//   : selector [ COMMA S* selector ]*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The selectors group node.
    /// selectors_group
    ///  : selector [ COMMA S* selector ]*
    /// ;</summary>
    public sealed class SelectorsGroupNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="SelectorsGroupNode"/> class.</summary>
        /// <param name="selectorNodes">The selector nodes.</param>
        public SelectorsGroupNode(ReadOnlyCollection<SelectorNode> selectorNodes)
        {
            Contract.Requires(selectorNodes != null && selectorNodes.Count > 0);
            this.SelectorNodes = selectorNodes;
        }

        /// <summary>Gets SelectorNodes.</summary>
        public ReadOnlyCollection<SelectorNode> SelectorNodes { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitSelectorsGroupNode(this);
        }
    }
}