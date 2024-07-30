// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceDirectoryPath.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Token folder path class incorporates the attributes related to token
//   folder path.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    /// <summary>Token folder path class incorporates the attributes related to token
    /// folder path.</summary>
    internal sealed class ResourceDirectoryPath
    {
        /// <summary>
        /// Gets or sets a folder to search the token files in.
        /// </summary>
        /// <value>
        /// String that contains the full path to the folder.
        /// </value>
        public string Directory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the error should be reported in case of token override occurs for
        /// any token key in one of the token files in folder paths.
        /// </summary>
        /// <value>
        /// Gets a value indicating whether overrides are allowed.
        /// </value>
        public bool AllowOverrides { get; set; }
    }
}
