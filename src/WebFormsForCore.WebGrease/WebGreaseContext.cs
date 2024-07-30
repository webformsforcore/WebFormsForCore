// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebGreaseContext.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using WebGrease.Activities;
    using WebGrease.Configuration;
    using WebGrease.Extensions;
    using WebGrease.Preprocessing;

    /// <summary>
    /// The web grease context.
    /// It contains all the global information necessary for all the tasks to run.
    /// Only very task specific values should be passed separately.
    /// It also contains all global functionality, like measuring, logging and caching.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Integral to all operations, expected to have lots of coupling.")]
    public class WebGreaseContext : IWebGreaseContext
    {
        /// <summary>The id parts delimiter.</summary>
        private const string IdPartsDelimiter = ".";

        #region Static Fields

        /// <summary>The cached file hashes</summary>
        private static readonly ConcurrentDictionary<string, Tuple<DateTime, long, string>> CachedFileHashes = new ConcurrentDictionary<string, Tuple<DateTime, long, string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>The md5 hasher</summary>
        private static readonly ThreadLocal<MD5CryptoServiceProvider> Hasher = new ThreadLocal<MD5CryptoServiceProvider>(() => new MD5CryptoServiceProvider());

        /// <summary>The no bom utf-8 default encoding (same defaulty encoding as the .net StreamWriter.</summary>
        private static readonly ThreadLocal<Encoding> DefaultEncoding = new ThreadLocal<Encoding>(() => new UTF8Encoding(false, true));

        #endregion

        #region Fields

        /// <summary>The session cached file hashes.</summary>
        private readonly ConcurrentDictionary<string, string> sessionCachedFileHashes = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Per session in memory cache of available files.</summary>
        private readonly IDictionary<string, IDictionary<string, string>> availableFileCollections = new Dictionary<string, IDictionary<string, string>>();

        /// <summary>The threaded measure results.</summary>
        private readonly List<KeyValuePair<string, IEnumerable<TimeMeasureResult>>> threadedMeasureResults = new List<KeyValuePair<string, IEnumerable<TimeMeasureResult>>>();

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="WebGreaseContext"/> class.</summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="configFile">The config file.</param>
        public WebGreaseContext(IWebGreaseContext parentContext, FileInfo configFile)
        {
            var configuration = new WebGreaseConfiguration(parentContext.Configuration, configFile);
            configuration.Validate();

            if (configuration.Global.TreatWarningsAsErrors != null && parentContext.Log != null)
            {
                parentContext.Log.TreatWarningsAsErrors = configuration.Global.TreatWarningsAsErrors == true;
            }

            var parentWebGreaseContext = parentContext as WebGreaseContext;
            if (parentWebGreaseContext != null)
            {
                this.threadedMeasureResults = parentWebGreaseContext.threadedMeasureResults;
            }

            this.Initialize(
                configuration,
                parentContext.Log,
                parentContext.Cache,
                parentContext.Preprocessing,
                parentContext.SessionStartTime,
                parentContext.Measure);
        }

        /// <summary>Initializes a new instance of the <see cref="WebGreaseContext"/> class. The web grease context.</summary>
        /// <param name="configuration">The configuration</param>
        /// <param name="logManager">The log Manager.</param>
        /// <param name="parentCacheSection">The parent Cache Section.</param>
        /// <param name="preprocessingManager">The preprocessing Manager.</param>
        public WebGreaseContext(WebGreaseConfiguration configuration, LogManager logManager, ICacheSection parentCacheSection = null, PreprocessingManager preprocessingManager = null)
        {
            var runStartTime = DateTimeOffset.Now;
            configuration.Validate();
            var timeMeasure = configuration.Measure ? new TimeMeasure() as ITimeMeasure : new NullTimeMeasure();
            var cacheManager = configuration.CacheEnabled ? new CacheManager(configuration, logManager, parentCacheSection) as ICacheManager : new NullCacheManager();
            this.Initialize(
                configuration,
                logManager,
                cacheManager,
                preprocessingManager != null ? new PreprocessingManager(preprocessingManager) : new PreprocessingManager(configuration, logManager, timeMeasure),
                runStartTime,
                timeMeasure);
        }

        /// <summary>Initializes a new instance of the <see cref="WebGreaseContext"/> class. The web grease context.</summary>
        /// <param name="configuration">The configuration</param>
        /// <param name="logInformation">The log information.</param>
        /// <param name="logWarning">The log Warning.</param>
        /// <param name="logExtendedWarning">The log warning.</param>
        /// <param name="logErrorMessage">The log Error Message.</param>
        /// <param name="logError">The log error.</param>
        /// <param name="logExtendedError">The log extended error.</param>
        public WebGreaseContext(WebGreaseConfiguration configuration, Action<string, MessageImportance> logInformation = null, Action<string> logWarning = null, LogExtendedError logExtendedWarning = null, Action<string> logErrorMessage = null, LogError logError = null, LogExtendedError logExtendedError = null)
            : this(configuration, new LogManager(logInformation, logWarning, logExtendedWarning, logErrorMessage, logError, logExtendedError, configuration.Global.TreatWarningsAsErrors))
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the cache manager.</summary>
        public ICacheManager Cache { get; private set; }

        /// <summary>Gets the configuration.</summary>
        public WebGreaseConfiguration Configuration { get; private set; }

        /// <summary>Gets the log.</summary>
        public LogManager Log { get; private set; }

        /// <summary>Gets the measure object.</summary>
        public ITimeMeasure Measure { get; private set; }

        /// <summary>Gets the preprocessing manager.</summary>
        public PreprocessingManager Preprocessing { get; private set; }

        /// <summary>Gets the session start time.</summary>
        public DateTimeOffset SessionStartTime { get; private set; }

        /// <summary>Gets the threaded measure results.</summary>
        public IEnumerable<KeyValuePair<string, IEnumerable<TimeMeasureResult>>> ThreadedMeasureResults
        {
            get
            {
                return this.threadedMeasureResults;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The section.</summary>
        /// <param name="idParts">The id parts.</param>
        /// <returns>The <see cref="IWebGreaseSection"/>.</returns>
        public IWebGreaseSection SectionedAction(params string[] idParts)
        {
            return WebGreaseSection.Create(this, idParts, false);
        }

        /// <summary>The section.</summary>
        /// <param name="idParts">The id parts.</param>
        /// <returns>The <see cref="IWebGreaseSection"/>.</returns>
        public IWebGreaseSection SectionedActionGroup(params string[] idParts)
        {
            return WebGreaseSection.Create(this, idParts, true);
        }

        /// <summary>The temporary ignore.</summary>
        /// <param name="fileSet">The file set.</param>
        /// <param name="contentItem">The content item.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool TemporaryIgnore(IFileSet fileSet, ContentItem contentItem)
        {
            return
                this.Configuration != null
                && this.Configuration.Overrides != null
                && (this.Configuration.Overrides.ShouldIgnore(fileSet) || this.Configuration.Overrides.ShouldIgnore(contentItem));
        }

        /// <summary>The temporary ignore.</summary>
        /// <param name="resourcePivotKey">The content Pivot.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool TemporaryIgnore(IEnumerable<ResourcePivotKey> resourcePivotKey)
        {
            return
                this.Configuration != null
                && this.Configuration.Overrides != null
                && this.Configuration.Overrides.ShouldIgnore(resourcePivotKey);
        }

        /// <summary>The clean cache.</summary>
        /// <param name="logManager">The log manager</param>
        public void CleanCache(LogManager logManager = null)
        {
            var cachePath = this.Cache.RootPath;
            (logManager ?? this.Log).Information("Cleaning Cache: {0}".InvariantFormat(cachePath), MessageImportance.High);
            this.CleanDirectory(cachePath, new[] { CacheManager.LockFileName });
        }

        /// <summary>The clean destination.</summary>
        public void CleanDestination()
        {
            var destinationDirectory = this.Configuration.DestinationDirectory;
            this.Log.Information("Cleaning Destination: {0}".InvariantFormat(destinationDirectory), MessageImportance.High);
            this.CleanDirectory(destinationDirectory);

            var logsDirectory = this.Configuration.LogsDirectory;
            this.Log.Information("Cleaning Destination: {0}".InvariantFormat(logsDirectory), MessageImportance.High);
            this.CleanDirectory(logsDirectory);
        }

        /// <summary>Gets the available files, only gets them once per session/context.</summary>
        /// <param name="rootDirectory">The root directory.</param>
        /// <param name="directories">The directories.</param>
        /// <param name="extensions">The extensions.</param>
        /// <param name="fileType">The file type.</param>
        /// <returns>The available files.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Need lowercase")]
        public IDictionary<string, string> GetAvailableFiles(string rootDirectory, IEnumerable<string> directories, IEnumerable<string> extensions, FileTypes fileType)
        {
            var key = new { rootDirectory, directories, extensions, fileType }.ToJson();
            IDictionary<string, string> availableFileCollection;
            if (!this.availableFileCollections.TryGetValue(key, out availableFileCollection))
            {
                var results = new Dictionary<string, string>();
                if (directories == null)
                {
                    return results;
                }

                foreach (var directory in directories)
                {
                    foreach (var extension in extensions)
                    {
                        results.AddRange(
                            Directory.GetFiles(directory, extension, SearchOption.AllDirectories)
                                     .Select(f => f.ToLowerInvariant())
                                     .ToDictionary(f => f.MakeRelativeToDirectory(rootDirectory), f => f));
                    }
                }

                this.availableFileCollections.Add(key, availableFileCollection = results);
            }

            return availableFileCollection;
        }

        /// <summary>The get content hash.</summary>
        /// <param name="value">The content.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetValueHash(string value)
        {
            return this.SectionedAction(SectionIdParts.ContentHash).Execute(() => ComputeContentHash(value ?? string.Empty));
        }

        /// <summary>The get bitmap hash.</summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="format">The format</param>
        /// <returns>The <see cref="string"/>.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2002:DoNotLockOnObjectsWithWeakIdentity", Justification = "Lock is to prevent thread issues, according to msdn doc should work this way.")]
        public string GetBitmapHash(Bitmap bitmap, ImageFormat format)
        {
            lock (bitmap)
            {
                return this.SectionedAction(SectionIdParts.BitmapHash).Execute(() => ComputeBitmapHash(bitmap, format));
            }
        }

        /// <summary>Gets the md5 hash for the content file.</summary>
        /// <param name="contentItem">The content file.</param>
        /// <returns>The MD5 hash.</returns>
        public string GetContentItemHash(ContentItem contentItem)
        {
            return this.SectionedAction(SectionIdParts.ContentHash).Execute(
                () => contentItem.GetContentHash(this));
        }

        /// <summary>Gets the hash for the content of the file provided in the file path.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The MD5 hash.</returns>
        public string GetFileHash(string filePath)
        {
            string hash = null;
            var fi = new FileInfo(filePath);

            if (!fi.Exists)
            {
                throw new FileNotFoundException("Could not find the file to create a hash for", filePath);
            }

            var uniqueId = fi.FullName;

            if (this.sessionCachedFileHashes.TryGetValue(uniqueId, out hash))
            {
                // Found in current session cache, just return.
                return hash;
            }

            Tuple<DateTime, long, string> cachedFileHash;
            CachedFileHashes.TryGetValue(uniqueId, out cachedFileHash);


            if (cachedFileHash != null && cachedFileHash.Item1 == fi.LastWriteTimeUtc && cachedFileHash.Item2 == fi.Length)
            {
                // found in static cache, between sessions, and is up to date return.
                return cachedFileHash.Item3;
            }

            // either new, or has changed, recompute, and add to cached file hash
            hash = ComputeFileHash(fi.FullName);
            CachedFileHashes[uniqueId] = new Tuple<DateTime, long, string>(fi.LastWriteTimeUtc, fi.Length, hash);
            this.sessionCachedFileHashes[uniqueId] = hash;

            return hash;
        }

        /// <summary>Makes the path relative to the application root.</summary>
        /// <param name="absolutePath">The absolute path.</param>
        /// <returns>The givcen path relative to the application root.</returns>
        public string MakeRelativeToApplicationRoot(string absolutePath)
        {
            return absolutePath.MakeRelativeTo(this.Configuration.ApplicationRootDirectory);
        }

        /// <summary>The make absolute to source directory.</summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetWorkingSourceDirectory(string relativePath)
        {
            var sourceDirectory = this.Configuration.SourceDirectory ?? string.Empty;
            var absolutePath = Path.Combine(sourceDirectory, relativePath);
            var si = new FileInfo(absolutePath);

            return (sourceDirectory.IsNullOrWhitespace() || si.FullName.StartsWith(sourceDirectory, StringComparison.OrdinalIgnoreCase))
                ? si.DirectoryName
                : sourceDirectory;
        }

        /// <summary>The touch.</summary>
        /// <param name="filePath">The file path.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all on purpose.")]
        public void Touch(string filePath)
        {
            var newTime = this.SessionStartTime.UtcDateTime;
            try
            {
                File.SetLastWriteTimeUtc(filePath, newTime);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>Ensures the file exists so it can be reported with an error.</summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="sourceContentItem">The input file.</param>
        /// <returns>The error file path.</returns>
        public string EnsureErrorFileOnDisk(string sourceFile, ContentItem sourceContentItem)
        {
            if (sourceContentItem == null)
            {
                return sourceFile;
            }

            if (sourceFile.IsNullOrWhitespace() || !File.Exists(sourceFile))
            {
                sourceFile = sourceContentItem.RelativeContentPath;
                if (sourceFile.IsNullOrWhitespace())
                {
                    sourceFile = Guid.NewGuid().ToString().Replace("-", string.Empty);
                }

                if (sourceContentItem.ResourcePivotKeys != null)
                {
                    var firstPivot = sourceContentItem.ResourcePivotKeys.FirstOrDefault();
                    if (firstPivot != null)
                    {
                        var extension = Path.GetExtension(sourceFile);
                        sourceFile = Path.ChangeExtension(sourceFile, "." + firstPivot.ToString("{0}.{1}") + extension);
                    }
                }
            }

            sourceFile = sourceFile.NormalizeUrl();
            if (!Path.IsPathRooted(sourceFile))
            {
                sourceFile = Path.Combine(this.Configuration.IntermediateErrorDirectory, sourceFile);
            }

            sourceContentItem.WriteTo(sourceFile);
            return sourceFile;
        }

        /// <summary>The parallel for each call, that ensures valid multi threaded opeartions for webgrease calls.</summary>
        /// <param name="idParts">The id parts.</param>
        /// <param name="items">The items.</param>
        /// <param name="parallelAction">The parallel action.</param>
        /// <param name="serialAction">The serial action.</param>
        /// <typeparam name="T">The type of items</typeparam>
        public void ParallelForEach<T>(Func<T, string[]> idParts, IEnumerable<T> items, Func<IWebGreaseContext, T, ParallelLoopState, bool> parallelAction, Func<IWebGreaseContext, T, bool> serialAction = null)
        {
            var id = idParts(default(T));
            this.SectionedAction(id).Execute(() =>
            {
                var serialLock = new object();
                var parallelForEachItems = new List<Tuple<IWebGreaseContext, DelayedLogManager, T>>();
                var done = 0;
                foreach (var item in items)
                {
                    // TODO: Better name then item ToString?
                    var delayedLogManager = new DelayedLogManager(this.Log, item.ToString());
                    var threadContext = new WebGreaseContext(new WebGreaseConfiguration(this.Configuration), delayedLogManager.LogManager, this.Cache.CurrentCacheSection, this.Preprocessing);

                    var success = true;
                    if (serialAction != null)
                    {
                        success = serialAction(threadContext, item);
                    }

                    if (success)
                    {
                        parallelForEachItems.Add(new Tuple<IWebGreaseContext, DelayedLogManager, T>(threadContext, delayedLogManager, item));
                    }
                }

                Parallel.ForEach(
                    parallelForEachItems,
                    (item, state) =>
                    {
                        var sectionId = ToStringId(idParts(item.Item3));
                        parallelAction(item.Item1, item.Item3, state);
                        var measureResult = item.Item1.Measure.GetResults();
                        Safe.Lock(
                            serialLock,
                            Safe.MaxLockTimeout,
                            () =>
                            {
                                this.threadedMeasureResults.AddRange(item.Item1.ThreadedMeasureResults);
                                this.threadedMeasureResults.Add(new KeyValuePair<string, IEnumerable<TimeMeasureResult>>(sectionId, measureResult));
                                item.Item2.Flush();
                                done++;
                                if (done == parallelForEachItems.Count - 1)
                                {
                                    parallelForEachItems.ForEach(i => i.Item2.Flush());
                                }
                            });
                    });
            });
        }

        #endregion

        #region Methods

        /// <summary>The get name.</summary>
        /// <param name="idParts">The names.</param>
        /// <returns>The id.</returns>
        internal static string ToStringId(IEnumerable<string> idParts)
        {
            return string.Join(IdPartsDelimiter, idParts);
        }

        /// <summary>The get name.</summary>
        /// <param name="id">The name.</param>
        /// <returns>The id parts</returns>
        internal static IEnumerable<string> ToIdParts(string id)
        {
            return id.Split(IdPartsDelimiter[0]);
        }

        /// <summary>The compute content hash.</summary>
        /// <param name="content">The content.</param>
        /// <param name="encoding"> The encoding</param>
        /// <returns>The <see cref="string"/>.</returns>
        internal static string ComputeContentHash(string content, Encoding encoding = null)
        {
            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms, encoding ?? DefaultEncoding.Value);
                sw.Write(content);
                sw.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                return BytesToHash(Hasher.Value.ComputeHash(ms));
            }
        }

        /// <summary>The compute file hash.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="string"/>.</returns>
        internal static string ComputeFileHash(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return BytesToHash(Hasher.Value.ComputeHash(fs));
            }
        }

        /// <summary>The del tree method, deletes a directory and all its sub files/directories.</summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filesToIgnore">The files to ignore when deleting.</param>
        /// <returns>If a files was ignored.</returns>
        private static bool DelTree(string directory, string[] filesToIgnore)
        {
            var fileWasSkipped = false;
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (filesToIgnore == null || !filesToIgnore.Any(fti => file.EndsWith(fti, StringComparison.OrdinalIgnoreCase)))
                {
                    File.Delete(file);
                }
                else
                {
                    fileWasSkipped = true;
                }
            }

            var subDirectories = Directory.GetDirectories(directory);
            foreach (var subDirectory in subDirectories)
            {
                fileWasSkipped |= DelTree(subDirectory, filesToIgnore);
            }

            if (!fileWasSkipped)
            {
                Directory.Delete(directory);
            }

            return fileWasSkipped;
        }

        /// <summary>The compute bitmap hash.</summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <param name="format">The format.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string ComputeBitmapHash(Bitmap bitmap, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, format);
                ms.Seek(0, SeekOrigin.Begin);
                return BytesToHash(Hasher.Value.ComputeHash(ms));
            }
        }

        /// <summary>The bytes to hash.</summary>
        /// <param name="hash">The hash.</param>
        /// <returns>The <see cref="string"/>.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "MD5 Lower case")]
        private static string BytesToHash(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>The clean directory.</summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filesToIgnore">The files to ignore when deleting.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all on purpose, hide any file system exceptions, make sure they don't stop the webgrease run.")]
        private void CleanDirectory(string directory, string[] filesToIgnore = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
                {
                    DelTree(directory, filesToIgnore);
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                this.Log.Warning("Error while cleaning {0}: {1}".InvariantFormat(directory, ex.Message));
            }
        }

        /// <summary>The initialize.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="preprocessingManager">The preprocessing manager.</param>
        /// <param name="runStartTime">The run start time.</param>
        /// <param name="timeMeasure">The time measure.</param>
        private void Initialize(
            WebGreaseConfiguration configuration,
            LogManager logManager,
            ICacheManager cacheManager,
            PreprocessingManager preprocessingManager,
            DateTimeOffset runStartTime,
            ITimeMeasure timeMeasure)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (configuration.Global.TreatWarningsAsErrors != null)
            {
                logManager.TreatWarningsAsErrors = configuration.Global.TreatWarningsAsErrors == true;
            }

            // Note: Configuration needs to be set before the other ones.
            this.Configuration = configuration;
            this.Configuration.Validate();

            this.Measure = timeMeasure;

            this.Log = logManager;

            this.Cache = cacheManager;

            this.Preprocessing = preprocessingManager;

            this.SessionStartTime = runStartTime;

            this.Cache.SetContext(this);
            this.Preprocessing.SetContext(this);
        }

        #endregion
    }
}