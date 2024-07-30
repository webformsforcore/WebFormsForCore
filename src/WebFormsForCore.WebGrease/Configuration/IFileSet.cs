// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSet.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   A fileset interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Configuration
{
    using System.Collections.Generic;

    /// <summary>The FileSet interface.</summary>
    public interface IFileSet
    {
        /// <summary>Gets the resource pivots.</summary>
        ResourcePivotGroupCollection ResourcePivots { get; }

        /// <summary>Gets the preprocessing configuration.</summary>
        IDictionary<string, PreprocessingConfig> Preprocessing { get; }

        /// <summary>
        /// Gets the dictionary of auto naming configs
        /// </summary>
        IDictionary<string, BundlingConfig> Bundling { get; }

        /// <summary>Gets the output specified.</summary>
        string Output { get; }

        /// <summary>Gets the list of <see cref="InputSpec"/> items specified.</summary>
        IList<InputSpec> InputSpecs { get; }

        /// <summary>Gets the external files.</summary>
        IList<string> LoadedConfigurationFiles { get; }

        IList<string> Locales { get; }

        IList<string> Themes { get; }
    }
}