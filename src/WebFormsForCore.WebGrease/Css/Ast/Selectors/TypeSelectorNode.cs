// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeSelectorNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The type selector node.
//   type_selector
//   : [ namespace_prefix ]? element_name
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The type selector node.
    /// type_selector
    /// : [ namespace_prefix ]? element_name
    /// ;</summary>
    public sealed class TypeSelectorNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="TypeSelectorNode"/> class.</summary>
        /// <param name="selectorNamespacePrefixNode">The namespace prefix node.</param>
        /// <param name="elementName">The element name.</param>
        public TypeSelectorNode(SelectorNamespacePrefixNode selectorNamespacePrefixNode, string elementName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(elementName));

            this.SelectorNamespacePrefixNode = selectorNamespacePrefixNode;
            this.ElementName = elementName;
        }

        /// <summary>Gets NamespacePrefixNode.</summary>
        public SelectorNamespacePrefixNode SelectorNamespacePrefixNode { get; private set; }

        /// <summary>Gets ElementName.</summary>
        public string ElementName { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitTypeSelectorNode(this);
        }
    }
}