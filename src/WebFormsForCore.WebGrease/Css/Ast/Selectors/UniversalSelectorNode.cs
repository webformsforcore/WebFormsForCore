// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniversalSelectorNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The universal selector node.
//   universal
//   : [ namespace_prefix ]? '*'
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using Visitor;

    /// <summary>The universal selector node.
    /// universal
    ///  : [ namespace_prefix ]? '*'
    /// ;</summary>
    public sealed class UniversalSelectorNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="UniversalSelectorNode"/> class.</summary>
        /// <param name="selectorNamespacePrefixNode">The namespace prefix node.</param>
        public UniversalSelectorNode(SelectorNamespacePrefixNode selectorNamespacePrefixNode)
        {
            this.SelectorNamespacePrefixNode = selectorNamespacePrefixNode;
        }

        /// <summary>Gets the namespace prefix node.</summary>
        public SelectorNamespacePrefixNode SelectorNamespacePrefixNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitUniversalSelectorNode(this);
        }
    }
}