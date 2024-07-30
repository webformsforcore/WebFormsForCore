// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclarationNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   declaration
//   property ':' S* expr prio? | /* empty *
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>declaration
    /// property ':' S* expr prio? | /* empty */</summary>
    public sealed class DeclarationNode : AstNode
    {
        /// <summary>Initializes a new instance of the DeclarationNode class</summary>
        /// <param name="property">Delcaration Property</param>
        /// <param name="exprNode">Expression objecy</param>
        /// <param name="prio">Priority string</param>
        public DeclarationNode(string property, ExprNode exprNode, string prio, ReadOnlyCollection<ImportantCommentNode> importantComments)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(property));
            Contract.Requires(exprNode != null);

            // Member Initialization
            this.Property = property;
            this.ExprNode = exprNode;
            this.Prio = prio ?? string.Empty;
            this.ImportantComments = importantComments ?? new List<ImportantCommentNode>().AsReadOnly();

        }

        /// <summary>
        /// Gets the list of Important Comment Nodes
        /// This comments is in the beggining of the declaration
        /// </summary>
        public ReadOnlyCollection<ImportantCommentNode> ImportantComments { get; private set; }

        /// <summary>
        /// Gets the Property value
        /// </summary>
        /// <value>DeclarationNode Property value</value>
        public string Property { get; private set; }

        /// <summary>
        /// Gets the _expr value
        /// </summary>
        /// <value>Expression value</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expr")]
        public ExprNode ExprNode { get; private set; }

        /// <summary>Gets the Prio.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Prio")]
        public string Prio { get; private set; }

        /// <summary>
        /// Determine if the declaration is equal to another declaration
        /// </summary>
        /// <param name="declarationNode"> another declaration node</param>
        /// <returns> Equals or not </returns>
        public bool Equals(DeclarationNode declarationNode)
        {
            return declarationNode.Property.Equals(this.Property)
                && declarationNode.ExprNode.Equals(this.ExprNode)
                && declarationNode.Prio.Equals(this.Prio);
        }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitDeclarationNode(this);
        }
    }
}