// ----------------------------------------------------------------------------------------------------
// <copyright file="PreprocessorActivity.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease.Activities
{
    using System.Collections.Generic;
    using System.IO;

    using WebGrease.Configuration;
    using WebGrease.Extensions;

    /// <summary>The pre processor activity.</summary>
    internal sealed class PreprocessorActivity
    {
        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PreprocessorActivity"/> class.</summary>
        /// <param name="context">The context.</param>
        internal PreprocessorActivity(IWebGreaseContext context)
        {
            this.context = context;
            this.Inputs = new List<InputSpec>();
        }

        #endregion

        #region Properties

        /// <summary>Gets the list of inputs which need to be assembled.</summary>
        internal List<InputSpec> Inputs { get; private set; }

        /// <summary>
        /// The folder to output the results to
        /// </summary>
        internal string OutputFolder { private get; set; }

        /// <summary>
        /// The preprocessing configuration
        /// </summary>
        internal PreprocessingConfig PreprocessingConfig { private get; set; }

        /// <summary>Gets or sets the value that determines if there should be minimal output.</summary>
        internal bool MinimalOutput { get; set; }

        #endregion

        #region Methods

        /// <summary>The execute.</summary>
        /// <returns>The list of processed files.</returns>
        internal IEnumerable<ContentItem> Execute()
        {
            var preprocessedFiles = new List<ContentItem>();
            var sourceDirectory = this.context.Configuration.SourceDirectory;
            foreach (var file in this.Inputs.GetFiles(sourceDirectory))
            {
                var fi = new FileInfo(file);
                if (!fi.Exists)
                {
                    throw new FileNotFoundException("Could not find the file {0} to preprocess on.");
                }

                if (!Directory.Exists(this.OutputFolder))
                {
                    Directory.CreateDirectory(this.OutputFolder);
                }

                var contentItem = ContentItem.FromFile(file, file.MakeRelativeToDirectory(sourceDirectory));

                contentItem = this.context.Preprocessing.Process(contentItem, this.PreprocessingConfig, this.MinimalOutput);
                if (contentItem == null)
                {
                    throw new WorkflowException("An error occurred while processing the file: " + file);
                }

                preprocessedFiles.Add(contentItem);
            }

            return preprocessedFiles;
        }

        #endregion
    }
}