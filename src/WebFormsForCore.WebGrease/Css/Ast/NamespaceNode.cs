// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The namespace prefix node.
//   namespace
//   : NAMESPACE_SYM S* [namespace_prefix S*]? [STRING|URI] S* ';' S*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The namespace prefix node.
    ///  namespace
    ///  : NAMESPACE_SYM S* [namespace_prefix S*]? [STRING|URI] S* ';' S*
    /// ;</summary>
    public sealed class NamespaceNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="NamespaceNode"/> class.</summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The value.</param>
        public NamespaceNode(string prefix, string value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));

            this.Prefix = prefix;
            this.Value = value;
        }

        /// <summary>Gets Prefix.</summary>
        public string Prefix { get; private set; }

        /// <summary>Gets Value.</summary>
        public string Value { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitNamespaceNode(this);
        }
    }
}
