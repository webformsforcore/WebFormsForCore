// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileHelper.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   FileHelper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Common
{
    using System.Diagnostics.Contracts;
    using System.IO;

    /// <summary>FileHelper class.</summary>
    internal static class FileHelper
    {
        /// <summary>Writes the file to hard drive</summary>
        /// <param name="path">Path of file</param>
        /// <param name="content">The contents of file</param>
        internal static void WriteFile(string path, string content)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(path));
            Contract.Requires(content != null);

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, content);
        }
    }
}