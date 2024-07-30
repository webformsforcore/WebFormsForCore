// ----------------------------------------------------------------------------------------------------
// <copyright file="ContentFileType.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
namespace WebGrease
{
    /// <summary>The result file type.</summary>
    public enum ContentItemType
    {
        /// <summary>Content is on disk.</summary>
        Path,

        /// <summary>Content as a value string in memory.</summary>
        Value
    }
}