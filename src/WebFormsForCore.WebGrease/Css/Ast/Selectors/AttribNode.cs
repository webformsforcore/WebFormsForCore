// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AttribNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   attrib
//   : '[' S* [ namespace_prefix ]? IDENT S*
//   [ [ PREFIXMATCH |
//   SUFFIXMATCH |
//   SUBSTRINGMATCH |
//   '=' |
//   INCLUDES |
//   DASHMATCH ] S* [ IDENT | STRING ] S*
//   ]? ']'
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>attrib
    ///  : '[' S* [ namespace_prefix ]? IDENT S*
    ///        [ [ PREFIXMATCH |
    ///            SUFFIXMATCH |
    ///            SUBSTRINGMATCH |
    ///            '=' |
    ///            INCLUDES |
    ///            DASHMATCH ] S* [ IDENT | STRING ] S*
    ///        ]? ']'
    /// ;</summary>
    public sealed class AttribNode : AstNode
    {
        /// <summary>Initializes a new instance of the AttribNode class</summary>
        /// <param name="selectorNamespacePrefixNode">The namespace Prefix Node.</param>
        /// <param name="identity">Identity string</param>
        /// <param name="attribOperatorAndValueNode">Attrib Operator and Value object</param>
        public AttribNode(SelectorNamespacePrefixNode selectorNamespacePrefixNode, string identity, AttribOperatorAndValueNode attribOperatorAndValueNode)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(identity));
            
            this.SelectorNamespacePrefixNode = selectorNamespacePrefixNode;
            this.Ident = identity;
            this.OperatorAndValueNode = attribOperatorAndValueNode ?? new AttribOperatorAndValueNode(AttribOperatorKind.None, string.Empty);
        }

        /// <summary>Gets NamespacePrefixNode.</summary>
        public SelectorNamespacePrefixNode SelectorNamespacePrefixNode { get; private set; }

        /// <summary>
        /// Gets the Attribute Identity
        /// </summary>
        /// <value>Attribute Identity</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ident")]
        public string Ident { get; private set; }

        /// <summary>
        /// Gets Operator and value
        /// </summary>
        /// <value>Operator and value</value>
        public AttribOperatorAndValueNode OperatorAndValueNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitAttribNode(this);
        }
    }
}
