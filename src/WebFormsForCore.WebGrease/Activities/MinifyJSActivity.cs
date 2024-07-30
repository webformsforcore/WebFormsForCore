// ----------------------------------------------------------------------------------------------------
// <copyright file="MinifyJSActivity.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// <summary>
//   This task will call the minifier with settings from the args, and output the files to the specified directory
// </summary>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Microsoft.Ajax.Utilities;

    using WebGrease.Extensions;

    /// <summary>This task will call the minifier with settings from the args, and output the files to the specified directory</summary>
    internal sealed class MinifyJSActivity
    {
        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>Initializes a new instance of the <see cref="MinifyJSActivity"/> class.</summary>
        /// <param name="context">The context.</param>
        public MinifyJSActivity(IWebGreaseContext context)
        {
            this.context = context;
        }

        /// <summary>Gets or sets SourceFile.</summary>
        internal string SourceFile { private get; set; }

        /// <summary>Gets or sets DestinationFile.</summary>
        internal string DestinationFile { private get; set; }

        /// <summary>Gets or sets the args to use beyond the default settings</summary>
        /// <value>String containing args</value>
        internal string MinifyArgs { private get; set; }

        /// <summary>Gets or sets the args given to minifier for CSL analyze step. These are just more switches for AjaxMin, but can come from different places in the build.</summary>
        /// <value>String containging args, e.g. "-WARN:4".</value>
        internal string AnalyzeArgs { private get; set; }

        /// <summary>Gets or sets a value indicating whether to analyze files (will use AnalyzeArgs when so).</summary>
        /// <value>True if the files should be analyzed.</value>
        internal bool ShouldAnalyze { private get; set; }

        /// <summary>Gets or sets a value indicating whether to compress files (will use MinifyArgs when so).</summary>
        /// <value>True if the files should be minified.</value>
        internal bool ShouldMinify { private get; set; }

        /// <summary>When overridden in a derived class, executes the task.</summary>
        /// <param name="contentItem">The result File.</param>
        internal void Execute(ContentItem contentItem = null)
        {
            var destinationDirectory = this.context.Configuration.DestinationDirectory;

            if ((contentItem == null) && string.IsNullOrWhiteSpace(this.SourceFile))
            {
                throw new ArgumentException("MinifyJSActivity - The source file cannot be null or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(this.DestinationFile))
            {
                throw new ArgumentException("MinifyJSActivity - The destination file cannot be null or whitespace.");
            }

            if (contentItem == null)
            {
                contentItem = ContentItem.FromFile(
                    this.SourceFile, Path.IsPathRooted(this.SourceFile) ? this.SourceFile.MakeRelativeToDirectory(destinationDirectory) : this.SourceFile);
            }

            var minifiedContentItem = this.Minify(contentItem);

            if (minifiedContentItem != null)
            {
                minifiedContentItem.WriteTo(this.DestinationFile);
            }
        }

        /// <summary>The minify.</summary>
        /// <param name="sourceContentItem">The content item.</param>
        /// <returns>The minified content item, or null if it failed.</returns>
        internal ContentItem Minify(ContentItem sourceContentItem)
        {
            ContentItem minifiedJsContentItem = null;
            this.context.SectionedAction(SectionIdParts.MinifyJsActivity)
                .MakeCachable(sourceContentItem, new { this.ShouldAnalyze, this.ShouldMinify, this.AnalyzeArgs, this.context.Configuration.Global.TreatWarningsAsErrors })
                .RestoreFromCacheAction(cacheSection =>
                {
                    minifiedJsContentItem = cacheSection.GetCachedContentItem(CacheFileCategories.MinifiedJsResult, sourceContentItem.RelativeContentPath, null, sourceContentItem.ResourcePivotKeys);
                    return minifiedJsContentItem != null;
                }).Execute(cacheSection =>
                {
                    var minifier = new Minifier { FileName = this.SourceFile };
                    var minifierSettings = this.GetMinifierSettings(minifier);
                    
                    var js = minifier.MinifyJavaScript(sourceContentItem.Content, minifierSettings.JSSettings);

                    // TODO: RTUIT: Store warnings in cache so that they can be reported even when coming from cache.
                    this.HandleMinifierErrors(sourceContentItem, minifier.ErrorList);

                    if (js != null)
                    {
                        minifiedJsContentItem = ContentItem.FromContent(js, sourceContentItem.RelativeContentPath, null, sourceContentItem.ResourcePivotKeys == null ? null : sourceContentItem.ResourcePivotKeys.ToArray());
                        cacheSection.AddResult(minifiedJsContentItem, CacheFileCategories.MinifiedJsResult);
                    }

                    return minifiedJsContentItem != null && !minifier.ErrorList.Any();
                });

            return minifiedJsContentItem;
        }

        /// <summary>Handles the minifier errors.</summary>
        /// <param name="contentItem">The content item</param>
        /// <param name="errorsAndWarnings">The errors And Warnings.</param>
        private void HandleMinifierErrors(ContentItem contentItem, ICollection<ContextError> errorsAndWarnings)
        {
            // throw if this file has errors, but show all that are found
            if (errorsAndWarnings != null && errorsAndWarnings.Count > 0)
            {
                var hasErrors = false;
                string exceptionMessage;
                if (this.context.Log.HasExtendedErrorHandler)
                {
                    // log each message individually so we can click through into the source
                    foreach (var errorMessage in errorsAndWarnings)
                    {
                        var sourceFile = this.context.EnsureErrorFileOnDisk(errorMessage.File, contentItem);
                        hasErrors |= this.context.Log.TreatWarningsAsErrors || errorMessage.IsError;

                        var errorHandler = errorMessage.IsError ? (LogExtendedError)this.context.Log.Error : this.context.Log.Warning;
                        errorHandler(
                            errorMessage.Subcategory,
                            errorMessage.ErrorCode,
                            errorMessage.HelpKeyword,
                            sourceFile,
                            errorMessage.StartLine,
                            errorMessage.StartColumn,
                            errorMessage.EndLine,
                            errorMessage.EndColumn,
                            errorMessage.Message);
                    }

                    exceptionMessage = "Error minifying the JS";
                }
                else
                {
                    // no logging method passed to us -- combine it all into one big ugly string
                    // and throw it in the exception.
                    hasErrors = true;
                    var errorMessageForException = new StringBuilder();
                    foreach (var errorMessage in errorsAndWarnings)
                    {
                        errorMessageForException.AppendLine(errorMessage.ToString());
                    }

                    exceptionMessage = errorMessageForException.ToString();
                }

                if (hasErrors)
                {
                    var activitySourceFile = this.context.EnsureErrorFileOnDisk(this.SourceFile ?? contentItem.RelativeContentPath, contentItem);
                    throw new BuildWorkflowException(
                        exceptionMessage, "MinifyJSActivity", ErrorCode.Default, null, activitySourceFile, 0, 0, 0, 0, null);
                }
            }
        }

        /// <summary>Gets the minifier settings.</summary>
        /// <param name="minifier">The minifier.</param>
        /// <returns>The minifier settings.</returns>
        private SwitchParser GetMinifierSettings(Minifier minifier)
        {
            // get the starting point default settings for the JavaScript minification,
            // depending on whether to ShouldMinify flag is set. If it is, just use
            // the default settings as the base. Otherwise start off with a set of switches
            // that do a minimum of alteration of the AST after parsing.
            CodeSettings defaultJSSettings;
            if (this.ShouldMinify)
            {
                // default minification settings (except make sure the file is properly terminated
                // in case it gets concatenated with other JS files later on)
                defaultJSSettings = new CodeSettings
                    {
                        TermSemicolons = true
                    };
            }
            else
            {
                // set a bunch of switches to skip most of the minification of the code
                defaultJSSettings = new CodeSettings
                    {
                        OutputMode = OutputMode.MultipleLines,
                        PreserveFunctionNames = true,
                        CollapseToLiteral = false,
                        LocalRenaming = LocalRenaming.KeepAll,
                        ReorderScopeDeclarations = false,
                        RemoveFunctionExpressionNames = false,
                        RemoveUnneededCode = false,
                        StripDebugStatements = false,
                        EvalLiteralExpressions = false,
                        TermSemicolons = true,
                        KillSwitch = -1
                    };
            }

            // we will parse a bunch of switches and apply them on top of the
            // default settings. If we want to analyze, use the analyze options
            // followed by the overrides. Otherwise just use the overrides.
            var args = this.ShouldAnalyze
                ? this.AnalyzeArgs + ' ' + this.MinifyArgs
                : this.MinifyArgs;

            // create a switch parser that starts with the default settings
            // (ignore the js settings) and parse the switches on top of them
            var switchParser = new SwitchParser(defaultJSSettings, null);
            switchParser.Parse(args);

            minifier.WarningLevel = switchParser.WarningLevel;
            return switchParser;
        }
    }
}