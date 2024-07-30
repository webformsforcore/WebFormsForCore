// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorOptimizationVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation of CSS color optimization visitor. It
//   visits all AST nodes for CSS files and optimizes for colors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Ast;
    using Extensions;

    /// <summary>Provides the implementation of CSS float optimization visitor. It
    /// visits all AST nodes for CSS files and optimizes for colors.</summary>
    public sealed class ColorOptimizationVisitor : NodeTransformVisitor
    {
        /// <summary>
        /// The compiled regular expression for identifying numbers with units
        /// </summary>
        private static readonly Regex NumberBasedValue = new Regex(@"^(([0-9]*)(\.[0-9]+)?)([a-z%]*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches 6-digit RGB color value where both r digits are the same, both
        /// g digits are the same, and both b digits are the same (but r, g, and b
        /// values are not necessarily the same). Used to identify #rrggbb values
        /// that can be collapsed to #rgb
        /// </summary>
        private static readonly Regex ColorGroupCapture = new Regex(@"^\#(?<r>[0-9a-f])\k<r>(?<g>[0-9a-f])\k<g>(?<b>[0-9a-f])\k<b>$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>The <see cref="Ast.TermNode"/> visit implementation</summary>
        /// <param name="termNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "This is by design.")]
        public override AstNode VisitTermNode(TermNode termNode)
        {
            if (termNode == null)
            {
                throw new ArgumentNullException("termNode");
            }

            var hexColor = termNode.Hexcolor;
            var funcNode = termNode.FunctionNode;
            if (funcNode != null)
            {
                // if it's an rgb( function, then we might want to convert it to #RRGGBB.
                // we don't want to recurse inside a function node if it's *not* rgb(
                // because some functions might not allow color hex parameters to be shortened from 
                // their #RRGGBB source forms to #RGB. so don't recurse this visitor into funcNode.
                if (string.Compare(funcNode.FunctionName, CssConstants.Rgb, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // convert to #RRGGBB
                    int red;
                    int green;
                    int blue;
                    if (TryGetRgb(funcNode.ExprNode, out red, out green, out blue))
                    {
                        // replace the term's function node with a hex-color string
                        funcNode = null;
                        hexColor = string.Format(CultureInfo.InvariantCulture, "#{0:x2}{1:x2}{2:x2}", red, green, blue);
                    }
                }
            }
            
            if (!string.IsNullOrWhiteSpace(hexColor))
            {
                var match = ColorGroupCapture.Match(hexColor);
                if (match.Success)
                {
                    hexColor = string.Format(CultureInfo.InvariantCulture, "#{0}{1}{2}", match.Result("${r}"), match.Result("${g}"), match.Result("${b}"));
                }

                hexColor = hexColor.ToLowerInvariant();
            }

            return new TermNode(termNode.UnaryOperator, termNode.NumberBasedValue, termNode.StringBasedValue, hexColor, funcNode, termNode.ImportantComments);
        }

        /// <summary>
        /// Try to get three integer numeric RGB values from the given expression.
        /// If the values are PERCENT values, returns the integer representation where 100% == 255.
        /// Ignore any values that are not integers or percentages, or if there are not exactly
        /// three terms in the expression, separated by commas.
        /// </summary>
        /// <param name="exprNode">expression should be three numeric values separated by commas</param>
        /// <param name="red">integer red value, 0-255</param>
        /// <param name="green">integer green value, 0-255</param>
        /// <param name="blue">integer blue value, 0-255</param>
        /// <returns>true if successful; false otherwise</returns>
        private static bool TryGetRgb(ExprNode exprNode, out int red, out int green, out int blue)
        {
            // zero them out, then try getting each one as an integer value, converting
            // percentages to intergers between 0 and 255.
            red = green = blue = 0;
            return IsThreeNumberArguments(exprNode)
                && TryGetColorFragment(exprNode.TermNode, out red)
                && TryGetColorFragment(exprNode.TermsWithOperators[0].TermNode, out green)
                && TryGetColorFragment(exprNode.TermsWithOperators[1].TermNode, out blue);
        }

        /// <summary>
        /// Check if the given expression node consists of three comma-separated numeric terms
        /// </summary>
        /// <param name="exprNode">expression node to check</param>
        /// <returns>true if expression represents three comma-separated numeric terms; false otherwise</returns>
        private static bool IsThreeNumberArguments(ExprNode exprNode)
        {
            return exprNode != null
                && IsNumberTerm(exprNode.TermNode)
                && exprNode.TermsWithOperators != null
                && exprNode.TermsWithOperators.Count == 2
                && IsCommaNumber(exprNode.TermsWithOperators[0])
                && IsCommaNumber(exprNode.TermsWithOperators[1]);
        }

        /// <summary>
        /// Check if the given term is a numeric value
        /// </summary>
        /// <param name="termNode">term to check</param>
        /// <returns>returns true if the given term is a numeric value; false otherwise</returns>
        private static bool IsNumberTerm(TermNode termNode)
        {
            return termNode != null && !string.IsNullOrWhiteSpace(termNode.NumberBasedValue);
        }

        /// <summary>
        /// Check if the given term with operator is a numeric value and a comma (respectively)
        /// </summary>
        /// <param name="termWithOperatorNode">the term with operator node to check</param>
        /// <returns>true if a numeric term with a comma; false otherwise</returns>
        private static bool IsCommaNumber(TermWithOperatorNode termWithOperatorNode)
        {
            // must be a comma for the operator, and a number-based value for the term
            return termWithOperatorNode != null
                && termWithOperatorNode.Operator == ","
                && IsNumberTerm(termWithOperatorNode.TermNode);
        }

        /// <summary>
        /// Try to get the integer color fragement from the given term.
        /// Must be an integer number or a percentage. Percentages are converted to
        /// integer where 100% == 255.
        /// Integers must also be within the 0 to 255 range (inclusive) to be successful.
        /// </summary>
        /// <param name="termNode">term to convert to integer.</param>
        /// <param name="fragment">integer value to return</param>
        /// <returns>true if successful; false otherwise</returns>
        private static bool TryGetColorFragment(TermNode termNode, out int fragment)
        {
            var success = false;
            fragment = 0;

            // we've already verified that it's a non-null numeric value
            var match = NumberBasedValue.Match(termNode.NumberBasedValue);
            if (match != null)
            {
                // no units OR percentage is okay; any other unit fails
                var units = match.Result("$4");
                if (string.IsNullOrWhiteSpace(units))
                {
                    // no units
                    // if there is no fractional portion, then convert it directly to int.
                    // if there is a fraction - ignore it. Only supposed to be integers and percentages,
                    // so it the author deviates from that, just leave it as-is because it might
                    // be some sort of cross-browser trick or something.
                    if (string.IsNullOrWhiteSpace(match.Result("$3")))
                    {
                        // integer
                        // should parse properly because it's already a verified numeric value, but
                        // it should also be between 0 and 255, or we ignore it
                        success = int.TryParse(match.Result("$2"), out fragment)
                            && 0 <= fragment && fragment <= 255;
                    }
                }
                else if (string.CompareOrdinal(units, "%") == 0)
                {
                    // percentage
                    // get the float value of the percentage, then convert 100% as 255
                    // don't worry about exceptions because we've already parsed this as a valid numeric value.
                    // and verify the result is within 0 and 255 (otherwise ignore it)
                    fragment = (int)Math.Round(match.Result("$1").ParseFloat() / 100d * 255d, 0);
                    success = 0 <= fragment && fragment <= 255;
                }
            }

            return success;
        }
    }
}