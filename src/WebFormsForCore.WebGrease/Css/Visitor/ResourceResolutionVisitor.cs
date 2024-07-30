// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceResolutionVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Css.Visitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using WebGrease.Activities;
    using WebGrease.Css.Ast;
    using WebGrease.Css.Ast.MediaQuery;
    using WebGrease.Css.Ast.Selectors;

    /// <summary>The resource resolution visitor.</summary>
    public class ResourceResolutionVisitor : NodeTransformVisitor
    {
        /// <summary>The resources.</summary>
        private readonly IEnumerable<IDictionary<string, string>> resources;

        private static char[] numberChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static char[] hexChars = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'A', 'B', 'C', 'D', 'E', 'F' };

        /// <summary>Initializes a new instance of the <see cref="ResourceResolutionVisitor"/> class.</summary>
        /// <param name="resources">The resources.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Create custom classes in  a later iteration.")]
        public ResourceResolutionVisitor(IEnumerable<IDictionary<string, string>> resources)
        {
            if (resources == null)
            {
                throw new ArgumentNullException("resources");
            }

            if (!resources.Any())
            {
                throw new ArgumentException("The resources should have at least 1 item.");
            }

            this.resources = resources;
        }

        /// <summary>The <see cref="HashClassAtNameAttribPseudoNegationNode"/> visit implementation</summary>
        /// <param name="hashClassAtNameAttribPseudoNegationNode">The hash class attrib pseudo negation node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitHashClassAtNameAttribPseudoNegationNode(HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
        {
            if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.ReplacementToken))
            {
                var newValue = ReplaceTokens(hashClassAtNameAttribPseudoNegationNode.ReplacementToken, this.resources);

                if (newValue.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {
                    return new HashClassAtNameAttribPseudoNegationNode(newValue, null, null, null, null, null, null);
                }

                if (newValue.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    return new HashClassAtNameAttribPseudoNegationNode(null, newValue, null, null, null, null, null);
                }

                if (newValue.StartsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    return new HashClassAtNameAttribPseudoNegationNode(null, newValue, null, null, null, null, null);
                }

                return new HashClassAtNameAttribPseudoNegationNode(null, null, newValue, null, null, null, null);
            }

            return base.VisitHashClassAtNameAttribPseudoNegationNode(hashClassAtNameAttribPseudoNegationNode);
        }

        /// <summary>The <see cref="Ast.TermNode"/> visit implementation</summary>
        /// <param name="termNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTermNode(TermNode termNode)
        {
            if (!string.IsNullOrWhiteSpace(termNode.ReplacementTokenBasedValue))
            {
                var newValue = ReplaceTokens(termNode.ReplacementTokenBasedValue, this.resources);
                return CreateTermNode(termNode, newValue);
            }

            if (HasTokens(termNode.StringBasedValue))
            {
                var newValue = ReplaceTokens(termNode.StringBasedValue, this.resources);
                return CreateTermNode(termNode, newValue);
            }

            return base.VisitTermNode(termNode);
        }

        private static AstNode CreateTermNode(TermNode termNode, string newValue)
        {
            newValue = newValue.Trim();
            if (IsNumberBasedValue(newValue))
            {
                return new TermNode(termNode.UnaryOperator, newValue, null, null, null, null);
            }

            if (IsHexColor(newValue))
            {
                return new TermNode(termNode.UnaryOperator, null, null, newValue, null, null);
            }

            return new TermNode(termNode.UnaryOperator, null, newValue, null, null, null);
        }

        private static bool IsNumberBasedValue(string newValue)
        {
            newValue = newValue.TrimStart('-');
            return newValue != null && newValue.Length > 0 && IsNumber(newValue[0]);
        }

        private static bool IsNumber(char c)
        {
            return numberChars.Contains(c);
        }

        private static bool IsHexColor(string newValue)
        {
            return newValue != null && newValue.Length > 3 && newValue[0] == '#' && IsHexColorValue(newValue.Substring(1));
        }

        private static bool IsHexColorValue(string value)
        {
            return value.All(hexChars.Contains);
        }

        /// <summary>The <see cref="Ast.DeclarationNode"/> visit implementation</summary>
        /// <param name="declarationNode">The declaration AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
        {
            if (HasTokens(declarationNode.Property))
            {
                return new DeclarationNode(
                    ReplaceTokens(declarationNode.Property, this.resources),
                    declarationNode.ExprNode.Accept(this) as ExprNode,
                    declarationNode.Prio,
                    declarationNode.ImportantComments);
            }

            return base.VisitDeclarationNode(declarationNode);
        }

        /// <summary>The <see cref="MediaExpressionNode"/> visit implementation</summary>
        /// <param name="mediaExpressionNode">The media expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode)
        {
            if (HasTokens(mediaExpressionNode.MediaFeature))
            {
                return new MediaExpressionNode(
                        ReplaceTokens(mediaExpressionNode.MediaFeature, this.resources),
                        mediaExpressionNode.ExprNode.Accept(this) as ExprNode);
            }

            return base.VisitMediaExpressionNode(mediaExpressionNode);
        }

        /// <summary>The has tokens.</summary>
        /// <param name="stringBasedValue">The string based value.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private static bool HasTokens(string stringBasedValue)
        {
            return !string.IsNullOrWhiteSpace(stringBasedValue) && stringBasedValue.Contains("%");
        }

        /// <summary>The replace tokens.</summary>
        /// <param name="value">The value.</param>
        /// <param name="resources">The resources.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string ReplaceTokens(string value, IEnumerable<IDictionary<string, string>> resources)
        {
            return ResourcesResolver.LocalizationResourceKeyRegex.Replace(
                value,
                match =>
                {
                    var key = match.Result("$1");
                    foreach (var resource in resources)
                    {
                        string newValue;
                        if (resource.TryGetValue(key, out newValue))
                        {
                            if (newValue.Contains("%"))
                            {
                                newValue = ReplaceTokens(newValue, resources);
                            }

                            return newValue;
                        }
                    }

                    return match.Value;
                });
        }
    }
}