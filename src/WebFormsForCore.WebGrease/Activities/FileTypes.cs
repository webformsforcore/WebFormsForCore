// ---------------------------------------------------------------------
// <copyright file="FileTypes.cs" company="Microsoft">
//    Copyright Microsoft Corporation, all rights reserved
// </copyright>
// ---------------------------------------------------------------------
namespace WebGrease.Activities
{
    using System;

    /// <summary>
    /// private enumeration for the type of files being worked on
    /// </summary>
    [Flags]
    public enum FileTypes
    {
        /// <summary>No file types.</summary>
        None = 0,

        /// <summary>The image file type.</summary>
        Image = 1, 

        /// <summary>The js file type for javascript.</summary>
        JS = 2, 

        /// <summary>The css file type for stylesheets.</summary>
        CSS = 4, 

        /// <summary>The all.</summary>
        All = 7
    }
}