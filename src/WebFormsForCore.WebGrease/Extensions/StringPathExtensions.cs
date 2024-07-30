// ----------------------------------------------------------------------------------------------------
// <copyright file="StringPathExtensions.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using WebGrease.Css;

    /// <summary>StringPathExtensions Class - Provides the extension on string types which deal with paths</summary>
    public static class StringPathExtensions
    {
        #region Methods

        /// <summary>Ensures there is an "\" at the end of a directory string.</summary>
        /// <param name="directory">The directory.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string EnsureEndSeparator(this string directory)
        {
            return !directory.EndsWith(new string(Path.DirectorySeparatorChar, 1), StringComparison.OrdinalIgnoreCase)
                       ? directory + Path.DirectorySeparatorChar
                       : directory;
        }

        /// <summary>Returns the full path in lower case</summary>
        /// <param name="originalPath">The original path</param>
        /// <returns>The full path in lower case</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all for invalid input paths.")]
        internal static string GetFullPathWithLowercase(this string originalPath)
        {
            if (string.IsNullOrWhiteSpace(originalPath))
            {
                return originalPath;
            }

            try
            {
                return string.IsNullOrWhiteSpace(originalPath) ? originalPath : Path.GetFullPath(originalPath).ToLower(CultureInfo.CurrentUICulture);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception occurred while trying to get the full path for path: {0}\r\n{1} ".InvariantFormat(originalPath, ex.ToString()));
                return originalPath.ToLower(CultureInfo.CurrentUICulture);
            }
        }

        /// <summary>Returns the full path in lower case</summary>
        /// <param name="pathToConvert">The path to convert</param>
        /// <param name="pathToConvertFrom">The base path</param>
        /// <returns>The full path in lower case</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all for invalid input paths.")]
        internal static string MakeAbsoluteTo(this string pathToConvert, string pathToConvertFrom)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pathToConvert) || string.IsNullOrWhiteSpace(pathToConvertFrom))
                {
                    return pathToConvert;
                }

                return Path.Combine(Path.GetDirectoryName(pathToConvertFrom), pathToConvert).GetFullPathWithLowercase();
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Exception occurred while trying make {0} absolute to {1}\r\n{2} ".InvariantFormat(pathToConvert, pathToConvertFrom, ex.ToString()));
                return pathToConvert;
            }
        }

        /// <summary>Generates the relative path in form "..\".</summary>
        /// <param name="pathToConvert">The path to convert</param>
        /// <param name="pathToConvertFrom">The base path</param>
        /// <param name="separators">The directory separators</param>
        /// <returns>Returns a path for the second file that is relative to the first in lower case</returns>
        internal static string MakeRelativeTo(this string pathToConvert, string pathToConvertFrom, params char[] separators)
        {
            if (string.IsNullOrWhiteSpace(pathToConvert))
            {
                throw new ArgumentNullException("pathToConvert");
            }

            if (pathToConvertFrom.IsNullOrWhitespace())
            {
                return null;
            }

            var inputDirectorySeparator = Path.DirectorySeparatorChar;
            var outputDirectorySeparator = Path.AltDirectorySeparatorChar;

            if (separators != null && separators.Length == 2)
            {
                inputDirectorySeparator = separators[0];
                outputDirectorySeparator = separators[1];
            }

            var pathToConvertTokens = pathToConvert.Split(new[] { inputDirectorySeparator });
            var pathToConvertFromTokens = pathToConvertFrom.Split(new[] { inputDirectorySeparator });

            if (((pathToConvertFromTokens.Length == 0) || (pathToConvertTokens.Length == 0)) || (pathToConvertFromTokens[0] != pathToConvertTokens[0]))
            {
                return pathToConvert;
            }

            // Index 0 is already verified above
            var index = 1;
            while (index < pathToConvertFromTokens.Length && index < pathToConvertTokens.Length)
            {
                if (pathToConvertFromTokens[index] != pathToConvertTokens[index])
                {
                    break;
                }

                index++;
            }

            var builder = new StringBuilder();
            for (var count = index; count < pathToConvertFromTokens.Length - 1; count++)
            {
                builder.Append(CssConstants.DoubleDot);
                builder.Append(outputDirectorySeparator);
            }

            for (var count = index; count < pathToConvertTokens.Length; count++)
            {
                builder.Append(pathToConvertTokens[count]);
                if (count < (pathToConvertTokens.Length - 1))
                {
                    builder.Append(outputDirectorySeparator);
                }
            }

            return builder.ToString().ToLower(CultureInfo.CurrentUICulture);
        }

        /// <summary>Makes a string path relative to a directory.</summary>
        /// <param name="absolutePath">The absolute path.</param>
        /// <param name="relativeTo">The directory to make relative to.</param>
        /// <returns>The relative path.</returns>
        internal static string MakeRelativeToDirectory(this string absolutePath, string relativeTo)
        {
            if (string.IsNullOrWhiteSpace(relativeTo))
            {
                return absolutePath;
            }

            if (absolutePath.Equals(relativeTo, StringComparison.OrdinalIgnoreCase))
            {
                return string.Empty;
            }

            relativeTo = relativeTo.EnsureEndSeparator();
            return new Uri(relativeTo).MakeRelativeUri(new Uri(absolutePath)).ToString().Replace("/", @"\");
        }

        /// <summary>Normalizes a url to be used to calculate and compare disk paths. (For example: /Images/IMage1.Jpg becomes: images\images.jpg)</summary>
        /// <param name="url">The url.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Lowercase is needed.")]
        internal static string NormalizeUrl(this string url)
        {
            url = url.Trim('\'', '"');
            if (url.StartsWith("hash(", StringComparison.OrdinalIgnoreCase) && url.EndsWith(")", StringComparison.OrdinalIgnoreCase))
            {
                url = url.Substring(5, url.Length - 6);
            }
            if (url.StartsWith("hash://", StringComparison.OrdinalIgnoreCase))
            {
                url = url.Substring(7);
            }

            return url.Replace('/', '\\').TrimStart('\\').ToLowerInvariant();
        }

        #endregion
    }
}