// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectorNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   selector
//   : simple_selector_sequence [ combinator simple_selector_sequence ]*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>selector
    ///  : simple_selector_sequence [ combinator simple_selector_sequence ]*
    /// ;</summary>
    public sealed class SelectorNode : AstNode
    {
        /// <summary>Initializes a new instance of the SelectorNode class</summary>
        /// <param name="simpleSelectorSequenceNode">Simple Selector Sequence Node</param>
        /// <param name="combinatorSimpleSelectorSequenceNodes">Combinator Simple Selectors</param>
        public SelectorNode(SimpleSelectorSequenceNode simpleSelectorSequenceNode, ReadOnlyCollection<CombinatorSimpleSelectorSequenceNode> combinatorSimpleSelectorSequenceNodes)
        {
            Contract.Requires(simpleSelectorSequenceNode != null);

            this.SimpleSelectorSequenceNode = simpleSelectorSequenceNode;
            this.CombinatorSimpleSelectorSequenceNodes = combinatorSimpleSelectorSequenceNodes ?? new List<CombinatorSimpleSelectorSequenceNode>(0).AsReadOnly();
        }

        /// <summary>
        /// Gets Simple Selector Sequence Node
        /// </summary>
        /// <value>Simple SelectorNode</value>
        public SimpleSelectorSequenceNode SimpleSelectorSequenceNode { get; private set; }

        /// <summary>
        /// Gets Combinator Simple Selector Sequence Nodes
        /// </summary>
        /// <value>Combinator Simple Selector Sequence Nodes</value>
        public ReadOnlyCollection<CombinatorSimpleSelectorSequenceNode> CombinatorSimpleSelectorSequenceNodes { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitSelectorNode(this);
        }
    }
}
