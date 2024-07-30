// -----------------------------------------------------------------------
// <copyright file="ErrorHelper.cs" company="Microsoft">
// Copyright Microsoft Corporation, all rights reserved
// </copyright>
// -----------------------------------------------------------------------

namespace WebGrease.Css
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using Antlr.Runtime;

    /// <summary>
    /// Helper class for filtering/formating error messages.
    /// </summary>
    internal static class ErrorHelper
    {
        /// <summary>
        /// Dedupes, formats, and returns appropriate error messages. 
        /// </summary>
        /// <param name="aggEx">AggregateException containing parser errors as internal exceptions</param>
        internal static IEnumerable<string> DedupeCSSErrors(this AggregateException aggEx)
        {
            Contract.Requires(aggEx != null);

            var dedupedErrorMessages = new HashSet<string>();

            foreach (var innerEx in aggEx.InnerExceptions)
            {
                var recognitionEx = innerEx as Antlr.Runtime.RecognitionException;
                if (recognitionEx != null)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, "({0},{1}): run-time error CSS1000: {2}", recognitionEx.Line, recognitionEx.CharPositionInLine, recognitionEx.Message);

                    // ANTLR right now is producing parsing exceptions that are, to an end-user, dupes, so filter those
                    dedupedErrorMessages.Add(message);
                }
            }

            return dedupedErrorMessages;
        }

        /// <summary>
        /// Creates a deduped set of build errors for a given aggregate exception thrown from antlr
        /// </summary>
        /// <param name="aggEx">aggregate exception to process</param>
        /// <param name="fileName">File responsible for the error</param>
        /// <returns>A unique collection of errors.</returns>
        internal static IEnumerable<BuildWorkflowException> CreateBuildErrors(this AggregateException aggEx, string fileName)
        {
            Contract.Requires(aggEx != null);
            return aggEx.InnerExceptions
                        .OfType<RecognitionException>()
                        .CreateBuildErrors(fileName);
        }

        /// <summary>Creates a deduped set of build errors for a given aggregate exception thrown from antlr</summary>
        /// <param name="exceptions">The exceptions.</param>
        /// <param name="fileName">File responsible for the error</param>
        /// <returns>A unique collection of errors.</returns>
        internal static IEnumerable<BuildWorkflowException> CreateBuildErrors(this IEnumerable<RecognitionException> exceptions, string fileName)
        {
            Contract.Requires(exceptions != null);
            return exceptions
                        .Where(ex => ex != null)
                        .Distinct(new ErrorDeduper())
                        .Select(ex => new BuildWorkflowException(ex.Message, "CSS", "CSS1000", null, fileName, ex.Line, ex.CharPositionInLine, 0, 0, ex));
        }

        /// <summary>
        /// object used to create a distinct set of errors.
        /// </summary>
        private class ErrorDeduper : IEqualityComparer<RecognitionException>
        {
            public bool Equals(RecognitionException x, RecognitionException y)
            {
                return x.Line == y.Line && x.CharPositionInLine == y.CharPositionInLine && x.Message == y.Message;
            }

            public int GetHashCode(RecognitionException obj)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", obj.Line, obj.CharPositionInLine, obj.Message).GetHashCode();
            }
        }

    }
}
