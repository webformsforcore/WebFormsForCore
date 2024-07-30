// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenamedFilesLogs.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Represents a strong typed hash files log which contains a collection
//   of hashed files
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>Represents a strong typed hash files log which contains a collection
    /// of hashed files</summary>
    internal sealed class RenamedFilesLogs
    {
        /// <summary>
        /// Loop up dictionary with key = input file name and value = hash path
        /// </summary>
        private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();

        /// <summary>
        /// Reverse loop up dictionary with key = hash path and value = list of input file names
        /// </summary>
        private readonly Dictionary<string, List<string>> m_reverseDictionary = new Dictionary<string, List<string>>();

        /// <summary>Initializes a new instance of the <see cref="RenamedFilesLogs"/> class.</summary>
        /// <param name="logFiles">The list of log files.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "This is by design.")]
        public RenamedFilesLogs(ICollection<string> logFiles)
        {
            //// Expected Xml format:
            //// ====================
            ////  <renamedFiles>
            ////      <File>
            ////          <Output>/sc/LinkListTestProduct/generic-generic/css/E7F59684F312DFE016D8312AD7AA4C74.css</Output> 
            ////          <Input>/generic-generic/css/layout_02_bluetheme.css</Input> 
            ////          </File>
            ////      <File>
            ////          <Output>/sc/LinkListTestProduct/generic-generic/css/59620BC8C211A82729559B795605CBCB.css</Output> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage.css</Input> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage_SlotLib_Page.css</Input> 
            ////      </File>
            ////      <File>
            ////          <Output>/sc/LinkListTestProduct/generic-generic/css/66A5465F6176306FA0E747794763E3D7.css</Output> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage_ie.css</Input> 
            ////          <Input>/generic-generic/css/layout_02_slotlib_portalpackage_SlotLib_Page_ie.css</Input> 
            ////      </File>
            ////  </renamedFiles>
            if (logFiles == null || logFiles.Count == 0)
            {
                return;
            }

            foreach (var logFile in logFiles)
            {
                var renamedFilesLog = new RenamedFilesLog(logFile);

                // We don't want to break the legacy builds which don't have the log files
                if (!File.Exists(logFile))
                {
                    continue;
                }

                // Update the dictionary for performant look up of input/output files
                renamedFilesLog.RenamedFiles.ForEach(renamedFile => renamedFile.InputNames.ForEach(inputName => this.dictionary.Add(NormalizeSlash(inputName).ToLowerInvariant(), renamedFile.OutputName)));

                // Update the reverse dictionary for performant look up of input/output files
                renamedFilesLog.RenamedFiles.ForEach(renamedFile => m_reverseDictionary.Add(renamedFile.OutputName, renamedFile.InputNames.Select(inputName => inputName.ToLowerInvariant()).ToList()));
            }
        }

        /// <summary>Loads the xml document.</summary>
        /// <returns>Return imagenames if present.</returns>
        public static RenamedFilesLogs LoadHashedImagesLogs(string hashedImagesLogFile)
        {
            // It may be a case of no images present in a feature
            if (!string.IsNullOrWhiteSpace(hashedImagesLogFile) && File.Exists(hashedImagesLogFile))
            {
                try
                {
                    return new RenamedFilesLogs(new[] { hashedImagesLogFile });
                }
                catch (Exception exception)
                {
                    // Assume any error here is because we didn't get a path to a valid hashed images log
                    throw new BuildWorkflowException(string.Format(System.Globalization.CultureInfo.CurrentCulture, "Unable to parse the log with the hashed image replacement names from '{0}'", hashedImagesLogFile), exception);
                }
            }

            return null;
        }

        /// <summary>Normalize the first slash (if present)</summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>Return the path.</returns>
        public static string NormalizeSlash(string path)
        {
            if (path != null && path.StartsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return path.Remove(0, 1);
            }

            return path;
        }

        /// <summary>Indicates is there has been items in dictionary</summary>
        /// <returns>True if items are present in dictionary</returns>
        public bool HasItems()
        {
            return this.dictionary.Count == 0 ? false : true;
        }

        /// <summary>Finds the hash path for a path supplied</summary>
        /// <param name="inputName">The input path</param>
        /// <returns>The hashed path</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "This is by design.")]
        public string FindHashPath(string inputName)
        {
            if (string.IsNullOrWhiteSpace(inputName))
            {
                return null;
            }

            inputName = NormalizeSlash(inputName).ToLowerInvariant();
            string hashPath;
            return this.dictionary.TryGetValue(inputName, out hashPath) ? hashPath : null;
        }

        /// <summary>
        /// Verifies that list of inputFileNames is the complete list for the given hashedFileName
        /// </summary>
        /// <param name="hashedFileName">The hashed file name</param>
        /// <param name="inputFileNames">The list of input file names</param>
        /// <returns>Result of the comparison</returns>
        public bool AllInputFileNamesMatch(string hashedFileName, List<string> inputFileNames)
        {
            if (string.IsNullOrWhiteSpace(hashedFileName) || !m_reverseDictionary.ContainsKey(hashedFileName))
            {
                return false;
            }

            var inputFileNamesFromLog = m_reverseDictionary[hashedFileName];
            if (inputFileNamesFromLog.Count != inputFileNames.Count)
            {
                return false;
            }

            foreach (var inputFileNameFromLog in inputFileNamesFromLog)
            {
                if (!inputFileNames.Contains(inputFileNameFromLog))
                {
                    return false;
                }

                inputFileNames.Remove(inputFileNameFromLog);
            }

            return true;
        }
    }
}
