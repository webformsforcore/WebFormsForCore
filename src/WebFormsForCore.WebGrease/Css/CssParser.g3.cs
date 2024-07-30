// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CssParser.g3.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The css parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Ast;

    using WebGrease.Configuration;

    /// <summary>The css parser.</summary>
    public partial class CssParser
    {
        /// <summary>The list of parser exceptions.</summary>
        private readonly IList<Exception> _exceptions = new List<Exception>();

        /// <summary>characters for trimming msie expressions</summary>
        private static char[] _semicolon = new[] { ';' };

        /// <summary>Parse the styleSheet node from the css file path.</summary>
        /// <param name="context">The context.</param>
        /// <param name="cssContent">The css Content.</param>
        /// <param name="shouldLogDiagnostics">Whether the tree should be printed.</param>
        /// <returns>The styleSheet Ast node.</returns>
        public static StyleSheetNode Parse(IWebGreaseContext context, string cssContent, bool shouldLogDiagnostics = true)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(cssContent));
            return ParseStyleSheet(context, cssContent, shouldLogDiagnostics);
        }

        /// <summary>Parse the styleSheet node from the css file path, this method is only used by tests.</summary>
        /// <param name="cssFile">The css file path.</param>
        /// <param name="shouldLogDiagnostics">Whether the tree should be printed.</param>
        /// <returns>The styleSheet Ast node.</returns>
        public static StyleSheetNode Parse(FileInfo cssFile, bool shouldLogDiagnostics = true)
        {
            Contract.Requires(cssFile != null);
            Contract.Requires(File.Exists(cssFile.FullName));

            var cssFilePath = cssFile.FullName;
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Parsing {0} ", cssFilePath));
            return ParseStyleSheet(new WebGreaseContext(new WebGreaseConfiguration()), File.ReadAllText(cssFilePath), shouldLogDiagnostics);
        }

        /// <summary>Throws on first error (For debugging purposes.)</summary>
        /// <param name="e">The exception.</param>
        /// <exception cref="RecognitionException"></exception>
        public override void ReportError(RecognitionException e)
        {
            if (e != null)
            {
                _exceptions.Add(e);
                base.ReportError(e);
            }
        }

        /// <summary>Parse the styleSheet.</summary>
        /// <param name="cssContent">The css content.</param>
        /// <param name="shouldLogDiagnostics">The should log diagnostics.</param>
        /// <returns>The styleSheet node.</returns>
        private static StyleSheetNode ParseStyleSheet(IWebGreaseContext context, string cssContent, bool shouldLogDiagnostics)
        {
            var lexer = new CssLexer(new ANTLRStringStream(cssContent));
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CssParser(tokenStream);

            var commonTree = context
                .SectionedAction("CssParser", "Antlr")
                .Execute(() =>
                {
                    // Assign listener to parser.
                    if (shouldLogDiagnostics)
                    {
                        var listener = Trace.Listeners.OfType<TextWriterTraceListener>().FirstOrDefault();
                        if (listener != null)
                        {
                            parser.TraceDestination = listener.Writer;
                        }
                    }

                    var styleSheet = parser.main();
                    return styleSheet.Tree as CommonTree;
                });

            if (commonTree != null)
            {
                return context.SectionedAction("CssParser", "CreateObjects").Execute(() =>
                {
                    if (shouldLogDiagnostics)
                    {
                        LogDiagnostics(cssContent, commonTree);
                    }

                    if (parser.NumberOfSyntaxErrors > 0)
                    {
                        throw new AggregateException("Syntax errors found.", parser._exceptions);
                    }

                    return CommonTreeTransformer.CreateStyleSheetNode(commonTree);
                });
            }

            return null;
        }

        /// <summary>Logs the Css diagnostics.</summary>
        /// <param name="css">The css content.</param>
        /// <param name="commonTree">The common tree.</param>
        private static void LogDiagnostics(string css, CommonTree commonTree)
        {
            Trace.WriteLine("Input Css:");
            Trace.WriteLine("____________________________________________________");
            Trace.WriteLine(css);
            Trace.WriteLine("____________________________________________________");
            Trace.WriteLine("Css String Tree:");
            Trace.WriteLine("____________________________________________________");
            //// var dotTreeGenerator = new DotTreeGenerator();
            //// Trace.WriteLine(dotTreeGenerator.ToDot(tree));
            Trace.WriteLine(commonTree.ToStringTree());
            Trace.WriteLine("____________________________________________________");
            Trace.WriteLine("Css Common Tree:");
            Trace.WriteLine("____________________________________________________");
            LogTree(commonTree);
            Trace.WriteLine("____________________________________________________");
        }

        /// <summary>
        /// Prints the common tree.
        /// </summary>
        /// <param name="tree">
        /// The common tree.
        /// </param>
        private static void LogTree(CommonTree tree)
        {
            // Tuple Item1 = Indent Level
            // Tuple Item2 = Node in Question
            const string Indent = "---";
            var stack = new Stack<Tuple<int, CommonTree>>();
            stack.Push(new Tuple<int, CommonTree>(0, tree));

            while (stack.Count > 0)
            {
                var tuple = stack.Pop();
                var currentIndentLevel = tuple.Item1;
                var currentNode = tuple.Item2;

                // Calculate Indent
                var currentIndent = new StringBuilder();
                for (var count = 0; count < currentIndentLevel; count++)
                {
                    currentIndent.Append(Indent);
                }

                // Print
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}{1}", currentIndent, currentNode));

                // Add the children to stack
                var children = currentNode.Children;
                if (children != null)
                {
                    foreach (var child in children.OfType<CommonTree>().Reverse())
                    {
                        stack.Push(new Tuple<int, CommonTree>(currentIndentLevel + 1, child));
                    }
                }
            }
        }

        /// <summary>Gets the whitespace token.</summary>
        /// <remarks>This method is invoked from the grammar.</remarks>
        /// <returns>The common token for the whitespace.</returns>
        private CommonToken GetWhitespaceToken()
        {
            // Get the previous token and see if it was a whitespace
            // If whitespace then report the length.
            if (input.Index > 0)
            {
                var previousToken = input.Get(input.Index - 1);
                if (previousToken != null &&
                    previousToken.Type == WS &&
                    previousToken.Text != null &&
                    string.IsNullOrWhiteSpace(previousToken.Text))
                {
                    return new CommonToken(WHITESPACE, previousToken.Text.Length.ToString());
                }
            }

            return new CommonToken(WHITESPACE, "0");
        }

        /// <summary>
        /// Trims any extra semicolons from an unparsed MSIE expression.
        /// </summary>
        /// <param name="text">text of expression</param>
        /// <returns>trimmed text</returns>
        private static CommonToken TrimMsieExpression(string text)
        {
            if (text.EndsWith(";"))
            {
                text = text.TrimEnd(_semicolon);
            }

            return new CommonToken(MSIE_EXPRESSION, text);
        }
    }
}
