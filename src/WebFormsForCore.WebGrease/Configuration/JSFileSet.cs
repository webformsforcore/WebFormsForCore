// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JSFileSet.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   A set of js files that are defined together.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    using WebGrease.Extensions;

    /// <summary>
    /// A set of js files that are defined together.
    /// </summary>
    internal sealed class JSFileSet : FileSetBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSFileSet"/> class.
        /// </summary>
        internal JSFileSet()
        {
            this.Minification = new Dictionary<string, JsMinificationConfig>(StringComparer.OrdinalIgnoreCase);
            this.Validation = new Dictionary<string, JSValidationConfig>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return "[JsFileSet:{0}]".InvariantFormat(this.Output);
        }

        /// <summary>Initializes a new instance of the <see cref="JSFileSet"/> class.</summary>
        /// <param name="jsFileSetElement">config element containing info for a set of js files</param>
        /// <param name="sourceDirectory">The base directory.</param>
        /// <param name="defaultMinification">The default set of minification configs.</param>
        /// <param name="defaultPreProcessing">The default pre processing config. </param>
        /// <param name="defaultBundling">The default Bundling.</param>
        /// <param name="defaultResourcePivots">The default Resource Pivots.</param>
        /// <param name="globalConfig">The global Config.</param>
        /// <param name="defaultOutputPathFormat">The default Output Path Format.</param>
        /// <param name="configurationFile">The configuration File.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Is not excessive")]
        internal JSFileSet(XElement jsFileSetElement, string sourceDirectory, IDictionary<string, JsMinificationConfig> defaultMinification, IDictionary<string, PreprocessingConfig> defaultPreProcessing, IDictionary<string, BundlingConfig> defaultBundling, ResourcePivotGroupCollection defaultResourcePivots, GlobalConfig globalConfig, string defaultOutputPathFormat, string configurationFile)
            : this()
        {
            Contract.Requires(jsFileSetElement != null);
            Contract.Requires(!string.IsNullOrWhiteSpace(sourceDirectory));

            this.InitializeDefaults(defaultResourcePivots, defaultPreProcessing, defaultBundling, defaultOutputPathFormat);
            this.InitializeDefaults(defaultMinification);
            var fileSetElements = this.Initialize(jsFileSetElement, globalConfig, configurationFile);
            this.Load(fileSetElements, sourceDirectory);
        }

        /// <summary>
        /// Gets the validation settings
        /// </summary>
        internal IDictionary<string, JSValidationConfig> Validation { get; private set; }

        /// <summary>Gets the dictionary of minification configurations.</summary>
        internal IDictionary<string, JsMinificationConfig> Minification { get; private set; }

        /// <summary>Loads the settings from the elements.</summary>
        /// <param name="fileSetElements">The elements.</param>
        /// <param name="sourceDirectory">The source Directory.</param>
        protected override void Load(IEnumerable<XElement> fileSetElements, string sourceDirectory)
        {
            base.Load(fileSetElements, sourceDirectory);
            foreach (var element in fileSetElements)
            {
                var name = element.Name.ToString();

                switch (name)
                {
                    case "Minification":
                        // generate a configuration and set it on the dictionary. If the name
                        // already exists, this clobbers it.
                        this.Minification.AddNamedConfig(new JsMinificationConfig(element));
                        break;
                    case "Validation":
                        this.Validation.AddNamedConfig(new JSValidationConfig(element));
                        break;
                }
            }
        }

        /// <summary>Initializes the defaults for the js file set.</summary>
        /// <param name="defaultMinification">The default minification.</param>
        private void InitializeDefaults(IDictionary<string, JsMinificationConfig> defaultMinification)
        {
            // if we were given a default set of minification configs, copy them now
            if (defaultMinification != null && defaultMinification.Count > 0)
            {
                foreach (var configuration in defaultMinification.Keys)
                {
                    this.Minification[configuration] = defaultMinification[configuration];
                }
            }
        }
    }
}
