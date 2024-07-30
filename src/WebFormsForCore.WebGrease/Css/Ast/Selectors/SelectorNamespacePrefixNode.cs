// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespacePrefixNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The namespace prefix node.
//   namespace_prefix
//   : [ IDENT | '*' ]? '|'
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using Visitor;

    /// <summary>The namespace prefix node.
    /// namespace_prefix
    ///  : [ IDENT | '*' ]? '|'
    /// ;</summary>
    public sealed class SelectorNamespacePrefixNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="SelectorNamespacePrefixNode"/> class.</summary>
        /// <param name="prefix">The prefix.</param>
        public SelectorNamespacePrefixNode(string prefix)
        {
            // Namespace node can be there but the namespace can be empty.
            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = string.Empty;
            }

            this.Prefix = prefix;
        }

        /// <summary>Gets the prefix.</summary>
        public string Prefix { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitSelectorNamespacePrefixNode(this);
        }
    }
}