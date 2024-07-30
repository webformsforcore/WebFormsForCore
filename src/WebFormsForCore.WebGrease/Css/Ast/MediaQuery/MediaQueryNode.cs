// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaQueryNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   media_query
//   : [ONLY | NOT]? S* media_type S* [ AND S* expression ]*
//   | expression [ AND S* expression ]*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.MediaQuery
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>media_query
    /// : [ONLY | NOT]? S* media_type S* [ AND S* expression ]*
    /// | expression [ AND S* expression ]*
    /// ;</summary>
    public sealed class MediaQueryNode : AstNode
    {
        /// <summary>Initializes a new instance of the <see cref="MediaQueryNode"/> class.</summary>
        /// <param name="onlyText">The only text.</param>
        /// <param name="notText">The not text.</param>
        /// <param name="mediaType">The media type.</param>
        /// <param name="mediaExpressions">The media expressions.</param>
        public MediaQueryNode(string onlyText, string notText, string mediaType, ReadOnlyCollection<MediaExpressionNode> mediaExpressions)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(mediaType) || (mediaExpressions != null && mediaExpressions.Count > 0));

            this.OnlyText = onlyText;
            this.NotText = notText;
            this.MediaType = mediaType;
            this.MediaExpressions = mediaExpressions ?? new List<MediaExpressionNode>(0).AsReadOnly();
        }

        /// <summary>Gets the only text.</summary>
        public string OnlyText { get; private set; }

        /// <summary>Gets the not text.</summary>
        public string NotText { get; private set; }

        /// <summary>Gets the media type.</summary>
        public string MediaType { get; private set; }

        /// <summary>Gets the media expressions.</summary>
        public ReadOnlyCollection<MediaExpressionNode> MediaExpressions { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitMediaQueryNode(this);
        }
    }
}