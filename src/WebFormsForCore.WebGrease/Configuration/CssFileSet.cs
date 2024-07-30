// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CssFileSet.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   A set of Css files that are defined together.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Xml.Linq;

    using WebGrease.Css.Extensions;
    using WebGrease.Extensions;

    /// <summary>
    /// A set of Css files that are defined together.
    /// </summary>
    internal sealed class CssFileSet : FileSetBase
    {
        /// <summary>The local dpi used.</summary>
        private bool localDpiUsed;

        /// <summary>Gets all the the dpi values.</summary>
        private IDictionary<string, HashSet<float>> allDpi = new Dictionary<string, HashSet<float>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CssFileSet"/> class.
        /// </summary>
        internal CssFileSet()
        {
            this.Minification = new Dictionary<string, CssMinificationConfig>(StringComparer.OrdinalIgnoreCase);
            this.ImageSpriting = new Dictionary<string, CssSpritingConfig>(StringComparer.OrdinalIgnoreCase);
            this.Autonaming = new Dictionary<string, AutoNameConfig>(StringComparer.OrdinalIgnoreCase);
            this.Dpi = new HashSet<float>();
        }

        /// <summary>Initializes a new instance of the <see cref="CssFileSet"/> class.</summary>
        /// <param name="cssFileSetElement">config element containing info for a set of css files</param>
        /// <param name="sourceDirectory">The base directory.</param>
        /// <param name="defaultMinification">The default set of minification configs.</param>
        /// <param name="defaultSpriting">The default set of spriting configs. </param>
        /// <param name="defaultPreprocessing">The default pre processing config.</param>
        /// <param name="defaultBundling">The defayult bundling configuration</param>
        /// <param name="defaultResourcePivots">The default resource pivots</param>
        /// <param name="globalConfig">The global Config.</param>
        /// <param name="defaultOutputPathFormat">The default Output Path Format.</param>
        /// <param name="defaultDpi">The default dpi values</param>
        /// <param name="configurationFile">The parent configuration file</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Is not excessive")]
        internal CssFileSet(XElement cssFileSetElement, string sourceDirectory, IDictionary<string, CssMinificationConfig> defaultMinification, IDictionary<string, CssSpritingConfig> defaultSpriting, IDictionary<string, PreprocessingConfig> defaultPreprocessing, IDictionary<string, BundlingConfig> defaultBundling, ResourcePivotGroupCollection defaultResourcePivots, GlobalConfig globalConfig, string defaultOutputPathFormat, IDictionary<string, HashSet<float>> defaultDpi, string configurationFile)
            : this()
        {
            Contract.Requires(cssFileSetElement != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(sourceDirectory));

            this.InitializeDefaults(defaultResourcePivots, defaultPreprocessing, defaultBundling, defaultOutputPathFormat);
            this.InitializeDefaults(defaultMinification, defaultSpriting, defaultDpi);
            var fileSetElements = this.Initialize(cssFileSetElement, globalConfig, configurationFile);
            this.Load(fileSetElements, sourceDirectory);
        }

        /// <summary>
        /// Gets the dictionary of auto naming configs
        /// </summary>
        public IDictionary<string, AutoNameConfig> Autonaming { get; private set; }

        /// <summary>Gets the dictionary of configurations.</summary>
        internal IDictionary<string, CssMinificationConfig> Minification { get; private set; }

        /// <summary>Gets the dpi values specific for this output.</summary>
        internal HashSet<float> Dpi { get; private set; }

        /// <summary>
        /// Gets the dictionary of spriting configurations.
        /// </summary>
        internal IDictionary<string, CssSpritingConfig> ImageSpriting { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "[CssFileSet:{0}]".InvariantFormat(this.Output);
        }

        /// <summary>Loads the settings from the elements.</summary>
        /// <param name="fileSetElements">The elements.</param>
        /// <param name="sourceDirectory">The source Directory.</param>
        protected override void Load(IEnumerable<XElement> fileSetElements, string sourceDirectory)
        {
            base.Load(fileSetElements, sourceDirectory);
            foreach (var element in fileSetElements)
            {
                var name = element.Name.ToString();
                var value = (string)element;
                switch (name)
                {
                    case "Dpi":
                        if (!this.localDpiUsed)
                        {
                            this.localDpiUsed = true;
                            this.allDpi.Clear();
                        }

                        var dpi = value.NullSafeAction(StringExtensions.SafeSplitSemiColonSeperatedValue)
                            .Select(d => d.TryParseFloat())
                            .Where(d => d != null)
                            .Select(d => d.Value);

                        var output = (string)element.Attribute("output");
                        this.allDpi[output.AsNullIfWhiteSpace() ?? string.Empty] = new HashSet<float>(dpi);

                        break;
                    case "Minification":
                        // create a configuration object from the markup and set it in the dictionary.
                        // if it already exists (a default was there), clobber it with the new one.
                        this.Minification.AddNamedConfig(new CssMinificationConfig(element));
                        break;

                    case "Spriting":
                        this.ImageSpriting.AddNamedConfig(new CssSpritingConfig(element));
                        break;

                    case "Autoname":
                        this.Autonaming.AddNamedConfig(new AutoNameConfig(element));
                        break;
                }
            }

            var outputSpecificKey = this.allDpi.Keys.FirstOrDefault(k => !k.IsNullOrWhitespace() && this.Output.IndexOf(k, StringComparison.OrdinalIgnoreCase) != -1) ?? string.Empty;

            HashSet<float> outputSpecificDpi;
            if (!this.allDpi.TryGetValue(outputSpecificKey, out outputSpecificDpi))
            {
                this.allDpi.TryGetValue(string.Empty, out outputSpecificDpi);
            }

            this.Dpi = outputSpecificDpi ?? new HashSet<float> { 1f };
        }

        /// <summary>Initializes the defaults for the css file set.</summary>
        /// <param name="defaultMinification">The default minification.</param>
        /// <param name="defaultSpriting">The default spriting.</param>
        /// <param name="defaultDpi">The default Dpi.</param>
        private void InitializeDefaults(IDictionary<string, CssMinificationConfig> defaultMinification, IDictionary<string, CssSpritingConfig> defaultSpriting, IDictionary<string, HashSet<float>> defaultDpi)
        {
            // apply default dpi's
            if (defaultDpi != null && defaultDpi.Any())
            {
                defaultDpi.ForEach(dd => this.allDpi[dd.Key] = dd.Value);
            }

            // if we were given a default set of minification configs, copy them now
            if (defaultMinification != null && defaultMinification.Count > 0)
            {
                foreach (var configuration in defaultMinification.Keys)
                {
                    this.Minification[configuration] = defaultMinification[configuration];
                }
            }

            // if we were given a default set of spriting configs, copy them now
            if (defaultSpriting != null && defaultSpriting.Count > 0)
            {
                foreach (var configuration in defaultSpriting.Keys)
                {
                    this.ImageSpriting[configuration] = defaultSpriting[configuration];
                }
            }
        }
    }
}
