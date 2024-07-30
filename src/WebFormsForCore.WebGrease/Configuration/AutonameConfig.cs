// -----------------------------------------------------------------------
// <copyright file="AutonameConfig.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;
    using Extensions;

    /// <summary>
    /// Auto Renaming config class
    /// </summary>
    public class AutoNameConfig : INamedConfig
    {
        /// <summary>
        /// Creates a new instance of the AutoNameConfig class.
        /// </summary>
        public AutoNameConfig()
        {
            this.ShouldAutoName = true;
        }

        /// <summary>
        /// Creates a new instance of the AutoNameConfig class.
        /// </summary>
        /// <param name="element">Xml element to parse from.</param>
        internal AutoNameConfig(XElement element)
            : this()
        {
            Contract.Requires(element != null);
            /* expect this format:
            <Autoname config="Debug">
             <RenameFiles>true</RenameFiles>
           </Autoname>
            */

            this.Name = (string)element.Attribute("config") ?? string.Empty;
            foreach (var descendant in element.Descendants())
            {
                var name = descendant.Name.ToString();
                var value = descendant.Value;

                switch (name)
                {
                    case "RenameFiles":
                        this.ShouldAutoName = value.TryParseBool();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether to autoname/hash files.
        /// </summary>
        public bool ShouldAutoName { get; private set; }

        /// <summary>
        /// Gets the name of this config.
        /// </summary>
        public string Name { get; private set; }
    }
}
