// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NegationArgNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The negation arg node.
//   negation_arg
//   : type_selector | universal | HASH | class | attrib | pseudo
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using Visitor;

    /// <summary>The negation arg node.
    /// negation_arg
    /// : type_selector | universal | HASH | class | attrib | pseudo
    /// ;</summary>
    public sealed class NegationArgNode : AstNode
    {
        /// <summary>The exception message.</summary>
        private const string ExceptionMessage = "Only a single value out of type selector, universal selector, hash or class or attrib node or pseudo node can be not null.";

        /// <summary>Initializes a new instance of the <see cref="NegationArgNode"/> class.</summary>
        /// <param name="typeSelectorNode">The type selector node.</param>
        /// <param name="universalSelectorNode">The universal selector node.</param>
        /// <param name="hash">The hash.</param>
        /// <param name="cssClass">The css class.</param>
        /// <param name="attribNode">The attrib node.</param>
        /// <param name="pseudoNode">The pseudo node.</param>
        public NegationArgNode(TypeSelectorNode typeSelectorNode, UniversalSelectorNode universalSelectorNode, string hash, string cssClass, AttribNode attribNode, PseudoNode pseudoNode)
        {
            if (typeSelectorNode != null)
            {
                if (universalSelectorNode != null ||
                    !string.IsNullOrWhiteSpace(hash) ||
                    !string.IsNullOrWhiteSpace(cssClass) ||
                    attribNode != null ||
                    pseudoNode != null)
                {
                    throw new AstException(ExceptionMessage);
                }
            }

            if (universalSelectorNode != null)
            {
                if (!string.IsNullOrWhiteSpace(hash) ||
                    !string.IsNullOrWhiteSpace(cssClass) ||
                    attribNode != null ||
                    pseudoNode != null)
                {
                    throw new AstException(ExceptionMessage);
                }
            }

            if (!string.IsNullOrWhiteSpace(hash))
            {
                if (!string.IsNullOrWhiteSpace(cssClass) ||
                    attribNode != null ||
                    pseudoNode != null)
                {
                    throw new AstException(ExceptionMessage);
                }
            }

            if (!string.IsNullOrWhiteSpace(cssClass))
            {
                if (attribNode != null ||
                    pseudoNode != null)
                {
                    throw new AstException(ExceptionMessage);
                }
            }

            if (attribNode != null)
            {
                if (pseudoNode != null)
                {
                    throw new AstException(ExceptionMessage);
                }
            }

            this.TypeSelectorNode = typeSelectorNode;
            this.UniversalSelectorNode = universalSelectorNode;
            this.Hash = hash;
            this.CssClass = cssClass;
            this.AttribNode = attribNode;
            this.PseudoNode = pseudoNode;
        }

        /// <summary>Gets the Type Selector Node.</summary>
        public TypeSelectorNode TypeSelectorNode { get; private set; }

        /// <summary>Gets the Universal Selector Node.</summary>
        public UniversalSelectorNode UniversalSelectorNode { get; private set; }

        /// <summary>Gets the Hash.</summary>
        public string Hash { get; private set; }

        /// <summary>Gets the Css Class.</summary>
        public string CssClass { get; private set; }

        /// <summary>Gets the Attrib Node.</summary>
        public AttribNode AttribNode { get; private set; }

        /// <summary>Gets the Pseudo Node.</summary>
        public PseudoNode PseudoNode { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitNegationArgNode(this);
        }
    }
}