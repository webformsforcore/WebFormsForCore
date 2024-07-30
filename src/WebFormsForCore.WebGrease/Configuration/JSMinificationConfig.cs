// -----------------------------------------------------------------------
// <copyright file="JSMinificationConfig.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// -----------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;
    using Extensions;

    /// <summary>
    /// Configuration for js-specific settings.
    /// </summary>
    internal sealed class JsMinificationConfig : INamedConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsMinificationConfig"/> class.
        /// </summary>
        public JsMinificationConfig()
        {
            // defaults
            this.ShouldMinify = true;
            this.GlobalsToIgnore = Strings.DefaultGlobalsToIgnore;
            this.MinificationArugments = Strings.DefaultMinifyArgs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsMinificationConfig"/> class.
        /// </summary>
        /// <param name="element">element containing js config info</param>
        internal JsMinificationConfig(XElement element)
            : this()
        {
            Contract.Requires(element != null);
            /* expect this format:
             <Minification name="Debug">
              <GlobalsToIgnore>jQuery</GlobalsToIgnore>
              <Minify>true</Minify>
            </Minification>
             */

            this.Name = (string)element.Attribute("config") ?? string.Empty;

            foreach (var descendant in element.Descendants())
            {
                var name = descendant.Name.ToString();
                var value = descendant.Value;

                switch (name)
                {
                    case "Minify":
                        this.ShouldMinify = value.TryParseBool();
                        break;
                    case "GlobalsToIgnore":
                        this.GlobalsToIgnore = !value.IsNullOrWhitespace() ? value : Strings.DefaultGlobalsToIgnore;
                        break;
                    case "MinifyArguments":
                        this.MinificationArugments = !value.IsNullOrWhitespace() ? value : Strings.DefaultMinifyArgs;
                        break;
                }
            }
        }

        /// <summary>Gets or sets the name of the configuration.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets a value indicating whether the set should be minified.</summary>
        internal bool ShouldMinify { get; set; }

        /// <summary>Gets or sets the list of global variables to ignore.</summary>
        internal string GlobalsToIgnore { get; set; }

        /// <summary>Gets or sets the minification arguments</summary>
        internal string MinificationArugments { get; set; }
    }
}
