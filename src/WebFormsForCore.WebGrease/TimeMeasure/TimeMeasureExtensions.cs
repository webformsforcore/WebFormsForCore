// ----------------------------------------------------------------------------------------------------
// <copyright file="TimeMeasureExtensions.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using WebGrease.Extensions;

    /// <summary>The web grease measure extensions.</summary>
    public static class TimeMeasureExtensions
    {
        /// <summary>The header values.</summary>
        private static readonly object[] HeaderValues = new object[] { "Type", "Duration (ms)", "%", "#", "ms/#" };

        /// <summary>Returns a csv representation of the results.</summary>
        /// <param name="results">The measure results.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string GetCsv(this IEnumerable<TimeMeasureResult> results)
        {
            var measureResults = results.OrderByDescending(r => r.Duration).ToArray();
            var totalTime = measureResults.Sum(r => r.Duration);
            var sb = new StringBuilder();
            sb.AppendLine(GetCsvRow(HeaderValues));
            foreach (var measureResult in measureResults)
            {
                sb.AppendLine(GetCsvRow(GetValues(measureResult, totalTime)));
            }

            return sb.ToString();
        }

        /// <summary>Returns a log/text table with the measure results.</summary>
        /// <param name="results">The measure results.</param>
        /// <param name="title">The title.</param>
        /// <returns>The measure results table</returns>
        public static string GetTextTable(this IEnumerable<TimeMeasureResult> results, string title)
        {
            var measureResults = results.OrderByDescending(r => r.Duration).ToArray();
            var sb = new StringBuilder();
            var totalTime = measureResults.Sum(r => r.Duration);
            sb.AppendLine("/=======================================================================================");
            sb.AppendLine("| " + title);
            sb.AppendLine("|--------------------------------------------------------------------------------------");
            sb.AppendLine("| {1,14} | {2,7} | {3,6} | {4,7} | {0}".InvariantFormat(HeaderValues));
            sb.AppendLine("|--------------------------------------------------------------------------------------");

            foreach (var measureResult in measureResults)
            {
                sb.AppendLine("| {1,14:N0} | {2,7:P1} | {3,6} | {4,7:N0} | {0}".InvariantFormat(GetValues(measureResult, totalTime)));
            }

            sb.AppendLine("|--------------------------------------------------------------------------------------");
            sb.AppendLine("| {1,14:N0} | {2,7:P1} | {3,6} | {4,7} | {0}".InvariantFormat("Total", totalTime, 1, string.Empty, string.Empty));
            sb.AppendLine("\\______________________________________________________________________________________");

            return sb.ToString();
        }

        /// <summary>Add resultsToAdd to results.</summary>
        /// <param name="resultsToAdd">The results To Add.</param>
        /// <param name="groupSelector">The group Selector.</param>
        /// <returns>The grouped result</returns>
        public static IEnumerable<TimeMeasureResult> Group(this IEnumerable<TimeMeasureResult> resultsToAdd, Func<TimeMeasureResult, string> groupSelector)
        {
            return
                resultsToAdd
                .GroupBy(groupSelector)
                .Select(s => new TimeMeasureResult
                                 {
                                     IdParts = WebGreaseContext.ToIdParts(s.Key),
                                     Count = s.Min(m => m.Count),
                                     Duration = s.Sum(m => m.Duration)
                                 })
                .OrderByDescending(r => r.Duration)
                .ToArray();
        }

        /// <summary>Gets a csv row.</summary>
        /// <param name="values">The values.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetCsvRow(object[] values)
        {
            return "\"" + string.Join("\",\"", values) + "\"";
        }

        /// <summary>The get values.</summary>
        /// <param name="measureResult">The measure result.</param>
        /// <param name="totalTime">The total time.</param>
        /// <returns>The values of the result as an enumeration of objects.</returns>
        private static object[] GetValues(TimeMeasureResult measureResult, double totalTime)
        {
            return new object[] { measureResult.Name, Math.Round(measureResult.Duration), measureResult.Duration / totalTime, measureResult.Count, measureResult.Duration / measureResult.Count };
        }
    }
}