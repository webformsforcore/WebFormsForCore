// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporaryOverrides.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using WebGrease.Configuration;
    using WebGrease.Extensions;

    /// <summary>The temporary overrides.</summary>
    public class TemporaryOverrides
    {
        /// <summary>The resource pivots.</summary>
        private readonly IDictionary<string, List<string>> resourcePivots = new Dictionary<string, List<string>>();

        /// <summary>The outputs.</summary>
        private readonly List<string> outputs = new List<string>();

        /// <summary>The output extensions.</summary>
        private readonly List<string> outputExtensions = new List<string>();

        /// <summary>The unique key.</summary>
        private string uniqueKey;

        /// <summary>Gets or sets a value indicating whether skip all.</summary>
        public bool SkipAll { get; set; }

        /// <summary>Gets the unique key.</summary>
        public string UniqueKey
        {
            get
            {
                return this.uniqueKey;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="TemporaryOverrides"/> class.</summary>
        /// <param name="overrideFile">The override file.</param>
        /// <returns>The <see cref="TemporaryOverrides"/>.</returns>
        public static TemporaryOverrides Load(string overrideFile)
        {
            var to = new TemporaryOverrides();
            to.LoadFromFile(overrideFile);
            to.uniqueKey = to.ToJson(true);

            // Only return when there are any values to override
            return to.resourcePivots.Any(rp => rp.Value.Any()) || to.outputs.Any() || to.SkipAll
                ? to
                : null;
        }

        /// <summary>Determines if the content item should be temporary ignored.</summary>
        /// <param name="contentItem">The content item.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool ShouldIgnore(ContentItem contentItem)
        {
            return
                contentItem != null
                && this.ShouldIgnore(contentItem.ResourcePivotKeys);
        }

        /// <summary>Determines if the resource pivot keys should be temporary ignored.</summary>
        /// <param name="resourcePivotKeys">The resource pivot keys.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool ShouldIgnore(IEnumerable<ResourcePivotKey> resourcePivotKeys)
        {
            return resourcePivotKeys != null
                && resourcePivotKeys.Any()
                && resourcePivotKeys
                                .GroupBy(rpk => rpk.GroupKey)
                                .Any(rpk => rpk.All(this.ShouldIgnore));      
        }

        /// <summary>Determines if the fileset should be temporary ignored.</summary>
        /// <param name="fileSet">The file set.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool ShouldIgnore(IFileSet fileSet)
        {
            return fileSet != null
                   && !string.IsNullOrWhiteSpace(fileSet.Output)
                   && (this.ShouldIgnoreOutputs(fileSet) || this.ShouldIgnoreOutputExtensions(fileSet));
        }

        /// <summary>Determines if the resource pivot key should be temporary ignored.</summary>
        /// <param name="resourcePivotKey">The resource pivot key.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ShouldIgnore(ResourcePivotKey resourcePivotKey)
        {
            return this.resourcePivots.ContainsKey(resourcePivotKey.GroupKey)
                   && this.resourcePivots[resourcePivotKey.GroupKey].Any()
                   && this.resourcePivots[resourcePivotKey.GroupKey].All(pivotToIgnore => resourcePivotKey.Key.IndexOf(pivotToIgnore, StringComparison.OrdinalIgnoreCase) == -1);
        }

        /// <summary>The get items from a string seperated by a semicolon.</summary>
        /// <param name="items">The override locales.</param>
        /// <returns>The items.</returns>
        private static IEnumerable<string> GetItems(string items)
        {
            return
                items == null
                    ? new string[] { }
                    : items.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>Get items from multiple elements where the content values are seperated by a semicolon.</summary>
        /// <param name="elements">The elements.</param>
        /// <param name="elementName">The element name.</param>
        /// <returns>The items.</returns>
        private static IEnumerable<string> GetElementItems(IEnumerable<XElement> elements, string elementName)
        {
            return Enumerable.Where(GetItems(elements.Elements(elementName).Select(e => (string)e).FirstOrDefault()), i => !i.IsNullOrWhitespace())
                             .Select(i => i.Trim());
        }

        /// <summary>The should ignore outputs.</summary>
        /// <param name="fileSet">The file set.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ShouldIgnoreOutputs(IFileSet fileSet)
        {
            return this.outputs.Any()
                   && !this.outputs.Any(
                       output =>
                       fileSet.Output.IndexOf(output, StringComparison.OrdinalIgnoreCase) >= 0
                       && (output.IndexOf(".", StringComparison.OrdinalIgnoreCase) == -1 || fileSet.Output.Count(o => o == '.') > 1));
        }

        /// <summary>The should ignore output extensions.</summary>
        /// <param name="fileSet">The file set.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool ShouldIgnoreOutputExtensions(IFileSet fileSet)
        {
            return this.outputExtensions.Any()
                   && !this.outputExtensions.Any(outputExtension => fileSet.Output.EndsWith(outputExtension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>The load from file.</summary>
        /// <param name="overrideFile">The override file.</param>
        private void LoadFromFile(string overrideFile)
        {
            if (File.Exists(overrideFile))
            {
                try
                {
                    var doc = XDocument.Load(overrideFile);
                    var overrideElements = doc.Elements("Overrides");
                    this.SkipAll = overrideElements.Attributes("SkipAll").Select(a => (bool?)a).FirstOrDefault() == true;
                    this.resourcePivots.Add(Strings.LocalesResourcePivotKey, GetElementItems(overrideElements, "Locales").ToList());
                    this.resourcePivots.Add(Strings.ThemesResourcePivotKey, GetElementItems(overrideElements, "Themes").ToList());
                    this.resourcePivots.Add(Strings.DpiResourcePivotKey, GetElementItems(overrideElements, "Dpi").ToList());
                    this.outputs.AddRange(GetElementItems(overrideElements, "Outputs"));
                    this.outputExtensions.AddRange(GetElementItems(overrideElements, "OutputExtensions"));
                    foreach (var resourcePivotElement in overrideElements.Elements("ResourcePivot"))
                    {
                        this.resourcePivots.Add((string)resourcePivotElement.Attribute("key"), ((string)resourcePivotElement).SafeSplitSemiColonSeperatedValue().ToList());
                    }
                }
                catch (Exception ex)
                {
                    throw new ConfigurationErrorsException(ResourceStrings.OverrideFileLoadErrorMessage.InvariantFormat(overrideFile), ex);
                }
            }
        }
    }
}