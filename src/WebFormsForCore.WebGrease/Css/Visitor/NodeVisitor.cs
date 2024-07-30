// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the implementation for AstNode visitor
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using Ast;
    using Ast.Animation;
    using Ast.MediaQuery;
    using Ast.Selectors;

    /// <summary>Provides the implementation for AstNode visitor</summary>
    public abstract class NodeVisitor
    {
        /// <summary>The <see cref="Ast.StyleSheetNode"/> visit implementation</summary>
        /// <param name="styleSheet">The styleSheet AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
        {
            return styleSheet;
        }

        /// <summary>The <see cref="Ast.ImportNode"/> visit implementation</summary>
        /// <param name="importNode">The import AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitImportNode(ImportNode importNode)
        {
            return importNode;
        }

        /// <summary>The <see cref="Ast.RulesetNode"/> visit implementation</summary>
        /// <param name="rulesetNode">The ruleset AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ruleset")]
        public virtual AstNode VisitRulesetNode(RulesetNode rulesetNode)
        {
            return rulesetNode;
        }

        /// <summary>The <see cref="MediaNode"/> visit implementation</summary>
        /// <param name="mediaNode">The media AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitMediaNode(MediaNode mediaNode)
        {
            return mediaNode;
        }

        /// <summary>The <see cref="Ast.PageNode"/> visit implementation</summary>
        /// <param name="pageNode">The page AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitPageNode(PageNode pageNode)
        {
            return pageNode;
        }

        /// <summary>The <see cref="Ast.Selectors.AttribNode"/> visit implementation</summary>
        /// <param name="attrib">The attrib AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitAttribNode(AttribNode attrib)
        {
            return attrib;
        }

        /// <summary>The <see cref="Ast.Selectors.AttribOperatorAndValueNode"/> visit implementation</summary>
        /// <param name="attribOperatorAndValueNode">The attribOperatorAndValue AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitAttribOperatorAndValueNode(AttribOperatorAndValueNode attribOperatorAndValueNode)
        {
            return attribOperatorAndValueNode;
        }

        /// <summary>The <see cref="Ast.DeclarationNode"/> visit implementation</summary>
        /// <param name="declarationNode">The declaration AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitDeclarationNode(DeclarationNode declarationNode)
        {
            return declarationNode;
        }

        /// <summary>The <see cref="Ast.ExprNode"/> visit implementation</summary>
        /// <param name="exprNode">The expr AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expr")]
        public virtual AstNode VisitExprNode(ExprNode exprNode)
        {
            return exprNode;
        }

        /// <summary>The <see cref="Ast.FunctionNode"/> visit implementation</summary>
        /// <param name="functionNode">The function AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitFunctionNode(FunctionNode functionNode)
        {
            return functionNode;
        }

        /// <summary>The <see cref="Ast.Selectors.PseudoNode"/> visit implementation</summary>
        /// <param name="pseudoNode">The pseudo AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitPseudoNode(PseudoNode pseudoNode)
        {
            return pseudoNode;
        }

        /// <summary>The <see cref="Ast.Selectors.SelectorNode"/> visit implementation</summary>
        /// <param name="selectorNode">The selector AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitSelectorNode(SelectorNode selectorNode)
        {
            return selectorNode;
        }

        /// <summary>The <see cref="Ast.TermNode"/> visit implementation</summary>
        /// <param name="termNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitTermNode(TermNode termNode)
        {
            return termNode;
        }

        /// <summary>The <see cref="Ast.TermWithOperatorNode"/> visit implementation</summary>
        /// <param name="termWithOperatorNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode)
        {
            return termWithOperatorNode;
        }

        /// <summary>The <see cref="FunctionalPseudoNode"/> visit implementation</summary>
        /// <param name="functionalPseudoNode">The functional pseudo node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitFunctionalPseudoNode(FunctionalPseudoNode functionalPseudoNode)
        {
            return functionalPseudoNode;
        }

        /// <summary>The <see cref="HashClassAtNameAttribPseudoNegationNode"/> visit implementation</summary>
        /// <param name="hashClassAtNameAttribPseudoNegationNode">The hash class attrib pseudo negation node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitHashClassAtNameAttribPseudoNegationNode(HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
        {
            return hashClassAtNameAttribPseudoNegationNode;
        }

        /// <summary>The <see cref="SelectorNamespacePrefixNode"/> visit implementation</summary>
        /// <param name="selectorNamespacePrefixNode">The namespace prefix node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitSelectorNamespacePrefixNode(SelectorNamespacePrefixNode selectorNamespacePrefixNode)
        {
            return selectorNamespacePrefixNode;
        }

        /// <summary>The <see cref="NegationArgNode"/> visit implementation</summary>
        /// <param name="negationArgNode">The negation arg node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitNegationArgNode(NegationArgNode negationArgNode)
        {
            return negationArgNode;
        }

        /// <summary>The <see cref="NegationNode"/> visit implementation</summary>
        /// <param name="negationNode">The negation node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitNegationNode(NegationNode negationNode)
        {
            return negationNode;
        }

        /// <summary>The <see cref="SelectorExpressionNode"/> visit implementation</summary>
        /// <param name="selectorExpressionNode">The selector expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitSelectorExpressionNode(SelectorExpressionNode selectorExpressionNode)
        {
            return selectorExpressionNode;
        }

        /// <summary>The <see cref="SelectorsGroupNode"/> visit implementation</summary>
        /// <param name="selectorsGroupNode">The selectors group node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitSelectorsGroupNode(SelectorsGroupNode selectorsGroupNode)
        {
            return selectorsGroupNode;
        }

        /// <summary>The <see cref="SimpleSelectorSequenceNode"/> visit implementation</summary>
        /// <param name="simpleSelectorSequenceNode">The simple selector sequence node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitSimpleSelectorSequenceNode(SimpleSelectorSequenceNode simpleSelectorSequenceNode)
        {
            return simpleSelectorSequenceNode;
        }

        /// <summary>The <see cref="TypeSelectorNode"/> visit implementation</summary>
        /// <param name="typeSelectorNode">The type selector node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitTypeSelectorNode(TypeSelectorNode typeSelectorNode)
        {
            return typeSelectorNode;
        }

        /// <summary>The <see cref="UniversalSelectorNode"/> visit implementation</summary>
        /// <param name="universalSelectorNode">The universal selector node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitUniversalSelectorNode(UniversalSelectorNode universalSelectorNode)
        {
            return universalSelectorNode;
        }

        /// <summary>The <see cref="CombinatorSimpleSelectorSequenceNode"/> visit implementation</summary>
        /// <param name="combinatorSimpleSelectorSequenceNode">The combinator simple selector sequence node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Combinator", Justification="Purposely chosen name.")]
        public virtual AstNode VisitCombinatorSimpleSelectorSequenceNode(CombinatorSimpleSelectorSequenceNode combinatorSimpleSelectorSequenceNode)
        {
            return combinatorSimpleSelectorSequenceNode;
        }

        /// <summary>The <see cref="NamespaceNode"/> visit implementation</summary>
        /// <param name="namespaceNode">The namespace node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitNamespaceNode(NamespaceNode namespaceNode)
        {
            return namespaceNode;
        }

        /// <summary>The <see cref="MediaQueryNode"/> visit implementation</summary>
        /// <param name="mediaQueryNode">The media expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitMediaQueryNode(MediaQueryNode mediaQueryNode)
        {
            return mediaQueryNode;
        }

        /// <summary>The <see cref="MediaExpressionNode"/> visit implementation</summary>
        /// <param name="mediaExpressionNode">The media expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode)
        {
            return mediaExpressionNode;
        }

        /// <summary>The <see cref="KeyFramesNode"/> visit implementation</summary>
        /// <param name="keyFramesNode">The key frames node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitKeyFramesNode(KeyFramesNode keyFramesNode)
        {
            return keyFramesNode;
        }

        /// <summary>The <see cref="KeyFramesBlockNode"/> visit implementation</summary>
        /// <param name="keyFramesBlockNode">The key frames block node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitKeyFramesBlockNode(KeyFramesBlockNode keyFramesBlockNode)
        {
            return keyFramesBlockNode;
        }

        /// <summary>The <see cref="DocumentQueryNode"/> visit implementation</summary>
        /// <param name="documentQueryNode">The DocumentQueryNode to visit.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public virtual AstNode VisitDocumentQueryNode(DocumentQueryNode documentQueryNode)
        {
            return documentQueryNode;
        }

        /// <summary>
        /// The <see cref=" ImportantCommentNode"/> visit implementation
        /// </summary>
        /// <param name="commentNode"> The ImportantCommentNode to visit</param>
        /// <returns> The modified AST node if modified otherwise the original node </returns>
        public virtual AstNode VisitImportantCommentNode(ImportantCommentNode commentNode)
        {
            return commentNode;
        }
    }
}