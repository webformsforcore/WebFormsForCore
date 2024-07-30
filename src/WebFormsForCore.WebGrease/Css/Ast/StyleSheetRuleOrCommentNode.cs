// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StyleSheetRuleOrCommentNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The stylesheetruleorcommentnode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// StyleSheetRuleOrCommentNode.
    /// </summary>
    public class StyleSheetRuleOrCommentNode : StyleSheetRuleNode
    {
        /// <summary>
        /// The StylesheetRuleOrCommentNode.
        /// </summary>
        /// <param name="comment"> Important comment node.</param>
        /// <param name="isComment"> Whether it is comment or not.</param>
        public StyleSheetRuleOrCommentNode(ImportantCommentNode comment, bool isComment)
        {
            this.ImportantCommentNode = comment;
            this.IsCommentNode = isComment;
        }

        /// <summary>
        /// Gets the comment node if it is comment.
        /// </summary>
        public ImportantCommentNode ImportantCommentNode { get; private set; }

        /// <summary>
        /// Gets whether the class is comment or style sheet.
        /// </summary>
        public bool IsCommentNode { get; set; }

        /// <summary>
        /// The Accept implementation.
        /// </summary>
        /// <param name="nodeVisitor">The visitor to visit.</param>
        /// <returns>The modified StylesheetRuleOrComment.</returns>
        public override AstNode Accept(Visitor.NodeVisitor nodeVisitor)
        {
            if (this.IsCommentNode)
            {
                return new StyleSheetRuleOrCommentNode((ImportantCommentNode)this.ImportantCommentNode.Accept(nodeVisitor), true);
            }

            return base.Accept(nodeVisitor);
        }
    }
}
