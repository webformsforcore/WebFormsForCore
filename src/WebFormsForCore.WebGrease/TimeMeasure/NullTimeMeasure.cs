// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullTimeMeasure.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease
{
    using System;

    /// <summary>
    /// The null time measure class, used when measure is is disabled through configuration.
    /// </summary>
    internal class NullTimeMeasure : ITimeMeasure
    {
        /// <summary>The empty result.</summary>
        private readonly TimeMeasureResult[] emptyResult = new TimeMeasureResult[] { };

        /// <summary>Gets the results.</summary>
        /// <returns>The results.</returns>
        public TimeMeasureResult[] GetResults()
        {
            return this.emptyResult;
        }

        /// <summary>The end.</summary>
        /// <param name="isGroup">The is Group.</param>
        /// <param name="idParts">The names.</param>
        public void End(bool isGroup, params string[] idParts)
        {
        }

        /// <summary>The start.</summary>
        /// <param name="isGroup">The is Group.</param>
        /// <param name="idParts">The names.</param>
        public void Start(bool isGroup, params string[] idParts)
        {
        }

        /// <summary>Writes the results to a txt and a csv file.</summary>
        /// <param name="filePathWithoutExtension">The file path without extension.</param>
        /// <param name="title">The title.</param>
        /// <param name="utcStart">The utc start.</param>
        public void WriteResults(string filePathWithoutExtension, string title, DateTimeOffset utcStart)
        {
        }
    }
}