// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcePivotApplyMode.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Configuration
{
    /// <summary>The resource pivot apply mode.</summary>
    public enum ResourcePivotApplyMode
    {
        /// <summary>The apply as string replace.</summary>
        ApplyAsStringReplace, 

        /// <summary>The css apply after parse.</summary>
        CssApplyAfterParse, 
    }
}