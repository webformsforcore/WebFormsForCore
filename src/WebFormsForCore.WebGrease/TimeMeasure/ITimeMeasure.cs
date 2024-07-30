// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimeMeasure.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease
{
    using System;

    /// <summary>The TimeMeasure interface.</summary>
    public interface ITimeMeasure
    {
        /// <summary>Gets the results.</summary>
        /// <returns>The results.</returns>
        TimeMeasureResult[] GetResults();

        /// <summary>The end.</summary>
        /// <param name="isGroup">The is Group.</param>
        /// <param name="idParts">The names.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "End", Justification = "Works well")]
        void End(bool isGroup, params string[] idParts);

        /// <summary>The start.</summary>
        /// <param name="isGroup">The is Group.</param>
        /// <param name="idParts">The names.</param>
        void Start(bool isGroup, params string[] idParts);

        /// <summary>Writes the results to a txt and a csv file.</summary>
        /// <param name="filePathWithoutExtension">The file path without extension.</param>
        /// <param name="title">The title.</param>
        /// <param name="utcStart">The utc start.</param>
        void WriteResults(string filePathWithoutExtension, string title, DateTimeOffset utcStart);
    }
}