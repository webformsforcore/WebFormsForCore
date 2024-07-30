// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CombinatorSimpleSelectorSequenceNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   [ combinator selector | S+ [ combinator? selector ]? ]?
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Diagnostics.Contracts;
    using Selectors;
    using Visitor;

    /// <summary>[ combinator selector | S+ [ combinator? selector ]? ]?</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Combinator", Justification = "We know what it means.")]
    public sealed class CombinatorSimpleSelectorSequenceNode : AstNode
    {
        /// <summary>Initializes a new instance of the CombinatorSimpleSelectorSequenceNode class</summary>
        /// <param name="combinator">Combinator obejct</param>
        /// <param name="simpleSelectorSequenceNode">Simple SelectorNode</param>
        public CombinatorSimpleSelectorSequenceNode(Combinator combinator, SimpleSelectorSequenceNode simpleSelectorSequenceNode)
        {
            Contract.Requires(combinator != Combinator.None);
            Contract.Requires(simpleSelectorSequenceNode != null);

            this.Combinator = combinator;
            this.SimpleSelectorSequenceNode = simpleSelectorSequenceNode;
        }

        /// <summary>
        /// Gets the Combinator
        /// </summary>
        /// <value>Combinator property</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Combinator", Justification="We know what it means.")]
        public Combinator Combinator { get; private set; }

        /// <summary>
        /// Gets the Simple SelectorNode
        /// </summary>
        /// <value>Simple SelectorNode</value>
        public SimpleSelectorSequenceNode SimpleSelectorSequenceNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitCombinatorSimpleSelectorSequenceNode(this);
        }
    }
}
