// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcePivotActivity.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using WebGrease.Configuration;
    using WebGrease.Extensions;

    /// <summary>The resource pivot activity.</summary>
    internal static class ResourcePivotActivity
    {
        /// <summary>Applies the resojurce keys to the content item.</summary>
        /// <param name="inputItem">The input item.</param>
        /// <param name="mergedResoures">The merged resoures.</param>
        /// <returns>The list of merged/applied content items..</returns>
        internal static IEnumerable<ContentItem> ApplyResourceKeys(
            ContentItem inputItem,
            Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResoures)
        {
            if (mergedResoures == null || !mergedResoures.Any())
            {
                return new[] { inputItem };
            }

            var contentItems = new List<ContentItem>();
            try
            {
                var originalContent = inputItem.Content;
                var usedAndGroupedResources = GetUsedGroupedResources(originalContent, mergedResoures);
                foreach (var usedAndGroupedResource in usedAndGroupedResources)
                {
                    var resourcedContent = originalContent;
                    foreach (var resources in usedAndGroupedResource.Value)
                    {
                        resourcedContent = ResourcesResolver.ExpandResourceKeys(resourcedContent, resources.Value);
                    }

                    contentItems.Add(ContentItem.FromContent(resourcedContent, inputItem, usedAndGroupedResource.Key));
                }
            }
            catch (ResourceOverrideException resourceOverrideException)
            {
                // There was a resource override in folder path that does not
                // allow resource overriding. For this case, we need to
                // show a build error.
                var errorMessage = string.Format(CultureInfo.CurrentCulture, ResourceStrings.ResourcePivotActivityDuplicateKeysError, resourceOverrideException.TokenKey);
                throw new WorkflowException(errorMessage, resourceOverrideException);
            }
            catch (Exception exception)
            {
                throw new WorkflowException(ResourceStrings.ResourcePivotActivityError, exception);
            }

            return contentItems;
        }

        internal static Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> GetUsedGroupedResources(string content, Dictionary<string, IDictionary<string, IDictionary<string, string>>> mergedResoures)
        {
            var groupedAndUsedResources = new Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> { { new ResourcePivotKey[] { }, new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase) } };
            if (mergedResoures == null || !mergedResoures.Any())
            {
                return groupedAndUsedResources;
            }

            foreach (var resource in mergedResoures)
            {
                groupedAndUsedResources = GetUsedGroupedResources(groupedAndUsedResources, content, resource.Key, resource.Value);
            }

            return groupedAndUsedResources;
        }

        private static Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> GetUsedGroupedResources(Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>> groupedAndUsedResources, string content, string resourcePivotGroupKey, IDictionary<string, IDictionary<string, string>> resourcePivotKeyValues)
        {
            if (resourcePivotKeyValues == null || !resourcePivotKeyValues.Any())
            {
                return groupedAndUsedResources;
            }

            var newGroupedAndUsedResources = new Dictionary<ResourcePivotKey[], IDictionary<string, IDictionary<string, string>>>();
            var groupedResources = ResourcesResolver.GetGroupedUsedResourceKeys(content, resourcePivotKeyValues);
            foreach (var groupedResource in groupedResources)
            {
                foreach (var groupedAndUsedResource in groupedAndUsedResources)
                {
                    var resourcePivotKeys = groupedResource.Item1.Select(key => new ResourcePivotKey(resourcePivotGroupKey, key));
                    var newResourcePivots = groupedAndUsedResource.Key.Concat(resourcePivotKeys).ToArray();

                    var resourceDictionaries = new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
                    resourceDictionaries.AddRange(groupedAndUsedResource.Value);
                    resourceDictionaries.Add(resourcePivotGroupKey, groupedResource.Item2);

                    newGroupedAndUsedResources.Add(newResourcePivots, resourceDictionaries);
                }
            }

            return newGroupedAndUsedResources;
        }
    }
}