// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebGreaseConfiguration.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   The web grease configuration root.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Activities;
    using Extensions;

    /// <summary>The web grease configuration root.</summary>
    public class WebGreaseConfiguration
    {
        /// <summary>The environment variables match pattern.</summary>
        private static readonly Regex EnvironmentVariablesMatchPattern = new Regex("%(?<name>[a-zA-Z]*?)%", RegexOptions.Compiled);

        /// <summary>The minimum cache timeout.</summary>
        private static readonly TimeSpan MinimumCacheTimeout = TimeSpan.FromHours(1);

        /// <summary>Gets or sets the global.</summary>
        private readonly Dictionary<string, GlobalConfig> global = new Dictionary<string, GlobalConfig>();

        /// <summary>Initializes a new instance of the <see cref="WebGreaseConfiguration"/> class.</summary>
        internal WebGreaseConfiguration()
        {
            this.global = new Dictionary<string, GlobalConfig>();
            this.Global = new GlobalConfig();
            this.ImageExtensions = new List<string>();
            this.ImageDirectories = new List<string>();
            this.ImageDirectoriesToHash = new List<string>();
            this.CssFileSets = new List<CssFileSet>();
            this.JSFileSets = new List<JSFileSet>();
            this.DefaultDpi = new Dictionary<string, HashSet<float>>(StringComparer.OrdinalIgnoreCase);
            this.DefaultPreprocessing = new Dictionary<string, PreprocessingConfig>();
            this.DefaultJSMinification = new Dictionary<string, JsMinificationConfig>();
            this.DefaultSpriting = new Dictionary<string, CssSpritingConfig>();
            this.DefaultCssMinification = new Dictionary<string, CssMinificationConfig>();
            this.DefaultBundling = new Dictionary<string, BundlingConfig>();
            this.DefaultCssResourcePivots = new ResourcePivotGroupCollection();
            this.DefaultJsResourcePivots = new ResourcePivotGroupCollection();
            this.LoadedConfigurationFiles = new List<string>();
        }

        /// <summary>Initializes a new instance of the <see cref="WebGreaseConfiguration"/> class.</summary>
        /// <param name="configType">Configuration type (debug/release)</param>
        /// <param name="preprocessingPluginPath">The path to the pre processing plugin assemblies.</param>
        internal WebGreaseConfiguration(string configType, string preprocessingPluginPath = null)
            : this()
        {
            this.ConfigType = configType;
            this.PreprocessingPluginPath = preprocessingPluginPath;
        }

        /// <summary>Initializes a new instance of the <see cref="WebGreaseConfiguration"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="configurationFile">The configuration file.</param>
        internal WebGreaseConfiguration(WebGreaseConfiguration configuration, FileInfo configurationFile)
            : this(
                configurationFile, 
                configuration.ConfigType, 
                configuration.SourceDirectory,
                configuration.DestinationDirectory,
                configuration.LogsDirectory,
                configuration.ToolsTempDirectory, 
                configuration.ApplicationRootDirectory, 
                configuration.PreprocessingPluginPath)
        {
            this.CacheEnabled = configuration.CacheEnabled;
            this.CacheRootPath = configuration.CacheRootPath;
            this.CacheTimeout = configuration.CacheTimeout;
            this.CacheUniqueKey = configuration.CacheUniqueKey;
            this.Measure = configuration.Measure;
            this.Overrides = configuration.Overrides;
            this.ReportPath = configuration.ReportPath;
        }

        /// <summary>Initializes a new instance of the <see cref="WebGreaseConfiguration"/> class.</summary>
        /// <param name="configuration">The configuration.</param>
        internal WebGreaseConfiguration(WebGreaseConfiguration configuration)
            : this(configuration.ConfigType, configuration.SourceDirectory, configuration.DestinationDirectory, configuration.LogsDirectory, configuration.ToolsTempDirectory, configuration.ApplicationRootDirectory, configuration.PreprocessingPluginPath)
        {
            this.CacheEnabled = configuration.CacheEnabled;
            this.CacheRootPath = configuration.CacheRootPath;
            this.CacheTimeout = configuration.CacheTimeout;
            this.CacheUniqueKey = configuration.CacheUniqueKey;
            this.Measure = configuration.Measure;
            this.Overrides = configuration.Overrides;
            this.ReportPath = configuration.ReportPath;
        }

        /// <summary>Initializes a new instance of the <see cref="WebGreaseConfiguration"/> class.</summary>
        /// <param name="configurationFile">The configuration File.</param>
        /// <param name="configType">Configuration type (debug/release)</param>
        /// <param name="sourceDirectory">The source directory, if not supplied, all relative paths are assumed
        /// to be relative to location of configuration file.</param>
        /// <param name="destinationDirectory">The destination directory where the statics should be generated.</param>
        /// <param name="logsDirectory">The directory where the logs will be generated.</param>
        /// <param name="toolsTempDirectory">The tools Temp Directory.</param>
        /// <param name="appRootDirectory">root directory of the application. Used for generating relative urls from the root. If not provided, the current directory is used.</param>
        /// <param name="preprocessingPluginPath">The path to the pre processing plugin assemblies.</param>
        internal WebGreaseConfiguration(FileInfo configurationFile, string configType, string sourceDirectory, string destinationDirectory, string logsDirectory, string toolsTempDirectory = null, string appRootDirectory = null, string preprocessingPluginPath = null)
            : this(configType, sourceDirectory, destinationDirectory, logsDirectory, toolsTempDirectory, appRootDirectory, preprocessingPluginPath)
        {
            Contract.Requires(configurationFile != null);
            Contract.Requires(configurationFile.Exists);

            if (configurationFile == null)
            {
                throw new ArgumentNullException("configType");
            }

            this.Parse(configurationFile.FullName);
        }

        /// <summary>Initializes a new instance of the <see cref="WebGreaseConfiguration"/> class.</summary>
        /// <param name="configType">The config type.</param>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="destinationDirectory">The destination directory.</param>
        /// <param name="logsDirectory">The logs directory.</param>
        /// <param name="toolsTempDirectory">The tools temp directory.</param>
        /// <param name="appRootDirectory">The app root directory.</param>
        /// <param name="preprocessingPluginPath">The preprocessing plugin path.</param>
        internal WebGreaseConfiguration(string configType, string sourceDirectory, string destinationDirectory, string logsDirectory, string toolsTempDirectory, string appRootDirectory = null, string preprocessingPluginPath = null)
            : this(configType, preprocessingPluginPath)
        {
            this.SourceDirectory = sourceDirectory;
            this.DestinationDirectory = destinationDirectory;
            this.LogsDirectory = logsDirectory;
            this.ToolsTempDirectory = toolsTempDirectory;
            this.ApplicationRootDirectory = appRootDirectory ?? Environment.CurrentDirectory;

            this.IntermediateErrorDirectory = Path.Combine(this.ApplicationRootDirectory, "IntermediateErrorFiles");

            if (!string.IsNullOrWhiteSpace(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            if (!string.IsNullOrWhiteSpace(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }
        }

        /// <summary>
        /// Gets or sets the source directory for all paths in configuration.
        /// </summary>
        public string SourceDirectory { get; set; }

        /// <summary>Gets the all dependent files.</summary>
        internal IEnumerable<string> AllLoadedConfigurationFiles
        {
            get
            {
                return this.LoadedConfigurationFiles.Concat(
                    this.CssFileSets.SelectMany(cfs => cfs.LoadedConfigurationFiles).Concat(
                    this.JSFileSets.SelectMany(cfs => cfs.LoadedConfigurationFiles)))
                    .Distinct();
            }
        }

        /// <summary>Gets the global configuration for the configuration type.</summary>
        internal GlobalConfig Global { get; private set; }

        /// <summary>
        /// The configuration type
        /// </summary>
        internal string ConfigType { get; private set; }

        /// <summary>
        /// The destination directory where the static files should be generated.
        /// </summary>
        internal string DestinationDirectory { get; set; }

        /// <summary>
        /// The directory within which the <see cref="ResourcesResolutionActivity"/> can find resource tokens.
        /// </summary>
        internal string TokensDirectory { get; set; }

        /// <summary>
        /// The directory within which the <see cref="ResourcesResolutionActivity"/> can find resource tokens meant to override any other tokens with.
        /// </summary>
        internal string OverrideTokensDirectory { get; private set; }

        /// <summary>
        /// Gets the root application directory.
        /// </summary>
        internal string ApplicationRootDirectory { get; private set; }

        /// <summary>
        /// The logs directory.
        /// </summary>
        internal string LogsDirectory { get; set; }

        /// <summary>
        /// The report directory.
        /// </summary>
        internal string ReportPath { get; set; }

        /// <summary>
        /// The tools temp directory.
        /// </summary>
        internal string ToolsTempDirectory { get; private set; }

        /// <summary>
        /// The path to the pre processing plugin assemblies
        /// </summary>
        internal string PreprocessingPluginPath { get; private set; }

        /// <summary>Gets the image directories to be used.</summary>
        internal IList<string> ImageDirectories { get; private set; }

        /// <summary>Gets the image directories to be hashed.</summary>
        internal IList<string> ImageDirectoriesToHash { get; private set; }

        /// <summary>Gets the image extensions to be used.</summary>
        internal IList<string> ImageExtensions { get; set; }

        /// <summary>Gets the css file sets to be used.</summary>
        internal IList<CssFileSet> CssFileSets { get; private set; }

        /// <summary>Gets the javascript file sets to be used.</summary>
        internal IList<JSFileSet> JSFileSets { get; private set; }

        /// <summary>Gets the external files.</summary>
        internal IList<string> LoadedConfigurationFiles { get; private set; }

        /// <summary>Gets or sets the value that determines if webgrease measures it tasks.</summary>
        internal bool Measure { get; set; }

        /// <summary>Gets or sets the default output path format.</summary>
        internal string DefaultOutputPathFormat { get; set; }

        /// <summary>
        /// Gets or sets the value that determines to use cache.
        /// </summary>
        internal bool CacheEnabled { get; set; }

        /// <summary>
        /// Gets or sets the root path used for caching, this defaults to the ToolsTempPath.
        /// Use the system temp folder (%temp%) if you want to enable this on the build server.
        /// </summary>
        internal string CacheRootPath { get; set; }

        /// <summary>
        /// Gets or sets the unique key for the unique key, is required when enabling cache.
        /// You should use the project Guid to make a distinction between cache for different projects when using a shared cache folder.
        /// </summary>
        internal string CacheUniqueKey { get; set; }

        /// <summary>
        /// gets or sets the value that determines how long to keep cache items that have not been touched. (both read and right will touch a file)
        /// </summary>
        internal TimeSpan CacheTimeout { get; set; }

        /// <summary>Gets or sets the intermediate error directory.</summary>
        internal string IntermediateErrorDirectory { get; set; }

        /// <summary>Gets or sets the dpi values</summary>
        internal IDictionary<string, HashSet<float>> DefaultDpi { get; set; }

        /// <summary>Gets or sets the overrides.</summary>
        internal TemporaryOverrides Overrides { get; set; }

        /// <summary>Gets or sets the default resource pivots.</summary>
        internal ResourcePivotGroupCollection DefaultCssResourcePivots { get; set; }

        /// <summary>Gets or sets the default resource pivots.</summary>
        internal ResourcePivotGroupCollection DefaultJsResourcePivots { get; set; }

        /// <summary>Gets or sets the default JavaScript minification configuration</summary>
        private IDictionary<string, JsMinificationConfig> DefaultJSMinification { get; set; }

        /// <summary>Gets or sets the default CSS minification configuration</summary>
        private IDictionary<string, CssMinificationConfig> DefaultCssMinification { get; set; }

        /// <summary>Gets or sets the default bundling configuration</summary>
        private IDictionary<string, BundlingConfig> DefaultBundling { get; set; }

        /// <summary>Gets or sets the default spriting configuration</summary>
        private IDictionary<string, CssSpritingConfig> DefaultSpriting { get; set; }

        /// <summary>Gets or sets the default preprocessing configuration.</summary>
        private IDictionary<string, PreprocessingConfig> DefaultPreprocessing { get; set; }

        /// <summary>Get the default theme list</summary>
        /// <param name="list">The list</param>
        /// <param name="seperatedValues">The string with seperated values</param>
        /// <param name="action">The action.</param>
        internal static void AddSeperatedValues(IList<string> list, string seperatedValues, Func<string, string> action = null)
        {
            // if it's null or whitespace, ignore it
            if (!string.IsNullOrWhiteSpace(seperatedValues))
            {
                // split it by semicolons, ignore empty entries, and add them to the list
                foreach (var theme in seperatedValues.SafeSplitSemiColonSeperatedValue())
                {
                    var trimmedValue = theme.Trim();
                    list.Add(action != null
                        ? action(trimmedValue)
                        : trimmedValue);
                }
            }
        }

        /// <summary>The get config source elements.</summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="parentFilePath">The parent file path.</param>
        /// <param name="configSourceAction">The config source action</param>
        internal static void ForEachConfigSourceElement(XElement parentElement, string parentFilePath, Action<XElement, string> configSourceAction)
        {
            var configSources = parentElement.Elements("ConfigSource").Select(e => (string)e).ToList();
            configSources.Add((string)parentElement.Attribute("configSource"));
            foreach (var configSource in configSources.Where(cs => !cs.IsNullOrWhitespace()))
            {
                var configSourceFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(parentFilePath), configSource));
                if (!File.Exists(configSourceFile))
                {
                    throw new ConfigurationErrorsException("Configuration file not found: {0}, referenced in : {1}".InvariantFormat(configSourceFile, parentFilePath));
                }

                try
                {
                    configSourceAction(XDocument.Load(configSourceFile).Root, configSourceFile);
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException("Could not load configuration file: {0}, references in {1}".InvariantFormat(configSource, parentFilePath), ex);
                }
            }
        }

        /// <summary>Validates the configuration.</summary>
        internal void Validate()
        {
            this.ApplicationRootDirectory = EnsureAndExpandDirectory(this.ApplicationRootDirectory, false);
            this.DestinationDirectory = EnsureAndExpandDirectory(this.DestinationDirectory, false);
            this.SourceDirectory = EnsureAndExpandDirectory(this.SourceDirectory, false);
            this.PreprocessingPluginPath = EnsureAndExpandDirectory(this.PreprocessingPluginPath, false);

            this.LogsDirectory = EnsureAndExpandDirectory(this.LogsDirectory, true);
            this.CacheRootPath = EnsureAndExpandDirectory(this.CacheRootPath, true);
            this.ToolsTempDirectory = EnsureAndExpandDirectory(this.ToolsTempDirectory, true);

            this.ReportPath = EnsureAndExpandDirectory(this.ReportPath ?? this.LogsDirectory, true);

            if (this.CacheTimeout > TimeSpan.Zero && this.CacheTimeout < MinimumCacheTimeout)
            {
                // Only timeout of an hour makes sense, otherwise don't use cache.
                this.CacheTimeout = MinimumCacheTimeout;
            }
        }

        /// <summary>Expands and ensures a directory exists and creates it if enabled.</summary>
        /// <param name="directory">The directory.</param>
        /// <param name="allowCreate">If it is allowed to create the directory.</param>
        /// <returns>The expanded directory.</returns>
        private static string EnsureAndExpandDirectory(string directory, bool allowCreate)
        {
            if (!string.IsNullOrWhiteSpace(directory))
            {
                directory = EnvironmentVariablesMatchPattern.Replace(
                    directory,
                    match => Environment.GetEnvironmentVariable(match.Groups["name"].Value));

                var di = new DirectoryInfo(directory);
                if (!di.Exists)
                {
                    if (allowCreate)
                    {
                        di.Create();
                    }
                    else
                    {
                        throw new DirectoryNotFoundException(directory);
                    }
                }

                return di.FullName;
            }

            return null;
        }

        /// <summary>Parses the configurations segments.</summary>
        /// <param name="configurationFile">The configuration file.</param>
        private void Parse(string configurationFile)
        {
            var element = XElement.Load(configurationFile);
            this.Parse(element, configurationFile);
        }

        /// <summary>The parse.</summary>
        /// <param name="element">The element.</param>
        /// <param name="configurationFile">The configuration file.</param>
        private void Parse(XElement element, string configurationFile)
        {
            this.ParseSettings(element.Descendants("Settings"), configurationFile);
            this.Global = this.global.GetNamedConfig(this.ConfigType);

            foreach (var cssFileSetElement in element.Descendants("CssFileSet"))
            {
                this.CssFileSets.Add(
                    new CssFileSet(
                        cssFileSetElement,
                        this.SourceDirectory,
                        this.DefaultCssMinification,
                        this.DefaultSpriting,
                        this.DefaultPreprocessing,
                        this.DefaultBundling,
                        this.DefaultCssResourcePivots,
                        this.Global,
                        this.DefaultOutputPathFormat,
                        this.DefaultDpi,
                        configurationFile));
            }

            foreach (var jsFileSetElement in element.Descendants("JsFileSet"))
            {
                this.JSFileSets.Add(
                    new JSFileSet(
                        jsFileSetElement,
                        this.SourceDirectory,
                        this.DefaultJSMinification,
                        this.DefaultPreprocessing,
                        this.DefaultBundling,
                        this.DefaultJsResourcePivots,
                        this.Global,
                        this.DefaultOutputPathFormat,
                        configurationFile));
            }
        }

        /// <summary>Parses the settings xml elements.</summary>
        /// <param name="settingsElements">The settings xml elements.</param>
        /// <param name="configurationFile">The configuration file</param>
        private void ParseSettings(IEnumerable<XElement> settingsElements, string configurationFile)
        {
            foreach (var settingsElement in settingsElements.Where(e => e != null))
            {
                this.ParseSettings(settingsElement, configurationFile);
            }
        }

        /// <summary>Parses the settings xml element.</summary>
        /// <param name="settingsElement">The settings xml element.</param>
        /// <param name="configurationFile">The configuration file</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Probably could use refactoring, but that would be a big change, todo for later.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Probably could use refactoring, but that would be a big change, todo for later.")]
        private void ParseSettings(XElement settingsElement, string configurationFile)
        {
            if (settingsElement == null)
            {
                throw new ArgumentNullException("settingsElement");
            }

            ForEachConfigSourceElement(
                settingsElement,
                configurationFile,
                (element, s) =>
                {
                    this.ParseSettings(element, s);
                    LoadedConfigurationFiles.Add(s);
                });

            foreach (var settingElement in settingsElement.Descendants())
            {
                var settingName = settingElement.Name.ToString();
                var settingValue = settingElement.Value;
                switch (settingName)
                {
                    case "ImageDirectories":
                        AddSeperatedValues(this.ImageDirectories, settingValue, value => Path.GetFullPath(Path.Combine(this.SourceDirectory, value)));
                        break;

                    case "ImageDirectoriesToHash":
                        AddSeperatedValues(this.ImageDirectoriesToHash, settingValue, value => Path.GetFullPath(Path.Combine(this.SourceDirectory, value)));
                        break;

                    case "ImageExtensions":
                        AddSeperatedValues(this.ImageExtensions, settingValue);
                        break;

                    case "Dpi":
                        var dpi = settingValue.NullSafeAction(StringExtensions.SafeSplitSemiColonSeperatedValue)
                            .Select(d => d.TryParseFloat())
                            .Where(d => d != null)
                            .Select(d => d.Value);

                        var output = (string)settingElement.Attribute("output");
                        this.DefaultDpi[output.AsNullIfWhiteSpace() ?? string.Empty] = new HashSet<float>(dpi);
                        break;

                    case "TokensDirectory":
                        this.TokensDirectory = settingValue;
                        break;

                    case "OutputPathFormat":
                        this.DefaultOutputPathFormat = settingValue;
                        break;

                    case "OverrideTokensDirectory":
                        this.OverrideTokensDirectory = settingValue;
                        break;

                    case "Locales":
                        // get the default set of locales
                        this.DefaultCssResourcePivots.Set(
                            Strings.LocalesResourcePivotKey,
                            ResourcePivotApplyMode.ApplyAsStringReplace,
                            settingValue.NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));
                        this.DefaultJsResourcePivots.Set(
                            Strings.LocalesResourcePivotKey,
                            ResourcePivotApplyMode.ApplyAsStringReplace,
                            settingValue.NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));
                        break;

                    case "Themes":
                        // get the default set of themes
                        this.DefaultCssResourcePivots.Set(
                            Strings.ThemesResourcePivotKey,
                            ResourcePivotApplyMode.ApplyAsStringReplace,
                            settingValue.NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));
                        break;

                    case "ResourcePivot":
                        this.DefaultJsResourcePivots.Set(
                            (string)settingElement.Attribute("key"),
                            ((string)settingElement.Attribute("applyMode")).TryParseToEnum<ResourcePivotApplyMode>() ?? ResourcePivotApplyMode.ApplyAsStringReplace,
                            ((string)settingElement).NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));

                        this.DefaultCssResourcePivots.Set(
                            (string)settingElement.Attribute("key"),
                            ((string)settingElement.Attribute("applyMode")).TryParseToEnum<ResourcePivotApplyMode>() ?? ResourcePivotApplyMode.ApplyAsStringReplace,
                            ((string)settingElement).NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));

                        break;
                    case "Bundling":
                        // get and the default CSS minification configuration
                        this.DefaultBundling.AddNamedConfig(new BundlingConfig(settingElement));
                        break;

                    case "Global":
                        // get and the default CSS minification configuration
                        this.global.AddNamedConfig(new GlobalConfig(settingElement));
                        break;

                    case "CssMinification":
                        // get and the default CSS minification configuration
                        this.DefaultCssMinification.AddNamedConfig(new CssMinificationConfig(settingElement));
                        break;

                    case "Spriting":
                        // get the default CSS minification configuration
                        this.DefaultSpriting.AddNamedConfig(new CssSpritingConfig(settingElement));
                        break;

                    case "JsMinification":
                        // get the default JavaScript minification configuration
                        this.DefaultJSMinification.AddNamedConfig(new JsMinificationConfig(settingElement));
                        break;

                    case "Preprocessing":
                        // get the default pre processing configuration
                        this.DefaultPreprocessing.AddNamedConfig(new PreprocessingConfig(settingElement));
                        break;
                }
            }
        }
    }
}
