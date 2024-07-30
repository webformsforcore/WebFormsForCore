// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcesResolver.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Parses the resources from files and loads into the dictionary.
//   Implements the Factory method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Text.RegularExpressions;
    using WebGrease;

    /// <summary>Parses the resources from files and loads into the dictionary. 
    /// Implements the Factory method.</summary>
    internal sealed class ResourcesResolver
    {
        /// <summary>
        /// Gets the localization resource key format
        /// </summary>
        /// <value>Regular expression pattern</value>
        internal static readonly Regex LocalizationResourceKeyRegex = new Regex(@"%([-./\w_]+)(\:\w*)?%", RegexOptions.Compiled);

        /// <summary>
        /// Directories to search the resource files.
        /// </summary>
        private readonly List<ResourceDirectoryPath> resourceDirectoryPaths = new List<ResourceDirectoryPath>();

        /// <summary>
        /// Output folder path.
        /// </summary>
        private string outputDirectoryPath;

        /// <summary>
        /// The resource keys for which the resources will be compressed.
        /// </summary>
        private IEnumerable<string> resourceKeys;

        /// <summary>Initializes a new instance of the <see cref="ResourcesResolver"/> class.</summary>
        /// <param name="context">The webgrease context</param>
        /// <param name="inputContentDirectory">The base Resources Directory.</param>
        /// <param name="resourceGroupKey">The compress Filter Directory Name.</param>
        /// <param name="applicationDirectoryName">The modules Aggregation Directory Name.</param>
        /// <param name="siteName">The site name</param>
        /// <param name="resourceKeys">The resource keys for which the resources will be compressed.</param>
        /// <param name="outputDirectoryPath">Output folder to write the resource files on hard drive</param>
        private ResourcesResolver(IWebGreaseContext context, string inputContentDirectory, string resourceGroupKey, string applicationDirectoryName, string siteName, IEnumerable<string> resourceKeys, string outputDirectoryPath)
        {
            // Directory Structure:
            // Content
            //      App
            //          Site1
            //              Resources
            //                  css
            //                      locales
            //                      themes
            //                      ?????
            //                  js
            //                      locales
            //                      ?????
            //          Site2
            //              Resources
            //                  css
            //                      locales
            //                      themes
            //                      ?????
            //                  js
            //                      locales
            //                      ?????
            // ..
            // ..
            //      F1
            //          Resources
            //              css
            //                  locales
            //                  themes
            //                  ?????
            //              js
            //                  locales
            //                  ?????
            //      F2
            //      F3
            //      ..
            //      ..
            var contentDirectoryInfo = new DirectoryInfo(inputContentDirectory);

            Safe.FileLock(contentDirectoryInfo, () => 
            {
                // Iterate the top level directories inside "Content"
                foreach (var contentChildDirectory in contentDirectoryInfo.EnumerateDirectories())
                {
                    // "Application" directory
                    if (string.Compare(contentChildDirectory.Name, applicationDirectoryName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // "Site" directory
                        var siteDirectory = Path.Combine(contentChildDirectory.FullName, siteName ?? string.Empty);

                        if (Directory.Exists(siteDirectory))
                        {
                            // This is "Site" directory which can override the resources
                            foreach (var filterDirectory in new DirectoryInfo(siteDirectory).EnumerateDirectories(resourceGroupKey, SearchOption.AllDirectories))
                            {
                                this.resourceDirectoryPaths.Add(new ResourceDirectoryPath { AllowOverrides = true, Directory = filterDirectory.FullName });
                                context.Cache.CurrentCacheSection.AddSourceDependency(filterDirectory.FullName, "*.resx");
                            }
                        }
                    }
                    else
                    {
                        // "Feature" directories
                        foreach (var filterDirectory in contentChildDirectory.EnumerateDirectories(resourceGroupKey, SearchOption.AllDirectories))
                        {
                            this.resourceDirectoryPaths.Add(new ResourceDirectoryPath { AllowOverrides = false, Directory = filterDirectory.FullName });
                            context.Cache.CurrentCacheSection.AddSourceDependency(filterDirectory.FullName, "*.resx");
                        }
                    }
                }

                this.outputDirectoryPath = outputDirectoryPath;
                this.resourceKeys = resourceKeys ?? new List<string> { Strings.DefaultLocale };
            });
        }

        /// <summary>Resource Manager factory.</summary>
        /// <param name="context">The webgrease context</param>
        /// <param name="inputContentDirectory">The base Resources Directory.</param>
        /// <param name="resourceGroupKey">The resource type.</param>
        /// <param name="applicationDirectoryName">The modules Aggregation Directory Name.</param>
        /// <param name="siteName">The site name</param>
        /// <param name="resourceKeys">The resource keys for which the resources will be compressed.</param>
        /// <param name="outputDirectoryPath">Output folder to write the resource files on hard drive.</param>
        /// <returns>A new instance of Resource Manager class.</returns>
        internal static ResourcesResolver Factory(
            IWebGreaseContext context,
            string inputContentDirectory,
            string resourceGroupKey,
            string applicationDirectoryName,
            string siteName,
            IEnumerable<string> resourceKeys,
            string outputDirectoryPath)
        {
            return new ResourcesResolver(context, inputContentDirectory, resourceGroupKey, applicationDirectoryName, siteName, resourceKeys, outputDirectoryPath);
        }

        /// <summary>Gets the merged resources.</summary>
        /// <returns>The <see cref="IDictionary"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Explicit choice.")]
        internal IDictionary<string, IDictionary<string, string>> GetMergedResources()
        {
            var results = new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            // False value of this variable indicates generic-generic is not present in the key collection.
            // After the ResourceKeys loop if genericProcessed value is still false it means generic-generic file is 
            // not written in the output folder. We want to output generic-generic resource file in all the cases
            // to process locales in the non locales folders in case of EPPR projects.
            foreach (var resourceKey in this.resourceKeys)
            {
                var localeOrThemeName = resourceKey.Trim().ToLower(CultureInfo.InvariantCulture);
                results.Add(localeOrThemeName, this.GetResources(resourceKey, localeOrThemeName));
            }

            return results;
        }

        /// <summary>Gets the resources based on locale or theme key. 
        /// The resource files are searched in input paths
        /// and resolved for precedence and a dictionary is returned.</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "Explicit choice.")]
        internal void ResolveHierarchy()
        {
            // False value of this variable indicates generic-generic is not present in the key collection.
            // After the ResourceKeys loop if genericProcessed value is still false it means generic-generic file is 
            // not written in the output folder. We want to output generic-generic resource file in all the cases
            // to process locales in the non locales folders in case of EPPR projects.
            foreach (var resourceKey in this.resourceKeys)
            {
                var localeOrThemeName = resourceKey.Trim().ToLower(CultureInfo.InvariantCulture);

                var resources = this.GetResources(resourceKey, localeOrThemeName);

                // Write the resx files to hard drive
                WriteResources(this.outputDirectoryPath, localeOrThemeName, resources);
            }
        }

        /// <summary>Gets the merged resources for the given locale or theme.</summary>
        /// <param name="resourceKey">The resource key.</param>
        /// <param name="localeOrThemeName">The locale or theme name.</param>
        /// <returns>The merged resources.</returns>
        private SortedDictionary<string, string> GetResources(string resourceKey, string localeOrThemeName)
        {
            // Keep the resolved resources sorted for better readability
            var resources = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Get the resources for all sets of folder paths
            foreach (var resourceDirectoryPath in this.resourceDirectoryPaths.OrderBy(resourceDirectoryPath => resourceDirectoryPath.AllowOverrides))
            {
                var resourceDirectoryInfo = new DirectoryInfo(resourceDirectoryPath.Directory);
                var genericResource = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // First of all consider the default locale file with in a directory
                if (resourceKey != Strings.DefaultLocale)
                {
                    var defaultLocaleFilePath = Path.Combine(resourceDirectoryInfo.FullName, Strings.DefaultResx);
                    if (File.Exists(defaultLocaleFilePath))
                    {
                        genericResource = ReadResources(defaultLocaleFilePath);
                    }
                }

                // Now check if the actual locale file is present in same directory.
                var resxResource = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                var localeFilePath = Path.Combine(resourceDirectoryInfo.FullName, localeOrThemeName + Strings.ResxExtension);

                if (File.Exists(localeFilePath))
                {
                    resxResource = ReadResources(localeFilePath);
                }

                // Merge the generic and the locale resources with in the same directory
                MergeResources(resxResource, genericResource, false, false);

                // Merge the directory resources into overall locale resources discovered so far
                // throws exception if cross feature duplicate key is defined)
                MergeResources(resources, resxResource, resourceDirectoryPath.AllowOverrides, resourceDirectoryPath.AllowOverrides);
            }

            return resources;
        }

        /// <summary>Parse the resources.</summary>
        /// <param name="filePath">Path to the resource file.</param>
        /// <returns>Read the resources from a file</returns>
        internal static Dictionary<string, string> ReadResources(string filePath)
        {
            var fileResources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var resXResourceReader = new ResXResourceReader(filePath))
            {
                foreach (DictionaryEntry resource in resXResourceReader)
                {
                    var key = resource.Key as string;
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    var value = resource.Value as string ?? string.Empty;

                    // If resx contains the key already then it is an error
                    // this will happen when resx file contains duplicate keys.
                    // Resx editor wont catch these since these are 2 different string keys
                    if (fileResources.ContainsKey(key))
                    {
                        throw new BuildWorkflowException(string.Format(CultureInfo.CurrentCulture, ResourceStrings.ResourceResolverDuplicateKeyExceptionMessage, key, filePath));
                    }

                    // Add the key to the fileResources
                    fileResources.Add(key, value);
                }
            }

            return fileResources;
        }

        /// <summary>Expand the input file content with resource values</summary>
        /// <param name="input">Input string</param>
        /// <param name="resources">Resources dictionary</param>
        /// <returns>True if any resource key is expanded</returns>
        internal static string ExpandResourceKeys(string input, IDictionary<string, string> resources)
        {
            if (input == null || resources == null || resources.Count == 0)
            {
                return input;
            }

            // This replacement technique is profiled orders of magnitude better than string.replace 
            // and stringbuilder.replace since there is a single pass of look up on regex DFA. Please 
            // don't modify the algorithm here without profiling on full load.
            return LocalizationResourceKeyRegex.Replace(
                input,
                match =>
                {
                    string resourceValue;

                    // Query the resources dictionary for a value corresponding to matched key
                    // but NOT including any optional fallback class.
                    var matchedKey = match.Result("$1");

                    // If there is a value found in resources dictionary, return it for expansion
                    return resources.TryGetValue(matchedKey, out resourceValue) ? resourceValue : match.Value;
                });
        }

        /// <summary>Merge the resources at folder into the master resources</summary>
        /// <param name="output">The output dictionary</param>
        /// <param name="input">The input dictionary to merge</param>
        /// <param name="allowOverrides">If set to true, then error will be thrown in case of resource override.</param>
        /// <param name="throwsException">Throws the exception if true</param>
        private static void MergeResources(IDictionary<string, string> output, Dictionary<string, string> input, bool allowOverrides, bool throwsException)
        {
            foreach (var inputKey in input.Keys)
            {
                if (output.ContainsKey(inputKey))
                {
                    if (allowOverrides)
                    {
                        output[inputKey] = input[inputKey];
                    }
                    else
                    {
                        if (throwsException)
                        {
                            throw new ResourceOverrideException(null, inputKey);
                        }
                    }
                }
                else
                {
                    output.Add(inputKey, input[inputKey]);
                }
            }
        }

        /// <summary>Writes the resx files to the hard drive</summary>
        /// <param name="outputDirectoryPath">Output folder path where the resx files will be created</param>
        /// <param name="key">Name of locales/theme</param>
        /// <param name="resources">Dictionary of compressed key/value pairs</param>
        private static void WriteResources(string outputDirectoryPath, string key, IDictionary<string, string> resources)
        {
            if (resources == null || resources.Count == 0)
            {
                return;
            }

            Directory.CreateDirectory(outputDirectoryPath);

            // Write the resource to hard drive
            using (var writer = new ResXResourceWriter(Path.Combine(outputDirectoryPath, key + Strings.ResxExtension)))
            {
                foreach (var resourceKey in resources.Keys)
                {
                    writer.AddResource(resourceKey, resources[resourceKey]);
                }
            }
        }

        /// <summary>The get used resource keys.</summary>
        /// <param name="css">The css.</param>
        /// <param name="resources">The resources.</param>
        /// <returns>The <see cref="IEnumerable"/>.</returns>
        public static IEnumerable<Tuple<List<string>, Dictionary<string, string>>> GetGroupedUsedResourceKeys(string css, IDictionary<string, IDictionary<string, string>> resources)
        {
            var availableResourceKeys = new HashSet<string>(resources.Values.SelectMany(v => v.Keys).Distinct());
            var usedResourceKeys = 
                    new HashSet<string>(LocalizationResourceKeyRegex
                        .Matches(css)
                        .OfType<Match>()
                        .Select(m => m.Groups[1].Value)
                        .Where(availableResourceKeys.Contains)
                        .OrderBy(rk => rk)
                        .ToArray());

            var result = new Dictionary<string, Tuple<List<string>, Dictionary<string, string>>>(StringComparer.OrdinalIgnoreCase);
            foreach (var resource in resources)
            {
                var usedKeyValues = new Dictionary<string, string>(resource.Value.Where(kvp => usedResourceKeys.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value), StringComparer.OrdinalIgnoreCase);
                var uniqueKey = string.Join("%", usedKeyValues.Select(kv => kv.ToString()));
                Tuple<List<string>, Dictionary<string, string>> matchingResourceKeys;
                if (!result.TryGetValue(uniqueKey, out matchingResourceKeys))
                {
                    matchingResourceKeys = new Tuple<List<string>, Dictionary<string, string>>(new List<string>(), usedKeyValues);
                    result.Add(uniqueKey, matchingResourceKeys);
                }

                matchingResourceKeys.Item1.Add(resource.Key);
            }

            return result.Values;
        }
    }
}
