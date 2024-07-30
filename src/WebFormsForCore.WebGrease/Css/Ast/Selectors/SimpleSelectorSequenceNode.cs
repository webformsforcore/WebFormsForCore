// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleSelectorSequenceNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   simple_selector_sequence
//   : [ type_selector | universal ]
//   [ HASH | class | attrib | pseudo | negation ]*
//   | [ HASH | class | attrib | pseudo | negation ]+
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.Selectors
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>simple_selector_sequence
    /// : [ type_selector | universal ]
    ///  [ HASH | class | attrib | pseudo | negation ]*
    /// | [ HASH | class | attrib | pseudo | negation ]+
    /// ;</summary>
    public sealed class SimpleSelectorSequenceNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="SimpleSelectorSequenceNode"/> class.</summary>
        /// <param name="typeSelectorNode">The type selector node.</param>
        /// <param name="universalSelectorNode">The universal selector node.</param>
        /// <param name="separator">The whitespace separator</param>
        /// <param name="simpleSelectorValues">The simple selector values.</param>
        public SimpleSelectorSequenceNode(TypeSelectorNode typeSelectorNode, UniversalSelectorNode universalSelectorNode, string separator, ReadOnlyCollection<HashClassAtNameAttribPseudoNegationNode> simpleSelectorValues)
        {
            Contract.Requires(typeSelectorNode == null || universalSelectorNode == null);

            this.TypeSelectorNode = typeSelectorNode;
            this.UniversalSelectorNode = universalSelectorNode;
            this.Separator = separator ?? string.Empty;
            this.HashClassAttribPseudoNegationNodes = simpleSelectorValues ?? new List<HashClassAtNameAttribPseudoNegationNode>(0).AsReadOnly();
        }

        /// <summary>Gets the type Selector Node.</summary>
        public TypeSelectorNode TypeSelectorNode { get; private set; }

        /// <summary>Gets the Universal Selector Node.</summary>
        public UniversalSelectorNode UniversalSelectorNode { get; private set; }

        /// <summary>Gets the white space separator.</summary>
        public string Separator { get; private set; }

        /// <summary>
        /// Gets the Simple SelectorNode values list
        /// </summary>
        /// <value>Simple SelectorNode values list</value>
        public ReadOnlyCollection<HashClassAtNameAttribPseudoNegationNode> HashClassAttribPseudoNegationNodes { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitSimpleSelectorSequenceNode(this);
        }
    }
}
