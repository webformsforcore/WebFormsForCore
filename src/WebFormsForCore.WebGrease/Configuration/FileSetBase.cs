// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSetBase.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    using WebGrease.Extensions;

    /// <summary>The abstract file set base class.</summary>
    internal abstract class FileSetBase : IFileSet
    {
        /// <summary>
        /// This flag is used to determine whether we are using the local file-set
        /// Resource pivots and not the global defaults. If we parse a file-set locale
        /// and this flag is false, we are going to clear anything we picked up from
        /// the global settings, thereby completely replacing the default list of locales
        /// with the local set.
        /// </summary>
        private readonly IList<string> usingLocalResourcePivot = new List<string>();

        /// <summary>Initializes a new instance of the <see cref="FileSetBase"/> class.</summary>
        protected FileSetBase()
        {
            this.ResourcePivots = new ResourcePivotGroupCollection();
            this.AutoNaming = new Dictionary<string, AutoNameConfig>(StringComparer.OrdinalIgnoreCase);
            this.InputSpecs = new List<InputSpec>();
            this.Bundling = new Dictionary<string, BundlingConfig>(StringComparer.OrdinalIgnoreCase);
            this.Preprocessing = new Dictionary<string, PreprocessingConfig>(StringComparer.OrdinalIgnoreCase);
            this.LoadedConfigurationFiles = new List<string>();
        }

        /// <summary>Gets the resource pivots.</summary>
        public ResourcePivotGroupCollection ResourcePivots { get; private set; }

        /// <summary>Gets the external files.</summary>
        public IList<string> LoadedConfigurationFiles { get; private set; }

        /// <summary>Gets the locales.</summary>
        public IList<string> Locales
        {
            get
            {
                return this.ResourcePivots[Strings.LocalesResourcePivotKey].NullSafeAction(l => l.Keys.ToArray()) ?? new string[] { };
            }
        }

        /// <summary>Gets the themes.</summary>
        public IList<string> Themes
        {
            get
            {
                return this.ResourcePivots[Strings.ThemesResourcePivotKey].NullSafeAction(l => l.Keys.ToArray()) ?? new string[] { };
            }
        }

        /// <summary>Gets the preprocessing configuration.</summary>
        public IDictionary<string, PreprocessingConfig> Preprocessing { get; private set; }

        /// <summary>
        /// Gets the dictionary of auto naming configs
        /// </summary>
        public IDictionary<string, BundlingConfig> Bundling { get; private set; }

        /// <summary>Gets the output specified.</summary>
        public string Output { get; set; }

        /// <summary>Gets the output path format specified.</summary>
        public string OutputPathFormat { get; set; }

        /// <summary>Gets the list of <see cref="InputSpec"/> items specified.</summary>
        public IList<InputSpec> InputSpecs { get; private set; }

        /// <summary>
        /// Gets the dictionary of auto naming configs
        /// </summary>
        public IDictionary<string, AutoNameConfig> AutoNaming { get; private set; }

        /// <summary>Gets the global config.</summary>
        internal GlobalConfig GlobalConfig { get; private set; }

        /// <summary>Loads the settings from the elements.</summary>
        /// <param name="fileSetElements">The elements.</param>
        /// <param name="sourceDirectory">The source Directory.</param>
        protected virtual void Load(IEnumerable<XElement> fileSetElements, string sourceDirectory)
        {
            foreach (var fileSetElement in fileSetElements)
            {
                var name = fileSetElement.Name.ToString();
                var value = fileSetElement.Value;

                switch (name)
                {
                    case "OutputPathFormat":
                        this.OutputPathFormat = value;
                        break;
                    case "Inputs":
                        this.InputSpecs.AddInputSpecs(sourceDirectory, fileSetElement);
                        break;

                    case "Preprocessing":
                        this.Preprocessing.AddNamedConfig(new PreprocessingConfig(fileSetElement));
                        break;

                    case "Bundling":
                        this.Bundling.AddNamedConfig(new BundlingConfig(fileSetElement));
                        break;

                    case "Autoname":
                        this.AutoNaming.AddNamedConfig(new AutoNameConfig(fileSetElement));
                        break;

                    case "Locales":
                        if (!this.usingLocalResourcePivot.Contains(Strings.LocalesResourcePivotKey))
                        {
                            // we haven't found any file-set locales yet, so we
                            // are going to clear the list (if it has anything)
                            // so we clobber the default in effect.
                            this.usingLocalResourcePivot.Add(Strings.LocalesResourcePivotKey);
                            this.ResourcePivots.Clear(Strings.LocalesResourcePivotKey);
                        }

                        this.ResourcePivots.Set(
                            Strings.LocalesResourcePivotKey,
                            ResourcePivotApplyMode.ApplyAsStringReplace,
                            value.NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));

                        break;

                    case "Themes":
                        if (!this.usingLocalResourcePivot.Contains(Strings.ThemesResourcePivotKey))
                        {
                            // we haven't found any file-set themes yet, so we
                            // are going to clear the list (if it has anything)
                            // so we clobber the default in effect.
                            this.usingLocalResourcePivot.Add(Strings.ThemesResourcePivotKey);
                            this.ResourcePivots.Clear(Strings.ThemesResourcePivotKey);
                        }

                        this.ResourcePivots.Set(
                            Strings.ThemesResourcePivotKey,
                            ResourcePivotApplyMode.ApplyAsStringReplace,
                            value.NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));

                        break;

                    case "ResourcePivot":
                        this.ResourcePivots.Set(
                            (string)fileSetElement.Attribute("key"),
                            ((string)fileSetElement.Attribute("applyMode")).TryParseToEnum<ResourcePivotApplyMode>() ?? ResourcePivotApplyMode.ApplyAsStringReplace,
                            ((string)fileSetElement).NullSafeAction(sv => sv.SafeSplitSemiColonSeperatedValue()));

                        break;
                }
            }
        }

        /// <summary>Initializes with the values from the constructor and returns the filesets.</summary>
        /// <param name="fileSetElement">The file set element.</param>
        /// <param name="globalConfig">The global config.</param>
        /// <param name="configurationFile">The configuration file.</param>
        /// <returns>The list of filesets</returns>
        protected IEnumerable<XElement> Initialize(XElement fileSetElement, GlobalConfig globalConfig, string configurationFile)
        {
            var outputAttribute = fileSetElement.Attribute("output");
            this.Output = (string)outputAttribute ?? string.Empty;

            this.GlobalConfig = globalConfig;

            var fileSetElements = fileSetElement.Descendants().ToList();
            WebGreaseConfiguration.ForEachConfigSourceElement(
                fileSetElement,
                configurationFile,
                (element, s) =>
                {
                    this.LoadedConfigurationFiles.Add(s);
                    fileSetElements.AddRange(element.Descendants());
                });

            return fileSetElements;
        }

        /// <summary>Initializes the default values for each fileset.</summary>
        /// <param name="defaultResourcePivots">The default resource pivots.</param>
        /// <param name="defaultPreprocessing">The default preprocessing.</param>
        /// <param name="defaultBundling">The default bundling.</param>
        protected void InitializeDefaults(ResourcePivotGroupCollection defaultResourcePivots, IDictionary<string, PreprocessingConfig> defaultPreprocessing, IDictionary<string, BundlingConfig> defaultBundling, string defaultOutputPathFormat)
        {
            // Set the default output path format
            if (!string.IsNullOrWhiteSpace(defaultOutputPathFormat))
            {
                this.OutputPathFormat = defaultOutputPathFormat;
            }

            // if we were given a default set of resource pivots, add them to the resource pivots
            if (defaultResourcePivots != null && defaultResourcePivots.Count() > 0)
            {
                foreach (var resourcePivotGroup in defaultResourcePivots)
                {
                    this.ResourcePivots.Set(resourcePivotGroup.Key, resourcePivotGroup.ApplyMode, resourcePivotGroup.Keys);
                }
            }

            // if we were given a default set of minification configs, copy them now
            if (defaultPreprocessing != null && defaultPreprocessing.Count > 0)
            {
                foreach (var configuration in defaultPreprocessing.Keys)
                {
                    this.Preprocessing[configuration] = defaultPreprocessing[configuration];
                }
            }

            // Set the default bundling
            if (defaultBundling != null && defaultBundling.Count > 0)
            {
                foreach (var configuration in defaultBundling.Keys)
                {
                    this.Bundling[configuration] = defaultBundling[configuration];
                }
            }
        }
    }
}