// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeTransformVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The node walker visitor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using System.Linq;
    using Ast;
    using Ast.Animation;
    using Ast.MediaQuery;
    using Ast.Selectors;
    using Extensions;
    using WebGrease.Extensions;

    /// <summary>The node walker visitor.</summary>
    public class NodeTransformVisitor : NodeVisitor
    {
        /// <summary>The <see cref="Ast.StyleSheetNode"/> visit implementation</summary>
        /// <param name="styleSheet">The styleSheet AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
        {
            if (styleSheet == null)
            {
                throw new ArgumentNullException("styleSheet");
            }

            return new StyleSheetNode(
                styleSheet.CharSetString,
                styleSheet.Imports,
                styleSheet.Namespaces,
                styleSheet.StyleSheetRules.Select(
                    styleSheetRule => (StyleSheetRuleNode)styleSheetRule.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="Ast.ImportNode"/> visit implementation</summary>
        /// <param name="importNode">The import AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitImportNode(ImportNode importNode)
        {
            return new ImportNode(
                importNode.AllowedImportDataType,
                importNode.ImportDataValue,
                importNode.MediaQueries.Select(mediaQueryNode => (MediaQueryNode)mediaQueryNode.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="Ast.RulesetNode"/> visit implementation</summary>
        /// <param name="rulesetNode">The ruleset AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
        {
            return new RulesetNode(
                rulesetNode.SelectorsGroupNode.Accept(this) as SelectorsGroupNode,
                rulesetNode.Declarations.Select(declarationNode => (DeclarationNode)declarationNode.Accept(this)).ToSafeReadOnlyCollection(), rulesetNode.ImportantComments);
        }

        /// <summary>The <see cref="MediaNode"/> visit implementation</summary>
        /// <param name="mediaNode">The media AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaNode(MediaNode mediaNode)
        {
            return new MediaNode(
                mediaNode.MediaQueries.Select(ruleset => (MediaQueryNode)ruleset.Accept(this)).ToSafeReadOnlyCollection(),
                mediaNode.Rulesets.Select(ruleset => (RulesetNode)ruleset.Accept(this)).ToSafeReadOnlyCollection(),
                mediaNode.PageNodes.Select(pages => (PageNode)pages.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="Ast.PageNode"/> visit implementation</summary>
        /// <param name="pageNode">The page AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitPageNode(PageNode pageNode)
        {
            return new PageNode(
                pageNode.PseudoPage,
                pageNode.Declarations.Select(declaration => (DeclarationNode)declaration.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="DocumentQueryNode"/> visit implementation</summary>
        /// <param name="documentQueryNode">The document AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitDocumentQueryNode(DocumentQueryNode documentQueryNode)
        {
            return new DocumentQueryNode(
                documentQueryNode.MatchFunctionName,
                documentQueryNode.DocumentSymbol,
                documentQueryNode.Rulesets.Select(ruleset => (RulesetNode)ruleset.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="Ast.Selectors.AttribNode"/> visit implementation</summary>
        /// <param name="attrib">The attrib AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitAttribNode(AttribNode attrib)
        {
            return new AttribNode(
                attrib.SelectorNamespacePrefixNode != null ? (SelectorNamespacePrefixNode)attrib.SelectorNamespacePrefixNode.Accept(this) : null,
                attrib.Ident,
                (AttribOperatorAndValueNode)attrib.OperatorAndValueNode.Accept(this));
        }

        /// <summary>The <see cref="Ast.Selectors.AttribOperatorAndValueNode"/> visit implementation</summary>
        /// <param name="attribOperatorAndValueNode">The attribOperatorAndValue AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitAttribOperatorAndValueNode(AttribOperatorAndValueNode attribOperatorAndValueNode)
        {
            return new AttribOperatorAndValueNode(attribOperatorAndValueNode.AttribOperatorKind, attribOperatorAndValueNode.IdentOrString);
        }

        /// <summary>The <see cref="Ast.DeclarationNode"/> visit implementation</summary>
        /// <param name="declarationNode">The declaration AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
        {
            return new DeclarationNode(declarationNode.Property, (ExprNode)declarationNode.ExprNode.Accept(this), declarationNode.Prio, declarationNode.ImportantComments);
        }

        /// <summary>The <see cref="Ast.ExprNode"/> visit implementation</summary>
        /// <param name="exprNode">The expr AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitExprNode(ExprNode exprNode)
        {
            return new ExprNode(
                (TermNode)exprNode.TermNode.Accept(this), 
                exprNode.TermsWithOperators.Select(termWithOperatorNode => (TermWithOperatorNode)termWithOperatorNode.Accept(this)).ToSafeReadOnlyCollection(),
                exprNode.ImportantComments);
        }

        /// <summary>The <see cref="Ast.FunctionNode"/> visit implementation</summary>
        /// <param name="functionNode">The function AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitFunctionNode(FunctionNode functionNode)
        {
            // function might not have ANY expression within it.
            var exprNode = functionNode.ExprNode != null ? functionNode.ExprNode.Accept(this) : null;
            return new FunctionNode(functionNode.FunctionName, (ExprNode)exprNode);
        }

        /// <summary>The <see cref="Ast.Selectors.PseudoNode"/> visit implementation</summary>
        /// <param name="pseudoNode">The pseudo AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitPseudoNode(PseudoNode pseudoNode)
        {
            return new PseudoNode(
                pseudoNode.NumberOfColons,
                pseudoNode.Ident,
                pseudoNode.FunctionalPseudoNode != null ? (FunctionalPseudoNode)pseudoNode.FunctionalPseudoNode.Accept(this) : null);
        }

        /// <summary>The <see cref="Ast.Selectors.SelectorNode"/> visit implementation</summary>
        /// <param name="selectorNode">The selector AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorNode(SelectorNode selectorNode)
        {
            return new SelectorNode(
                (SimpleSelectorSequenceNode)selectorNode.SimpleSelectorSequenceNode.Accept(this),
                selectorNode.CombinatorSimpleSelectorSequenceNodes.Select(combinatorSimpleSelectorSequenceNode => (CombinatorSimpleSelectorSequenceNode)combinatorSimpleSelectorSequenceNode.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="Ast.TermNode"/> visit implementation</summary>
        /// <param name="termNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTermNode(TermNode termNode)
        {
            return new TermNode(termNode.UnaryOperator, termNode.NumberBasedValue, termNode.StringBasedValue, termNode.Hexcolor, (FunctionNode)termNode.FunctionNode.NullSafeAction(nsa => nsa.Accept(this)), termNode.ImportantComments, termNode.ReplacementTokenBasedValue);
        }

        /// <summary>The <see cref="Ast.TermWithOperatorNode"/> visit implementation</summary>
        /// <param name="termWithOperatorNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode)
        {
            return new TermWithOperatorNode(termWithOperatorNode.Operator, (TermNode)termWithOperatorNode.TermNode.Accept(this));
        }

        /// <summary>The <see cref="FunctionalPseudoNode"/> visit implementation</summary>
        /// <param name="functionalPseudoNode">The functional pseudo node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitFunctionalPseudoNode(FunctionalPseudoNode functionalPseudoNode)
        {
            return new FunctionalPseudoNode(
                functionalPseudoNode.FunctionName,
                (SelectorExpressionNode)functionalPseudoNode.SelectorExpressionNode.Accept(this));
        }

        /// <summary>The <see cref="HashClassAtNameAttribPseudoNegationNode"/> visit implementation</summary>
        /// <param name="hashClassAtNameAttribPseudoNegationNode">The hash class attrib pseudo negation node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitHashClassAtNameAttribPseudoNegationNode(HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
        {
            return new HashClassAtNameAttribPseudoNegationNode(
                hashClassAtNameAttribPseudoNegationNode.Hash,
                hashClassAtNameAttribPseudoNegationNode.CssClass,
                hashClassAtNameAttribPseudoNegationNode.ReplacementToken,
                hashClassAtNameAttribPseudoNegationNode.AtName,
                hashClassAtNameAttribPseudoNegationNode.AttribNode != null ? (AttribNode)hashClassAtNameAttribPseudoNegationNode.AttribNode.Accept(this) : null,
                hashClassAtNameAttribPseudoNegationNode.PseudoNode != null ? (PseudoNode)hashClassAtNameAttribPseudoNegationNode.PseudoNode.Accept(this) : null,
                hashClassAtNameAttribPseudoNegationNode.NegationNode != null ? (NegationNode)hashClassAtNameAttribPseudoNegationNode.NegationNode.Accept(this) : null
                );
        }

        /// <summary>The <see cref="SelectorNamespacePrefixNode"/> visit implementation</summary>
        /// <param name="selectorNamespacePrefixNode">The namespace prefix node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorNamespacePrefixNode(SelectorNamespacePrefixNode selectorNamespacePrefixNode)
        {
            return new SelectorNamespacePrefixNode(selectorNamespacePrefixNode.Prefix);
        }

        /// <summary>The <see cref="NegationArgNode"/> visit implementation</summary>
        /// <param name="negationArgNode">The negation arg node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitNegationArgNode(NegationArgNode negationArgNode)
        {
            return new NegationArgNode(
                negationArgNode.TypeSelectorNode != null ? (TypeSelectorNode)negationArgNode.TypeSelectorNode.Accept(this) : null,
                negationArgNode.UniversalSelectorNode != null ? (UniversalSelectorNode)negationArgNode.UniversalSelectorNode.Accept(this) : null,
                negationArgNode.Hash,
                negationArgNode.CssClass,
                negationArgNode.AttribNode != null ? (AttribNode)negationArgNode.AttribNode.Accept(this) : null,
                negationArgNode.PseudoNode != null ? (PseudoNode)negationArgNode.PseudoNode.Accept(this) : null);
        }

        /// <summary>The <see cref="NegationNode"/> visit implementation</summary>
        /// <param name="negationNode">The negation node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitNegationNode(NegationNode negationNode)
        {
            return new NegationNode((NegationArgNode)negationNode.NegationArgNode.Accept(this));
        }

        /// <summary>The <see cref="SelectorExpressionNode"/> visit implementation</summary>
        /// <param name="selectorExpressionNode">The selector expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorExpressionNode(SelectorExpressionNode selectorExpressionNode)
        {
            return new SelectorExpressionNode(selectorExpressionNode.SelectorExpressions);
        }

        /// <summary>The <see cref="SelectorsGroupNode"/> visit implementation</summary>
        /// <param name="selectorsGroupNode">The selectors group node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorsGroupNode(SelectorsGroupNode selectorsGroupNode)
        {
            return new SelectorsGroupNode(selectorsGroupNode.SelectorNodes.Select(selectorNode => (SelectorNode)selectorNode.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="SimpleSelectorSequenceNode"/> visit implementation</summary>
        /// <param name="simpleSelectorSequenceNode">The simple selector sequence node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSimpleSelectorSequenceNode(SimpleSelectorSequenceNode simpleSelectorSequenceNode)
        {
            return new SimpleSelectorSequenceNode(
                simpleSelectorSequenceNode.TypeSelectorNode != null ? (TypeSelectorNode)simpleSelectorSequenceNode.TypeSelectorNode.Accept(this) : null,
                simpleSelectorSequenceNode.UniversalSelectorNode != null ? (UniversalSelectorNode)simpleSelectorSequenceNode.UniversalSelectorNode.Accept(this) : null,
                simpleSelectorSequenceNode.Separator,
                simpleSelectorSequenceNode.HashClassAttribPseudoNegationNodes.Select(hashClassAtNameAttribPseudoNegationNode => (HashClassAtNameAttribPseudoNegationNode)hashClassAtNameAttribPseudoNegationNode.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="TypeSelectorNode"/> visit implementation</summary>
        /// <param name="typeSelectorNode">The type selector node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTypeSelectorNode(TypeSelectorNode typeSelectorNode)
        {
            return new TypeSelectorNode(
                typeSelectorNode.SelectorNamespacePrefixNode != null ? (SelectorNamespacePrefixNode)typeSelectorNode.SelectorNamespacePrefixNode.Accept(this) : null,
                typeSelectorNode.ElementName);
        }

        /// <summary>The <see cref="UniversalSelectorNode"/> visit implementation</summary>
        /// <param name="universalSelectorNode">The universal selector node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitUniversalSelectorNode(UniversalSelectorNode universalSelectorNode)
        {
            return new UniversalSelectorNode(universalSelectorNode.SelectorNamespacePrefixNode != null ? (SelectorNamespacePrefixNode)universalSelectorNode.SelectorNamespacePrefixNode.Accept(this) : null);
        }

        /// <summary>The <see cref="CombinatorSimpleSelectorSequenceNode"/> visit implementation</summary>
        /// <param name="combinatorSimpleSelectorSequenceNode">The combinator simple selector sequence node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitCombinatorSimpleSelectorSequenceNode(CombinatorSimpleSelectorSequenceNode combinatorSimpleSelectorSequenceNode)
        {
            return new CombinatorSimpleSelectorSequenceNode(
                combinatorSimpleSelectorSequenceNode.Combinator,
                (SimpleSelectorSequenceNode)combinatorSimpleSelectorSequenceNode.SimpleSelectorSequenceNode.Accept(this));
        }

        /// <summary>The <see cref="NamespaceNode"/> visit implementation</summary>
        /// <param name="namespaceNode">The namespace node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitNamespaceNode(NamespaceNode namespaceNode)
        {
            return new NamespaceNode(namespaceNode.Prefix, namespaceNode.Value);
        }

        /// <summary>The <see cref="MediaQueryNode"/> visit implementation</summary>
        /// <param name="mediaQueryNode">The media expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaQueryNode(MediaQueryNode mediaQueryNode)
        {
            return new MediaQueryNode(
                mediaQueryNode.OnlyText,
                mediaQueryNode.NotText,
                mediaQueryNode.MediaType,
                mediaQueryNode.MediaExpressions.Select(mediaExpressionNode => (MediaExpressionNode)mediaExpressionNode.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="MediaExpressionNode"/> visit implementation</summary>
        /// <param name="mediaExpressionNode">The media expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode)
        {
            return new MediaExpressionNode(
                mediaExpressionNode.MediaFeature,
                mediaExpressionNode.ExprNode != null ? (ExprNode)mediaExpressionNode.ExprNode.Accept(this) : null);
        }

        /// <summary>The <see cref="KeyFramesNode"/> visit implementation</summary>
        /// <param name="keyFramesNode">The key frames node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitKeyFramesNode(KeyFramesNode keyFramesNode)
        {
            return new KeyFramesNode(
                keyFramesNode.KeyFramesSymbol,
                keyFramesNode.IdentValue,
                keyFramesNode.StringValue,
                keyFramesNode.KeyFramesBlockNodes.Select(keyFramesBlockNode => (KeyFramesBlockNode)keyFramesBlockNode.Accept(this)).ToSafeReadOnlyCollection());
        }

        /// <summary>The <see cref="KeyFramesBlockNode"/> visit implementation</summary>
        /// <param name="keyFramesBlockNode">The key frames block node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitKeyFramesBlockNode(KeyFramesBlockNode keyFramesBlockNode)
        {
            return new KeyFramesBlockNode(
                keyFramesBlockNode.KeyFramesSelectors,
                keyFramesBlockNode.DeclarationNodes.Select(declarationNode => (DeclarationNode)declarationNode.Accept(this)).ToSafeReadOnlyCollection());
        }
    }
}
