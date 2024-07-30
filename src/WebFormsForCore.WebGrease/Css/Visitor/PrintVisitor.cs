// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrintVisitor.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Provides the print visitor for the ASTs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Visitor
{
    using System;
    using System.Globalization;
    using Ast;
    using Ast.Animation;
    using Ast.MediaQuery;
    using Ast.Selectors;
    using Extensions;

    /// <summary>Provides the print visitor for the ASTs</summary>
    public class PrintVisitor : NodeVisitor
    {
        /// <summary>
        /// The print formatter
        /// </summary>
        private readonly PrinterFormatter _printerFormatter = new PrinterFormatter();

        /// <summary>
        /// Prevents a default instance of the PrintVisitor class from being created
        /// </summary>
        private PrintVisitor()
        {
            IndentSize = 2;
            IndentCharacter = ' ';
        }

        /// <summary>
        /// Gets or sets the indent string for pretty print
        /// </summary>
        /// <value>The indentation character</value>
        public static char IndentCharacter { get; set; }

        /// <summary>
        /// Gets or sets the indent string for pretty print
        /// </summary>
        /// <value>The indent size</value>
        public static int IndentSize { get; set; }

        /// <summary>Factory method for Print visitor</summary>
        /// <param name="node">The node to print</param>
        /// <param name="prettyPrint">The pretty print</param>
        /// <returns>The string representation of AST node</returns>
        public static string Print(AstNode node, bool prettyPrint)
        {
            return new PrintVisitor().Print(prettyPrint, node);
        }

        /// <summary>The <see cref="StyleSheetNode"/> visit implementation</summary>
        /// <param name="styleSheet">The attribute AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
        {
            // styleSheet
            // : [ CHARSET_SYM STRING ';' ]?
            // [S|CDO|CDC]* [ import [ CDO S* | CDC S* ]* ]*
            // [ [ ruleset | media | page | keyframes ] [ CDO S* | CDC S* ]* ]*
            ////  ;
            if (styleSheet == null)
            {
                return null;
            }

            // [ CHARSET_SYM STRING ';' ]?
            if (!string.IsNullOrWhiteSpace(styleSheet.CharSetString))
            {
                _printerFormatter.Append(CssConstants.Charset);
                _printerFormatter.Append(styleSheet.CharSetString);
                _printerFormatter.AppendLine(CssConstants.Semicolon);
            }

            // [S|CDO|CDC]* [ import [ CDO S* | CDC S* ]* ]*
            // Invoke the import visitor
            styleSheet.Imports.ForEach(importNode => importNode.Accept(this));

            // [S|CDO|CDC]* [ namespace [ CDO S* | CDC S* ]* ]*
            // Invoke the import visitor
            styleSheet.Namespaces.ForEach(namespaceNode => namespaceNode.Accept(this));

            // [ [ ruleset | media | page | keyframes | document ] [S|CDO|CDC]* ]*
            // Invoke the styleSheetRuleNode visitor
            styleSheet.StyleSheetRules.ForEach(styleSheetRuleNode => styleSheetRuleNode.Accept(this));

            return styleSheet;
        }

        /// <summary>The <see cref="ImportNode"/> visit implementation</summary>
        /// <param name="importNode">The attribute AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitImportNode(ImportNode importNode)
        {
            // import
            // : IMPORT_SYM S*
            // [STRING|URI] S* media_list? Semicolon S*
            // ;
            // media_list
            // : medium [ COMMA S* medium]*
            // ;

            // : IMPORT_SYM S* [STRING|URI]
            _printerFormatter.Append(CssConstants.Import);

            switch (importNode.AllowedImportDataType)
            {
                case AllowedImportData.String:
                case AllowedImportData.Uri:
                    _printerFormatter.Append(importNode.ImportDataValue);
                    break;
            }

            // medium [ COMMA S* medium]*
            if (importNode.MediaQueries.Count > 0)
            {
                _printerFormatter.Append(CssConstants.SingleSpace);
                importNode.MediaQueries.ForEach((mediaQuery, last) =>
                {
                    mediaQuery.Accept(this);
                    if (!last)
                    {
                        _printerFormatter.Append(CssConstants.Comma);
                    }
                });
            }

            // append for: Semicolon S*
            _printerFormatter.AppendLine(CssConstants.Semicolon);

            return importNode;
        }

        /// <summary>The <see cref="NamespaceNode"/> visit implementation</summary>
        /// <param name="namespaceNode">The namespace node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitNamespaceNode(NamespaceNode namespaceNode)
        {
            // namespace
            // : NAMESPACE_SYM S* [namespace_prefix S*]? [STRING|URI] S* ';' S*
            // ;
            _printerFormatter.Append(CssConstants.Namespace);

            if (!string.IsNullOrWhiteSpace(namespaceNode.Prefix))
            {
                _printerFormatter.Append(CssConstants.SingleSpace);
                _printerFormatter.Append(namespaceNode.Prefix);
            }

            _printerFormatter.Append(CssConstants.SingleSpace);
            _printerFormatter.Append(namespaceNode.Value);
            _printerFormatter.AppendLine(CssConstants.Semicolon);

            return namespaceNode;
        }

        /// <summary>The <see cref="RulesetNode"/> visit implementation</summary>
        /// <param name="rulesetNode">The ruleset AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
        {
            // ruleset
            // : selectors_group
            // '{' S* declaration? [ ';' S* declaration? ]* '}' S*
            // ;
            rulesetNode.SelectorsGroupNode.Accept(this);


            // '{' S* declaration? [ ';' S* declaration? ]* '}' S*
            _printerFormatter.WriteIndent();
            _printerFormatter.AppendLine(CssConstants.OpenCurlyBracket);
            _printerFormatter.IncrementIndentLevel();
            rulesetNode.Declarations.ForEach((declaration, last) =>
                                                 {
                                                     // Invoke the Declaration visitor
                                                     var result = declaration.Accept(this);
                                                     if (!last && result != null)
                                                     {
                                                         _printerFormatter.AppendLine(CssConstants.Semicolon);
                                                     }
                                                 });
            //Visit Important CommentNodes 
            rulesetNode.ImportantComments.ForEach(comment => comment.Accept(this));

            _printerFormatter.DecrementIndentLevel();

            // End the declarations with a line
            _printerFormatter.AppendLine();
            _printerFormatter.WriteIndent();
            _printerFormatter.AppendLine(CssConstants.CloseCurlyBracket);

            return rulesetNode;
        }

        /// <summary>The <see cref="SelectorsGroupNode"/> visit implementation</summary>
        /// <param name="selectorsGroupNode">The selectors group node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorsGroupNode(SelectorsGroupNode selectorsGroupNode)
        {
            // selectors_group
            // : selector [ COMMA S* selector ]*
            // ;
            selectorsGroupNode.SelectorNodes.ForEach((selector, last) =>
            {
                selector.Accept(this);
                if (!last)
                {
                    _printerFormatter.AppendLine(CssConstants.Comma);
                }
            });

            // End the selector with a line
            _printerFormatter.AppendLine();

            return selectorsGroupNode;
        }

        /// <summary>The <see cref="SelectorNode"/> visit implementation</summary>
        /// <param name="selectorNode">The selector AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorNode(SelectorNode selectorNode)
        {
            //// selector
            ////  : simple_selector_sequence [ combinator simple_selector_sequence ]*
            ////  ;
            _printerFormatter.WriteIndent();

            // Invoke the simple_selector_sequence
            selectorNode.SimpleSelectorSequenceNode.Accept(this);

            // [ combinator selector | S+ [ combinator? selector ]? ]?
            selectorNode.CombinatorSimpleSelectorSequenceNodes.ForEach((combinatorSimpleSelectorSequenceNode, selectorIndex) =>
                                                           {
                                                               // TODO - Spec issue - Should this be configuration driven?
                                                               // Remove extra end space in case of IE6 pseudo cases
                                                               if (combinatorSimpleSelectorSequenceNode.Combinator == Combinator.SingleSpace &&
                                                                   _printerFormatter.ToString().EndsWith(CssConstants.SingleSpace.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                                                               {
                                                                   _printerFormatter.Remove(_printerFormatter.Length() - 1, 1);
                                                               }

                                                               // Invoke the CombinatorSimpleSelector
                                                               combinatorSimpleSelectorSequenceNode.Accept(this);
                                                           });

            return selectorNode;
        }

        /// <summary>The <see cref="VisitSimpleSelectorSequenceNode"/> visit implementation</summary>
        /// <param name="simpleSelectorSequenceNode">The simpleSelector AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSimpleSelectorSequenceNode(SimpleSelectorSequenceNode simpleSelectorSequenceNode)
        {
            // simple_selector_sequence
            // : [ type_selector | universal ]
            // [ HASH | class | attrib | pseudo | negation ]*
            // | [ HASH | class | attrib | pseudo | negation ]+
            // ;
            if (simpleSelectorSequenceNode.TypeSelectorNode != null)
            {
                simpleSelectorSequenceNode.TypeSelectorNode.Accept(this);
            }

            if (simpleSelectorSequenceNode.UniversalSelectorNode != null)
            {
                simpleSelectorSequenceNode.UniversalSelectorNode.Accept(this);
            }

            if (simpleSelectorSequenceNode.HashClassAttribPseudoNegationNodes.Count > 0)
            {
                _printerFormatter.Append(simpleSelectorSequenceNode.Separator);
            }

            // [ HASH | class | attrib | pseudo | negation ]*
            // | [ HASH | class | attrib | pseudo | negation ]+
            simpleSelectorSequenceNode.HashClassAttribPseudoNegationNodes.ForEach(hashClassAttribPseudoNegationNode => hashClassAttribPseudoNegationNode.Accept(this));

            return simpleSelectorSequenceNode;
        }

        /// <summary>The <see cref="UniversalSelectorNode"/> visit implementation</summary>
        /// <param name="universalSelectorNode">The universal selector node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitUniversalSelectorNode(UniversalSelectorNode universalSelectorNode)
        {
            // universal
            // : [ namespace_prefix ]? '*'
            // ;
            if (universalSelectorNode.SelectorNamespacePrefixNode != null)
            {
                universalSelectorNode.SelectorNamespacePrefixNode.Accept(this);
            }

            _printerFormatter.Append(CssConstants.Star);
            return universalSelectorNode;
        }

        /// <summary>The <see cref="TypeSelectorNode"/> visit implementation</summary>
        /// <param name="typeSelectorNode">The type selector node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTypeSelectorNode(TypeSelectorNode typeSelectorNode)
        {
            // type_selector
            // : [ namespace_prefix ]? element_name
            // ;
            // Example:
            // @namespace a "http://foo.com";
            // @namespace b "http://bar.com";
            // a|b {}
            // |b {}
            // a {}
            if (typeSelectorNode.SelectorNamespacePrefixNode != null)
            {
                typeSelectorNode.SelectorNamespacePrefixNode.Accept(this);
            }

            _printerFormatter.Append(typeSelectorNode.ElementName);
            return typeSelectorNode;
        }

        /// <summary>The <see cref="SelectorNamespacePrefixNode"/> visit implementation</summary>
        /// <param name="selectorNamespacePrefixNode">The namespace prefix node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorNamespacePrefixNode(SelectorNamespacePrefixNode selectorNamespacePrefixNode)
        {
            _printerFormatter.Append(selectorNamespacePrefixNode.Prefix);
            _printerFormatter.Append(CssConstants.Pipe);
            return selectorNamespacePrefixNode;
        }

        /// <summary>The <see cref="HashClassAtNameAttribPseudoNegationNode"/> visit implementation</summary>
        /// <param name="hashClassAtNameAttribPseudoNegationNode">The simpleSelector AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitHashClassAtNameAttribPseudoNegationNode(HashClassAtNameAttribPseudoNegationNode hashClassAtNameAttribPseudoNegationNode)
        {
            // [ HASH | class | atname | attrib | pseudo | negation ]*
            // | [ HASH | class | atname | attrib | pseudo | negation ]+

            // Add the appropriate combinator or class output
            if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.Hash))
            {
                _printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.Hash);
            }
            else if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.CssClass))
            {
                _printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.CssClass);
            }
            else if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.AtName))
            {
                _printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.AtName);
            }
            else if (!string.IsNullOrWhiteSpace(hashClassAtNameAttribPseudoNegationNode.ReplacementToken))
            {
                // TODO: REPLACECSSTOKENONPRINT
                _printerFormatter.Append(hashClassAtNameAttribPseudoNegationNode.ReplacementToken);
            }
            else if (hashClassAtNameAttribPseudoNegationNode.AttribNode != null)
            {
                hashClassAtNameAttribPseudoNegationNode.AttribNode.Accept(this);
            }
            else if (hashClassAtNameAttribPseudoNegationNode.PseudoNode != null)
            {
                hashClassAtNameAttribPseudoNegationNode.PseudoNode.Accept(this);
            }
            else if (hashClassAtNameAttribPseudoNegationNode.NegationNode != null)
            {
                hashClassAtNameAttribPseudoNegationNode.NegationNode.Accept(this);
            }

            return hashClassAtNameAttribPseudoNegationNode;
        }

        /// <summary>The <see cref="AttribNode"/> visit implementation</summary>
        /// <param name="attrib">The attrib AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitAttribNode(AttribNode attrib)
        {
            // attrib
            // : '[' S* [ namespace_prefix ]? IDENT S*
            // [ [ PREFIXMATCH |
            // SUFFIXMATCH |
            // SUBSTRINGMATCH |
            // '=' |
            // INCLUDES |
            // DASHMATCH ] S* [ IDENT | STRING ] S*
            // ]? ']'
            // ;

            // append for: '[' S* IDENT S* [ [ '=' | INCLUDES | DASHMATCH ] S* [ IDENT | STRING ] S* ]? ']'
            _printerFormatter.Append(CssConstants.OpenSquareBracket);

            if (attrib.SelectorNamespacePrefixNode != null)
            {
                attrib.SelectorNamespacePrefixNode.Accept(this);
            }

            _printerFormatter.Append(attrib.Ident);

            if (attrib.OperatorAndValueNode != null)
            {
                attrib.OperatorAndValueNode.Accept(this);
            }

            _printerFormatter.Append(CssConstants.CloseSquareBracket);

            return attrib;
        }

        /// <summary>The <see cref="AttribOperatorAndValueNode"/> visit implementation</summary>
        /// <param name="attribOperatorAndValueNode">The attribOperatorAndValue AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitAttribOperatorAndValueNode(AttribOperatorAndValueNode attribOperatorAndValueNode)
        {
            // [ [ PREFIXMATCH |
            // SUFFIXMATCH |
            // SUBSTRINGMATCH |
            // '=' |
            // INCLUDES |
            // DASHMATCH ] S* [ IDENT | STRING ] S*
            // ]?
            if (string.IsNullOrWhiteSpace(attribOperatorAndValueNode.IdentOrString))
            {
                return attribOperatorAndValueNode;
            }

            // Add the appropriate enum for: [ '=' | INCLUDES | DASHMATCH ]
            switch (attribOperatorAndValueNode.AttribOperatorKind)
            {
                case AttribOperatorKind.Prefix:
                    _printerFormatter.Append(CssConstants.PrefixMatch);
                    break;
                case AttribOperatorKind.Suffix:
                    _printerFormatter.Append(CssConstants.SuffixMatch);
                    break;
                case AttribOperatorKind.Substring:
                    _printerFormatter.Append(CssConstants.SubstringMatch);
                    break;
                case AttribOperatorKind.Equal:
                    _printerFormatter.Append(CssConstants.Equal);
                    break;
                case AttribOperatorKind.Includes:
                    _printerFormatter.Append(CssConstants.Includes);
                    break;
                case AttribOperatorKind.DashMatch:
                    _printerFormatter.Append(CssConstants.DashMatch);
                    break;
            }

            // append for: [ IDENT | STRING ]
            _printerFormatter.Append(attribOperatorAndValueNode.IdentOrString);

            return attribOperatorAndValueNode;
        }

        /// <summary>The <see cref="PseudoNode"/> visit implementation</summary>
        /// <param name="pseudoNode">The pseudo AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitPseudoNode(PseudoNode pseudoNode)
        {
            // pseudo
            // /* '::' starts a pseudo-element, ':' a pseudo-class */
            // /* Exceptions: :first-line, :first-letter, :before and :after. */
            // /* Note that pseudo-elements are restricted to one per selector and */
            // /* occur only in the last simple_selector_sequence. */
            // : ':' ':'? [ IDENT | functional_pseudo ]
            // ;
            for (var count = 0; count < pseudoNode.NumberOfColons; count++)
            {
                _printerFormatter.Append(CssConstants.Colon);
            }

            if (pseudoNode.FunctionalPseudoNode != null)
            {
                pseudoNode.FunctionalPseudoNode.Accept(this);
            }
            else if (!string.IsNullOrWhiteSpace(pseudoNode.Ident))
            {
                _printerFormatter.Append(pseudoNode.Ident);

                // TODO - Spec issue - Should this be configuration driven?
                // IE6 has a bug where the "first-letter" and "first-line" 
                // pseudo-classes need to be separated from the opening curly-brace 
                // of the following rule set by a space or it doesn't get picked up. 
                // So if the last-outputted word was "first-letter" or "first-line",
                // add a space now (since we know the next character at this point 
                // is the opening brace of a rule-set).
                if (pseudoNode.Ident == "first-letter" || pseudoNode.Ident == "first-line")
                {
                    _printerFormatter.Append(CssConstants.SingleSpace);
                }
            }

            return pseudoNode;
        }

        /// <summary>The <see cref="NegationNode"/> visit implementation</summary>
        /// <param name="negationNode">The negation node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitNegationNode(NegationNode negationNode)
        {
            // negation
            // : NOT S* negation_arg S* ')'
            // ;
            _printerFormatter.Append(CssConstants.Colon);
            _printerFormatter.Append(CssConstants.Not);
            _printerFormatter.Append(CssConstants.OpenRoundBracket);
            negationNode.NegationArgNode.Accept(this);
            _printerFormatter.Append(CssConstants.CloseRoundBracket);
            return negationNode;
        }

        /// <summary>The <see cref="NegationArgNode"/> visit implementation</summary>
        /// <param name="negationArgNode">The negation arg node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitNegationArgNode(NegationArgNode negationArgNode)
        {
            // negation_arg
            // : type_selector | universal | HASH | class | attrib | pseudo
            // ;
            if (negationArgNode.TypeSelectorNode != null)
            {
                negationArgNode.TypeSelectorNode.Accept(this);
            }
            else if (negationArgNode.UniversalSelectorNode != null)
            {
                negationArgNode.UniversalSelectorNode.Accept(this);
            }
            else if (!string.IsNullOrWhiteSpace(negationArgNode.Hash))
            {
                _printerFormatter.Append(negationArgNode.Hash);
            }
            else if (!string.IsNullOrWhiteSpace(negationArgNode.CssClass))
            {
                _printerFormatter.Append(negationArgNode.CssClass);
            }
            else if (negationArgNode.AttribNode != null)
            {
                negationArgNode.AttribNode.Accept(this);
            }
            else if (negationArgNode.PseudoNode != null)
            {
                negationArgNode.PseudoNode.Accept(this);
            }

            return negationArgNode;
        }

        /// <summary>The <see cref="DeclarationNode"/> visit implementation</summary>
        /// <param name="declarationNode">The declaration AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitDeclarationNode(DeclarationNode declarationNode)
        {
            // Exclude what would have been a comment.
            var isComment = declarationNode.Property.StartsWith("/", StringComparison.OrdinalIgnoreCase);
            var isWebGreaseDirectiveProperty = declarationNode.Property.StartsWith("-wg-", StringComparison.OrdinalIgnoreCase);
            if (!_printerFormatter.PrettyPrint && (isComment || isWebGreaseDirectiveProperty))
            {
                return null;
            }

            // Important Comments first
            foreach (var comment in declarationNode.ImportantComments)
            {
                comment.Accept(this);
            }

            // declaration
            // : property ':' S* expr prio?
            // ;

            // property ':'
            _printerFormatter.WriteIndent();
            
            // TODO: REPLACECSSTOKENONPRINT
            _printerFormatter.Append(declarationNode.Property);
            _printerFormatter.Append(CssConstants.Colon);

            // expr prio?
            // Invoke the ExprNode visitor
            declarationNode.ExprNode.Accept(this);
            if (isComment)
            {
                _printerFormatter.AppendLine();
                return null;
            }

            _printerFormatter.Append(declarationNode.Prio);

            return declarationNode;
        }

        /// <summary>The <see cref="ExprNode"/> visit implementation</summary>
        /// <param name="exprNode">The expr AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitExprNode(ExprNode exprNode)
        {

            // Comments
            foreach (var comment in exprNode.ImportantComments)
            {
                comment.Accept(this);
            }

            // expr
            // : term [ operator? term ]*
            // ;

            // Invoke the TermNode visitor
            exprNode.TermNode.Accept(this);

            // append for: [ operator term ]*
            exprNode.TermsWithOperators.ForEach(termWithOperator => termWithOperator.Accept(this));

            return exprNode;
        }

        /// <summary>The <see cref="TermNode"/> visit implementation</summary>
        /// <param name="termNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTermNode(TermNode termNode)
        {
            // term
            // : unary_operator?
            // [ NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* |
            // TIME S* | FREQ S* ]
            // | STRING S* | IDENT S* | URI S* | hexcolor | function
            // ;

            // append for: unary_operator?
            // TODO - Shall we remove the '+' operator here?
            _printerFormatter.Append(termNode.UnaryOperator);
            if (termNode.IsBinary && FunctionNode.IsBinaryOperator(termNode.UnaryOperator))
            {
                _printerFormatter.Append(" ");
            }

            // append for: [ NUMBER S* | PERCENTAGE S* | LENGTH S* | EMS S* | EXS S* | ANGLE S* | TIME S* | FREQ S* ]
            if (!string.IsNullOrWhiteSpace(termNode.NumberBasedValue))
            {
                _printerFormatter.Append(termNode.NumberBasedValue);
            }
            else if (!string.IsNullOrWhiteSpace(termNode.StringBasedValue))
            {
                // append for: | STRING S* | IDENT S* | URI S*
                _printerFormatter.Append(termNode.StringBasedValue);
            }
            else if (!string.IsNullOrWhiteSpace(termNode.ReplacementTokenBasedValue))
            {
                // TODO: REPLACECSSTOKENONPRINT
                _printerFormatter.Append(termNode.ReplacementTokenBasedValue);
            }
            else if (!string.IsNullOrWhiteSpace(termNode.Hexcolor))
            {
                // append for: hexcolor
                _printerFormatter.Append(termNode.Hexcolor);
            }
            else if (termNode.FunctionNode != null)
            {
                // append for: function
                // Invoke the Function visitor
                termNode.FunctionNode.Accept(this);
            }

            foreach (var comment in termNode.ImportantComments)
            {
                comment.Accept(this);
            }

            return termNode;
        }

        /// <summary>
        /// The <see cref=" ImportantCommentNode"/> visit implementation
        /// </summary>
        /// <param name="commentNode">ImportantCommentNode to visit</param>
        /// <returns>he modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitImportantCommentNode(ImportantCommentNode commentNode)
        {
            _printerFormatter.Append(commentNode.Text);
            return base.VisitImportantCommentNode(commentNode);
        }

        /// <summary>The <see cref="TermWithOperatorNode"/> visit implementation</summary>
        /// <param name="termWithOperatorNode">The term AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode)
        {
            // expr
            // : term [ operator? term ]*
            // ;

            // append for: [ operator term ]
            _printerFormatter.Append(termWithOperatorNode.Operator);
            termWithOperatorNode.TermNode.Accept(this);

            return termWithOperatorNode;
        }

        /// <summary>The <see cref="MediaNode"/> visit implementation</summary>
        /// <param name="mediaNode">The media AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaNode(MediaNode mediaNode)
        {
            // media
            // : MEDIA_SYM S* media_list LBRACE S* ruleset* CloseCurlyBracket S*
            // ;

            // append for: MEDIA_SYM S*
            _printerFormatter.Append(CssConstants.Media);

            mediaNode.MediaQueries.ForEach((mediaQuery, last) =>
            {
                mediaQuery.Accept(this);
                if (!last)
                {
                    _printerFormatter.Append(CssConstants.Comma);
                }
            });

            // append for: LBRACE S* ruleset* CloseCurlyBracket S*
            _printerFormatter.AppendLine();
            _printerFormatter.AppendLine(CssConstants.OpenCurlyBracket);
            _printerFormatter.IncrementIndentLevel();
            foreach (var ruleset in mediaNode.Rulesets)
            {
                ruleset.Accept(this);
            }

            // add and indent any nested Page nodes.
            foreach (var page in mediaNode.PageNodes)
            {
                _printerFormatter.WriteIndent();
                page.Accept(this);
            }

            _printerFormatter.DecrementIndentLevel();
            _printerFormatter.AppendLine(CssConstants.CloseCurlyBracket);

            return mediaNode;
        }

        /// <summary>The <see cref="PageNode"/> visit implementation</summary>
        /// <param name="pageNode">The page AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitPageNode(PageNode pageNode)
        {
            // page
            // : PAGE_SYM S* pseudo_page?
            // OpenCurlyBracket S* declaration? [ Semicolon S* declaration? ]* CloseCurlyBracket S*
            // ;

            // append for: PAGE_SYM S*
            _printerFormatter.Append(CssConstants.Page);

            // append for: pseudo_page? S*
            if (!string.IsNullOrWhiteSpace(pageNode.PseudoPage))
            {
                if (!pageNode.PseudoPage.StartsWith(CssConstants.Colon.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                {
                    _printerFormatter.Append(CssConstants.SingleSpace);
                }

                _printerFormatter.Append(pageNode.PseudoPage);
            }

            _printerFormatter.AppendLine();


            // append output for: LBRACE S* declaration [ Semicolon S* declaration ]* CloseCurlyBracket S*
            _printerFormatter.WriteIndent();
            _printerFormatter.AppendLine(CssConstants.OpenCurlyBracket);
            _printerFormatter.IncrementIndentLevel();
            pageNode.Declarations.ForEach((declaration, last) =>
                                              {
                                                  var result = declaration.Accept(this);
                                                  if (!last && result != null)
                                                  {
                                                      _printerFormatter.AppendLine(CssConstants.Semicolon);
                                                  }
                                              });

            _printerFormatter.AppendLine();
            _printerFormatter.DecrementIndentLevel();
            _printerFormatter.WriteIndent();
            _printerFormatter.AppendLine(CssConstants.CloseCurlyBracket);

            return pageNode;
        }

        /// <summary>
        /// The <see cref="DocumentQueryNode"/> visit implementation for print.
        /// </summary>
        /// <param name="documentQueryNode">The document query node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitDocumentQueryNode(DocumentQueryNode documentQueryNode)
        {
            _printerFormatter.Append(documentQueryNode.DocumentSymbol);
            _printerFormatter.Append(CssConstants.SingleSpace);
            _printerFormatter.Append(documentQueryNode.MatchFunctionName);
            _printerFormatter.AppendLine();
            _printerFormatter.AppendLine(CssConstants.OpenCurlyBracket);
            _printerFormatter.IncrementIndentLevel();
            foreach (var ruleset in documentQueryNode.Rulesets)
            {
                ruleset.Accept(this);
            }

            _printerFormatter.DecrementIndentLevel();
            _printerFormatter.AppendLine(CssConstants.CloseCurlyBracket);
            return documentQueryNode;
        }

        /// <summary>The <see cref="CombinatorSimpleSelectorSequenceNode"/> visit implementation</summary>
        /// <param name="combinatorSimpleSelectorSequenceNode">The CombinatorSimpleSelector AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitCombinatorSimpleSelectorSequenceNode(CombinatorSimpleSelectorSequenceNode combinatorSimpleSelectorSequenceNode)
        {
            // combinator
            // /* combinators can be surrounded by whitespace */
            // : PLUS S* | GREATER S* | TILDE S* | S+
            // ;

            // Add the appropriate combinator
            switch (combinatorSimpleSelectorSequenceNode.Combinator)
            {
                case Combinator.PlusSign:
                    _printerFormatter.Append(CssConstants.Plus);
                    break;
                case Combinator.GreaterThanSign:
                    _printerFormatter.Append(CssConstants.Greater);
                    break;
                case Combinator.Tilde:
                    _printerFormatter.Append(CssConstants.Tilde);
                    break;
                case Combinator.SingleSpace:
                    _printerFormatter.Append(CssConstants.SingleSpace);
                    break;
            }

            // add the simple_selector
            combinatorSimpleSelectorSequenceNode.SimpleSelectorSequenceNode.Accept(this);

            return combinatorSimpleSelectorSequenceNode;
        }

        /// <summary>The <see cref="FunctionNode"/> visit implementation</summary>
        /// <param name="functionNode">The function AST node</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitFunctionNode(FunctionNode functionNode)
        {
            // function
            // : FUNCTION S* expr ')' S*
            ////  ;

            if (functionNode.FunctionName == CssConstants.Rgb)
            {
                // Nuke up a new self reference to compute the expression node string
                var exprNodeString = functionNode.ExprNode.MinifyPrint();
                if (exprNodeString.StartsWith(CssConstants.Hash.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                {
                    _printerFormatter.Append(exprNodeString);
                }
            }

            // FUNCTION S* expr ')' S*
            // note: FUNCTION = ident + "("
            _printerFormatter.Append(functionNode.FunctionName);
            _printerFormatter.Append(CssConstants.OpenRoundBracket);

            //// Invoke the ExprNode visitor
            if (functionNode.ExprNode != null)
            {
                functionNode.ExprNode.Accept(this);
            }

            _printerFormatter.Append(CssConstants.CloseRoundBracket);

            return functionNode;
        }

        /// <summary>The <see cref="FunctionalPseudoNode"/> visit implementation</summary>
        /// <param name="functionalPseudoNode">The functional pseudo node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitFunctionalPseudoNode(FunctionalPseudoNode functionalPseudoNode)
        {
            // functional_pseudo
            // : FUNCTION S* expression ')'
            // ;
            _printerFormatter.Append(functionalPseudoNode.FunctionName);
            _printerFormatter.Append(CssConstants.OpenRoundBracket);
            functionalPseudoNode.SelectorExpressionNode.Accept(this);
            _printerFormatter.Append(CssConstants.CloseRoundBracket);
            return functionalPseudoNode;
        }

        /// <summary>The <see cref="SelectorExpressionNode"/> visit implementation</summary>
        /// <param name="selectorExpressionNode">The selector expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitSelectorExpressionNode(SelectorExpressionNode selectorExpressionNode)
        {
            foreach (var selectorExpression in selectorExpressionNode.SelectorExpressions)
            {
                _printerFormatter.Append(selectorExpression);
            }

            return selectorExpressionNode;
        }

        /// <summary>The <see cref="MediaQueryNode"/> visit implementation</summary>
        /// <param name="mediaQueryNode">The media expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaQueryNode(MediaQueryNode mediaQueryNode)
        {
            // media_query
            // : [ONLY | NOT]? S* media_type S* [ AND S* expression ]*
            // | expression [ AND S* expression ]*
            // ;

            // Per W3C Spec:
            // The following is an malformed media query because having no space between ‘and’ and
            // the expression is not allowed. (That is reserved for the functional notation syntax.)
            // @media all and(color) { … }
            if (!string.IsNullOrWhiteSpace(mediaQueryNode.OnlyText))
            {
                _printerFormatter.Append(mediaQueryNode.OnlyText);
                _printerFormatter.Append(CssConstants.SingleSpace);
            }
            else if (!string.IsNullOrWhiteSpace(mediaQueryNode.NotText))
            {
                _printerFormatter.Append(mediaQueryNode.NotText);
                _printerFormatter.Append(CssConstants.SingleSpace);
            }

            if (!string.IsNullOrWhiteSpace(mediaQueryNode.MediaType))
            {
                _printerFormatter.Append(mediaQueryNode.MediaType);
                if (mediaQueryNode.MediaExpressions.Count > 0)
                {
                    mediaQueryNode.MediaExpressions.ForEach(mediaExpression =>
                    {
                        _printerFormatter.Append(CssConstants.SingleSpace);
                        _printerFormatter.Append(CssConstants.And);
                        _printerFormatter.Append(CssConstants.SingleSpace);
                        mediaExpression.Accept(this);
                    });
                }
            }
            else
            {
                mediaQueryNode.MediaExpressions.ForEach((mediaExpression, last) =>
                {
                    mediaExpression.Accept(this);
                    if (!last)
                    {
                        _printerFormatter.Append(CssConstants.SingleSpace);
                        _printerFormatter.Append(CssConstants.And);
                        _printerFormatter.Append(CssConstants.SingleSpace);
                    }
                });
            }

            return mediaQueryNode;
        }

        /// <summary>The <see cref="MediaExpressionNode"/> visit implementation</summary>
        /// <param name="mediaExpressionNode">The media expression node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitMediaExpressionNode(MediaExpressionNode mediaExpressionNode)
        {
            // expression
            // : '(' S* media_feature S* [ ':' S* expr ]? ')' S*
            // ;
            _printerFormatter.Append(CssConstants.OpenRoundBracket);
            _printerFormatter.Append(mediaExpressionNode.MediaFeature);
            if (mediaExpressionNode.ExprNode != null)
            {
                _printerFormatter.Append(CssConstants.Colon);
                mediaExpressionNode.ExprNode.Accept(this);
            }

            _printerFormatter.Append(CssConstants.CloseRoundBracket);

            return mediaExpressionNode;
        }

        /// <summary>The <see cref="KeyFramesNode"/> visit implementation</summary>
        /// <param name="keyFramesNode">The key frames node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitKeyFramesNode(KeyFramesNode keyFramesNode)
        {
            // keyframes-rule: '@keyframes' IDENT '{' keyframes-blocks '}';
            // keyframes-blocks: [ keyframe-selectors block ]* ;
            // keyframe-selectors: [ 'from' | 'to' | PERCENTAGE ] [ ',' [ 'from' | 'to' | PERCENTAGE ] ]*;
            _printerFormatter.Append(keyFramesNode.KeyFramesSymbol);
            _printerFormatter.Append(CssConstants.SingleSpace);

            if (!string.IsNullOrWhiteSpace(keyFramesNode.IdentValue))
            {
                _printerFormatter.Append(keyFramesNode.IdentValue);
            }
            else if (!string.IsNullOrWhiteSpace(keyFramesNode.StringValue))
            {
                _printerFormatter.Append(keyFramesNode.StringValue);
            }

            _printerFormatter.AppendLine();
            _printerFormatter.WriteIndent();
            _printerFormatter.Append(CssConstants.OpenCurlyBracket);
            keyFramesNode.KeyFramesBlockNodes.ForEach(keyFramesBlockNode => keyFramesBlockNode.Accept(this));
            _printerFormatter.AppendLine();
            _printerFormatter.WriteIndent();
            _printerFormatter.AppendLine(CssConstants.CloseCurlyBracket);

            return keyFramesNode;
        }

        /// <summary>The <see cref="KeyFramesBlockNode"/> visit implementation</summary>
        /// <param name="keyFramesBlockNode">The key frames block node.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        public override AstNode VisitKeyFramesBlockNode(KeyFramesBlockNode keyFramesBlockNode)
        {
            _printerFormatter.AppendLine();
            _printerFormatter.IncrementIndentLevel();
            _printerFormatter.WriteIndent();
            _printerFormatter.Append(string.Join(CssConstants.Comma.ToString(), keyFramesBlockNode.KeyFramesSelectors));
            _printerFormatter.AppendLine();
            _printerFormatter.WriteIndent();
            _printerFormatter.Append(CssConstants.OpenCurlyBracket);
            _printerFormatter.AppendLine();
            _printerFormatter.IncrementIndentLevel();
            keyFramesBlockNode.DeclarationNodes.ForEach((declarationNode, last) =>
                                                            {
                                                                var result = declarationNode.Accept(this);
                                                                if (!last && result != null)
                                                                {
                                                                    _printerFormatter.AppendLine(CssConstants.Semicolon);
                                                                }
                                                            });
            _printerFormatter.AppendLine();
            _printerFormatter.DecrementIndentLevel();
            _printerFormatter.WriteIndent();
            _printerFormatter.Append(CssConstants.CloseCurlyBracket);
            _printerFormatter.DecrementIndentLevel();
            return keyFramesBlockNode;
        }

        /// <summary>Print the <see cref="AstNode"/></summary>
        /// <param name="prettyPrint">The pretty print</param>
        /// <param name="node">The node to print</param>
        /// <returns>The string representation of AST node</returns>
        internal string Print(bool prettyPrint, AstNode node)
        {
            _printerFormatter.PrettyPrint = prettyPrint;
            _printerFormatter.IndentCharacter = IndentCharacter;
            _printerFormatter.IndentSize = IndentSize;

            if (node != null)
            {
                node.Accept(this);
            }

            return _printerFormatter.ToString();
        }
    }
}
