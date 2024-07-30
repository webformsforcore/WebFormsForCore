// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalPseudoNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The functional pseudo node.
//   functional_pseudo
//   : FUNCTION S* expression ')'
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The functional pseudo node.
    /// functional_pseudo
    ///  : FUNCTION S* expression ')'
    /// ;</summary>
    public sealed class FunctionalPseudoNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="FunctionalPseudoNode"/> class.</summary>
        /// <param name="functionName">The function name.</param>
        /// <param name="selectorExpressionNode">The selector expression node.</param>
        public FunctionalPseudoNode(string functionName, SelectorExpressionNode selectorExpressionNode)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(functionName));
            Contract.Requires(selectorExpressionNode != null);

            this.FunctionName = functionName;
            this.SelectorExpressionNode = selectorExpressionNode;
        }

        /// <summary>Gets the function name.</summary>
        public string FunctionName { get; private set; }

        /// <summary>Gets the selector expression node.</summary>
        public SelectorExpressionNode SelectorExpressionNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitFunctionalPseudoNode(this);
        }
    }
}