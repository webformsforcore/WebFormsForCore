// -----------------------------------------------------------------------
// <copyright file="BundlingConfig.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;
    using Extensions;

    /// <summary>
    /// BundlingConfig class.
    /// </summary>
    public class BundlingConfig : INamedConfig
    {
        /// <summary>
        /// Creates a new instance of the BundlingConfig class.
        /// </summary>
        public BundlingConfig()
        {
            this.ShouldBundleFiles = true;
        }

        /// <summary>
        /// Creates a new instance of the BundlingConfig class.
        /// </summary>
        internal BundlingConfig(XElement element)
            : this()
        {
            Contract.Requires(element != null);
            /* expect this format:
             <Bundling config="Debug">
              <CombineFiles>true</CombineFiles>
            </Bundling>
             */


            this.Name = (string)element.Attribute("config") ?? string.Empty;

            foreach (var descendant in element.Descendants())
            {
                var name = descendant.Name.ToString();
                var value = descendant.Value;

                switch (name)
                {
                    case "AssembleFiles":
                        this.ShouldBundleFiles = value.TryParseBool();
                        break;
                    case "MinimalOutput":
                        this.MinimalOutput = value.TryParseBool();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether to autoname/hash files.
        /// </summary>
        public bool ShouldBundleFiles { get; private set; }

        /// <summary>
        /// Determines if the output should be minimal (no added comments)
        /// </summary>
        public bool MinimalOutput { get; private set; }

        /// <summary>
        /// Gets the name of this config.
        /// </summary>
        public string Name { get; private set; }

    }
}
