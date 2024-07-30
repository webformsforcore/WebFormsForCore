// ----------------------------------------------------------------------------------------------------
// <copyright file="InputSpecExtensions.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using WebGrease.Configuration;

    /// <summary>InputSpec extensions.</summary>
    public static class InputSpecExtensions
    {
        #region Public Methods and Operators

        /// <summary>Gets all the files for a enumeration of input specs.</summary>
        /// <param name="inputs">The input specs.</param>
        /// <param name="rootPath">The root path to calculate relative paths from</param>
        /// <param name="log">The logmanager to log progress to.</param>
        /// <param name="throwWhenMissingAndNotOptional">Throws an exception of set to true and a file does nog exist.</param>
        /// <returns>The files for the input spec</returns>
        public static IEnumerable<string> GetFiles(this IEnumerable<InputSpec> inputs, string rootPath, LogManager log = null, bool throwWhenMissingAndNotOptional = false)
        {
            return inputs.Where(_ => _ != null && !String.IsNullOrWhiteSpace(_.Path)).SelectMany(i => i.GetFiles(rootPath, log, throwWhenMissingAndNotOptional));
        }

        /// <summary>Gets all the files for an input spec.</summary>
        /// <param name="input">The input spec.</param>
        /// <param name="rootPath">The root path to calculate relative paths from</param>
        /// <param name="log">The logmanager to log progress to.</param>
        /// <param name="throwWhenMissingAndNotOptional">Throws an exception of set to true and a file does nog exist.</param>
        /// <returns>The files for the input spec</returns>
        public static IEnumerable<string> GetFiles(this InputSpec input, string rootPath = null, LogManager log = null, bool throwWhenMissingAndNotOptional = false)
        {
            var files = new List<string>();
            var path = Path.Combine(rootPath ?? String.Empty, input.Path);
            
            if (File.Exists(path))
            {
                // If the file exists it is a file, return the file.
                if (log != null)
                {
                    log.Information("- {0}".InvariantFormat(path));
                }

                files.Add(path);
            }
            else if (Directory.Exists(path))
            {
                // If a directory with the name exists
                if (log != null)
                {
                    log.Information("Folder: {0}, Pattern: {1}, Options: {2}".InvariantFormat(path, input.SearchPattern, input.SearchOption));
                }

                // Get and Add all files using the searchpattern and options
                files.AddRange(
                    Directory.EnumerateFiles(path, String.IsNullOrWhiteSpace(input.SearchPattern) ? "*.*" : input.SearchPattern, input.SearchOption)
                             .OrderBy(name => name, StringComparer.OrdinalIgnoreCase));

                if (log != null)
                {
                    foreach (var file in files)
                    {
                        log.Information("- {0}".InvariantFormat(file));
                    }
                }
            }
            else if (!input.IsOptional && throwWhenMissingAndNotOptional)
            {
                // Else if the path does not exists and is not optional
                throw new FileNotFoundException(
                    "Could not find the file for non option input spec: Path:{0}, SearchPattern:{1}, Options:{2}".InvariantFormat(
                        path, input.SearchPattern, input.SearchOption), 
                    path);
            }

            return files;
        }

        #endregion

        /// <summary>The add input specs.</summary>
        /// <param name="inputSpecs">The input specs.</param>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="element">The element.</param>
        internal static void AddInputSpecs(this IList<InputSpec> inputSpecs, string sourceDirectory, XElement element)
        {
            foreach (var inputElement in element.Descendants())
            {
                var input = new InputSpec(inputElement, sourceDirectory);
                if (!string.IsNullOrWhiteSpace(input.Path))
                {
                    inputSpecs.Add(input);
                }
            }
        }
    }
}