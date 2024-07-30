// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PseudoNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   pseudo
//   * '::' starts a pseudo-element, ':' a pseudo-class *
//   * Exceptions: :first-line, :first-letter, :before and :after. *
//   * Note that pseudo-elements are restricted to one per selector and *
//   * occur only in the last simple_selector_sequence. *
//   : ':' ':'? [ IDENT | functional_pseudo ]
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>pseudo
    ///  /* '::' starts a pseudo-element, ':' a pseudo-class */
    ///  /* Exceptions: :first-line, :first-letter, :before and :after. */
    ///  /* Note that pseudo-elements are restricted to one per selector and */
    ///  /* occur only in the last simple_selector_sequence. */
    ///  : ':' ':'? [ IDENT | functional_pseudo ]
    /// ;</summary>
    public sealed class PseudoNode : AstNode
    {
        /// <summary>Initializes a new instance of the PseudoNode class</summary>
        /// <param name="numberOfColons">The number of colons</param>
        /// <param name="ident">Identity string</param>
        /// <param name="functionalPseudoNode">The functional Pseudo Node.</param>
        public PseudoNode(int numberOfColons, string ident, FunctionalPseudoNode functionalPseudoNode)
        {
            Contract.Requires(numberOfColons > 0 && numberOfColons <= 2);
            Contract.Requires(string.IsNullOrWhiteSpace(ident) || functionalPseudoNode == null);

            this.NumberOfColons = numberOfColons;
            this.Ident = ident;
            this.FunctionalPseudoNode = functionalPseudoNode;
        }

        /// <summary>Gets the number of colons.</summary>
        public int NumberOfColons { get; private set; }

        /// <summary>
        /// Gets the identity value
        /// </summary>
        /// <value>Identity value</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ident")]
        public string Ident { get; private set; }

        /// <summary>Gets the functional Pseudo Node.</summary>
        public FunctionalPseudoNode FunctionalPseudoNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitPseudoNode(this);
        }
    }
}
