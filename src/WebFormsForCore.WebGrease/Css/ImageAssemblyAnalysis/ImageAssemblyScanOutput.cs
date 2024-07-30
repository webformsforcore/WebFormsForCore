// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageAssemblyScanOutput.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The image sprite scan output.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.ImageAssemblyAnalysis
{
    using System.Collections.Generic;
    using ImageAssemble;

    /// <summary>The image sprite scan output.</summary>
    internal sealed class ImageAssemblyScanOutput
    {
        /// <summary>Initializes a new instance of the <see cref="ImageAssemblyScanOutput"/> class.</summary>
        internal ImageAssemblyScanOutput()
        {
            this.ImageReferencesToAssemble = new List<InputImage>();
        }

        /// <summary>Gets or sets ImageAssemblyScanInput.</summary>
        internal ImageAssemblyScanInput ImageAssemblyScanInput { get; set; }

        /// <summary>Gets ImageReferencesToAssemble.</summary>
        internal IList<InputImage> ImageReferencesToAssemble { get; private set; }
    }
}
