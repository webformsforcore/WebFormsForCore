// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaNode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   media
//   : MEDIA_SYM S* media_query_list '{' S* ruleset* '}' S*
//   ;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast.MediaQuery
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Visitor;

    /// <summary>media
    /// : MEDIA_SYM S* media_query_list '{' S* ruleset* '}' S*
    /// ;</summary>
    public sealed class MediaNode : StyleSheetRuleNode
    {
        /// <summary>Initializes a new instance of the MediaNode class</summary>
        /// <param name="mediaQueries">MediaQueries list</param>
        /// <param name="rulesets">Rules list</param>
        /// <param name="pages">Page node list</param>
        public MediaNode(ReadOnlyCollection<MediaQueryNode> mediaQueries, ReadOnlyCollection<RulesetNode> rulesets, ReadOnlyCollection<PageNode> pages)
        {
            Contract.Requires(mediaQueries != null && mediaQueries.Count > 0);

            this.MediaQueries = mediaQueries;
            this.Rulesets = rulesets ?? new List<RulesetNode>(0).AsReadOnly();
            this.PageNodes = pages ?? new List<PageNode>(0).AsReadOnly();
        }

        /// <summary>
        /// Gets MediaQueries
        /// </summary>
        /// <value>MediaQueries list</value>
        public ReadOnlyCollection<MediaQueryNode> MediaQueries { get; private set; }

        /// <summary>
        /// Gets Rules sets
        /// </summary>
        /// <value>Rules List</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rulesets")]
        public ReadOnlyCollection<RulesetNode> Rulesets { get; private set; }

        /// <summary>
        /// Gets Page nodes
        /// </summary>
        /// <value>Page node list</value>
        public ReadOnlyCollection<PageNode> PageNodes { get; private set; }

        /// <summary>Defines an accept operation</summary>
        /// <param name="nodeVisitor">The visitor to invoke</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode Accept(NodeVisitor nodeVisitor)
        {
            return nodeVisitor.VisitMediaNode(this);
        }
    }
}
