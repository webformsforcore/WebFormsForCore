// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CssLexer.g3.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The css lexer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>The css lexer.</summary>
    public partial class CssLexer
    {
        /// <summary>
        /// The regex for comments 
        /// </summary>
        private static readonly Regex CommentsRegex = new Regex(@"(/\*.*\*/)", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// The regex for space with in url segment
        /// </summary>
        private static readonly Regex UrlWhitespaceRegex = new Regex(@"^url\(\s*(.*)\s*\)$", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Removes the comments from the string.
        /// </summary>
        /// <param name="text">
        /// The text which need to be cleaned up.
        /// </param>
        /// <returns>
        /// The comment free text.
        /// </returns>
        private static string RemoveComments(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? text : CommentsRegex.Replace(text, string.Empty);
        }

        /// <summary>
        /// Removes the url whitespace from the edges.
        /// </summary>
        /// <param name="text">
        /// The text which need to be cleaned up.
        /// </param>
        /// <returns>
        /// The url with no whitespaces on edges.
        /// </returns>
        private static string RemoveUrlEdgeWhitespaces(string text)
        {
            var match = UrlWhitespaceRegex.Match(text);
            
            string urlMatch;
            if (match.Success && !string.IsNullOrWhiteSpace(urlMatch = match.Result("$1")))
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}", CssConstants.Url, CssConstants.OpenRoundBracket, urlMatch.Trim(), CssConstants.CloseRoundBracket);
            }

            return text;
        }
    }
}
