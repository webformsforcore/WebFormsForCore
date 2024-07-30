// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NegationNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The negation node.
//   negation
//   : NOT S* negation_arg S* ')'
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The negation node.
    /// negation
    /// : NOT S* negation_arg S* ')'
    /// ;</summary>
    public sealed class NegationNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="NegationNode"/> class.</summary>
        /// <param name="negationArgNode">The negation arg node.</param>
        public NegationNode(NegationArgNode negationArgNode)
        {
            Contract.Requires(negationArgNode != null);
            this.NegationArgNode = negationArgNode;
        }

        /// <summary>Gets the negation node.</summary>
        public NegationArgNode NegationArgNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitNegationNode(this);
        }
    }
}