// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogDelegates.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;

    /// <summary>
    /// Delegate signature for logging extended errors.
    /// </summary>
    public delegate void LogExtendedError(string subcategory, string errorCode, string helpKeyword, string file, int? lineNumber, int? columnNumber, int? endLineNumber, int? endColumnNumber, string message);

    /// <summary>
    /// Delegate signature for logging errors.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    /// <param name="message">Custom error message</param>
    /// <param name="fileName">File the error ocurred in.</param>
    public delegate void LogError(Exception ex, string message = null, string fileName = null);
}