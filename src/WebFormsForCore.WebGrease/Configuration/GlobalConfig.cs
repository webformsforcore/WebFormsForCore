// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalConfig.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease.Configuration
{
    using System.Xml.Linq;

    /// <summary>The global config.</summary>
    public class GlobalConfig : INamedConfig
    {
        /// <summary>Initializes a new instance of the <see cref="GlobalConfig"/> class.</summary>
        /// <param name="settingElement">The setting element.</param>
        public GlobalConfig(XElement settingElement)
        {
            this.Name = (string)settingElement.Attribute("config") ?? string.Empty;
            this.TreatWarningsAsErrors = (bool?)settingElement.Attribute("treatWarningsAsErrors") ?? (bool?)settingElement.Element("TreatWarningsAsErrors");
        }

        /// <summary>Initializes a new instance of the <see cref="GlobalConfig"/> class.</summary>
        public GlobalConfig()
        {
        }

        /// <summary>Gets the treat warnings as errors.</summary>
        public bool? TreatWarningsAsErrors { get; private set; }

        /// <summary>Gets the name.</summary>
        public string Name { get; private set; }
    }
}