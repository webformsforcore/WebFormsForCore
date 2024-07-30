// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectorExpressionNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The selector expression node.
//   expression
//   * In CSS3, the expressions are identifiers, strings, *
//   * or of the form "an+b" *
//   : [ [ PLUS | '-' | DIMENSION | NUMBER | STRING | IDENT ] S* ]+
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>The selector expression node.
    /// expression
    /// * In CSS3, the expressions are identifiers, strings, */
    /// * or of the form "an+b" */
    /// : [ [ PLUS | '-' | DIMENSION | NUMBER | STRING | IDENT ] S* ]+
    /// ;</summary>
    public sealed class SelectorExpressionNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="SelectorExpressionNode"/> class.</summary>
        /// <param name="selectorExpressions">The selector expressions.</param>
        public SelectorExpressionNode(ReadOnlyCollection<string> selectorExpressions)
        {
            Contract.Requires(selectorExpressions != null);
            Contract.Requires(selectorExpressions.Count > 0);

            this.SelectorExpressions = selectorExpressions;
        }

        /// <summary>Gets the list of selector expressions.</summary>
        public ReadOnlyCollection<string> SelectorExpressions { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitSelectorExpressionNode(this);
        }
    }
}