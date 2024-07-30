// ----------------------------------------------------------------------------------------------------
// <copyright file="ImageType.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease.ImageAssemble
{
    /// <summary>Type of image</summary>
    internal enum ImageType
    {
        /// <summary>The not supported.</summary>
        NotSupported,

        /// <summary>The photo.</summary>
        Photo,

        /// <summary>The nonphoto nonindexed.</summary>
        NonphotoNonindexed,

        /// <summary>The nonphoto indexed.</summary>
        NonphotoIndexed,
    }
}