// ----------------------------------------------------------------------------------------------------
// <copyright file="PreprocessingConfig.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    /// <summary>
    /// PreProcess Configuration Class, that determines which preprocessors to use. 
    /// Expects a semi colon seperated list of preprocessor names as the Engines element.
    /// All the other values will be passed on to the preprocessors as configuration.
    /// </summary>
    public class PreprocessingConfig : INamedConfig
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PreprocessingConfig"/> class.</summary>
        public PreprocessingConfig()
        {
            this.PreprocessingEngines = new Collection<string>();
        }

        /// <summary>Initializes a new instance of the <see cref="PreprocessingConfig"/> class. 
        /// expect this format in the element:
        /// &lt;PreProcess&gt;
        ///  &lt;Engines&gt;engine1;engine2;engine3&lt;/Engines&gt;
        ///      &lt;SettingForEngine1&gt;Value1&lt;/SettingForEngine1&gt;
        ///      &lt;SettingForEngine2&gt;Value2&lt;/SettingForEngine2&gt;
        ///      &lt;SettingForEngine3&gt;Value3&lt;/SettingForEngine3&gt;
        /// &lt;/PreProcess&gt;
        /// or
        /// &lt;PreProcess 
        ///      Engines="engine1;engine2;engine3"
        ///      SettingForEngine1="SomeValue"&gt;
        ///          &lt;SettingForEngine3&gt;Value3&lt;/SettingForEngine3&gt;
        /// &lt;/PreProcess&gt;
        /// or any combination of the 2 above.</summary>
        /// <param name="element">The element containing the configuration</param>
        public PreprocessingConfig(XElement element)
            : this()
        {
            Contract.Requires(element != null);

            this.Name = (string)element.Attribute("config") ?? string.Empty;

            var preProcessors = 
                (string)element.Element("Engines") 
                ?? (string)element.Attribute("Engines")
                ?? (string)element.Attribute("engines");

            if (!string.IsNullOrWhiteSpace(preProcessors))
            {
                foreach (var preProcessor in preProcessors.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    this.Enabled = true;
                    this.PreprocessingEngines.Add(preProcessor);
                }
            }

            this.Element = element;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Configuration element to be passed on to PreProcessEngines.
        /// </summary>
        public XElement Element { get; private set; }

        /// <summary>
        /// Shortcut to check if there are any preprocessors set.
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// The list of preprocessors to use
        /// </summary>
        public Collection<string> PreprocessingEngines { get; private set; }

        /// <summary>Gets the name of the config.</summary>
        public string Name { get; private set; }

        #endregion
    }
}