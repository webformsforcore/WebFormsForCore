// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcesManager.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   ResourcesManager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Resources;

    /// <summary>ResourcesManager class.</summary>
    internal static class ResourcesManager
    {
        /// <summary>Gets the tokens based on locale or theme key. 
        /// The token files are searched in input paths
        /// and resolved for precedence and a dictionary is returned.</summary>
        /// <param name="resourcesDirectoryPath">Resources folder path.</param>
        /// <param name="localeOrThemeName">Theme or locale key.</param>
        /// <param name="resources">The resources dictionary</param>
        internal static void TryGetResources(string resourcesDirectoryPath, string localeOrThemeName, out Dictionary<string, string> resources)
        {
            resources = new Dictionary<string, string>();
            string resxFilePath;

            if (HasResources(resourcesDirectoryPath, localeOrThemeName, out resxFilePath))
            {
                using (var resXResourceReader = new ResXResourceReader(resxFilePath))
                {
                    foreach (DictionaryEntry resource in resXResourceReader)
                    {
                        var key = resource.Key as string;
                        var value = resource.Value as string;
                        if (key != null)
                        {
                            resources.Add(key, value);
                        }
                    }
                }
            }
        }

        /// <summary>Checks if the resx file is present for a theme or a locale</summary>
        /// <param name="resourcesDirectoryPath">Resources folder path.</param>
        /// <param name="localeOrThemeName">Theme or locale key.</param>
        /// <param name="resourcePath">The path of the resx file</param>
        /// <returns>True if the resx file is found</returns>
        private static bool HasResources(string resourcesDirectoryPath, string localeOrThemeName, out string resourcePath)
        {
            resourcePath = Path.Combine(resourcesDirectoryPath, localeOrThemeName + Strings.ResxExtension);
            return File.Exists(resourcePath);
        }
    }
}