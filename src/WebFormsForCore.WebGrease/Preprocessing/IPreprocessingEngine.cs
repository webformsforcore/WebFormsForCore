// ----------------------------------------------------------------------------------------------------
// <copyright file="IPreprocessingEngine.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Preprocessing
{
    using WebGrease.Configuration;

    /// <summary>
    /// The IPreprocessingEngine describes a preprocessing plugin for webgrease.
    /// When implemented and [Export} applied, a class will be loaded through MEF and called when processing files in WebGrease.
    /// </summary>
    public interface IPreprocessingEngine
    {
        #region Public Properties

        /// <summary>Gets the name of this pre-processor (Name has to be set in a configuration for the pre-processor to be used)</summary>
        string Name { get; }

        #endregion

        #region Public Methods and Operators

        /// <summary>This method will be called to check if the processor believes it can handle the file based on the filename.</summary>
        /// <param name="context">The context.</param>
        /// <param name="contentItem">The full path to the file.</param>
        /// <param name="preprocessConfig">The configuration</param>
        /// <returns>If it thinks it can process it.</returns>
        bool CanProcess(IWebGreaseContext context, ContentItem contentItem, PreprocessingConfig preprocessConfig = null);

        /// <summary>The main method for Preprocessing, this is where the preprocessor gets passed the full content, parses it and returns the parsed content.</summary>
        /// <param name="context">The context.</param>
        /// <param name="contentItem">Content of the file to parse.</param>
        /// <param name="preprocessingConfig">The configuration.</param>
        /// <param name="minimalOutput">Is the goal to have the most minimal output (true skips lots of comments)</param>
        /// <returns>The processed content.</returns>
        ContentItem Process(IWebGreaseContext context, ContentItem contentItem, PreprocessingConfig preprocessingConfig, bool minimalOutput);

        #endregion
    }
}