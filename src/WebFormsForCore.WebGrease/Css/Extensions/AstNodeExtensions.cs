// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AstNodeExtensions.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   AstNodeExtensions Class - Provides the extension on AstNode types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Extensions
{
    using System.Linq;
    using System.Text;
    using Ast;
    using Visitor;

    using WebGrease.Css.Ast.MediaQuery;

    /// <summary>AstNodeExtensions Class - Provides the extension on AstNode types</summary>
    public static class AstNodeExtensions
    {
        /// <summary>Extension method for pretty print of an AST node</summary>
        /// <param name="node">The ast node</param>
        /// <returns>The pretty string representation</returns>
        public static string PrettyPrint(this AstNode node)
        {
            return node == null ? string.Empty : PrintVisitor.Print(node, true);
        }

        /// <summary>Extension method for minification print of an AST node</summary>
        /// <param name="node">The ast node</param>
        /// <returns>The minified string representation</returns>
        public static string MinifyPrint(this AstNode node)
        {
            return node == null ? string.Empty : PrintVisitor.Print(node, false);
        }

        /// <summary>
        /// Prints the @media selector for the media node.
        /// </summary>
        /// <param name="node">The media node</param>
        /// <returns>The @media .... string.</returns>
        public static string PrintSelector(this MediaNode node)
        {
            return CssConstants.Media + string.Join(",", node.MediaQueries.Select(mq => mq.MinifyPrint()));
        }

        /// <summary>Computes the selectors hash from a ruleset</summary>
        /// <param name="rulesetNode">The ruleset node from which the selector need to be computed</param>
        /// <returns>String format</returns>
        internal static string PrintSelector(this RulesetNode rulesetNode)
        {
            if (rulesetNode == null)
            {
                return string.Empty;
            }

            var rulesetBuilder = new StringBuilder();
            rulesetNode.SelectorsGroupNode.SelectorNodes.ForEach((selector, last) =>
            {
                rulesetBuilder.Append(selector.MinifyPrint());
                if (!last)
                {
                    rulesetBuilder.Append(CssConstants.Comma);
                }
            });
            return rulesetBuilder.ToString();
        }
    }
}
