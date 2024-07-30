// -----------------------------------------------------------------------
// <copyright file="JSValidationConfig.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;
    using Extensions;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class JSValidationConfig : INamedConfig
    {
         /// <summary>
        /// Initializes a new instance of the <see cref="JSValidationConfig"/> class.
        /// </summary>
        public JSValidationConfig()
        {
            // defaults
             this.ShouldAnalyze = true;
            this.AnalyzeArguments = Strings.DefaultAnalyzeArgs;
        }

          /// <summary>
        /// Initializes a new instance of the <see cref="JSValidationConfig"/> class.
        /// </summary>
        /// <param name="element">element containing js config info</param>
        internal JSValidationConfig(XElement element)
            : this()
        {
            Contract.Requires(element != null);
            /* expect this format:
             <Analyze name="Debug">
              <ShouldAnalyze>True</ShouldAnalyze>
              <AnalyzerArguments>-analyze -WARN:4</AnalyzerArguments>
            </Analyze>
             */

            this.Name = (string)element.Attribute("config") ?? string.Empty;

            foreach (var descendant in element.Descendants())
            {
                var name = descendant.Name.ToString();
                var value = descendant.Value;

                switch (name)
                {
                    case "Analyze":
                        this.ShouldAnalyze = value.TryParseBool();
                        break;
                    case "AnalayzeArguments": // TYPO! but let's keep it in case someone noticed and it using it.
                    case "AnalyzeArguments":
                        this.AnalyzeArguments = !value.IsNullOrWhitespace() ? value : Strings.DefaultAnalyzeArgs;
                        break;
                }
            }
        }


        /// <summary>Gets or sets the name of the configuration.</summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the set should be validated.
        /// </summary>
        internal bool ShouldAnalyze { get; set; }

        /// <summary>
        /// Gets or sets the analyze arguments.
        /// </summary>
        internal string AnalyzeArguments { get; set; }
    }
}
