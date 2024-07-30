// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinifyCssActivity.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Implements the multiple steps in Css pipeline
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Common;
    using Css;
    using Css.Ast;
    using Css.Extensions;
    using Css.ImageAssemblyAnalysis;
    using Css.Visitor;
    using ImageAssemble;

    using WebGrease.Configuration;
    using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
    using WebGrease.Extensions;

    /// <summary>Implements the multiple steps in Css pipeline</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Should probably be refactored at some point.")]
    internal sealed class MinifyCssActivity
    {
        /// <summary>The url hash regex pattern.</summary>
        private static readonly Regex UrlHashRegexPattern = new Regex(@"url\((?<quote>[""']?)(?:hash\((?<url>[^)]*))\)(?<extra>.*?)\k<quote>\)", RegexOptions.Compiled);

        /// <summary>The context.</summary>
        private readonly IWebGreaseContext context;

        /// <summary>The available source images.</summary>
        private IDictionary<string, string> availableSourceImages;

        /// <summary>Initializes a new instance of the <see cref="MinifyCssActivity"/> class.</summary>
        /// <param name="context">The context.</param>
        internal MinifyCssActivity(IWebGreaseContext context)
        {
            this.context = context;
            this.HackSelectors = new HashSet<string>();
            this.BannedSelectors = new HashSet<string>();
            this.ShouldExcludeProperties = true;
            this.ShouldValidateForLowerCase = false;
            this.ShouldOptimize = true;
            this.ShouldAssembleBackgroundImages = true;
            this.ImageAssembleReferencesToIgnore = new HashSet<string>();
            this.OutputUnitFactor = 1;
            this.ShouldPreventOrderBasedConflict = false;
            this.ShouldMergeBasedOnCommonDeclarations = false;
        }

        /// <summary>Gets or sets the image base prefix to remove from output path in log.</summary>
        internal string ImageBasePrefixToRemoveFromOutputPathInLog { get; set; }

        /// <summary>Gets or sets the image base prefix to add to output path.</summary>
        internal string ImageBasePrefixToAddToOutputPath { get; set; }

        /// <summary>Gets OutputUnit (Default: px, other possible values: rem/em etc..).</summary>
        internal string OutputUnit { private get; set; }

        /// <summary>Gets or sets the url to be used when an image in a css file cannot be found.</summary>
        internal string MissingImageUrl { private get; set; }

        /// <summary>Gets OutputUnitFactor (Default: 1, example value for 10px based REM: 0.625</summary>
        internal double OutputUnitFactor { private get; set; }

        /// <summary>Gets or sets the forced spriting image type.</summary>
        internal ImageType? ForcedSpritingImageType { get; set; }

        /// <summary>Gets or sets a value indicating whether to ignore images that have a background-size property set to non-default ('auto' or 'auto auto').</summary>
        internal bool IgnoreImagesWithNonDefaultBackgroundSize { private get; set; }

        /// <summary>Gets or sets the image directories.</summary>
        internal IList<string> ImageDirectories { private get; set; }

        /// <summary>Gets or sets the image extensions.</summary>
        internal IList<string> ImageExtensions { private get; set; }

        /// <summary>Gets or sets Source File.</summary>
        internal string SourceFile { private get; set; }

        /// <summary>Gets or sets Destination File.</summary>
        internal string DestinationFile { get; set; }

        /// <summary>Gets or sets a value indicating whether css should exclude properties marked with "Exclude".</summary>
        internal bool ShouldExcludeProperties { get; set; }

        /// <summary>Gets or sets a value indicating whether css should be validated for lower case.</summary>
        internal bool ShouldValidateForLowerCase { get; set; }

        /// <summary>Gets or sets a value indicating whether css should be optimized for colors, float, duplicate selectors etc.</summary>
        internal bool ShouldOptimize { private get; set; }

        /// <summary>Gets or sets a value indicating whether the activity should merge media queries, only gets used when ShouldOptimize is set to true.</summary>
        internal bool ShouldMergeMediaQueries { private get; set; }

        /// <summary>Gets or sets whether to assemble CSS background Images and update the coordinates.</summary>
        internal bool ShouldAssembleBackgroundImages { private get; set; }

        /// <summary>Gets or sets a value indicating whether ShouldMinify.</summary>
        internal bool ShouldMinify { get; set; }

        /// <summary>Gets HackSelectors.</summary>
        internal HashSet<string> HackSelectors { get; set; }

        /// <summary>Gets BannedSelectors.</summary>
        internal HashSet<string> BannedSelectors { get; set; }

        /// <summary>Gets or sets the none merge selectors.</summary>
        internal HashSet<string> NonMergeSelectors { get; set; }

        /// <summary>Gets or sets Image Assembly Scan Output.</summary>
        /// <remarks>Optional - Needed only when image spriting is needed.</remarks>
        internal string ImageAssembleScanDestinationFile { get; set; }

        /// <summary>Gets or sets the image spriting log path, if set this is the location it will save the image spriting log for this pass.</summary>
        internal string ImageSpritingLogPath { get; set; }

        /// <summary>Gets or sets the image output directory.</summary>
        internal string ImagesOutputDirectory { private get; set; }

        /// <summary>Gets ImageAssembleReferencesToIgnore.</summary>
        internal HashSet<string> ImageAssembleReferencesToIgnore { get; set; }

        /// <summary>Gets or sets the image assembly padding.</summary>
        internal int? ImageAssemblyPadding { private get; set; }

        /// <summary>Gets or sets a value indicating whether it logs an error on invalid sprites.</summary>
        internal bool ErrorOnInvalidSprite { get; set; }

        /// <summary>Gets or sets the dpi.</summary>
        internal HashSet<float> Dpi { get; set; }

        /// <summary>Gets or sets the dpi resources.</summary>
        internal IDictionary<string, IDictionary<string, string>> DpiResources { get; set; }

        /// <summary>Gets or sets the merged resources.</summary>
        internal Dictionary<string, IDictionary<string, IDictionary<string, string>>> MergedResources { get; set; }

        /// <summary> 
        /// Gets or sets the ShouldPreventOrderBasedConflic
        /// </summary>
        internal bool ShouldPreventOrderBasedConflict { get; set; }

        /// <summary>
        /// Gets or sets the ShouldMergeBasedOnCommonDeclarations
        /// </summary>
        internal bool ShouldMergeBasedOnCommonDeclarations { get; set; }

        /// <summary>The process.</summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="imageHasher">The image hasher.</param>
        /// <returns>The <see cref="MinifyCssResult"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Extension methods count as classes part of complexity.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Sprited", Justification = "Debug ONLY, remove before checkin")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "WebGrease.LogManager.Error(System.String)", Justification = "Debug ONLY, remove before checkin")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Refactor in a later iteration")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all by default")]
        internal MinifyCssResult Process(ContentItem contentItem, FileHasherActivity imageHasher = null)
        {
            if (imageHasher != null)
            {
                this.availableSourceImages = this.context.GetAvailableFiles(this.context.Configuration.SourceDirectory, this.ImageDirectories, this.ImageExtensions, FileTypes.Image);
            }

            var cssContent = contentItem.Content;

            var minifiedContentItems = new BlockingCollection<ContentItem>();
            var hashedImageContentItems = new BlockingCollection<ContentItem>();
            var spritedImageContentItems = new BlockingCollection<ContentItem>();
            var mergedResources = ResourcePivotActivity.GetUsedGroupedResources(cssContent, this.MergedResources);

            var dpiValues = this.Dpi;
            if (dpiValues == null || !dpiValues.Any())
            {
                dpiValues = new HashSet<float>(new[] { 1f });
            }

            var pivots = GetMinifyCssPivots(contentItem, dpiValues, mergedResources, this.DpiResources).ToArray();

            var nonIgnoredPivots = pivots.Where(p => !this.context.TemporaryIgnore(p.NewContentResourcePivotKeys)).ToArray();

            var parsedStylesheetNode = CssParser.Parse(this.context, cssContent, false);
            this.context.ParallelForEach(
                item => new[] { SectionIdParts.MinifyCssActivity },
                nonIgnoredPivots,
                (threadContext, pivot, parallelLoopState) =>
                {
                    ContentItem minifiedContentItem = null;
                    var resourceContentItem = ContentItem.FromContent(contentItem.Content, pivot.NewContentResourcePivotKeys);
                    var result = threadContext
                        .SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.Process)
                        .MakeCachable(resourceContentItem, this.GetVarBySettings(imageHasher, pivot.NewContentResourcePivotKeys, pivot.MergedResource))
                        .RestoreFromCacheAction(cacheSection =>
                        {
                            minifiedContentItem = cacheSection.GetCachedContentItem(CacheFileCategories.MinifiedCssResult, contentItem.RelativeContentPath, contentItem.AbsoluteDiskPath, pivot.NewContentResourcePivotKeys);
                            hashedImageContentItems.AddRange(cacheSection.GetCachedContentItems(CacheFileCategories.HashedImage));
                            spritedImageContentItems.AddRange(cacheSection.GetCachedContentItems(CacheFileCategories.HashedSpriteImage));

                            if (minifiedContentItem == null)
                            {
                                context.Log.Error("Css minify cache result is null");
                                return false;
                            }

                            if (spritedImageContentItems.Any(hi => hi == null))
                            {
                                context.Log.Error("Sprited image cache result is null");
                                return false;
                            }

                            if (hashedImageContentItems.Any(hi => hi == null))
                            {
                                context.Log.Error("Hashed image cache result is null");
                                return false;
                            }

                            return true;
                        })
                        .Execute(cacheSection =>
                        {
                            try
                            {
                                // Apply all configured visitors, including, validating, optimizing, minifying and spriting.

                                // Applying of resources
                                var stylesheetNode = ApplyResources(parsedStylesheetNode, pivot.MergedResource, threadContext) as StyleSheetNode;

                                // Validation
                                stylesheetNode = this.ApplyValidation(stylesheetNode, threadContext) as StyleSheetNode;

                                // Optimization
                                stylesheetNode = this.ApplyOptimization(stylesheetNode, threadContext) as StyleSheetNode;

                                // Spriting
                                stylesheetNode = this.ApplySpriting(stylesheetNode, pivot.Dpi, spritedImageContentItems, threadContext) as StyleSheetNode;

                                // Output css as string
                                var processedCssContent = threadContext.SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.PrintCss).Execute(() =>
                                        this.ShouldMinify ? stylesheetNode.MinifyPrint() : stylesheetNode.PrettyPrint());

                                // TODO: Hash the images on the styielsheetnode not on the css result.
                                // Hash images on the result css
                                if (imageHasher != null)
                                {
                                    var hashResult = HashImages(processedCssContent, imageHasher, cacheSection, threadContext, this.availableSourceImages, this.MissingImageUrl);
                                    processedCssContent = hashResult.Item1;
                                    hashedImageContentItems.AddRange(hashResult.Item2);
                                }

                                minifiedContentItem = ContentItem.FromContent(processedCssContent, this.DestinationFile, null, pivot.NewContentResourcePivotKeys);
                                cacheSection.AddResult(minifiedContentItem, CacheFileCategories.MinifiedCssResult);
                            }
                            catch (Exception ex)
                            {
                                context.Log.Error(ex, ex.ToString());
                                return false;
                            }

                            return true;
                        });

                    Safe.Lock(minifiedContentItems, () => minifiedContentItems.Add(minifiedContentItem));

                    if (!result)
                    {
                        context.Log.Error("An errror occurred while minifying '{0}' with resources '{1}'".InvariantFormat(contentItem.RelativeContentPath, pivot));
                    }

                    return result;
                });

            return new MinifyCssResult(
                minifiedContentItems,
                spritedImageContentItems.DistinctBy(hi => hi.RelativeContentPath).ToArray(),
                hashedImageContentItems.DistinctBy(hi => hi.RelativeContentPath).ToArray());
        }

        /// <summary>The execute.</summary>
        /// <param name="contentItem">The content Item.</param>
        /// <param name="imageHasher">The image hasher</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Needs refactoring into multiple classes")]
        internal void Execute(ContentItem contentItem = null, FileHasherActivity imageHasher = null)
        {
            if (contentItem == null)
            {
                if (string.IsNullOrWhiteSpace(this.SourceFile))
                {
                    throw new ArgumentException("MinifyCssActivity - The source file cannot be null or whitespace.");
                }

                if (!File.Exists(this.SourceFile))
                {
                    throw new FileNotFoundException("MinifyCssActivity - The source file cannot be found.", this.SourceFile);
                }
            }

            if (string.IsNullOrWhiteSpace(this.DestinationFile))
            {
                throw new ArgumentException("MinifyCssActivity - The destination file cannot be null or whitespace.");
            }

            if (contentItem == null)
            {
                contentItem = ContentItem.FromFile(this.SourceFile, Path.IsPathRooted(this.SourceFile) ? this.SourceFile.MakeRelativeToDirectory(this.context.Configuration.SourceDirectory) : this.SourceFile);
            }

            var minifyresult = this.Process(contentItem, imageHasher);

            var css = minifyresult.Css.FirstOrDefault();
            if (css != null)
            {
                css.WriteTo(this.DestinationFile);
            }

            if (minifyresult.SpritedImages != null && minifyresult.SpritedImages.Any())
            {
                foreach (var spritedImage in minifyresult.SpritedImages)
                {
                    spritedImage.WriteToContentPath(this.context.Configuration.DestinationDirectory);
                }
            }

            if (minifyresult.HashedImages != null && minifyresult.HashedImages.Any())
            {
                foreach (var hashedImage in minifyresult.HashedImages)
                {
                    hashedImage.WriteToRelativeHashedPath(this.context.Configuration.DestinationDirectory);
                }
            }
        }

        /// <summary>Hash the images.</summary>
        /// <param name="cssContent">The css content.</param>
        /// <param name="imageHasher">The image Hasher.</param>
        /// <param name="cacheSection">The cache section.</param>
        /// <param name="threadContext">The context.</param>
        /// <param name="sourceImages">The source Images.</param>
        /// <param name="missingImage">The missing Image.</param>
        /// <returns>The css with hashed images.</returns>
        private static Tuple<string, IEnumerable<ContentItem>> HashImages(string cssContent, FileHasherActivity imageHasher, ICacheSection cacheSection, IWebGreaseContext threadContext, IDictionary<string, string> sourceImages, string missingImage)
        {
            return threadContext.SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.ImageHash).Execute(() =>
            {
                var contentImagesToHash = new HashSet<string>();
                var hashedContentItems = new List<ContentItem>();
                var hashedImages = new Dictionary<string, string>();
                cssContent = UrlHashRegexPattern.Replace(
                    cssContent,
                    match =>
                    {
                        var urlToHash = match.Groups["url"].Value;
                        var extraInfo = match.Groups["extra"].Value;

                        if (ResourcesResolver.LocalizationResourceKeyRegex.IsMatch(urlToHash))
                        {
                            return match.Value;
                        }

                        var normalizedHashUrl = urlToHash.NormalizeUrl();

                        var imageContentFile = sourceImages.TryGetValue(normalizedHashUrl);
                        if (imageContentFile == null && !string.IsNullOrWhiteSpace(missingImage))
                        {
                            imageContentFile = sourceImages.TryGetValue(missingImage);
                        }

                        if (imageContentFile == null)
                        {
                            throw new BuildWorkflowException("Could not find a matching source image for url: {0}".InvariantFormat(urlToHash));
                        }

                        if (contentImagesToHash.Add(normalizedHashUrl))
                        {
                            // Add the image as end result
                            var imageContentItem = ContentItem.FromFile(imageContentFile, normalizedHashUrl);
                            imageContentItem = imageHasher.Hash(imageContentItem);
                            cacheSection.AddSourceDependency(imageContentFile);
                            hashedContentItems.Add(imageContentItem);

                            imageContentFile =
                                Path.Combine(
                                    imageHasher.BasePrefixToAddToOutputPath ?? Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture),
                                    imageContentItem.RelativeHashedContentPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

                            hashedImages.Add(normalizedHashUrl, imageContentFile);
                        }
                        else
                        {
                            imageContentFile = hashedImages[normalizedHashUrl];
                        }

                        return "url(" + imageContentFile + extraInfo + ")";
                    });

                return Tuple.Create(cssContent, (IEnumerable<ContentItem>)hashedContentItems);
            });
        }

        /// <summary>The get minify css pivots.</summary>
        /// <param name="contentItem">The content item.</param>
        /// <param name="dpiValues">The dpi values.</param>
        /// <param name="mergedResources">The merged resources.</param>
        /// <param name="allDpiResources">The all Dpi Resources.</param>
        /// <returns>The pivots of the css to minify</returns>
        private static IEnumerable<MinifyCssPivot> GetMinifyCssPivots(ContentItem contentItem, IEnumerable<float> dpiValues, Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> mergedResources, IDictionary<string, IDictionary<string, string>> allDpiResources)
        {
            var contentResourcePivotKeys = contentItem.ResourcePivotKeys ?? new ResourcePivotKey[] { };

            var dpiPivots = dpiValues.Select(
                dpi =>
                {
                    var dpiResolutionName = EverythingActivity.DpiToResolutionName(dpi);
                    IDictionary<string, string> dpiResources = null;
                    if (allDpiResources != null)
                    {
                        allDpiResources.TryGetValue(dpiResolutionName, out dpiResources);
                    }

                    var dpiResourcePivotKey = new ResourcePivotKey(Strings.DpiResourcePivotKey, dpiResolutionName);

                    return new { dpi, dpiResolutionName, dpiResourcePivotKey, dpiResources };
                });

            // Make sure we do dpi each before pivot to make it optimal when parallel 
            var pivots = mergedResources.SelectMany(
                mergedResourceValues =>
                {
                    var mergedResource = mergedResourceValues.Value.Values.ToList();
                    return dpiPivots.Select(
                        dpiPivot =>
                        {
                            var dpiSpecificMergedResources = mergedResource.ToList();
                            if (dpiPivot.dpiResources != null)
                            {
                                dpiSpecificMergedResources.Add(dpiPivot.dpiResources);
                            }

                            return new MinifyCssPivot(
                                dpiSpecificMergedResources,
                                contentResourcePivotKeys.Concat(mergedResourceValues.Key).Concat(new[] { dpiPivot.dpiResourcePivotKey }).ToArray(),
                                dpiPivot.dpi);
                        });
                });

            return pivots;
        }

        /// <summary>Applies the css resource visitors.</summary>
        /// <param name="stylesheetNode">The stylesheet node.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <returns>The processed node.</returns>
        private static AstNode ApplyResources(AstNode stylesheetNode, IEnumerable<IDictionary<string, string>> resources, IWebGreaseContext threadContext)
        {
            if (resources.Any())
            {
                threadContext.SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.ResourcesResolution)
                             .Execute(() => { stylesheetNode = stylesheetNode.Accept(new ResourceResolutionVisitor(resources)); });
            }

            return stylesheetNode;
        }

        /// <summary>The restore sprited images from cache.</summary>
        /// <param name="mapXmlFile">The map xml file.</param>
        /// <param name="cacheSection">The cache section.</param>
        /// <param name="results">The results.</param>
        /// <param name="destinationDirectory">The destination Directory.</param>
        /// <param name="imageAssembleScanDestinationFile">The image Assemble Scan Destination File.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private static ImageLog RestoreSpritedImagesFromCache(string mapXmlFile, ICacheSection cacheSection, BlockingCollection<ContentItem> results, string destinationDirectory, string imageAssembleScanDestinationFile)
        {
            // restore log file, is required by next step in applying sprites to the css.
            var spriteLogFileContentItem = cacheSection.GetCachedContentItems(CacheFileCategories.SpriteLogFile).FirstOrDefault();
            if (spriteLogFileContentItem == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(imageAssembleScanDestinationFile))
            {
                var spriteLogFileXmlContentItem = cacheSection.GetCachedContentItems(CacheFileCategories.SpriteLogFileXml).FirstOrDefault();
                if (spriteLogFileXmlContentItem != null)
                {
                    spriteLogFileXmlContentItem.WriteTo(mapXmlFile);
                }
            }

            var imageLog = spriteLogFileContentItem.Content.FromJson<ImageLog>(true);

            // Restore images
            var spritedImageContentItems = cacheSection.GetCachedContentItems(CacheFileCategories.HashedSpriteImage);
            spritedImageContentItems.ForEach(sici => sici.WriteToContentPath(destinationDirectory));
            results.AddRange(spritedImageContentItems);

            return imageLog;
        }

        /// <summary>The get relative sprite cache key.</summary>
        /// <param name="imageReferencesToAssemble">The image references to assemble</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <returns>The unique cache key.</returns>
        private static string GetRelativeSpriteCacheKey(IEnumerable<InputImage> imageReferencesToAssemble, IWebGreaseContext threadContext)
        {
            return string.Join(
                ">",
                imageReferencesToAssemble.Select(ir =>
                                                 "{0}|{1}|{2}".InvariantFormat(
                                                     threadContext.MakeRelativeToApplicationRoot(ir.AbsoluteImagePath),
                                                     ir.Position,
                                                     string.Join(":", ir.DuplicateImagePaths.Select(threadContext.MakeRelativeToApplicationRoot)))));
        }

        /// <summary>Gets the unique object for all the settings, used for determining unique cache id.</summary>
        /// <param name="imageHasher">The image Hasher.</param>
        /// <param name="resourcePivotKeys">The resource Pivot Keys.</param>
        /// <param name="dpiResources">The dpi Resources.</param>
        /// <returns>The settings object.</returns>
        private object GetVarBySettings(FileHasherActivity imageHasher, IEnumerable<ResourcePivotKey> resourcePivotKeys, IEnumerable<IDictionary<string, string>> dpiResources)
        {
            return new
            {
                resourcePivotKeys,
                dpiResources,
                this.ShouldExcludeProperties,
                this.ShouldValidateForLowerCase,
                this.ShouldMergeMediaQueries,
                this.ShouldOptimize,
                this.ShouldAssembleBackgroundImages,
                this.ShouldMinify,
                this.ShouldPreventOrderBasedConflict,
                this.ShouldMergeBasedOnCommonDeclarations,
                this.IgnoreImagesWithNonDefaultBackgroundSize,
                this.HackSelectors,
                this.BannedSelectors,
                this.NonMergeSelectors,
                this.ImageAssembleReferencesToIgnore,
                this.OutputUnit,
                this.OutputUnitFactor,
                this.ImageAssemblyPadding,
                HashImages = imageHasher == null,
                this.ForcedSpritingImageType,
                this.ErrorOnInvalidSprite,
                this.ImageAssembleScanDestinationFile,
                this.ImageSpritingLogPath,
            };
        }

        /// <summary>Applies the spriting visitors.</summary>
        /// <param name="stylesheetNode">The stylesheet node.</param>
        /// <param name="dpi">The dpi.</param>
        /// <param name="spritedImageContentItems">The sprited image content items.</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <returns>The processed node.</returns>
        private AstNode ApplySpriting(AstNode stylesheetNode, float dpi, BlockingCollection<ContentItem> spritedImageContentItems, IWebGreaseContext threadContext)
        {
            if (this.ShouldAssembleBackgroundImages)
            {
                stylesheetNode = this.SpriteBackgroundImages(stylesheetNode, dpi, threadContext, spritedImageContentItems);
            }

            return stylesheetNode;
        }

        /// <summary>Applies the css optimization visitors.</summary>
        /// <param name="stylesheetNode">The stylesheet node.</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <returns>The processed node.</returns>
        private AstNode ApplyOptimization(AstNode stylesheetNode, IWebGreaseContext threadContext)
        {
            // Step # 5 - Run the Css optimization visitors
            if (this.ShouldOptimize)
            {
                threadContext.SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.Optimize).Execute(
                    () =>
                    {
                        stylesheetNode = stylesheetNode.Accept(new OptimizationVisitor { ShouldMergeMediaQueries = this.ShouldMergeMediaQueries, ShouldPreventOrderBasedConflict = this.ShouldPreventOrderBasedConflict, ShouldMergeBasedOnCommonDeclarations = this.ShouldMergeBasedOnCommonDeclarations, NonMergeRuleSetSelectors = this.NonMergeSelectors });
                        stylesheetNode = stylesheetNode.Accept(new ColorOptimizationVisitor());
                        stylesheetNode = stylesheetNode.Accept(new FloatOptimizationVisitor());
                    });
            }

            return stylesheetNode;
        }

        /// <summary>Applies the css validation visitors.</summary>
        /// <param name="stylesheetNode">The stylesheet node.</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <returns>The processed node.</returns>
        private AstNode ApplyValidation(AstNode stylesheetNode, IWebGreaseContext threadContext)
        {
            threadContext.SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.Validate).Execute(() =>
            {
                // Step # 1 - Remove the Css properties from Ast which need to be excluded (Bridging)
                if (this.ShouldExcludeProperties)
                {
                    stylesheetNode = stylesheetNode.Accept(new ExcludePropertyVisitor());
                }

                // Step # 2 - Validate for lower case
                if (this.ShouldValidateForLowerCase)
                {
                    stylesheetNode = stylesheetNode.Accept(new ValidateLowercaseVisitor());
                }

                // Step # 3 - Validate for Css hacks which don't work cross browser
                if (this.HackSelectors != null && this.HackSelectors.Any())
                {
                    stylesheetNode = stylesheetNode.Accept(new SelectorValidationOptimizationVisitor(this.HackSelectors, false, true));
                }

                // Step # 4 - Remove any banned selectors which are exposed for page efficiency
                if (this.BannedSelectors != null && this.BannedSelectors.Any())
                {
                    stylesheetNode = stylesheetNode.Accept(new SelectorValidationOptimizationVisitor(this.BannedSelectors, false, false));
                }
            });

            return stylesheetNode;
        }

        /// <summary>Assembles the background images</summary>
        /// <param name="stylesheetNode">the style sheet node</param>
        /// <param name="dpi">The dpi.</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <param name="spritedImageContentItems">The sprited Image Content Items.</param>
        /// <returns>The stylesheet node with the sprited images aplied.</returns>
        private AstNode SpriteBackgroundImages(AstNode stylesheetNode, float dpi, IWebGreaseContext threadContext, BlockingCollection<ContentItem> spritedImageContentItems)
        {
            // The image assembly is a 3 step process:
            // 1. Scan the Css followed by Pretty Print
            // 2. Run the image assembly tool
            // 3. Update the Css with generated images followed by Pretty Print
            return threadContext.SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.Spriting).Execute(() =>
            {
                // Execute the pipeline for image assembly scan
                var scanLog = this.ExecuteImageAssemblyScan(stylesheetNode, threadContext);

                // Execute the pipeline for image assembly tool
                var imageMaps = new List<ImageLog>();
                var count = 0;
                foreach (var scanOutput in scanLog.ImageAssemblyScanOutputs)
                {
                    var spriteResult = this.SpriteImageFromLog(scanOutput, this.ImageAssembleScanDestinationFile + (count == 0 ? string.Empty : "." + count.ToString(CultureInfo.InvariantCulture)) + ".xml", scanLog.ImageAssemblyAnalysisLog, threadContext, spritedImageContentItems);
                    if (spriteResult != null)
                    {
                        imageMaps.Add(spriteResult);
                        count++;
                    }
                }

                // Step # 8 - Execute the pipeline for image assembly update
                stylesheetNode = this.ExecuteImageAssemblyUpdate(stylesheetNode, imageMaps, dpi);

                if (!string.IsNullOrWhiteSpace(this.ImageSpritingLogPath))
                {
                    scanLog.ImageAssemblyAnalysisLog.Save(this.ImageSpritingLogPath);
                }

                var imageAssemblyAnalysisLog = scanLog.ImageAssemblyAnalysisLog;
                if (this.ErrorOnInvalidSprite && imageAssemblyAnalysisLog.FailedSprites.Any())
                {
                    foreach (var failedSprite in imageAssemblyAnalysisLog.FailedSprites)
                    {
                        var failureMessage = ImageAssemblyAnalysisLog.GetFailureMessage(failedSprite);
                        if (!string.IsNullOrWhiteSpace(failedSprite.Image))
                        {
                            threadContext.Log.Error(
                                "Failed to sprite image {0}\r\nReason:{1}\r\nCss:{2}".InvariantFormat(
                                    failedSprite.Image, failureMessage, failedSprite.AstNode.PrettyPrint()));
                        }
                        else
                        {
                            threadContext.Log.Error(
                                "Failed to sprite:{0}\r\nReason:{1}".InvariantFormat(
                                    failedSprite.Image, failureMessage));
                        }
                    }
                }

                return stylesheetNode;
            });
        }

        /// <summary>The sprite image.</summary>
        /// <param name="scanOutput">The scan Output.</param>
        /// <param name="mapXmlFile">The map xml file</param>
        /// <param name="imageAssemblyAnalysisLog">The image Assembly Analysis Log.</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <param name="spritedImageContentItems">The sprited Image Content Items.</param>
        /// <returns>The <see cref="ImageLog"/>.</returns>
        private ImageLog SpriteImageFromLog(ImageAssemblyScanOutput scanOutput, string mapXmlFile, ImageAssemblyAnalysisLog imageAssemblyAnalysisLog, IWebGreaseContext threadContext, BlockingCollection<ContentItem> spritedImageContentItems)
        {
            if (scanOutput == null || !scanOutput.ImageReferencesToAssemble.Any())
            {
                return null;
            }

            ImageLog imageLog = null;
            var imageReferencesToAssemble = scanOutput.ImageReferencesToAssemble;
            if (imageReferencesToAssemble == null || imageReferencesToAssemble.Count == 0)
            {
                return null;
            }

            var varBySettings = new { imageMap = GetRelativeSpriteCacheKey(imageReferencesToAssemble, threadContext), this.ImageAssemblyPadding };
            var success = threadContext.SectionedAction(SectionIdParts.MinifyCssActivity, SectionIdParts.Spriting, SectionIdParts.Assembly)
                .MakeCachable(varBySettings, false, true)
                .RestoreFromCacheAction(cacheSection =>
                    {
                        imageLog = RestoreSpritedImagesFromCache(mapXmlFile, cacheSection, spritedImageContentItems, threadContext.Configuration.DestinationDirectory, this.ImageAssembleScanDestinationFile);
                        return imageLog != null;
                    })
                .Execute(cacheSection =>
                    {
                        imageLog = this.CreateSpritedImages(mapXmlFile, imageAssemblyAnalysisLog, imageReferencesToAssemble, cacheSection, spritedImageContentItems, threadContext);
                        return imageLog != null;
                    });

            return
                success
                ? imageLog
                : null;
        }

        /// <summary>The create sprited images.</summary>
        /// <param name="mapXmlFile">The map xml file.</param>
        /// <param name="imageAssemblyAnalysisLog">The image assembly analysis log.</param>
        /// <param name="imageReferencesToAssemble">The image references to assemble.</param>
        /// <param name="cacheSection">The cache section.</param>
        /// <param name="results">The results.</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private ImageLog CreateSpritedImages(
            string mapXmlFile,
            ImageAssemblyAnalysisLog imageAssemblyAnalysisLog,
            IEnumerable<InputImage> imageReferencesToAssemble,
            ICacheSection cacheSection,
            BlockingCollection<ContentItem> results,
            IWebGreaseContext threadContext)
        {
            if (!Directory.Exists(this.ImagesOutputDirectory))
            {
                Directory.CreateDirectory(this.ImagesOutputDirectory);
            }

            var imageMap = ImageAssembleGenerator.AssembleImages(
                imageReferencesToAssemble.ToSafeReadOnlyCollection(),
                SpritePackingType.Vertical,
                this.ImagesOutputDirectory,
                string.Empty,
                true,
                threadContext,
                this.ImageAssemblyPadding,
                imageAssemblyAnalysisLog,
                this.ForcedSpritingImageType);

            if (imageMap == null || imageMap.Document == null)
            {
                return null;
            }

            var destinationDirectory = threadContext.Configuration.DestinationDirectory;
            if (!string.IsNullOrWhiteSpace(this.ImageAssembleScanDestinationFile))
            {
                var scanXml = imageMap.Document.ToString();
                FileHelper.WriteFile(mapXmlFile, scanXml);
                cacheSection.AddResult(ContentItem.FromFile(mapXmlFile), CacheFileCategories.SpriteLogFileXml);
            }

            var imageLog = new ImageLog(imageMap.Document);
            cacheSection.AddResult(ContentItem.FromContent(imageLog.ToJson(true)), CacheFileCategories.SpriteLogFile);

            foreach (var spritedFile in imageLog.InputImages.Select(il => il.OutputFilePath).Distinct())
            {
                var spritedImageContentItem = ContentItem.FromFile(spritedFile, spritedFile.MakeRelativeToDirectory(destinationDirectory));
                results.Add(spritedImageContentItem);
                cacheSection.AddResult(spritedImageContentItem, CacheFileCategories.HashedSpriteImage);
            }

            return imageLog;
        }

        /// <summary>Scans the css for the image path references</summary>
        /// <param name="stylesheetNode">The stylesheet node</param>
        /// <param name="threadContext">The thread Context.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        private ImageAssemblyScanVisitor ExecuteImageAssemblyScan(AstNode stylesheetNode, IWebGreaseContext threadContext)
        {
            var imageAssemblyScanVisitor = new ImageAssemblyScanVisitor(
                this.SourceFile,
                this.ImageAssembleReferencesToIgnore,
                this.IgnoreImagesWithNonDefaultBackgroundSize,
                this.OutputUnit,
                this.OutputUnitFactor,
                this.availableSourceImages,
                this.MissingImageUrl,
                true)
            {
                Context = threadContext
            };

            // Scan log visitor should and does not change the stylesheet, no need to use the result.
            stylesheetNode.Accept(imageAssemblyScanVisitor);

            // return the can log results.
            return imageAssemblyScanVisitor;
        }

        /// <summary>Executes the image assembly update visitor</summary>
        /// <param name="stylesheetNode">The stylesheet Ast node</param>
        /// <param name="imageLogs">The sprite Log Files.</param>
        /// <param name="dpi">The dpi.</param>
        /// <returns>The modified AST node if modified otherwise the original node</returns>
        private AstNode ExecuteImageAssemblyUpdate(AstNode stylesheetNode, IEnumerable<ImageLog> imageLogs, float dpi)
        {
            var imageAssemblyUpdateVisitor = new ImageAssemblyUpdateVisitor(
                this.SourceFile,
                imageLogs,
                dpi,
                this.OutputUnit,
                this.OutputUnitFactor,
                this.ImageBasePrefixToRemoveFromOutputPathInLog,
                this.ImageBasePrefixToAddToOutputPath,
                this.availableSourceImages,
                this.MissingImageUrl);

            stylesheetNode = stylesheetNode.Accept(imageAssemblyUpdateVisitor);

            // Return the updated Ast
            return stylesheetNode;
        }
    }
}
