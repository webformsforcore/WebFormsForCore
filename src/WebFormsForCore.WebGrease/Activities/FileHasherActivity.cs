// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileHasherActivity.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Copies sources to destination, with new file name based on hash of content.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    using Common;

    using WebGrease.Extensions;

    /// <summary>Copies sources to destination, with new file name based on hash of content.</summary>
    internal sealed class FileHasherActivity
    {
        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>
        /// Renamed Files Log.
        /// </summary>
        private readonly ConcurrentDictionary<string, string> renamedFilesLog = new ConcurrentDictionary<string, string>();

        /// <summary>Initializes a new instance of the <see cref="FileHasherActivity"/> class.</summary>
        /// <param name="context">The web grease context</param>
        internal FileHasherActivity(IWebGreaseContext context)
        {
            this.context = context;
            this.SourceDirectories = new List<string>();
            this.ConfigType = context.Configuration.ConfigType;
        }

        /// <summary>Gets or sets the config type.</summary>
        internal string ConfigType { get; set; }

        /// <summary>
        /// Gets the list of directories to copy from.
        /// </summary>
        /// <value>The source directories.</value>
        internal IList<string> SourceDirectories { get; private set; }

        /// <summary>
        /// Gets or sets the Directory to copy to.
        /// </summary>
        /// <value>The destination directory.</value>
        internal string DestinationDirectory { private get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to create an extra level of subdirectories based on hashed file names (e.g. /ab/cd123.css vs. /abcd123.css).
        /// </summary>
        /// <value>
        /// <c>True</c> if [create extra folder level from hashes]; otherwise, <c>false</c>.
        /// </value>
        internal bool CreateExtraDirectoryLevelFromHashes { private get; set; }

        /// <summary>
        /// Gets or sets the string prefix to add for output path.
        /// If not blank, this string will be prepended to the paths of the static output location. (e.g. for an image, we may need to add /br before the path).
        /// </summary>
        /// <value>The optional base prefix to add for output path.</value>
        internal string BasePrefixToAddToOutputPath { get; set; }

        /// <summary>Gets or sets the file type.</summary>
        internal FileTypes FileType { private get; set; }

        /// <summary>
        /// Gets or sets the BasePrefixToRemoveFromOutputPathInLog.
        /// In the paths in the output log, we'll remove the portion up to and including this stem.
        /// </summary>
        /// <value>The ref static directory stem.</value>
        internal string BasePrefixToRemoveFromOutputPathInLog { get; set; }

        /// <summary>
        /// Gets or sets the BasePrefixToRemoveFromInputPathInLog.
        /// In the paths in the output log, we'll remove the portion up to and including this stem.
        /// </summary>
        /// <value>The ref static directory stem.</value>
        internal string BasePrefixToRemoveFromInputPathInLog { get; set; }

        /// <summary>
        /// Gets or sets the Location and name for the log file to be written to.
        /// </summary>
        /// <value>The log path.</value>
        internal string LogFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Preserve Source Directory Structure.
        /// Setting this to true will preserve all subdirectories of SourceDirectories.
        /// </summary>
        /// <value>
        /// <c>True</c> if [preserve source folder structure]; otherwise, <c>false</c>.
        /// </value>
        internal bool ShouldPreserveSourceDirectoryStructure { private get; set; }

        /// <summary>
        /// Gets or sets the FileTypeFilter.
        /// Should be a value for DirectoryInfo.GetFiles(), like  '*.css' or '*.js'. Can also be comma separated, like '*.gif,*.jpeg.*.jpg,*.png'
        /// defaults to '*'.
        /// </summary>
        /// <value>The file type filter.</value>
        internal string FileTypeFilter { private get; set; }

        /// <summary>When overridden in a derived class, executes the task.</summary>
        internal void Execute()
        {
            // Clear out the collection since activities objects may be pooled
            this.renamedFilesLog.Clear();

            this.context.SectionedAction(SectionIdParts.FileHasherActivity, this.FileType.ToString()).Execute(() =>
            {
                try
                {
                    if (this.SourceDirectories == null || this.SourceDirectories.Count == 0)
                    {
                        // No action for directory not present and no files as input.
                        Trace.TraceInformation("FileHasherActivity - No source directories passed and hence no action taken for the activity.");
                        return;
                    }

                    // Default the destination directory
                    if (string.IsNullOrWhiteSpace(this.DestinationDirectory))
                    {
                        this.DestinationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    }

                    // Create an array to filter the file types to retrieve
                    var fileTypeFilter = GetFilters(this.FileTypeFilter);

                    // Default the prefix to empty
                    if (string.IsNullOrWhiteSpace(this.BasePrefixToRemoveFromOutputPathInLog))
                    {
                        this.BasePrefixToRemoveFromOutputPathInLog = string.Empty;
                    }

                    // Ensure BasePrefixToRemoveFromInputPathInLog is not null
                    if (string.IsNullOrWhiteSpace(this.BasePrefixToRemoveFromInputPathInLog))
                    {
                        this.BasePrefixToRemoveFromInputPathInLog = string.Empty;
                    }

                    foreach (var sourceDirectory in this.SourceDirectories)
                    {
                        if (!Directory.Exists(sourceDirectory))
                        {
                            // No action for directory not present
                            Trace.TraceWarning(
                                string.Format(
                                    CultureInfo.InvariantCulture, ResourceStrings.FileHasheActivityCouldNotLocateDirectory, sourceDirectory));
                            continue;
                        }

                        this.Hash(sourceDirectory, this.DestinationDirectory, fileTypeFilter);
                    }

                    // All done, if we need to save, save the output
                    this.Save();
                }
                catch (Exception exception)
                {
                    throw new WorkflowException(ResourceStrings.FileHasherActivityErrorOccurred, exception);
                }
            });
        }

        internal IEnumerable<ContentItem> Hash(ContentItem contentItem, IEnumerable<string> originalFiles)
        {

            var hashedFiles = new List<ContentItem>();

            if (originalFiles.Any())
            {
                var firstOriginalFile = originalFiles.FirstOrDefault();
                var hashedContentItem = this.Hash(ContentItem.FromContentItem(contentItem, firstOriginalFile));
                hashedFiles.Add(hashedContentItem);

                hashedFiles.AddRange(this.AppendToWorkLog(hashedContentItem, originalFiles.Skip(1)));
            }

            return hashedFiles;
        }

        /// <summary>The append to work log.</summary>
        /// <param name="hashedContentItem">The hashed content item.</param>
        /// <param name="originalFiles">The original files.</param>
        /// <returns>The hashed items.</returns>
        internal IEnumerable<ContentItem> AppendToWorkLog(ContentItem hashedContentItem, IEnumerable<string> originalFiles)
        {
            var hashedFiles = new List<ContentItem>();

            foreach (var destinationFile in originalFiles)
            {
                var alternateHashedContentItem = ContentItem.FromContentItem(hashedContentItem, destinationFile);
                this.AppendToWorkLog(alternateHashedContentItem);
                hashedFiles.Add(alternateHashedContentItem);
            }

            return hashedFiles;
        }

        /// <summary>Hash the file.</summary>
        /// <param name="contentItem">The content item.</param>
        /// <returns>The result file after the hash.</returns>
        internal ContentItem Hash(ContentItem contentItem)
        {
            var originRelativePath = contentItem.RelativeContentPath;
            var hashedFileName = contentItem.GetContentHash(this.context) + Path.GetExtension(originRelativePath);
            var destinationFilePath = this.GetDestinationFilePath(this.DestinationDirectory, hashedFileName, contentItem.RelativeContentPath);

            var hashedDestinationFolder = this.context.Configuration.DestinationDirectory ?? this.DestinationDirectory;

            var relativeHashedPath = destinationFilePath;
            if (!string.IsNullOrWhiteSpace(hashedDestinationFolder) && Path.IsPathRooted(relativeHashedPath))
            {
                relativeHashedPath = relativeHashedPath.MakeRelativeToDirectory(hashedDestinationFolder);
            }

            contentItem = ContentItem.FromContentItem(contentItem, null, relativeHashedPath);

            // Do not overwrite if exists, since filename is md5 hash, filename changes if content changes.
            contentItem.WriteToRelativeHashedPath(hashedDestinationFolder);

            // Append to the log
            this.AppendToWorkLog(contentItem);

            // Return it as a result file.
            return contentItem;
        }

        /// <summary>Saves the log.</summary>
        /// <param name="append">If the save should append or overwrite.</param>
        internal void Save(bool append = true)
        {
            this.WriteLog(append);
        }

        /// <summary>Appends to the work log from cache results.</summary>
        /// <param name="cacheResults">The cache results.</param>
        internal void AppendToWorkLog(IEnumerable<ContentItem> cacheResults)
        {
            foreach (var cacheResult in cacheResults)
            {
                this.AppendToWorkLog(cacheResult);
            }
        }

        /// <summary>Appends to the work log from a cache result.</summary>
        /// <param name="cacheResult">The cache result.</param>
        internal void AppendToWorkLog(ContentItem cacheResult)
        {
            this.AppendToWorkLog(cacheResult.RelativeContentPath, cacheResult.RelativeHashedContentPath);
        }

        /// <summary>Gets filters for use with DirectoryInfo.GetFiles().</summary>
        /// <param name="filterType">A '*.css' or camma-separated like '*.gif,*.jpeg'.</param>
        /// <returns>A string array with appropriate file filters (e.g. [*.css] or [*.gif,*.jpeg,...]).</returns>
        private static IEnumerable<string> GetFilters(string filterType)
        {
            return string.IsNullOrWhiteSpace(filterType) ? new[] { "*" } : filterType.Split(Strings.FileFilterSeparator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>The get url path.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetUrlPath(string key)
        {
            return key.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        /// <summary>Copies and hashes the Directory.</summary>
        /// <param name="sourceDirectory">Path to source directory.</param>
        /// <param name="destinationDirectory">Path to destination directory.</param>
        /// <param name="filters">Array of file filters to apply.</param>
        /// <param name="rootSourceDirectory">The root source directory</param>
        /// <returns>The result files after the hash.</returns>
        private IEnumerable<ContentItem> Hash(string sourceDirectory, string destinationDirectory, IEnumerable<string> filters, string rootSourceDirectory = null)
        {
            var results = new List<ContentItem>();

            // Create the directory if does not exist.
            Directory.CreateDirectory(destinationDirectory);

            var sourceDirectoryInfo = new DirectoryInfo(sourceDirectory);

            rootSourceDirectory = rootSourceDirectory ?? sourceDirectoryInfo.FullName;

            // Need to do this for only the file type(s) desired
            results.AddRange(
                filters.SelectMany(filter =>
                    sourceDirectoryInfo
                    .EnumerateFiles(filter, SearchOption.TopDirectoryOnly)
                        .Select(sourceFileInfo =>
                            this.Hash(ContentItem.FromFile(sourceFileInfo.FullName, sourceFileInfo.FullName.MakeRelativeToDirectory(rootSourceDirectory))))));

            // recurse through subdirs, either keeping the source dir structure in the destination or flattening it
            foreach (var subDirectoryInfo in sourceDirectoryInfo.GetDirectories())
            {
                var subDestinationDirectory =
                    this.ShouldPreserveSourceDirectoryStructure
                    ? Path.Combine(destinationDirectory, subDirectoryInfo.Name)
                    : destinationDirectory;

                results.AddRange(
                    this.Hash(subDirectoryInfo.FullName, subDestinationDirectory, filters, rootSourceDirectory));
            }

            return results;
        }

        /// <summary>Gets the destination file path for a hashed file name: /12/34567xxxxx.png.</summary>
        /// <param name="destination">The destination path.</param>
        /// <param name="hashedFileName">The hashed file name.</param>
        /// <param name="relativePath">The relative Path.</param>
        /// <returns>The destination file path.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Hash md5 lowercase")]
        private string GetDestinationFilePath(string destination, string hashedFileName, string relativePath)
        {
            string destinationFilePath;

            if (this.CreateExtraDirectoryLevelFromHashes)
            {
                // Use the first 2 chars for a directory name, the last chars for the file name
                destinationFilePath = Path.Combine(destination, hashedFileName.Substring(0, 2)).ToLowerInvariant();

                // This will be the 2 char subdir, and may not exist yet
                if (!Directory.Exists(destinationFilePath))
                {
                    Directory.CreateDirectory(destinationFilePath);
                }

                // Now get the file 
                destinationFilePath = Path.Combine(destinationFilePath, hashedFileName.Remove(0, 2));
            }
            else if (this.ShouldPreserveSourceDirectoryStructure)
            {
                destinationFilePath = Path.Combine(destination, Path.GetDirectoryName(relativePath), hashedFileName);
            }
            else
            {
                destinationFilePath = Path.Combine(destination, hashedFileName);
            }

            return destinationFilePath.ToLowerInvariant();
        }

        /// <summary>Simple logger for generating report of work done.</summary>
        /// <param name="fileBeforeHashing">Path plus file name before hash renaming.</param>
        /// <param name="fileAfterHashing">Path plus file name after hash renaming.</param>
        /// <param name="skipIfExists">skips the add if it already exists.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need lowercase")]
        private void AppendToWorkLog(string fileBeforeHashing, string fileAfterHashing, bool skipIfExists = false)
        {
            fileAfterHashing = Path.Combine(this.context.Configuration.DestinationDirectory ?? this.DestinationDirectory, fileAfterHashing);
            fileBeforeHashing = this.NormalizeFileForWorkLog(fileBeforeHashing, this.BasePrefixToRemoveFromInputPathInLog);
            fileAfterHashing = this.NormalizeFileForWorkLog(fileAfterHashing, this.BasePrefixToRemoveFromOutputPathInLog);

            if (Path.IsPathRooted(fileBeforeHashing))
            {
                fileBeforeHashing = fileBeforeHashing.MakeRelativeToDirectory(this.BasePrefixToRemoveFromInputPathInLog);
            }

            // if it already exists, we need to ignore it and delete it from disk if it exists.
            if (this.renamedFilesLog.ContainsKey(fileBeforeHashing) && !this.renamedFilesLog[fileBeforeHashing].Equals(fileAfterHashing))
            {
                if (skipIfExists)
                {
                    if (File.Exists(fileAfterHashing))
                    {
                        File.Delete(fileAfterHashing);
                        var directoryName = Path.GetDirectoryName(fileAfterHashing);
                        if (!Directory.EnumerateFiles(directoryName).Any())
                        {
                            Directory.Delete(directoryName);
                        }
                    }

                    return;
                }

                throw new BuildWorkflowException(
                    "The renamed filename already has a rename to a different file: \r\nBeforehashing:{0} \r\nNewAfterHashing:{1} ExistingAfterhashing:{2}"
                        .InvariantFormat(fileBeforeHashing, fileAfterHashing, string.Join(",", this.renamedFilesLog.Where(rfl => rfl.Key.Equals(fileBeforeHashing)).Select(e => e.Key))));
            }

            // add the entry
            this.renamedFilesLog[fileBeforeHashing] = fileAfterHashing;
        }

        /// <summary>The make output absolute.</summary>
        /// <param name="output">The output.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string MakeOutputAbsolute(string output)
        {
            if (!string.IsNullOrWhiteSpace(this.BasePrefixToAddToOutputPath) && output.StartsWith(this.BasePrefixToAddToOutputPath, StringComparison.OrdinalIgnoreCase))
            {
                output = output.Substring(this.BasePrefixToAddToOutputPath.Length);
            }

            return Path.Combine(this.BasePrefixToRemoveFromOutputPathInLog ?? this.DestinationDirectory, output.NormalizeUrl());
        }

        /// <summary>The normalize file for work log.</summary>
        /// <param name="file">The file after hashing.</param>
        /// <param name="preFixToRemoveFromWorkLog">The pre fix to remove from work log.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private string NormalizeFileForWorkLog(string file, string preFixToRemoveFromWorkLog)
        {
            if (Path.IsPathRooted(file))
            {
                file = file.MakeRelativeToDirectory(preFixToRemoveFromWorkLog);
            }
            else if (!string.IsNullOrWhiteSpace(preFixToRemoveFromWorkLog))
            {
                var relativeRemoveFromOutputPath = preFixToRemoveFromWorkLog.MakeRelativeToDirectory(this.DestinationDirectory);
                if (!string.IsNullOrWhiteSpace(relativeRemoveFromOutputPath))
                {
                    if (file.StartsWith(relativeRemoveFromOutputPath, StringComparison.OrdinalIgnoreCase))
                    {
                        file = file.Substring(relativeRemoveFromOutputPath.Length);
                    }
                }
            }

            return file.NormalizeUrl();
        }

        /// <summary>Writes a log in an xml file showing which files were copied, with old and new names in it.</summary>
        /// <param name="appendToLog">If we append or overwrite</param>
        private void WriteLog(bool appendToLog = true)
        {
            if (string.IsNullOrWhiteSpace(this.LogFileName))
            {
                return;
            }

            if (appendToLog)
            {
                this.LoadBeforeWrite(this.LogFileName);
            }

            var stringBuilder = new StringBuilder();
            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true };

            using (var writer = XmlWriter.Create(stringBuilder, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("RenamedFiles");
                writer.WriteAttributeString("configType", this.ConfigType);

                // we still want a log file even if nothing was done to it, just no Flie nodes in it
                if (this.renamedFilesLog == null || this.renamedFilesLog.Keys.Count < 1)
                {
                    writer.WriteComment(ResourceStrings.NoFilesProcessed);
                }
                else
                {
                    var groupedRenamedFiles = this.renamedFilesLog
                        .OrderBy(rfl => rfl.Value)
                        .GroupBy(rfl => rfl.Value, rfl => rfl.Key, (key, g) => new {FileAfterHashing = key, FilesBeforeHashing = g.ToList()});
                    foreach (var group in groupedRenamedFiles)
                    {
                        if (!group.FilesBeforeHashing.Any())
                        {
                            continue;
                        }

                        writer.WriteStartElement("File");
                        writer.WriteStartElement("Output");
                        var outputPath = GetUrlPath(group.FileAfterHashing);

                        // if desired, add a base to the path otherwise add the default "/"
                        outputPath = (this.BasePrefixToAddToOutputPath ?? Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture))
                                     + outputPath.TrimStart(Path.AltDirectorySeparatorChar);

                        writer.WriteValue(outputPath);
                        writer.WriteEndElement();
                        foreach (var oldName in group.FilesBeforeHashing.OrderBy(r => r))
                        {
                            writer.WriteStartElement("Input");
                            writer.WriteValue(Path.AltDirectorySeparatorChar + GetUrlPath(oldName).TrimStart(Path.AltDirectorySeparatorChar));
                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }

            // write the log value out to a file
            FileHelper.WriteFile(this.LogFileName, stringBuilder.ToString());
        }

        /// <summary>Load the log from disk.</summary>
        /// <param name="logFileName">The log file name.</param>
        private void LoadBeforeWrite(string logFileName)
        {
            // Get the possible specific log file name for this content type
            var currentConfigTypeLogFileName = GetConfigTypeLogFile(logFileName, this.ConfigType);

            XElement rootElement = null;

            // If the log file does not exist
            if (!File.Exists(logFileName))
            {
                // also check if there is not a content type specific log file. (for example css_log.debug.xml)
                if (File.Exists(currentConfigTypeLogFileName))
                {
                    rootElement = GetLogRoot(currentConfigTypeLogFileName);
                }
            }
            else
            {
                // Or load the normal log.
                rootElement = GetLogRoot(logFileName);
            }

            if (rootElement != null)
            {
                var configType = (string)rootElement.Attribute("configType");

                // If the loaded log file has a different content type.
                if (configType != this.ConfigType)
                {
                    // And the content type is not empty
                    if (!string.IsNullOrWhiteSpace(configType))
                    {
                        // Copy the current log file to a content type specific log file.
                        var xmlConfigType = GetConfigTypeLogFile(logFileName, configType);
                        File.Copy(logFileName, xmlConfigType, true);
                    }

                    // If there is a config type specific log file (for example css_log.debug.xml)
                    if (File.Exists(currentConfigTypeLogFileName))
                    {
                        // Loa it from the log specific file
                        rootElement = GetLogRoot(currentConfigTypeLogFileName);
                    }
                    else
                    {
                        return;
                    }
                }

                var files = rootElement.Elements("File");
                foreach (var fileElement in files)
                {
                    var relativeOutputPath = fileElement.Elements("Output").Select(e => (string)e).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(relativeOutputPath))
                    {
                        var absoluteOutputPath = this.MakeOutputAbsolute(relativeOutputPath);
                        if (File.Exists(absoluteOutputPath))
                        {
                            var inputs = fileElement.Elements("Input").Select(e => (string)e);
                            foreach (var input in inputs)
                            {
                               this.AppendToWorkLog(input, absoluteOutputPath, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>Gets the log root for the log file.</summary>
        /// <param name="logFileName">The log file name.</param>
        /// <returns>The <see cref="XElement"/>.</returns>
        private static XElement GetLogRoot(string logFileName)
        {
            var doc = XDocument.Load(logFileName);
            var rootElement = doc.Element("RenamedFiles");
            return rootElement;
        }

        /// <summary>Gets the config file name for the config type.</summary>
        /// <param name="logFileName">The log file name.</param>
        /// <param name="configType">The config type.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string GetConfigTypeLogFile(string logFileName, string configType)
        {
            if (string.IsNullOrWhiteSpace(configType))
            {
                return logFileName;
            }

            var newExtension = configType + "." + Path.GetExtension(logFileName);
            return Path.ChangeExtension(logFileName, newExtension);
        }
    }
}
