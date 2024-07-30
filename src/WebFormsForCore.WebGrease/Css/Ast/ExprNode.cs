// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExprNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   expr
//   term [ operator term ]*
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Visitor;

    /// <summary>expr
    /// term [ operator term ]*</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expr")]
    public sealed class ExprNode : AstNode
    {
        /// <summary>
        /// Gets wether the expr is binary or not.
        /// </summary>
        private bool usesBinary;

        /// <summary>Initializes a new instance of the ExprNode class</summary>
        /// <param name="termNode">Term object</param>
        /// <param name="termsWithOperators">Terms with Operators</param>
        public ExprNode(TermNode termNode, ReadOnlyCollection<TermWithOperatorNode> termsWithOperators, ReadOnlyCollection<ImportantCommentNode> importantComments)
        {
            Contract.Requires(termNode != null);

            this.TermNode = termNode;
            this.TermsWithOperators = termsWithOperators ?? (new List<TermWithOperatorNode>()).AsReadOnly();
            this.ImportantComments = importantComments ?? (new List<ImportantCommentNode>()).AsReadOnly();
            this.UsesBinary = false;
        }

        /// <summary>
        /// Gets wether the expr is binary or not.
        /// </summary>
        public bool UsesBinary
        {
            get
            {
                return this.usesBinary;
            }
            set
            {
                this.usesBinary = value;
                foreach (var termWithOperators in this.TermsWithOperators)
                {
                    termWithOperators.UsesBinary = value;
                }
            }
        }

        /// <summary>
        /// Gets list of comment nodes
        /// </summary>
        /// <value>ImportantCommentNodes List</value>
        public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }

        /// <summary>
        /// Gets the TermNode value
        /// </summary>
        /// <value>TermNode value</value>
        public TermNode TermNode { get; private set; }

        /// <summary>
        /// Gets TermNode with Operators List
        /// </summary>
        /// <value>TermNode with Operators List</value>
        public ReadOnlyCollection<TermWithOperatorNode> TermsWithOperators { get; private set; }

        /// <summary>
        /// Determines if the node is equal to another node
        /// </summary>
        /// <param name="exprNode"> another exprNode</param>
        /// <returns> Determine if the node is equal to another node</returns>
        public bool Equals(ExprNode exprNode)
        {
            if (!exprNode.TermNode.Equals(this.TermNode)
                || exprNode.UsesBinary != this.UsesBinary
                || exprNode.TermsWithOperators.Count != this.TermsWithOperators.Count)
            {
                return false;
            }

            for (int i = 0; i < TermsWithOperators.Count; i++)
            {
                if(!exprNode.TermsWithOperators[i].Equals(this.TermsWithOperators[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitExprNode(this);
        }
    }
}