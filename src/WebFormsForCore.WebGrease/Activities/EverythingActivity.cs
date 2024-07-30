// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EverythingActivity.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// <summary>
//   An activity that runs all the other activities based on configs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Configuration;
    using Css;
    using Extensions;

    using WebGrease.Css.Extensions;

    /// <summary>The main activity.</summary>
    internal sealed class EverythingActivity
    {
        /// <summary>The images destination directory name.</summary>
        private const string ImagesDestinationDirectoryName = "i";

        /// <summary>the folder name of where the js files will be stored.</summary>
        private const string JsDestinationDirectoryName = "js";

        /// <summary>directory where final css files are stored</summary>
        private const string CssDestinationDirectoryName = "css";

        /// <summary>The tools temp directory name.</summary>
        private const string ToolsTempDirectoryName = "ToolsTemp";

        /// <summary>The static assembler directory name.</summary>
        private const string StaticAssemblerDirectoryName = "StaticAssemblerOutput";

        /// <summary>The pre processing directory name.</summary>
        private const string PreprocessingTempDirectory = "PreCompiler";

        /// <summary>The tools temp directory.</summary>
        private readonly string toolsTempDirectory;

        /// <summary>The static assembler directory.</summary>
        private readonly string staticAssemblerDirectory;

        /// <summary>The log directory.</summary>
        private readonly string logDirectory;

        /// <summary>The pre processing temp directory.</summary>
        private readonly string preprocessingTempDirectory;

        /// <summary>The images log file.</summary>
        private readonly string imagesLogFile;

        /// <summary>The web grease configuration root.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>Initializes a new instance of the <see cref="EverythingActivity"/> class.</summary>
        /// <param name="context">The context.</param>
        internal EverythingActivity(IWebGreaseContext context)
        {
            Contract.Requires(context != null);

            // Assuming we get a validated WebGreaseConfiguration object here.
            this.context = context;
            this.logDirectory = context.Configuration.LogsDirectory;
            this.toolsTempDirectory = context.Configuration.ToolsTempDirectory.IsNullOrWhitespace()
                ? Path.Combine(context.Configuration.LogsDirectory, ToolsTempDirectoryName)
                : context.Configuration.ToolsTempDirectory;
            this.imagesLogFile = Path.Combine(this.logDirectory, Strings.ImagesLogFile);
            this.preprocessingTempDirectory = Path.Combine(this.toolsTempDirectory, PreprocessingTempDirectory);
            this.staticAssemblerDirectory = Path.Combine(this.toolsTempDirectory, StaticAssemblerDirectoryName);
        }

        /// <summary>The dpi to resolution name.</summary>
        /// <param name="dpi">The dpi.</param>
        /// <returns>The <see cref="string"/>.</returns>
        internal static string DpiToResolutionName(float dpi)
        {
            return "Resolution{0:0.##}X".InvariantFormat(dpi.ToString(CultureInfo.InvariantCulture).Replace(".", string.Empty));
        }

        /// <summary>Will execute the pipeline for all filesets in the provided context configuration.</summary>
        /// <returns>If it is successfull.</returns>
        internal bool Execute()
        {
            this.ExecuteHashImages();
            return this.Execute(this.context.Configuration.CssFileSets.OfType<IFileSet>().Concat(this.context.Configuration.JSFileSets));
        }

        /// <summary>The main execution point, executes for all given file sets.</summary>
        /// <param name="fileSets">The file Types.</param>
        /// <param name="fileType">The file type</param>
        /// <returns>If it failed or succeeded.</returns>
        internal bool Execute(IEnumerable<IFileSet> fileSets, FileTypes fileType = FileTypes.All)
        {
            var success = true;

            var jsFileSets = fileSets.OfType<JSFileSet>().ToArray();
            if (jsFileSets.Any())
            {
                success &= this.ExecuteJS(
                    jsFileSets,
                    this.context.Configuration.ConfigType,
                    this.context.Configuration.SourceDirectory,
                    this.context.Configuration.DestinationDirectory);
            }

            var cssFileSets = fileSets.OfType<CssFileSet>().ToArray();
            if (cssFileSets.Any())
            {
                success &= this.ExecuteCss(
                    cssFileSets,
                    this.context.Configuration.SourceDirectory,
                    this.context.Configuration.DestinationDirectory,
                    this.context.Configuration.ConfigType,
                    this.context.Configuration.ImageDirectories,
                    this.context.Configuration.ImageExtensions);
            }

            if (fileType.HasFlag(FileTypes.Image))
            {
                this.ExecuteHashImages();
            }

            return success;
        }

        /// <summary>Hashes images defined in the configuration as to be hashed.</summary>
        internal void ExecuteHashImages()
        {
            if (this.context.Configuration.ImageDirectoriesToHash.Any())
            {
                var imageHasher = this.GetImageFileHasher(this.context.Configuration.DestinationDirectory, this.context.Configuration.ImageExtensions);
                HashImages(this.context, imageHasher, this.context.Configuration.ImageDirectoriesToHash, this.context.Configuration.ImageExtensions);
                imageHasher.Save();
            }
        }

        /// <summary>Hashes a selection of files in the input path, and copies them to the output folder.</summary>
        /// <param name="context">The context.</param>
        /// <param name="hashOutputPath">Path to copy the output.</param>
        /// <param name="logFileName">log path for log data</param>
        /// <param name="fileType">The file type</param>
        /// <param name="outputRelativeToPath">The output Relative To Path.</param>
        /// <param name="basePrefixToAddToOutputPath">The base Prefix To Add To Output Path.</param>
        /// <param name="fileTypeFilters">The file type filters.</param>
        /// <returns>True if successfull false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch to handle all errors.")]
        private static FileHasherActivity GetFileHasher(IWebGreaseContext context, string hashOutputPath, string logFileName, FileTypes fileType, string outputRelativeToPath, string basePrefixToAddToOutputPath = null, IEnumerable<string> fileTypeFilters = null)
        {
            var fileTypeFilter = fileTypeFilters != null
                ? string.Join(new string(Strings.FileFilterSeparator), fileTypeFilters)
                : null;
            return new FileHasherActivity(context)
                       {
                           DestinationDirectory = hashOutputPath,
                           BasePrefixToRemoveFromOutputPathInLog = outputRelativeToPath,
                           CreateExtraDirectoryLevelFromHashes = true,
                           ShouldPreserveSourceDirectoryStructure = false,
                           LogFileName = logFileName,
                           FileType = fileType,
                           FileTypeFilter = fileTypeFilter,
                           BasePrefixToAddToOutputPath = basePrefixToAddToOutputPath
                       };
        }

        /// <summary>Gets the merged resources.</summary>
        /// <param name="context">The context.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="resourceGroupKey">The resource type filter.</param>
        /// <param name="resourceKeys">The resource resolution activity.</param>
        /// <returns>The merged resources.</returns>
        private static IDictionary<string, IDictionary<string, string>> GetMergedResources(IWebGreaseContext context, FileTypes fileType, string resourceGroupKey, IEnumerable<string> resourceKeys)
        {
            var resourcesResolutionActivity =
                new ResourcesResolutionActivity(context)
                {
                    SourceDirectory = context.Configuration.SourceDirectory,
                    ApplicationDirectoryName = context.Configuration.TokensDirectory,
                    SiteDirectoryName = context.Configuration.OverrideTokensDirectory,
                    ResourceGroupKey = resourceGroupKey,
                    FileType = fileType
                };

            resourcesResolutionActivity.ResourceKeys.AddRange(resourceKeys);
            return resourcesResolutionActivity.GetMergedResources();
        }

        /// <summary>The ensure css log file.</summary>
        /// <param name="cssHasher">The css hasher.</param>
        /// <param name="imageHasher">The image hasher.</param>
        /// <param name="cacheSection">The cache section.</param>
        private static void EnsureCssLogFile(FileHasherActivity cssHasher, FileHasherActivity imageHasher, ICacheSection cacheSection)
        {
            EnsureLogFile(cssHasher, cacheSection.GetCachedContentItems(CacheFileCategories.HashedMinifiedCssResult));
            EnsureLogFile(imageHasher, cacheSection.GetCachedContentItems(CacheFileCategories.HashedImage));
        }

        /// <summary>The get content pivot destination file path.</summary>
        /// <param name="relativeContentPath">The relative Content Path.</param>
        /// <param name="destinationDirectoryName">The destination directory name.</param>
        /// <param name="destinationExtension">The destination Extension.</param>
        /// <param name="destinationPathFormat">The pivot File Format.</param>
        /// <param name="resourcePivotKeys">The resource Pivot Keys.</param>
        /// <returns>The destination path.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Lowercase for url output.")]
        private static string GetContentPivotDestinationFilePath(string relativeContentPath, string destinationDirectoryName, string destinationExtension, string destinationPathFormat, IEnumerable<ResourcePivotKey> resourcePivotKeys = null)
        {
            // Legacy run when no format is given
            if (string.IsNullOrWhiteSpace(destinationPathFormat))
            {
                var theme = resourcePivotKeys != null ? resourcePivotKeys.FirstOrDefault(rpk => rpk.GroupKey.Equals(Strings.ThemesResourcePivotKey)) : null;
                var themeFilePrefix = theme != null && !theme.Key.IsNullOrWhitespace()
                                          ? theme.Key + "_"
                                          : string.Empty;

                var locale = resourcePivotKeys != null ? resourcePivotKeys.FirstOrDefault(rpk => rpk.GroupKey.Equals(Strings.LocalesResourcePivotKey)) : null;
                var localePath = locale != null && !locale.Key.IsNullOrWhitespace()
                                     ? locale.Key
                                     : string.Empty;

                return Path.Combine(localePath, destinationDirectoryName, themeFilePrefix + Path.ChangeExtension(relativeContentPath, destinationExtension));
            }

            // Example: {locale}/{theme}/{dpi}/{output} --> en-us/red/resolution1x/destinationPage.css
            destinationPathFormat = destinationPathFormat.ToLowerInvariant();

            if (resourcePivotKeys != null)
            {
                foreach (var resourcePivotKey in resourcePivotKeys)
                {
                    destinationPathFormat = destinationPathFormat.Replace("{" + resourcePivotKey.GroupKey.ToLowerInvariant() + "}", resourcePivotKey.Key);
                }
            }

            destinationPathFormat = destinationPathFormat.Replace("{output}", relativeContentPath);

            if (destinationPathFormat.IndexOf("{", StringComparison.OrdinalIgnoreCase) != -1)
            {
                throw new BuildWorkflowException("Could not generate the correct output file, one key was not replaced: {0}".InvariantFormat(destinationPathFormat));
            }

            return Path.Combine(destinationDirectoryName, Path.ChangeExtension(destinationPathFormat, destinationExtension));
        }

        /// <summary>The ensure js log file.</summary>
        /// <param name="jsHasher">The js hasher.</param>
        /// <param name="cacheSection">The cache section.</param>
        private static void EnsureJsLogFile(FileHasherActivity jsHasher, ICacheSection cacheSection)
        {
            EnsureLogFile(jsHasher, cacheSection.GetCachedContentItems(CacheFileCategories.HashedMinifiedJsResult));
        }

        /// <summary>The ensure log file.</summary>
        /// <param name="hasher">The hasher.</param>
        /// <param name="contentItems">The content items.</param>
        private static void EnsureLogFile(FileHasherActivity hasher, IEnumerable<ContentItem> contentItems)
        {
            hasher.AppendToWorkLog(contentItems);
        }

        /// <summary>The hash images.</summary>
        /// <param name="context">The context.</param>
        /// <param name="imageHasher">The image hasher.</param>
        /// <param name="imageDirectoriesToHash">The image directories to hash.</param>
        /// <param name="imageExtensions">The image extensions.</param>
        private static void HashImages(IWebGreaseContext context, FileHasherActivity imageHasher, IEnumerable<string> imageDirectoriesToHash, IEnumerable<string> imageExtensions)
        {
            context
                .SectionedAction(SectionIdParts.ImageHash)
                .MakeCachable(new { imageDirectoriesToHash, imageExtensions })
                .RestoreFromCacheAction(cacheSection =>
                {
                    var contentItems = cacheSection.GetCachedContentItems(CacheFileCategories.HashedImage);
                    contentItems.ForEach(ci => ci.WriteToRelativeHashedPath(context.Configuration.DestinationDirectory));
                    EnsureLogFile(imageHasher, contentItems);
                    return true;
                })
                .WhenSkipped(cacheSection => EnsureLogFile(imageHasher, cacheSection.GetCachedContentItems(CacheFileCategories.HashedImage)))
                .Execute(cacheSection =>
                {
                    var directoryInputSpecs = imageDirectoriesToHash.Select(imageDirectoryToHash => new InputSpec { Path = imageDirectoryToHash, IsOptional = true, SearchPattern = "*.*", SearchOption = SearchOption.AllDirectories });
                    directoryInputSpecs.ForEach(cacheSection.AddSourceDependency);

                    var imagesToHash = context.GetAvailableFiles(context.Configuration.SourceDirectory, imageDirectoriesToHash, imageExtensions, FileTypes.Image);
                    foreach (var imageToHash in imagesToHash)
                    {
                        var imageContentItem = ContentItem.FromFile(imageToHash.Value, imageToHash.Key);
                        var hashedContentItem = imageHasher.Hash(imageContentItem);
                        cacheSection.AddResult(hashedContentItem, CacheFileCategories.HashedImage, true);
                    }

                    return true;
                });
        }

        /// <summary>The get grouped resource keys.</summary>
        /// <param name="flatResourceKeyList">The flat resource key list.</param>
        /// <returns>The grouped resource pivot keys.</returns>
        private static IEnumerable<IEnumerable<ResourcePivotKey>> GetGroupedResourceKeys(ResourcePivotKey[] flatResourceKeyList)
        {
            var groupKeys = flatResourceKeyList.Select(k => k.GroupKey).Distinct();
            var results = new List<IEnumerable<ResourcePivotKey>>();
            foreach (var groupKey in groupKeys)
            {
                var newResults = new List<IEnumerable<ResourcePivotKey>>();
                var keys = flatResourceKeyList.Where(k => k.GroupKey.Equals(groupKey));
                foreach (var resourcePivotKey in keys)
                {
                    if (!results.Any())
                    {
                        newResults.Add(new List<ResourcePivotKey>(new[] { resourcePivotKey }));
                    }
                    else
                    {
                        foreach (var result in results)
                        {
                            newResults.Add(result.Concat(new[] { resourcePivotKey }));
                        }
                    }

                }

                results = newResults;
            }

            return results;
        }

        /// <summary>Gets the destination file paths.</summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="destinationDirectoryName">The destination directory name.</param>
        /// <param name="destinationExtension">The destination Extension.</param>
        /// <param name="destinationPathFormat">The pivot File Format.</param>
        /// <returns>The destination files as a colon seperated list.</returns>
        private IEnumerable<string> GetDestinationFilePaths(ContentItem inputFile, string destinationDirectoryName, string destinationExtension, string destinationPathFormat)
        {
            if (inputFile.ResourcePivotKeys == null || !inputFile.ResourcePivotKeys.Any())
            {
                return new[] { GetContentPivotDestinationFilePath(inputFile.RelativeContentPath, destinationDirectoryName, destinationExtension, destinationPathFormat) };
            }

            var fileNames = new List<string>();
            foreach (var groupedResourcePivotKeys in GetGroupedResourceKeys(inputFile.ResourcePivotKeys.ToArray()))
            {
                if (this.context.TemporaryIgnore(groupedResourcePivotKeys))
                {
                    continue;
                }

                fileNames.Add(GetContentPivotDestinationFilePath(inputFile.RelativeContentPath, destinationDirectoryName, destinationExtension, destinationPathFormat, groupedResourcePivotKeys));
            }

            return fileNames;
        }

        /// <summary>Execute the css pipeline.</summary>
        /// <param name="cssFileSets">The css File Sets.</param>
        /// <param name="sourceDirectory">The source Directory.</param>
        /// <param name="destinationDirectory">The destination Directory.</param>
        /// <param name="configType">The config Type.</param>
        /// <param name="imageDirectories">The image Directories.</param>
        /// <param name="imageExtensions">The image Extensions.</param>
        /// <returns>If it was successfull</returns>
        private bool ExecuteCss(IEnumerable<CssFileSet> cssFileSets, string sourceDirectory, string destinationDirectory, string configType, IList<string> imageDirectories, IList<string> imageExtensions)
        {
            var cssLogPath = Path.Combine(this.context.Configuration.LogsDirectory, Strings.CssLogFile);
            var cssHashedOutputPath = Path.Combine(destinationDirectory, CssDestinationDirectoryName);
            var imageHashedOutputPath = Path.Combine(destinationDirectory, ImagesDestinationDirectoryName);

            if (!cssFileSets.Any())
            {
                return true;
            }

            var imageHasher = this.GetImageFileHasher(destinationDirectory, imageExtensions);
            var cssHasher = GetFileHasher(this.context, cssHashedOutputPath, cssLogPath, FileTypes.CSS, this.context.Configuration.ApplicationRootDirectory);

            var totalSuccess = this.context.SectionedAction(SectionIdParts.EverythingActivity, SectionIdParts.Css)
                .MakeCachable(new { cssFileSets, sourceDirectory, destinationDirectory, configType, imageExtensions, imageDirectories }, isSkipable: true)
                .WhenSkipped(cacheSection => EnsureCssLogFile(cssHasher, imageHasher, cacheSection))
                .Execute(cacheSection =>
                {
                    var success = true;

                    foreach (var cssFileSet in cssFileSets)
                    {
                        success &= this.ExecuteCssFileSet(configType, imageDirectories, imageExtensions, cssFileSet, cssHasher, imageHasher, imageHashedOutputPath);
                    }

                    return success;
                });

            if (totalSuccess)
            {
                imageHasher.Save();
                cssHasher.Save();
            }

            return totalSuccess;
        }

        private FileHasherActivity GetImageFileHasher(string destinationDirectory, IList<string> imageExtensions)
        {
            return GetFileHasher(
                this.context,
                Path.Combine(destinationDirectory, ImagesDestinationDirectoryName),
                this.imagesLogFile,
                FileTypes.Image,
                destinationDirectory,
                "../../", // The hashed image path relative to the css path.
                imageExtensions);
        }

        /// <summary>The execute css file set.</summary>
        /// <param name="configType">The config type.</param>
        /// <param name="imageDirectories">The image directories.</param>
        /// <param name="imageExtensions">The image extensions.</param>
        /// <param name="cssFileSet">The css file set.</param>
        /// <param name="cssHasher">The css hasher.</param>
        /// <param name="imageHasher">The image hasher.</param>
        /// <param name="imagesDestinationDirectory">The images Destination Directory.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Needs refactoring at some point, just like all other methods in everything activity.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Needs refactoring at some point, just like all other methods in everything activity.")]
        private bool ExecuteCssFileSet(string configType, IList<string> imageDirectories, IList<string> imageExtensions, CssFileSet cssFileSet, FileHasherActivity cssHasher, FileHasherActivity imageHasher, string imagesDestinationDirectory)
        {
            var cssSpritingConfig = cssFileSet.ImageSpriting.GetNamedConfig(configType);
            var cssMinificationConfig = cssFileSet.Minification.GetNamedConfig(configType);
            var cssFileSetVarBySettings = new
            {
                configType,
                ImageSpriting = cssSpritingConfig,
                Global = cssFileSet.GlobalConfig,
                Bundling = cssFileSet.Bundling.GetNamedConfig(configType),
                Minification = cssMinificationConfig,
                Preprocessing = cssFileSet.Preprocessing.GetNamedConfig(configType),
                cssFileSet.Locales,
                cssFileSet.Themes,
            };

            return this.context
                .SectionedAction(SectionIdParts.CssFileSet)
                .MakeCachable(cssFileSet, cssFileSetVarBySettings, true)
                .WhenSkipped(cacheSection => EnsureCssLogFile(cssHasher, imageHasher, cacheSection))
                .RestoreFromCacheAction(cacheSection =>
                {
                    cssFileSet.LoadedConfigurationFiles.ForEach(cacheSection.AddSourceDependency);

                    var hashedContentItems = cacheSection.GetCachedContentItems(CacheFileCategories.HashedMinifiedCssResult);
                    hashedContentItems.ForEach(ci => ci.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory));
                    EnsureLogFile(cssHasher, hashedContentItems);

                    var hashedImages = cacheSection.GetCachedContentItems(CacheFileCategories.HashedImage);
                    hashedImages.ForEach(ci => ci.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory));
                    EnsureLogFile(imageHasher, hashedImages);

                    var hashedSprites = cacheSection.GetCachedContentItems(CacheFileCategories.HashedSpriteImage);
                    hashedSprites.ForEach(ci => ci.WriteToContentPath(this.context.Configuration.DestinationDirectory));

                    return hashedContentItems.Any();
                })
                .Execute(cacheSection =>
                {
                    var mergedResouresToApplyAsStringReplace = cssFileSet.ResourcePivots
                        .Where(rp => rp.ApplyMode == ResourcePivotApplyMode.ApplyAsStringReplace)
                        .ToDictionary(
                            rpg => rpg.Key,
                            rpg => GetMergedResources(this.context, FileTypes.CSS, rpg.Key, rpg.Keys));

                    var mergedResouresToApplyAsCss = cssFileSet.ResourcePivots
                        .Where(rp => rp.ApplyMode != ResourcePivotApplyMode.ApplyAsStringReplace)
                        .ToDictionary(
                            rpg => rpg.Key,
                            rpg => GetMergedResources(this.context, FileTypes.CSS, rpg.Key, rpg.Keys));

                    var dpiResources = GetMergedResources(this.context, FileTypes.CSS, Strings.DpiResourcePivotKey, cssFileSet.Dpi.Select(DpiToResolutionName));

                    var cssMinifier = this.CreateCssMinifier(imageHasher, imageExtensions, imageDirectories, imagesDestinationDirectory, cssMinificationConfig, cssSpritingConfig, cssFileSet.Dpi, mergedResouresToApplyAsCss, dpiResources);
                    var outputFile = Path.Combine(this.staticAssemblerDirectory, cssFileSet.Output);
                    var inputFiles = this.Bundle(cssFileSet, outputFile, FileTypes.CSS, configType, cssMinifier.ShouldMinify);
                    if (inputFiles == null)
                    {
                        return false;
                    }

                    // Resource pivots
                    this.context.Log.Information(ResourceStrings.ResolvingTokensAndPerformingLocalization);

                    var resourcedContentItems = this.ApplyResources(inputFiles, mergedResouresToApplyAsStringReplace);
                    var resourceSuccess = resourcedContentItems.All(l => l != null);

                    if (!resourceSuccess)
                    {
                        // localization failed for this batch
                        this.context.Log.Error(null, ResourceStrings.ThereWereErrorsWhileApplyingCssresources);
                        return false;
                    }

                    var minifiedCssItems = this.MinifyCss(resourcedContentItems, cssMinifier, imageHasher, cssSpritingConfig.WriteLogFile, mergedResouresToApplyAsCss);

                    if (minifiedCssItems.Any(i => i == null))
                    {
                        // localization failed for this batch
                        this.context.Log.Error(null, ResourceStrings.ThereWereErrorsWhileMinifyingTheCssFiles);
                        return false;
                    }

                    var hashedCssItems = this.HashContentItems(cssHasher, minifiedCssItems.SelectMany(i => i.Css).Where(n => n != null), CssDestinationDirectoryName, Strings.Css, cssFileSet.OutputPathFormat);
                    hashedCssItems.ForEach(hi => cacheSection.AddResult(hi, CacheFileCategories.HashedMinifiedCssResult));

                    var hashedImageItems = minifiedCssItems.SelectMany(mci => mci.HashedImages).Where(n => n != null);
                    hashedImageItems.ForEach(hi => cacheSection.AddResult(hi, CacheFileCategories.HashedImage));

                    return true;
                });
        }

        /// <summary>Execute the javascript pipeline.</summary>
        /// <param name="jsFileSets">The js File Sets.</param>
        /// <param name="configType">The config Type.</param>
        /// <param name="sourceDirectory">The source Directory.</param>
        /// <param name="destinationDirectory">The destination Directory.</param>
        /// <returns>If it was successfull</returns>
        private bool ExecuteJS(IEnumerable<JSFileSet> jsFileSets, string configType, string sourceDirectory, string destinationDirectory)
        {
            var jsLogPath = Path.Combine(this.context.Configuration.LogsDirectory, Strings.JsLogFile);
            var jsHashedOutputPath = Path.Combine(destinationDirectory, JsDestinationDirectoryName);

            if (!jsFileSets.Any())
            {
                return true;
            }

            var jsHasher = GetFileHasher(this.context, jsHashedOutputPath, jsLogPath, FileTypes.JS, this.context.Configuration.ApplicationRootDirectory);
            var varBySettings = new { jsFileSets, configType, sourceDirectory, destinationDirectory };
            var totalSuccess = this.context.SectionedAction(SectionIdParts.EverythingActivity, SectionIdParts.Js)
                .MakeCachable(varBySettings, isSkipable: true)
                .WhenSkipped(cacheSection => EnsureJsLogFile(jsHasher, cacheSection))
                .Execute(cacheSection =>
                {
                    var success = true;

                    // process each js file set.
                    foreach (var jsFileSet in jsFileSets)
                    {
                        success &= this.ExecuteJSFileSet(jsFileSet, jsHasher, configType);
                    }

                    return success;
                });

            if (totalSuccess)
            {
                jsHasher.Save();
            }

            return totalSuccess;
        }

        /// <summary>The execute js file set.</summary>
        /// <param name="jsFileSet">The js file set.</param>
        /// <param name="jsHasher">The js hasher.</param>
        /// <param name="configType">The config Type.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "WebGrease.LogManager.Information(System.String,WebGrease.MessageImportance)", Justification = "Debug messages")]
        private bool ExecuteJSFileSet(JSFileSet jsFileSet, FileHasherActivity jsHasher, string configType)
        {
            return this.context.SectionedAction(SectionIdParts.JsFileSet)
                .MakeCachable(jsFileSet, new { configType }, true)
                .RestoreFromCacheAction(cacheSection =>
                {
                    var hashedContentItems = cacheSection.GetCachedContentItems(CacheFileCategories.HashedMinifiedJsResult);
                    hashedContentItems.ForEach(ci => ci.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory));
                    EnsureLogFile(jsHasher, hashedContentItems);
                    return hashedContentItems.Any();
                })
                .WhenSkipped(cacheSection => EnsureJsLogFile(jsHasher, cacheSection))
                .Execute(cacheSection =>
                    {
                        jsFileSet.LoadedConfigurationFiles.ForEach(cacheSection.AddSourceDependency);
                        var jsMinificationConfig = jsFileSet.Minification.GetNamedConfig(configType);
                        var outputFile = Path.Combine(this.staticAssemblerDirectory, jsFileSet.Output);

                        // bundling
                        var bundledFiles = this.Bundle(jsFileSet, outputFile, FileTypes.JS, configType, jsMinificationConfig.ShouldMinify);

                        if (bundledFiles == null)
                        {
                            return false;
                        }

                        // resolve the resources
                        var mergedResoures = jsFileSet.ResourcePivots
                            .ToDictionary(
                                rpg => rpg.Key,
                                rpg => GetMergedResources(this.context, FileTypes.CSS, rpg.Key, rpg.Keys));

                        // localize
                        this.context.Log.Information(ResourceStrings.ResolvingTokensAndPerformingLocalization);
                        var localizedJsFiles = this.ApplyResources(
                            bundledFiles,
                            mergedResoures);

                        if (localizedJsFiles == null)
                        {
                            this.context.Log.Error(null, "There were errors encountered while resolving tokens.");
                            return false;
                        }

                        this.context.Log.Information("Minimizing javascript files");

                        var minifiedContentItems = this.MinifyJs(
                            localizedJsFiles,
                            jsMinificationConfig,
                            jsFileSet.Validation.GetNamedConfig(configType));

                        if (minifiedContentItems.Any(ci => ci == null))
                        {
                            this.context.Log.Error(
                                null, "There were errors encountered while minimizing javascript files.");

                            return false;
                        }

                        var hashedContentItems = this.HashContentItems(jsHasher, minifiedContentItems, JsDestinationDirectoryName, Strings.JS, jsFileSet.OutputPathFormat);
                        hashedContentItems.ForEach(ci => cacheSection.AddResult(ci, CacheFileCategories.HashedMinifiedJsResult, true));

                        return true;
                    });
        }

        /// <summary>The hash content items.</summary>
        /// <param name="hasher">The hasher.</param>
        /// <param name="contentItems">The content items.</param>
        /// <param name="destinationDirectoryName">The destination directory name.</param>
        /// <param name="destinationExtension">The destination extension.</param>
        /// <param name="destinationPathFormat">The destination Path Format.</param>
        /// <returns>The hashed content items</returns>
        private IEnumerable<ContentItem> HashContentItems(FileHasherActivity hasher, IEnumerable<ContentItem> contentItems, string destinationDirectoryName, string destinationExtension, string destinationPathFormat)
        {
            var hashedFiles = new List<ContentItem>();
            foreach (var contentItem in contentItems.Where(ci => ci != null))
            {
                var destinationFilePaths = this.GetDestinationFilePaths(contentItem, destinationDirectoryName, destinationExtension, destinationPathFormat);
                hashedFiles.AddRange(hasher.Hash(contentItem, destinationFilePaths));
            }

            return hashedFiles;
        }

        /// <summary>Executes bundling.</summary>
        /// <param name="fileSet">The file set.</param>
        /// <param name="outputFile">The output file.</param>
        /// <param name="fileType">The file type.</param>
        /// <param name="configType">The config type.</param>
        /// <param name="minimalOutput">Is the ggoal to have the most minimal output (true skips lots of comments)</param>
        /// <returns>The resulting files.</returns>
        private IEnumerable<ContentItem> Bundle(IFileSet fileSet, string outputFile, FileTypes fileType, string configType, bool minimalOutput)
        {
            var bundleConfig = fileSet.Bundling.GetNamedConfig(configType);
            var preprocessingConfig = fileSet.Preprocessing.GetNamedConfig(this.context.Configuration.ConfigType);

            if (bundleConfig.ShouldBundleFiles)
            {
                this.context.Log.Information(ResourceStrings.BundlingFiles);
                var resultFile = this.BundleFiles(fileSet.InputSpecs, outputFile, preprocessingConfig, fileType, minimalOutput || bundleConfig.MinimalOutput);
                if (resultFile == null)
                {
                    // bundling failed
                    this.context.Log.Error(null, ResourceStrings.ThereWereErrorsWhileBundlingFiles);
                    return null;
                }

                // input for the next step is the output file from bundling
                return new[] { resultFile };
            }

            if (preprocessingConfig != null && preprocessingConfig.Enabled)
            {
                // bundling calls the preprocessor, so we need to do it seperately if there was no bundling.
                return this.PreprocessFiles(this.preprocessingTempDirectory, fileSet.InputSpecs, preprocessingConfig);
            }

            fileSet.InputSpecs.ForEach(this.context.Cache.CurrentCacheSection.AddSourceDependency);
            return fileSet.InputSpecs
                .GetFiles(this.context.Configuration.SourceDirectory)
                .Select(f => ContentItem.FromFile(f, f, this.context.Configuration.SourceDirectory));
        }

        /// <summary>The create css minifier.</summary>
        /// <param name="imageHasher">The image hasher.</param>
        /// <param name="imageExtensions">The image Extensions.</param>
        /// <param name="imageDirectories">The image Directories.</param>
        /// <param name="imagesDestinationDirectory">The images Destination Directory.</param>
        /// <param name="minificationConfig">The minification Config.</param>
        /// <param name="spritingConfig">The spriting Config.</param>
        /// <param name="dpi">The dpi values</param>
        /// <param name="mergedResoures">The merged Resoures.</param>
        /// <param name="dpiResources">The dpi Resources.</param>
        /// <returns>The <see cref="MinifyCssActivity"/>.</returns>
        private MinifyCssActivity CreateCssMinifier(FileHasherActivity imageHasher, IList<string> imageExtensions, IList<string> imageDirectories, string imagesDestinationDirectory, CssMinificationConfig minificationConfig, CssSpritingConfig spritingConfig, HashSet<float> dpi, Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResoures, IDictionary<string, IDictionary<string, string>> dpiResources)
        {
            var cssMinifier = new MinifyCssActivity(this.context)
            {
                ShouldAssembleBackgroundImages = spritingConfig.ShouldAutoSprite,
                ShouldMinify = minificationConfig.ShouldMinify,
                ShouldMergeMediaQueries = minificationConfig.ShouldMergeMediaQueries,
                ShouldOptimize = minificationConfig.ShouldMinify || minificationConfig.ShouldOptimize,
                ShouldValidateForLowerCase = minificationConfig.ShouldValidateLowerCase,
                ShouldExcludeProperties = minificationConfig.ShouldExcludeProperties,
                ShouldMergeBasedOnCommonDeclarations =  minificationConfig.ShouldMergeBasedOnCommonDeclarations,
                ShouldPreventOrderBasedConflict = minificationConfig.ShouldPreventOrderBasedConflict,
                ImageExtensions = imageExtensions,
                ImageDirectories = imageDirectories,
                BannedSelectors = new HashSet<string>(minificationConfig.RemoveSelectors.ToArray()),
                HackSelectors = new HashSet<string>(minificationConfig.ForbiddenSelectors.ToArray()),
                NonMergeSelectors = new HashSet<string>(minificationConfig.NonMergeSelectors.ToArray()),
                ImageAssembleReferencesToIgnore = new HashSet<string>(spritingConfig.ImagesToIgnore.ToArray()),
                ImageAssemblyPadding = spritingConfig.ImagePadding,
                ErrorOnInvalidSprite = spritingConfig.ErrorOnInvalidSprite,
                OutputUnit = spritingConfig.OutputUnit,
                OutputUnitFactor = spritingConfig.OutputUnitFactor,
                ImagesOutputDirectory = imagesDestinationDirectory,
                IgnoreImagesWithNonDefaultBackgroundSize = spritingConfig.IgnoreImagesWithNonDefaultBackgroundSize,
                ImageBasePrefixToRemoveFromOutputPathInLog = imageHasher != null ? imageHasher.BasePrefixToRemoveFromOutputPathInLog : null,
                ImageBasePrefixToAddToOutputPath = imageHasher != null ? imageHasher.BasePrefixToAddToOutputPath : null,
                ForcedSpritingImageType = spritingConfig.ForceImageType,
                Dpi = dpi,
                MergedResources = mergedResoures,
                DpiResources = dpiResources
            };
            return cssMinifier;
        }

        /// <summary>
        /// Pre processes each file in the inputs list, outputs them into the target folder, using filename.defaultTargetExtensions, or if the same as input extension, .processed.defaultTargetExtensions
        /// </summary>
        /// <param name="targetFolder">Target folder</param>
        /// <param name="inputFiles">Input files</param>
        /// <param name="preprocessingConfig">The pre processing config </param>
        /// <returns>The preprocessed file</returns>
        private IEnumerable<ContentItem> PreprocessFiles(string targetFolder, IEnumerable<InputSpec> inputFiles, PreprocessingConfig preprocessingConfig)
        {
            var preprocessorActivity = new PreprocessorActivity(this.context)
                {
                    OutputFolder = targetFolder,
                    PreprocessingConfig = preprocessingConfig,
                };

            preprocessorActivity.Inputs.AddRange(inputFiles);
            return preprocessorActivity.Execute();
        }

        /// <summary>Minifies css files.</summary>
        /// <param name="inputCssItems">The css content input item.</param>
        /// <param name="minifier">The css minifier</param>
        /// <param name="imageHasher">The image Hasher.</param>
        /// <param name="writeSpriteLogFile">If set to tue, it will write a log file for each sprite.</param>
        /// <param name="mergedResources">The merged Resources.</param>
        /// <returns>True is successfull false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all in order to log errors.")]
        private IEnumerable<MinifyCssResult> MinifyCss(
            IEnumerable<ContentItem> inputCssItems,
            MinifyCssActivity minifier,
            FileHasherActivity imageHasher,
            bool writeSpriteLogFile,
            Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResources)
        {
            var results = new List<MinifyCssResult>();
            foreach (var inputFile in inputCssItems)
            {
                var sourceFile = inputFile.RelativeContentPath;
                var destinationFile = inputFile.RelativeContentPath;

                var pivots = inputFile.ResourcePivotKeys.Select(p => p.ToString());
                var pivotFileExtensions = string.Join(".", pivots);
                this.context.Log.Information("Css Minify start: {0} : {1}".InvariantFormat(destinationFile, pivotFileExtensions));

                minifier.SourceFile = sourceFile;
                minifier.MergedResources = mergedResources;
                minifier.DestinationFile = destinationFile;

                if (writeSpriteLogFile)
                {
                    var firstPivot = inputFile.ResourcePivotKeys.FirstOrDefault();
                    minifier.ImageSpritingLogPath = Path.Combine(this.context.Configuration.ReportPath, destinationFile + ((firstPivot != null) ? "." + firstPivot.ToString("{0}.{1}") : string.Empty) + ".spritingLog.xml");
                }

                try
                {
                    // execute the minifier on the css.
                    var result = minifier.Process(inputFile, imageHasher);

                    // Add the result to the results.
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(null);
                    this.HandleCssAggregateException(ex, sourceFile, inputFile);
                }
            }

            return results;
        }

        private void HandleCssAggregateException(Exception ex, string sourceFile, ContentItem inputFile)
        {
            AggregateException aggEx;
            if ((aggEx = ex as AggregateException) != null || ((ex.InnerException != null) && (aggEx = ex.InnerException as AggregateException) != null))
            {
                var cssExceptions = new List<Antlr.Runtime.RecognitionException>();
                var aggregateExceptions = new List<AggregateException>();
                var otherExceptions = new List<Exception>();
                foreach (var innerException in aggEx.InnerExceptions)
                {
                    var cssException = innerException as Antlr.Runtime.RecognitionException;
                    if (cssException != null)
                    {
                        cssExceptions.Add(cssException);
                    }
                    else
                    {
                        var aggregateException = innerException as AggregateException;
                        if (aggregateException != null)
                        {
                            aggregateExceptions.Add(aggregateException);
                        }
                        else
                        {
                            otherExceptions.Add(innerException);
                        }
                    }
                }

                var errors = cssExceptions.CreateBuildErrors(sourceFile);
                foreach (var error in errors)
                {
                    this.HandleError(inputFile, error, sourceFile);
                }

                foreach (var aggregateException in aggregateExceptions)
                {
                    this.HandleCssAggregateException(aggregateException, sourceFile, inputFile);
                }

                foreach (var otherException in otherExceptions)
                {
                    this.HandleError(inputFile, otherException, sourceFile);
                }
            }
            else
            {
                // Catch, record and display error
                this.HandleError(inputFile, ex, sourceFile);
            }
        }

        /// <summary>Minify js activity</summary>
        /// <param name="inputFiles">path to localized js files to be minified</param>
        /// <param name="jsConfig">The js Config.</param>
        /// <param name="jsValidateConfig">The js Validate Config.</param>
        /// <returns>True if successfull false if not.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all in order to log errors.")]
        private IEnumerable<ContentItem> MinifyJs(IEnumerable<ContentItem> inputFiles, JsMinificationConfig jsConfig, JSValidationConfig jsValidateConfig)
        {
            var results = new List<ContentItem>();
            var minifier = new MinifyJSActivity(this.context)
            {
                ShouldMinify = jsConfig.ShouldMinify,
                ShouldAnalyze = jsValidateConfig.ShouldAnalyze,
                AnalyzeArgs = jsValidateConfig.AnalyzeArguments
            };

            // if we specified some globals to ignore, format them on the command line with the
            // other minification arguments
            if (!string.IsNullOrWhiteSpace(jsConfig.GlobalsToIgnore))
            {
                minifier.MinifyArgs = Strings.GlobalsToIgnoreArg + jsConfig.GlobalsToIgnore + ' ' + jsConfig.MinificationArugments;
            }
            else
            {
                minifier.MinifyArgs = jsConfig.MinificationArugments;
            }

            foreach (var inputFile in inputFiles)
            {
                var sourceFile = inputFile.RelativeContentPath;
                if (!this.context.TemporaryIgnore(inputFile.ResourcePivotKeys))
                {
                    this.context.Log.Information("Js Minify start: {0}{1}".InvariantFormat(sourceFile, string.Join(string.Empty, inputFile.ResourcePivotKeys.Select(p => p.ToString()))));
                    try
                    {
                        results.Add(minifier.Minify(inputFile));
                    }
                    catch (Exception ex)
                    {
                        results.Add(null);
                        this.HandleError(inputFile, ex, sourceFile);
                    }
                }
            }

            return results;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch and handle/pass down all errors on purpose.")]
        private IEnumerable<ContentItem> ApplyResources(IEnumerable<ContentItem> inputItems, Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResource)
        {
            if (!mergedResource.Any())
            {
                return inputItems;
            }

            var results = new List<ContentItem>();

            foreach (var inputItem in inputItems)
            {
                try
                {
                    results.AddRange(ResourcePivotActivity.ApplyResourceKeys(inputItem, mergedResource));
                }
                catch (Exception ex)
                {
                    this.HandleError(inputItem, ex, inputItem.RelativeContentPath);
                    results.Add(null);
                }
            }

            return results;
        }

        /// <summary>Combine files discovered through the input specs into the output file</summary>
        /// <param name="inputSpecs">A collection of files to be processed</param>
        /// <param name="outputFile">name of the output file</param>
        /// <param name="preprocessing">The preprocessing.</param>
        /// <param name="fileType">JavaScript of Stylesheets</param>
        /// <param name="minimalOutput">Is the ggoal to have the most minimal output (true skips lots of comments)</param>
        /// <returns>a value indicating whether the operation was successful</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to catch all in order to log errors.")]
        private ContentItem BundleFiles(IEnumerable<InputSpec> inputSpecs, string outputFile, PreprocessingConfig preprocessing, FileTypes fileType, bool minimalOutput)
        {
            // now we have the input prepared, so use Assembler activity to create the one file to use as input (if we were't assembling, we'd need to grab all) 
            // we are bundling either JS or CSS files -- for JS files we want to append semicolons between them and use single-line comments; for CSS file we don't.
            var assemblerActivity = new AssemblerActivity(this.context)
                {
                    PreprocessingConfig = preprocessing,
                    AddSemicolons = fileType == FileTypes.JS,
                    MinimalOutput = minimalOutput
                };

            foreach (var inputSpec in inputSpecs)
            {
                assemblerActivity.Inputs.Add(inputSpec);
            }

            assemblerActivity.OutputFile = outputFile;

            try
            {
                return assemblerActivity.Execute(ContentItemType.Value);
            }
            catch (Exception ex)
            {
                // catch/record/display error.
                this.HandleError(null, ex);
            }

            return null;
        }

        /// <summary>
        /// general handler for errors
        /// </summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="ex">exception caught</param>
        /// <param name="file">File being processed that caused the error.</param>
        private void HandleError(ContentItem contentItem, Exception ex, string file = null)
        {
            if (ex.InnerException is BuildWorkflowException)
            {
                ex = ex.InnerException;
            }

            var bwe = ex as BuildWorkflowException;
            if (contentItem != null)
            {
                file = this.context.EnsureErrorFileOnDisk(bwe != null ? bwe.File : file, contentItem);
                if (bwe != null)
                {
                    bwe.File = file;
                }
            }

            if (!string.IsNullOrWhiteSpace(file) && (bwe == null || bwe.File.IsNullOrWhitespace()))
            {
                this.context.Log.Error(null, string.Format(CultureInfo.InvariantCulture, ResourceStrings.ErrorsInFileFormat, file), file);
            }

            this.context.Log.Error(ex, ex.ToString());

            var aggEx = ex as AggregateException;
            if (aggEx != null)
            {
                foreach (var innerException in aggEx.InnerExceptions)
                {
                    this.HandleError(contentItem, innerException);
                }
            }
            else if (ex.InnerException != null)
            {
                this.HandleError(contentItem, ex.InnerException);
            }
        }
    }
}
