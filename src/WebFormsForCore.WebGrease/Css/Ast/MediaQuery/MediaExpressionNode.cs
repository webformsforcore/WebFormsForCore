// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaExpressionNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The media expression.
//   expression
//   : '(' S* media_feature S* [ ':' S* expr ]? ')' S*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.MediaQuery
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The media expression.
    /// expression
    /// : '(' S* media_feature S* [ ':' S* expr ]? ')' S*
    /// ;</summary>
    public sealed class MediaExpressionNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="MediaExpressionNode"/> class.</summary>
        /// <param name="mediaFeature">The media feature.</param>
        /// <param name="exprNode">The expr node.</param>
        public MediaExpressionNode(string mediaFeature, ExprNode exprNode)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(mediaFeature));

            this.MediaFeature = mediaFeature;
            this.ExprNode = exprNode;
        }

        /// <summary>Gets the media feature.</summary>
        public string MediaFeature { get; private set; }

        /// <summary>Gets the expr node.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expr")]
        public ExprNode ExprNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitMediaExpressionNode(this);
        }
    }
}