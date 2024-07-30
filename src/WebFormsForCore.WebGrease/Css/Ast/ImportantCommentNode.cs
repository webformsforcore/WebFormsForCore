// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportantCommentNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The important comment node.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WebGrease.Css.Visitor;

    /// <summary>
    /// The important Comment Node. 
    /// </summary>
    public sealed class ImportantCommentNode : AstNode
    {
        /// <summary>
        /// Initializes the new instance of ImportantCommentNode.
        /// </summary>
        /// <param name="text">The actual text of the important comment.</param>
        public ImportantCommentNode(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets the text of the comment.
        /// </summary>
        /// <value>The actual text of the comment.</value>
        public string Text { get; private set; }

        /// <summary>Defines an accept operation.</summary>
        /// <param name="nodeVisitor">The visitor to invoke.</param>
        /// <returns>The modified AST node if modified otherwise the original node.</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitImportantCommentNode(this);
        }
    }
}