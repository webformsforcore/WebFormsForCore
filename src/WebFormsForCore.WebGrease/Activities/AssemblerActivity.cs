// ----------------------------------------------------------------------------------------------------
// <copyright file="AssemblerActivity.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// <summary>
//   The assembler activity.
// </summary>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    using Configuration;

    using WebGrease.Extensions;

    /// <summary>The assembler activity.</summary>
    internal sealed class AssemblerActivity
    {
        /// <summary>regular expression to match a string ending with a semicolon optionally followed by any amount of multiline whitespace.</summary>
        private static readonly Regex EndsWithSemicolon = new Regex(@";\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>if the last call ended in a semi-colon.</summary>
        private bool endedInSemicolon;

        /// <summary>Initializes a new instance of the <see cref="AssemblerActivity"/> class.</summary>
        /// <param name="context">The context.</param>
        internal AssemblerActivity(IWebGreaseContext context)
        {
            this.context = context;
            this.Inputs = new List<InputSpec>();
        }

        /// <summary>Gets the list of inputs which need to be assembled.</summary>
        internal List<InputSpec> Inputs { get; private set; }

        /// <summary>Gets or sets the output file.</summary>
        internal string OutputFile { get; set; }

        /// <summary>Gets or sets the output file.</summary>
        internal PreprocessingConfig PreprocessingConfig { private get; set; }

        /// <summary>Gets or sets a flag indicating whether to append semicolons between bundled files that don't already end in them</summary>
        internal bool AddSemicolons { private get; set; }

        /// <summary>Gets or sets the value that determines if there should be minimal output.</summary>
        internal bool MinimalOutput { get; set; }

        /// <summary>The execute method for the activity under question.</summary>
        /// <param name="resultContentItemType">The result Content Type.</param>
        /// <returns>The <see cref="ContentItem"/> or null if it failed.</returns>
        internal ContentItem Execute(ContentItemType resultContentItemType = ContentItemType.Path)
        {
            if (string.IsNullOrWhiteSpace(this.OutputFile))
            {
                throw new ArgumentException("AssemblerActivity - The output file path cannot be null or whitespace.");
            }

            var assembleType = Path.GetExtension(this.OutputFile);
            if (!string.IsNullOrWhiteSpace(assembleType))
            {
                assembleType = assembleType.Trim('.');
            }

            ContentItem contentItem = null;
            this.context.SectionedAction(SectionIdParts.AssemblerActivity, assembleType)
                .MakeCachable(new { this.Inputs, this.PreprocessingConfig, this.AddSemicolons, output = resultContentItemType == ContentItemType.Path ? this.OutputFile : null })
                .RestoreFromCacheAction(cacheSection =>
                    {
                        var cachedContentItem = cacheSection.GetCachedContentItem(CacheFileCategories.AssemblerResult);
                        if (cachedContentItem == null)
                        {
                            return false;
                        }

                        contentItem = ContentItem.FromContentItem(
                                        cachedContentItem,
                                        Path.GetFileName(this.OutputFile));
                        return true;
                    })
                .Execute(cacheSection =>
                    {
                        try
                        {
                            // Add source inputs
                            this.Inputs.ForEach(this.context.Cache.CurrentCacheSection.AddSourceDependency);

                            // Create if the directory does not exist.
                            var outputDirectory = Path.GetDirectoryName(this.OutputFile);
                            if (resultContentItemType == ContentItemType.Path && !string.IsNullOrWhiteSpace(outputDirectory))
                            {
                                Directory.CreateDirectory(outputDirectory);
                            }

                            // set the semicolon flag to true so the first file doesn't get a semicolon added before it. 
                            // IF we are interesting in adding semicolons between bundled files (eg: JavaScript), then this flag 
                            // will get set after outputting each file, depending on whether or not that file ends in a semicolon.
                            // the NEXT file will look at the flag and add one if the previous one didn't end in a semicolon.
                            this.endedInSemicolon = true;

                            contentItem = this.Bundle(resultContentItemType, outputDirectory, this.OutputFile, this.context.Configuration.SourceDirectory);

                            cacheSection.AddResult(contentItem, CacheFileCategories.AssemblerResult);
                        }
                        catch (Exception exception)
                        {
                            throw new WorkflowException("AssemblerActivity - Error happened while executing the assembler activity", exception);
                        }

                        return true;
                    });

            return contentItem;
        }

        /// <summary>Bundles into a result file.</summary>
        /// <param name="targetContentItemType">The result content type.</param>
        /// <param name="outputDirectory">The output directory.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <returns>The <see cref="ContentItem"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Undetected using()")]
        private ContentItem Bundle(ContentItemType targetContentItemType, string outputDirectory, string outputFile, string sourceDirectory)
        {
            var contentBuilder = new StringBuilder();
            using (
                var writer = targetContentItemType == ContentItemType.Path
                                 ? new StreamWriter(outputFile, false, Encoding.UTF8)
                                 : new StringWriter(contentBuilder, CultureInfo.InvariantCulture) as TextWriter)
            {
                this.context.Log.Information("Start bundling output file: {0}".InvariantFormat(outputFile));
                foreach (var file in this.Inputs.GetFiles(sourceDirectory, this.context.Log, true))
                {
                    this.Append(writer, file, sourceDirectory, this.PreprocessingConfig);
                }

                this.context.Log.Information("End bundling output file: {0}".InvariantFormat(outputFile));
            }

            return targetContentItemType == ContentItemType.Path
                ? ContentItem.FromFile(outputFile, outputFile.MakeRelativeTo(outputDirectory))
                : ContentItem.FromContent(contentBuilder.ToString(), outputFile.MakeRelativeTo(outputDirectory));
        }

        /// <summary>The append file.</summary>
        /// <param name="writer">The writer.</param>
        /// <param name="filePath">The file path</param>
        /// <param name="sourceDirectory">The source directory</param>
        /// <param name="preprocessingConfig">The configuration for the preprocessing.</param>
        private void Append(TextWriter writer, string filePath, string sourceDirectory, PreprocessingConfig preprocessingConfig = null)
        {
            // Add a newline to make sure what comes next doesn't get mistakenly attached to the end of
            // a single-line comment or anything. add two so we get an easy-to-read separation between files
            // for debugging purposes.
            writer.WriteLine();
            writer.WriteLine();

            // if we want to separate files with semicolons and the previous file didn't have one, add one now
            if (this.AddSemicolons && !this.endedInSemicolon)
            {
                writer.Write(';');
            }

            string relativeFilePath =
                Path.IsPathRooted(filePath) && !sourceDirectory.IsNullOrWhitespace()
                ? filePath.MakeRelativeTo(sourceDirectory)
                : filePath;

            if (!this.MinimalOutput)
            {
                writer.WriteLine("/* {0} {1} */".InvariantFormat(relativeFilePath, filePath != relativeFilePath ? "(" + filePath + ")" : string.Empty));
                writer.WriteLine();
            }

            var contentItem = ContentItem.FromFile(filePath, relativeFilePath);

            // Executing any applicable preprocessors from the list of configured preprocessors on the file content
            if (preprocessingConfig != null && preprocessingConfig.Enabled)
            {
                contentItem = this.context.Preprocessing.Process(contentItem, preprocessingConfig, this.MinimalOutput);
                if (contentItem == null)
                {
                    throw new WorkflowException("Could not assembly the file {0} because one of the preprocessors threw an error.".InvariantFormat(filePath));
                }
            }

            // TODO:RTUIT: Use a writer/reader instead of getting the content and check differently for the endoign semicolon. Also fix not passing encoding. ONly when not using any preprocessors.
            var content = contentItem.Content;
            writer.Write(content);
            writer.WriteLine();

            // don't even bother checking for a semicolon if we aren't interested in adding one
            if (this.AddSemicolons)
            {
                this.endedInSemicolon = EndsWithSemicolon.IsMatch(content);
            }
        }
    }
}
