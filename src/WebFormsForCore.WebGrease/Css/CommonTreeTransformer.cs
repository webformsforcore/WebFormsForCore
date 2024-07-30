// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonTreeTransformer.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The common tree transformer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime.Tree;
    using Ast;
    using Ast.Animation;
    using Ast.MediaQuery;
    using Ast.Selectors;
    using Extensions;

    /// <summary>The common tree transformer.</summary>
    internal static class CommonTreeTransformer
    {
        /// <summary>The create styleSheet node.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <returns>The styleSheet node.</returns>
        internal static StyleSheetNode CreateStyleSheetNode(CommonTree commonTree)
        {
            Contract.Requires(commonTree != null);

            var styleSheetTree = commonTree.Children(T(CssParser.STYLESHEET)).FirstOrDefault();

            return new StyleSheetNode(
                CreateCharsetNode(styleSheetTree),
                CreateImportNodes(styleSheetTree),
                CreateNamespaceNodes(styleSheetTree),
                CreateStyleSheetRulesNodes(styleSheetTree));
        }

        /// <summary>Creates the charset node.</summary>
        /// <param name="styleSheetTree">The styleSheet tree.</param>
        /// <returns>The charset value.</returns>
        private static string CreateCharsetNode(CommonTree styleSheetTree)
        {
            if (styleSheetTree == null)
            {
                return null;
            }

            var charset = styleSheetTree.Children(T(CssParser.CHARSET)).FirstOrDefault();
            return charset != null ? StringOrUriBasedValue(charset.Children(T(CssParser.STRINGBASEDVALUE)).FirstChildText()) : null;
        }

        /// <summary>Gets the ruleset media page nodes.</summary>
        /// <param name="styleSheetTree">The styleSheet common tree.</param>
        /// <returns>The list of ruleset media page nodes.</returns>
        private static ReadOnlyCollection<StyleSheetRuleNode> CreateStyleSheetRulesNodes(CommonTree styleSheetTree)
        {
            if (styleSheetTree == null)
            {
                return Enumerable.Empty<StyleSheetRuleNode>().ToSafeReadOnlyCollection();
            }

            var ruleSetMediaPageNodes = new List<StyleSheetRuleNode>();
            foreach (var styleSheetChild in styleSheetTree.Children())
            {
                if (styleSheetChild.Text == T(CssParser.RULESET))
                {
                    ruleSetMediaPageNodes.Add(CreateRulesetNode(styleSheetChild));
                }
                else if (styleSheetChild.Text == T(CssParser.MEDIA))
                {
                    ruleSetMediaPageNodes.Add(CreateMediaNode(styleSheetChild));
                }
                else if (styleSheetChild.Text == T(CssParser.PAGE))
                {
                    ruleSetMediaPageNodes.Add(CreatePageNode(styleSheetChild));
                }
                else if (styleSheetChild.Text == T(CssParser.KEYFRAMES))
                {
                    ruleSetMediaPageNodes.Add(CreateKeyFramesNode(styleSheetChild));
                }
                else if (styleSheetChild.Text == T(CssParser.DOCUMENT))
                {
                    ruleSetMediaPageNodes.Add(CreateDocumentQueryNode(styleSheetChild));
                }
                else if (styleSheetChild.Type.Equals(CssParser.IMPORTANT_COMMENTS))
                {
                    ruleSetMediaPageNodes.Add(new StyleSheetRuleOrCommentNode(new ImportantCommentNode(styleSheetChild.Text), true));
                }
            }

            return ruleSetMediaPageNodes.AsReadOnly();
        }

        /// <summary>Gets the import nodes.</summary>
        /// <param name="styleSheetTree">The styleSheet common tree.</param>
        /// <returns>The import nodes.</returns>
        private static ReadOnlyCollection<ImportNode> CreateImportNodes(CommonTree styleSheetTree)
        {
            if (styleSheetTree == null)
            {
                return Enumerable.Empty<ImportNode>().ToSafeReadOnlyCollection();
            }

            return styleSheetTree
            .GrandChildren(T(CssParser.IMPORTS))
            .Select(import =>
            {
                var importChild = import.Children().FirstOrDefault();
                if (importChild != null)
                {
                    var allowedImportData = AllowedImportData.None;
                    string importValue = null;
                    if (importChild.Text == T(CssParser.STRINGBASEDVALUE))
                    {
                        allowedImportData = AllowedImportData.String;
                        importValue = StringOrUriBasedValue(importChild.FirstChildText());
                    }
                    else if (importChild.Text == T(CssParser.URIBASEDVALUE))
                    {
                        allowedImportData = AllowedImportData.Uri;
                        importValue = StringOrUriBasedValue(importChild.FirstChildText());
                    }

                    return new ImportNode(
                        allowedImportData,
                        importValue,
                        import.GrandChildren(T(CssParser.MEDIA_QUERY_LIST)).Select(CreateMediaQueryNode).ToSafeReadOnlyCollection());
                }

                return null;
            }).ToSafeReadOnlyCollection();
        }

        /// <summary>Creates the media query node.</summary>
        /// <param name="mediaQueryTree">The media query tree.</param>
        /// <returns>The media query node.</returns>
        private static MediaQueryNode CreateMediaQueryNode(CommonTree mediaQueryTree)
        {
            return new MediaQueryNode(
                mediaQueryTree.Children(T(CssParser.ONLY_TEXT)).FirstChildText(),
                mediaQueryTree.Children(T(CssParser.NOT_TEXT)).FirstChildText(),
                mediaQueryTree.Children(T(CssParser.MEDIA_TYPE)).FirstChildText(),
                mediaQueryTree.GrandChildren(T(CssParser.MEDIA_EXPRESSIONS)).Select(CreateMediaExpressionNode).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the media expression node.</summary>
        /// <param name="mediaExpressionTree">The media expression tree.</param>
        /// <returns>The media expression node.</returns>
        private static MediaExpressionNode CreateMediaExpressionNode(CommonTree mediaExpressionTree)
        {
            return new MediaExpressionNode(
                mediaExpressionTree.Children(T(CssParser.MEDIA_FEATURE)).FirstChildText(),
                CreateExpressionNode(mediaExpressionTree.Children(T(CssParser.EXPR)).FirstOrDefault()));
        }

        /// <summary>Creates the namespace nodes.</summary>
        /// <param name="styleSheetTree">The styleSheet tree.</param>
        /// <returns>The list of namespace nodes.</returns>
        private static ReadOnlyCollection<NamespaceNode> CreateNamespaceNodes(CommonTree styleSheetTree)
        {
            if (styleSheetTree == null)
            {
                return Enumerable.Empty<NamespaceNode>().ToSafeReadOnlyCollection();
            }

            return styleSheetTree
            .GrandChildren(T(CssParser.NAMESPACES))
            .Select(ns =>
            {
                var value = StringOrUriBasedValue(ns.Children(T(CssParser.STRINGBASEDVALUE)).FirstChildText());
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = StringOrUriBasedValue(ns.Children(T(CssParser.URIBASEDVALUE)).FirstChildText());
                }

                return new NamespaceNode(ns.Children(T(CssParser.NAMESPACE_PREFIX)).FirstChildText(), value);
            }).ToSafeReadOnlyCollection();
        }

        /// <summary>Creates the ruleset nodes.</summary>
        /// <param name="rulesetTree">The list of rulesets.</param>
        /// <returns>The list of ruleset nodes.</returns>
        private static RulesetNode CreateRulesetNode(CommonTree rulesetTree)
        {
            if (rulesetTree == null)
            {
                return null;
            }

            return new RulesetNode(
                CreateSelectorsGroupNode(rulesetTree.GrandChildren(T(CssParser.SELECTORS_GROUP))),
                CreateDeclarationNodes(rulesetTree.GrandChildren(T(CssParser.DECLARATIONS))).ToSafeReadOnlyCollection(),
                CreateImportantCommentNodes(rulesetTree));
        }

        /// <summary>
        /// Create importantComment Nodes
        /// </summary>
        /// <param name="commonTree"> the parent of list of the comment trees</param>
        /// <returns>The list of comment nodes</returns>
        private static ReadOnlyCollection<ImportantCommentNode> CreateImportantCommentNodes(CommonTree commonTree)
        {
            Contract.Requires(!commonTree.Equals(null));
            List<ImportantCommentNode> comments = new List<ImportantCommentNode>();
            foreach (var child in commonTree.Children)
            {
                if (child.Type.Equals(CssParser.IMPORTANT_COMMENTS))
                {
                    comments.Add(new ImportantCommentNode(child.Text));
                }
            }

            return comments.AsReadOnly();
        }

        /// <summary>Creates the media nodes.</summary>
        /// <param name="mediaTree">The list of media tree.</param>
        /// <returns>The list of media nodes.</returns>
        private static MediaNode CreateMediaNode(CommonTree mediaTree)
        {
            if (mediaTree == null)
            {
                return null;
            }

            return new MediaNode(
                    mediaTree.GrandChildren(T(CssParser.MEDIA_QUERY_LIST)).Select(CreateMediaQueryNode).ToSafeReadOnlyCollection(),
                    mediaTree.GrandChildren(T(CssParser.RULESETS)).Select(CreateRulesetNode).ToSafeReadOnlyCollection(),
                    mediaTree.GrandChildren(T(CssParser.PAGE)).Select(CreatePageNode).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the page node.</summary>
        /// <param name="pageTree">The list of page tree.</param>
        /// <returns>The list of page nodes.</returns>
        private static PageNode CreatePageNode(CommonTree pageTree)
        {
            if (pageTree == null)
            {
                return null;
            }

            return new PageNode(
                    string.Join(string.Empty, pageTree.GrandChildren(T(CssParser.PSEUDO_PAGE)).Select(pseudo => pseudo.Text)),
                    CreateDeclarationNodes(pageTree.GrandChildren(T(CssParser.DECLARATIONS))).ToSafeReadOnlyCollection());
        }

        /// <summary>
        /// Creates the document query node.
        /// </summary>
        /// <param name="documentTree">tree with document descendants</param>
        /// <returns>new instance of document query node</returns>
        private static DocumentQueryNode CreateDocumentQueryNode(CommonTree documentTree)
        {
            return new DocumentQueryNode(
                string.Join(string.Empty, documentTree.GrandChildren(T(CssParser.DOCUMENT_MATCHNAME)).Select(_ => _.Text)),
                documentTree.Children(T(CssParser.DOCUMENT_SYMBOL)).FirstChildText(),
                documentTree.GrandChildren(T(CssParser.RULESETS)).Select(CreateRulesetNode).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the key frame node.</summary>
        /// <param name="styleSheetChild">The style sheet child.</param>
        /// <returns>The key frame node.</returns>
        private static KeyFramesNode CreateKeyFramesNode(CommonTree styleSheetChild)
        {
            return new KeyFramesNode(
                styleSheetChild.Children(T(CssParser.KEYFRAMES_SYMBOL)).FirstChildText(),
                styleSheetChild.Children(T(CssParser.IDENTBASEDVALUE)).FirstChildText(),
                StringOrUriBasedValue(styleSheetChild.Children(T(CssParser.STRINGBASEDVALUE)).FirstChildText()),
                styleSheetChild.GrandChildren(T(CssParser.KEYFRAMES_BLOCKS)).Select(CreateKeyFramesBlockNode).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the key frames block node.</summary>
        /// <param name="keyFramesBlockTree">The key frames block tree.</param>
        /// <returns>The key frames block node.</returns>
        private static KeyFramesBlockNode CreateKeyFramesBlockNode(CommonTree keyFramesBlockTree)
        {
            return new KeyFramesBlockNode(
                keyFramesBlockTree.GrandChildren(T(CssParser.KEYFRAMES_SELECTORS)).Select(keyFramesSelector => keyFramesSelector.FirstChildText()).ToSafeReadOnlyCollection(),
                CreateDeclarationNodes(keyFramesBlockTree.GrandChildren(T(CssParser.DECLARATIONS))).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the declaration nodes.</summary>
        /// <param name="declarationTreeNodes">The declaration tree.</param>
        /// <returns>The list of declarations.</returns>
        private static IEnumerable<DeclarationNode> CreateDeclarationNodes(IEnumerable<CommonTree> declarationTreeNodes)
        {
            return declarationTreeNodes.Select(
                declaration => new DeclarationNode(
                    string.Join(string.Empty, declaration.GrandChildren(T(CssParser.PROPERTY)).Select(_ => _.Text)),
                    CreateExpressionNode(declaration.Children(T(CssParser.EXPR)).FirstOrDefault()),
                    declaration.Children(T(CssParser.IMPORTANT)).FirstChildText(), 
                    CreateImportantCommentNodes(declaration)));
        }

        /// <summary>Creates the expression node</summary>
        /// <param name="exprTree">The expression tree.</param>
        /// <returns>The expression node.</returns>
        private static ExprNode CreateExpressionNode(CommonTree exprTree)
        {
            if (exprTree == null)
            {
                return null;
            }

            return new ExprNode(
                CreateTermNode(exprTree.Children(T(CssParser.TERM)).FirstOrDefault()),
                CreateTermWithOperatorsNode(exprTree.GrandChildren(T(CssParser.TERMWITHOPERATORS))).ToSafeReadOnlyCollection(),
                CreateImportantCommentNodes(exprTree));
        }

        /// <summary>Creates the term with operator nodes.</summary>
        /// <param name="termWithOperatorTreeNodes">The term with operator tree.</param>
        /// <returns>The list of term with operators.</returns>
        private static IEnumerable<TermWithOperatorNode> CreateTermWithOperatorsNode(IEnumerable<CommonTree> termWithOperatorTreeNodes)
        {
            return termWithOperatorTreeNodes.Select(termWithOperatorNode =>
            {
                // Operator
                var op = termWithOperatorNode.Children(T(CssParser.OPERATOR)).FirstChildText();

                return new TermWithOperatorNode(op, CreateTermNode(termWithOperatorNode.Children(T(CssParser.TERM)).FirstOrDefault()));
            });
        }

        /// <summary>Creates the term node.</summary>
        /// <param name="termTree">The term tree.</param>
        /// <returns>The term node.</returns>
        private static TermNode CreateTermNode(CommonTree termTree)
        {
            if (termTree == null)
            {
                return null;
            }

            // Unary
            var unaryOperator = termTree.Children(T(CssParser.UNARY)).FirstChildText();

            // Number based value
            var numberBasedValue = termTree.Children(T(CssParser.NUMBERBASEDVALUE)).FirstChildText();

            // Token value
            var replacementTokenBasedValue = termTree.Children(T(CssParser.REPLACEMENTTOKENBASEDVALUE)).FirstChildText();

            // Url based value
            var uriStringOrIdentBasedValue = StringOrUriBasedValue(termTree.Children(T(CssParser.URIBASEDVALUE)).FirstChildText());

            // String based value
            if (string.IsNullOrWhiteSpace(uriStringOrIdentBasedValue))
            {
                uriStringOrIdentBasedValue = StringOrUriBasedValue(termTree.Children(T(CssParser.STRINGBASEDVALUE)).FirstChildText());
            }

            // Ident based value
            if (string.IsNullOrWhiteSpace(uriStringOrIdentBasedValue))
            {
                uriStringOrIdentBasedValue = StringOrUriBasedValue(termTree.Children(T(CssParser.IDENTBASEDVALUE)).FirstChildText());
            }

            // Hex based value
            var hexBasedNode = termTree.Children(T(CssParser.HEXBASEDVALUE)).FirstOrDefault();
            var hexBasedValue = hexBasedNode != null ? hexBasedNode.Children(T(CssParser.HASHIDENTIFIER)).FirstChildText() : null;

            // Comments
            var comments = CreateImportantCommentNodes(termTree);
            
            return new TermNode(unaryOperator, numberBasedValue, uriStringOrIdentBasedValue, hexBasedValue, CreateFunctionNode(termTree.Children(T(CssParser.FUNCTIONBASEDVALUE)).FirstOrDefault()), comments, replacementTokenBasedValue);
        }
        
        /// <summary>Creates the function node.</summary>
        /// <param name="functionTree">The function tree.</param>
        /// <returns>The function node.</returns>
        private static FunctionNode CreateFunctionNode(CommonTree functionTree)
        {
            if (functionTree == null)
            {
                return null;
            }

            return new FunctionNode(
                functionTree.Children(T(CssParser.FUNCTIONNAME)).FirstChildText(),
                CreateExpressionNode(functionTree.Children(T(CssParser.EXPR)).FirstOrDefault()));
        }

        /// <summary>Creates the selector nodes.</summary>
        /// <param name="selectorTreeNodes">The list of selectors.</param>
        /// <returns>The list of selector nodes.</returns>
        private static SelectorsGroupNode CreateSelectorsGroupNode(IEnumerable<CommonTree> selectorTreeNodes)
        {
            return new SelectorsGroupNode(
                selectorTreeNodes.Select(
                selector => new SelectorNode(
                CreateSimpleSelectorSequenceNode(selector.Children(T(CssParser.SIMPLE_SELECTOR_SEQUENCE)).FirstOrDefault()),
                CreateCombinatorSimpleSelectorSequenceNode(selector.GrandChildren(T(CssParser.COMBINATOR_SIMPLE_SELECTOR_SEQUENCES))).ToSafeReadOnlyCollection())).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the combinator simple selector.</summary>
        /// <param name="combinatorSimpleSelectorSequenceTreeNodes">The combinator simple selector sequence nodes.</param>
        /// <returns>The list of selectors.</returns>
        private static IEnumerable<CombinatorSimpleSelectorSequenceNode> CreateCombinatorSimpleSelectorSequenceNode(IEnumerable<CommonTree> combinatorSimpleSelectorSequenceTreeNodes)
        {
            return combinatorSimpleSelectorSequenceTreeNodes.Select(
                combinatorSimpleSelectorSequenceNode =>
                    new CombinatorSimpleSelectorSequenceNode(
                        CreateCombinatorNode(combinatorSimpleSelectorSequenceNode.Children(T(CssParser.COMBINATOR)).FirstOrDefault()),
                        CreateSimpleSelectorSequenceNode(combinatorSimpleSelectorSequenceNode.Children(T(CssParser.SIMPLE_SELECTOR_SEQUENCE)).FirstOrDefault())));
        }

        /// <summary>Creates the combinator.</summary>
        /// <param name="combinatorTree">The combinator tree.</param>
        /// <returns>The combinator value.</returns>
        private static Combinator CreateCombinatorNode(CommonTree combinatorTree)
        {
            var combinator = Combinator.None;
            if (combinatorTree == null)
            {
                return combinator;
            }

            var text = combinatorTree.FirstChildText();
            switch (text)
            {
                case CssConstants.Plus:
                    combinator = Combinator.PlusSign;
                    break;
                case CssConstants.Greater:
                    combinator = Combinator.GreaterThanSign;
                    break;
                case CssConstants.Tilde:
                    combinator = Combinator.Tilde;
                    break;
                case CssConstants.Whitespace:
                    combinator = GetWhitespaceCount(combinatorTree) > 0 ? Combinator.SingleSpace : Combinator.ZeroSpace;
                    break;
                default:
                    throw new AstException("Encountered an invalid combinator.");
            }

            return combinator;
        }

        /// <summary>Gets the whitespace count.</summary>
        /// <param name="commonTree">The common tree.</param>
        /// <returns>The whitespace count.</returns>
        private static int GetWhitespaceCount(CommonTree commonTree)
        {
            Contract.Requires(commonTree != null);

            var whitespaceChildText = commonTree.Children(T(CssParser.WHITESPACE)).FirstChildText();
            int count;
            return int.TryParse(whitespaceChildText, out count) ? count : 0;
        }

        /// <summary>Creates the simple selector.</summary>
        /// <param name="simpleSelectorSequenceTree">The simple selector.</param>
        /// <returns>The simple selector node.</returns>
        private static SimpleSelectorSequenceNode CreateSimpleSelectorSequenceNode(CommonTree simpleSelectorSequenceTree)
        {
            return simpleSelectorSequenceTree == null ? null : new SimpleSelectorSequenceNode(
                CreateTypeSelectorNode(simpleSelectorSequenceTree.Children(T(CssParser.TYPE_SELECTOR)).FirstOrDefault()),
                CreateUniversalSelectorNode(simpleSelectorSequenceTree.Children(T(CssParser.UNIVERSAL)).FirstOrDefault()),
                GetWhitespaceCount(simpleSelectorSequenceTree) > 0 ? CssConstants.SingleSpace.ToString() : null,
                CreateHashClassAttribPseudoNegationNodes(simpleSelectorSequenceTree.GrandChildren(T(CssParser.HASHCLASSATNAMEATTRIBPSEUDONEGATIONNODES))).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the universal selector node.</summary>
        /// <param name="universalSelectorTree">The universal selector tree.</param>
        /// <returns>The universal selector node.</returns>
        private static UniversalSelectorNode CreateUniversalSelectorNode(CommonTree universalSelectorTree)
        {
            return universalSelectorTree == null ? null : new UniversalSelectorNode(
                CreateNamespacePrefixNode(universalSelectorTree.Children(T(CssParser.SELECTOR_NAMESPACE_PREFIX)).FirstOrDefault()));
        }

        /// <summary>Creates the type selector node.</summary>
        /// <param name="typeSelectorTree">The common tree.</param>
        /// <returns>The type selectoe node.</returns>
        private static TypeSelectorNode CreateTypeSelectorNode(CommonTree typeSelectorTree)
        {
            return typeSelectorTree == null ? null : new TypeSelectorNode(
                CreateNamespacePrefixNode(typeSelectorTree.Children(T(CssParser.SELECTOR_NAMESPACE_PREFIX)).FirstOrDefault()),
                typeSelectorTree.Children(T(CssParser.ELEMENT_NAME)).FirstChildText());
        }

        /// <summary>Creates the namespace prefix node.</summary>
        /// <param name="namespacePrefixTree">The namespace prefix tree.</param>
        /// <returns>The namespace prefix node.</returns>
        private static SelectorNamespacePrefixNode CreateNamespacePrefixNode(CommonTree namespacePrefixTree)
        {
            return namespacePrefixTree == null ? null : new SelectorNamespacePrefixNode(
                namespacePrefixTree.Children(T(CssParser.ELEMENT_NAME)).FirstChildTextOrDefault(string.Empty));
        }

        /// <summary>Creates the list of hash class attrib pseudo nodes.</summary>
        /// <param name="hashClassAttribPseudoNegationTreeNodes">The hash class attrib pseudo node tree.</param>
        /// <returns>The list of hash class attrib pseudo nodes.</returns>
        private static IEnumerable<HashClassAtNameAttribPseudoNegationNode> CreateHashClassAttribPseudoNegationNodes(IEnumerable<CommonTree> hashClassAttribPseudoNegationTreeNodes)
        {
            return hashClassAttribPseudoNegationTreeNodes.Select(hashClassAttribPseudoNegationNode =>
            {
                var child = hashClassAttribPseudoNegationNode.Children().FirstOrDefault();
                string hash = null;
                string replacementToken = null;
                string cssClass = null;
                string atName = null;
                AttribNode attribNode = null;
                PseudoNode pseudoNode = null;
                NegationNode negationNode = null;

                if (child != null)
                {
                    var nodeText = child.Text;
                    if (nodeText == T(CssParser.HASHIDENTIFIER))
                    {
                        hash = child.FirstChildText();
                    }
                    else if (nodeText == T(CssParser.CLASSIDENTIFIER))
                    {
                        cssClass = child.FirstChildText();
                    }
                    else if (nodeText == T(CssParser.ATIDENTIFIER))
                    {
                        atName = child.FirstChildText();
                    }
                    else if (nodeText == T(CssParser.ATTRIBIDENTIFIER))
                    {
                        attribNode = CreateAttribNode(child);
                    }
                    else if (nodeText == T(CssParser.PSEUDOIDENTIFIER))
                    {
                        pseudoNode = CreatePseudoNode(child);
                    }
                    else if (nodeText == T(CssParser.NEGATIONIDENTIFIER))
                    {
                        negationNode = CreateNegationNode(child);
                    }
                    else if (nodeText == T(CssParser.REPLACEMENTTOKENIDENTIFIER))
                    {
                        replacementToken = child.FirstChildText();
                    }
                }

                return new HashClassAtNameAttribPseudoNegationNode(hash, cssClass, replacementToken, atName, attribNode, pseudoNode, negationNode);
            });
        }

        /// <summary>Creates the negation node.</summary>
        /// <param name="negationTree">The negation tree.</param>
        /// <returns>The negation node.</returns>
        private static NegationNode CreateNegationNode(CommonTree negationTree)
        {
            return negationTree == null ? null : new NegationNode(
                CreateNegationArgNode(negationTree.Children(T(CssParser.NEGATION_ARG)).FirstOrDefault()));
        }

        /// <summary>Creates the negation arg node.</summary>
        /// <param name="negationArgTree">The negation arg tree.</param>
        /// <returns>The negation arg node.</returns>
        private static NegationArgNode CreateNegationArgNode(CommonTree negationArgTree)
        {
            return negationArgTree == null ? null : new NegationArgNode(
                CreateTypeSelectorNode(
                negationArgTree.Children(T(CssParser.TYPE_SELECTOR)).FirstOrDefault()),
                CreateUniversalSelectorNode(negationArgTree.Children(T(CssParser.UNIVERSAL)).FirstOrDefault()),
                negationArgTree.Children(T(CssParser.HASHIDENTIFIER)).FirstChildText(),
                negationArgTree.Children(T(CssParser.CLASSIDENTIFIER)).FirstChildText(),
                CreateAttribNode(negationArgTree.Children(T(CssParser.ATTRIBIDENTIFIER)).FirstOrDefault()),
                CreatePseudoNode(negationArgTree.Children(T(CssParser.PSEUDOIDENTIFIER)).FirstOrDefault()));
        }

        /// <summary>Creates the pseudo node.</summary>
        /// <param name="pseudoTree">The pseudo tree.</param>
        /// <returns>The pseudo node.</returns>
        private static PseudoNode CreatePseudoNode(CommonTree pseudoTree)
        {
            return pseudoTree == null ? null : new PseudoNode(
                pseudoTree.GrandChildren(T(CssParser.COLONS)).Count(),
                pseudoTree.Children(T(CssParser.PSEUDONAME)).FirstChildText(),
                CreateFunctionalPseudoNode(pseudoTree.Children(T(CssParser.FUNCTIONAL_PSEUDO)).FirstOrDefault()));
        }

        /// <summary>Creates the functional pseudo tree.</summary>
        /// <param name="functionalPseudoTree">The functional pseudo tree.</param>
        /// <returns>The functional pseudo node.</returns>
        private static FunctionalPseudoNode CreateFunctionalPseudoNode(CommonTree functionalPseudoTree)
        {
            return functionalPseudoTree == null ? null : new FunctionalPseudoNode(
                functionalPseudoTree.Children(T(CssParser.FUNCTIONNAME)).FirstChildText(),
                CreateSelectorExpressionNode(functionalPseudoTree.Children(T(CssParser.SELECTOR_EXPRESSION)).FirstOrDefault()));
        }

        /// <summary>Creates the selector expression node.</summary>
        /// <param name="selectorExpressionTree">The selector expression tree.</param>
        /// <returns>The selector expression node.</returns>
        private static SelectorExpressionNode CreateSelectorExpressionNode(CommonTree selectorExpressionTree)
        {
            return selectorExpressionTree == null ? null : new SelectorExpressionNode(
                selectorExpressionTree.Children().Select(_ => _.TextOrDefault()).ToSafeReadOnlyCollection());
        }

        /// <summary>Creates the attrib node.</summary>
        /// <param name="attribTree">The attrib tree.</param>
        /// <returns>The attrib node.</returns>
        private static AttribNode CreateAttribNode(CommonTree attribTree)
        {
            return attribTree == null ? null : new AttribNode(
                CreateNamespacePrefixNode(attribTree.Children(T(CssParser.SELECTOR_NAMESPACE_PREFIX)).FirstOrDefault()),
                attribTree.Children(T(CssParser.ATTRIBNAME)).FirstChildText(),
                CreateAttribOperatorValueNode(attribTree.Children(T(CssParser.ATTRIBOPERATORVALUE)).FirstOrDefault()));
        }

        /// <summary>Creates the attrib operator value node.</summary>
        /// <param name="attribOperatorAndValueTree">The attrib operator and value tree.</param>
        /// <returns>The attrib operator value node.</returns>
        private static AttribOperatorAndValueNode CreateAttribOperatorValueNode(CommonTree attribOperatorAndValueTree)
        {
            if (attribOperatorAndValueTree == null)
            {
                return null;
            }

            // The operator node
            var attribOperatorKind = AttribOperatorKind.None;
            switch (attribOperatorAndValueTree.Children(T(CssParser.ATTRIBOPERATOR)).FirstChildText())
            {
                case CssConstants.PrefixMatch:
                    attribOperatorKind = AttribOperatorKind.Prefix;
                    break;
                case CssConstants.SuffixMatch:
                    attribOperatorKind = AttribOperatorKind.Suffix;
                    break;
                case CssConstants.SubstringMatch:
                    attribOperatorKind = AttribOperatorKind.Substring;
                    break;
                case CssConstants.Equal:
                    attribOperatorKind = AttribOperatorKind.Equal;
                    break;
                case CssConstants.Includes:
                    attribOperatorKind = AttribOperatorKind.Includes;
                    break;
                case CssConstants.DashMatch:
                    attribOperatorKind = AttribOperatorKind.DashMatch;
                    break;
            }

            // The operator value node
            string attribValue = null;
            var attribValueNode = attribOperatorAndValueTree.Children(T(CssParser.ATTRIBVALUE)).FirstOrDefault();
            if (attribValueNode != null)
            {
                attribValue = attribValueNode.FirstChildText() == T(CssParser.STRINGBASEDVALUE) ?
                StringOrUriBasedValue(attribValueNode.Children(T(CssParser.STRINGBASEDVALUE)).FirstChildText()) :
                attribValueNode.FirstChildText();
            }

            return new AttribOperatorAndValueNode(attribOperatorKind, attribValue);
        }

        /// <summary>Cleans the string based value to remove the carriage returns.</summary>
        /// <param name="text">The first child text.</param>
        /// <returns>The cleaned string based value.</returns>
        private static string StringOrUriBasedValue(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                //// The string allows the following grammar and therefore the 'nl' escapes need to be stripped off.
                //// string {string1}|{string2}
                //// string1 \"([^\n\r\f\\"]|\\{nl}|{escape})*\"
                //// string2 \'([^\n\r\f\\']|\\{nl}|{escape})*\'
                text = text
                    .Replace(CssConstants.EscapedNewLine, string.Empty)
                    .Replace(CssConstants.EscapedCarriageReturnNewLine, string.Empty)
                    .Replace(CssConstants.EscapedFormFeed, string.Empty);
            }

            return text;
        }

        /// <summary>Gets the token name.</summary>
        /// <param name="tokenIndex">The token index.</param>
        /// <returns>The token name.</returns>
        private static string T(int tokenIndex)
        {
            return CssParser.tokenNames[tokenIndex];
        }
    }
}
