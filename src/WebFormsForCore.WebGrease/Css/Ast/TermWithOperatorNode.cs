// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TermWithOperatorNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   expr
//   term [ operator term ]*
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>expr
    /// term [ operator term ]*</summary>
    public sealed class TermWithOperatorNode : AstNode
    {
        /// <summary>
        /// Gets whether the termWithOperator is binary or not.
        /// </summary>
        private bool usesBinary;

        /// <summary>Initializes a new instance of the TermWithOperatorNode class</summary>
        /// <param name="op">Operator string</param>
        /// <param name="termNode">Term object</param>
        public TermWithOperatorNode(string op, TermNode termNode)
        {
            Contract.Requires(termNode != null);

            if (string.IsNullOrWhiteSpace(op))
            {
                op = CssConstants.SingleSpace.ToString();
            }

            // Member Initialization
            this.Operator = op;
            this.TermNode = termNode;
        }

        /// <summary>
        /// Gets whether the termWithOperator is binary or not.
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
                this.TermNode.IsBinary = value;
            }
        }

        /// <summary>
        /// Gets the Operator string
        /// </summary>
        /// <value>Operator string</value>
        public string Operator { get; private set; }

        /// <summary>
        /// Gets the term value
        /// </summary>
        /// <value>TermNode value</value>
        public TermNode TermNode { get; private set; }

        /// <summary>
        /// Determine if the node is equal to another node
        /// </summary>
        /// <param name="termWithOperator"> another termWithOperatorNode</param>
        /// <returns> Equals or not</returns>
        public bool Equals(TermWithOperatorNode termWithOperator)
        {
            return termWithOperator.UsesBinary == this.UsesBinary
                && termWithOperator.TermNode.Equals(this.TermNode)
                && termWithOperator.Operator.Equals(this.Operator);
        }
        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitTermWithOperatorNode(this);
        }
    }
}