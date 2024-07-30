// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundImage.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents the Css "background-image" declaration
//   Example:
//   #selector
//   {
//   background-image: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif);
//   }
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis.PropertyModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Ast;
    using Extensions;
    using LogModel;

    using WebGrease.Extensions;

    /// <summary>Represents the Css "background-image" declaration
    /// Example:
    /// #selector
    /// {
    ///    background-image: url(../../i/02/3118D8F3781159C8341246BBF2B4CA.gif);
    /// }</summary>
    internal sealed class BackgroundImage
    {
        /// <summary>The url reg ex.</summary>
        internal static readonly string UrlRegEx = @"url\((?<quote>[""']?)\s*((hash\(.*?\))|(%?([-./\w_]+)(\:\w*)?%?))\s*\k<quote>\)";

        /// <summary>
        /// The compiled regular expression for identifying multiple urls
        /// </summary>
        private static readonly Regex MultipleUrlsRegex = new Regex(UrlRegEx, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// The compiled regular expression for identifying exact url
        /// We don't want to match the data uri based images for spriting.
        /// </summary>
        private static readonly Regex UrlRegex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}$", UrlRegEx), RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the BackgroundImage class
        /// </summary>
        internal BackgroundImage()
        {
        }

        /// <summary>Initializes a new instance of the BackgroundImage class</summary>
        /// <param name="declarationNode">The declaration node</param>
        internal BackgroundImage(DeclarationNode declarationNode)
        {
            if (declarationNode == null)
            {
                throw new ArgumentNullException("declarationNode");
            }

            this.DeclarationNode = declarationNode;
            var expr = declarationNode.ExprNode;

            this.ParseTerm(expr.TermNode);
            expr.TermsWithOperators.ForEach(this.ParseTermWithOperator);
        }

        /// <summary>
        /// Gets the declaration node
        /// </summary>
        public DeclarationNode DeclarationNode { get; private set; }

        /// <summary>
        /// Gets the url term node
        /// </summary>
        internal TermNode UrlTermNode { get; private set; }

        /// <summary>
        /// Gets the image url
        /// </summary>
        internal string Url { get; private set; }

        /// <summary>Determines if there are multiple urls in the declaration.</summary>
        /// <param name="text">The declaration text.</param>
        /// <returns>True if multiple urls are present.</returns>
        internal static bool HasMultipleUrls(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            return MultipleUrlsRegex.Matches(text).Count > 1 && text.IndexOf("background", StringComparison.OrdinalIgnoreCase) != -1;
        }

        /// <summary>Matches the url pattern and returns the value of url term</summary>
        /// <param name="termNode">The term node which contains the url pattern</param>
        /// <param name="url">The url value which is found in the term node</param>
        /// <returns>True if the url is found</returns>
        internal static bool TryGetUrl(TermNode termNode, out string url)
        {
            if (termNode != null &&
                !string.IsNullOrWhiteSpace(termNode.StringBasedValue))
            {
                var termValue = termNode.StringBasedValue;
                var match = UrlRegex.Match(termValue);

                if (match.Success &&
                    match.Groups.Count > 2 &&
                    !string.IsNullOrWhiteSpace(url = match.Groups[1].Value))
                {
                    return true;
                }
            }

            url = null;
            return false;
        }

        /// <summary>Verify that url has some value</summary>
        /// <param name="parent">The parent AST node</param>
        /// <param name="imageReferencesToIgnore">The image reference to ignore</param>
        /// <param name="imageAssemblyAnalysisLog">The logging object</param>
        /// <param name="shouldIgnore">The result of scan if we should ignore the image reference</param>
        /// <returns>True if px units are used</returns>
        internal bool VerifyBackgroundUrl(AstNode parent, HashSet<string> imageReferencesToIgnore, ImageAssemblyAnalysisLog imageAssemblyAnalysisLog, out bool shouldIgnore)
        {
            shouldIgnore = false;
            if (string.IsNullOrWhiteSpace(this.Url))
            {
                imageAssemblyAnalysisLog.SafeAdd(parent, this.Url, FailureReason.NoUrl);
                return false;
            }

            if (imageReferencesToIgnore != null)
            {
                var url = this.Url;
                var fullImageUrl = url.NormalizeUrl();

                if (imageReferencesToIgnore.Contains(fullImageUrl))
                {
                    // Log diagnostics
                    imageAssemblyAnalysisLog.SafeAdd(parent, this.Url, FailureReason.IgnoreUrl);
                    shouldIgnore = true;
                    return false;
                }
            }

            return true;
        }

        /// <summary>Parses the term AST node</summary>
        /// <param name="termNode">The AST to parse</param>
        internal void ParseTerm(TermNode termNode)
        {
            if (termNode == null)
            {
                return;
            }

            string url;
            if (!TryGetUrl(termNode, out url))
            {
                return;
            }

            // Update the properties
            this.UrlTermNode = termNode;
            this.Url = url;
        }

        /// <summary>Parses the termwithoperator AST node</summary>
        /// <param name="termWithOperatorNode">The AST to parse</param>
        internal void ParseTermWithOperator(TermWithOperatorNode termWithOperatorNode)
        {
            if (termWithOperatorNode == null)
            {
                return;
            }

            this.ParseTerm(termWithOperatorNode.TermNode);
        }

        /// <summary>Updates the term for url</summary>
        /// <param name="originalTermNode">The original term node</param>
        /// <param name="updatedTermNode">The new term node</param>
        /// <param name="updatedUrl">The new url</param>
        /// <returns>True if the term is updated</returns>
        internal bool UpdateTermForUrl(TermNode originalTermNode, out TermNode updatedTermNode, string updatedUrl)
        {
            if (originalTermNode == this.UrlTermNode)
            {
                updatedUrl = string.Format(CultureInfo.CurrentUICulture, ImageAssembleConstants.UrlTerm, updatedUrl);

                // Create a term with new assembled image url
                updatedTermNode = new TermNode(originalTermNode.UnaryOperator, originalTermNode.NumberBasedValue, updatedUrl, originalTermNode.Hexcolor, originalTermNode.FunctionNode, originalTermNode.ImportantComments);
                return true;
            }

            updatedTermNode = originalTermNode;
            return false;
        }

        /// <summary>Updates the background image node with new url</summary>
        /// <param name="updatedUrl">The updated url</param>
        /// <returns>The new declaration node with updated values</returns>
        internal DeclarationNode UpdateBackgroundImageNode(string updatedUrl)
        {
            if (this.DeclarationNode == null)
            {
                return null;
            }

            var originalExpr = this.DeclarationNode.ExprNode;
            var originalTermNode = originalExpr.TermNode;

            TermNode newBackgroundImageTermNode;

            // Try update for url
            if (this.UpdateTermForUrl(originalTermNode, out newBackgroundImageTermNode, updatedUrl))
            {
                // No need to update the term with operators since there is only url element allowed in the expression
                // which is primary term.
                return new DeclarationNode(this.DeclarationNode.Property, new ExprNode(newBackgroundImageTermNode, originalExpr.TermsWithOperators, originalExpr.ImportantComments), this.DeclarationNode.Prio, this.DeclarationNode.ImportantComments);
            }

            return this.DeclarationNode;
        }
    }
}