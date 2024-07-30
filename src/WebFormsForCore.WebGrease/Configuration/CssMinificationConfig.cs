// -----------------------------------------------------------------------
// <copyright file="CssMinificationConfig.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// -----------------------------------------------------------------------

namespace WebGrease.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;
    using Extensions;

    /// <summary>
    /// Configuration for CSS Minification settings.
    /// </summary>
    internal class CssMinificationConfig : INamedConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CssMinificationConfig"/> class.
        /// </summary>
        public CssMinificationConfig()
        {
            this.ShouldMinify = true;
            this.ForbiddenSelectors = new string[0];
            this.RemoveSelectors = new string[0];
            this.NonMergeSelectors = new string[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CssMinificationConfig"/> class.
        /// </summary>
        /// <param name="element">element containing css config info</param>
        public CssMinificationConfig(XElement element)
            : this()
        {
            Contract.Requires(element != null);
            /* expect this format:
            <Minification config="Debug">
             <Minify>false</Minify>
           </Minification>
            */

            this.Name = (string)element.Attribute("config") ?? string.Empty;

            foreach (var descendant in element.Descendants())
            {
                var name = descendant.Name.ToString();
                var value = descendant.Value;

                switch (name)
                {
                    case "MergeMediaQueries":
                        this.ShouldMergeMediaQueries = value.TryParseBool();
                        break;
                    case "Optimize":
                        this.ShouldOptimize = value.TryParseBool();
                        break;
                    case "Minify":
                        this.ShouldMinify = value.TryParseBool();
                        break;
                    case "ValidateLowerCase":
                    case "ValidateForLowerCase": // Found some old tests that used this one, assume it is needed.
                        this.ShouldValidateLowerCase = value.TryParseBool();
                        break;
                    case "ExcludeProperties":
                        this.ShouldExcludeProperties = value.TryParseBool();
                        break;
                    case "ProhibitedSelectors":
                        this.ForbiddenSelectors = value.IsNullOrWhitespace() ? new string[0] : value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "RemoveSelectors":
                        this.RemoveSelectors = value.IsNullOrWhitespace() ? new string[0] : value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "NonMergeSelectors":
                        this.NonMergeSelectors = value.IsNullOrWhitespace() ? new string[0] : value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        break;
                    case "PreventOrderBasedConflict":
                        this.ShouldPreventOrderBasedConflict = value.TryParseBool();
                        break;
                    case "MergeBasedOnCommonDeclarations":
                        this.ShouldMergeBasedOnCommonDeclarations = value.TryParseBool();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name for this config instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>Gets or sets a value indicating whether the set should be minified, also force enabled optimize.</summary>
        internal bool ShouldMinify { get; set; }

        /// <summary>Gets or sets a value indicating whether webgrease should optimize the css.</summary>
        internal bool ShouldOptimize { get; set; }

        /// <summary>Gets or sets a value indicating whether the minification should merge media queries, only used when optimize is true.</summary>
        internal bool ShouldMergeMediaQueries { get; set; }

        /// <summary>
        /// Gets or sets a value indicating wheter to validate selectores and properties to be all lower case.
        /// </summary>
        internal bool ShouldValidateLowerCase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether css should be validated for lower case.
        /// </summary>
        internal bool ShouldExcludeProperties { get; set; }

        /// <summary>
        /// Gets or sets the ShouldPreventOrderBasedConflic, default is false
        /// </summary>
        internal bool ShouldPreventOrderBasedConflict { get; set; }

        /// <summary>
        /// Gets or sets the ShouldMergeBasedOnCommonDeclarations, default is false
        /// </summary>
        internal bool ShouldMergeBasedOnCommonDeclarations { get; set; }

        /// <summary>
        /// Gets or sets a collection of selectors which are forbidden to be in the file.
        /// </summary>
        internal IEnumerable<string> ForbiddenSelectors { get; set; }

        /// <summary>
        /// Gets or sets a collection of selectors that will be ignored and not outputted.
        /// </summary>
        internal IEnumerable<string> RemoveSelectors { get; set; }

        /// <summary>
        /// Gets or sets a collection of selectors that will not be merged in optimization.
        /// </summary>
        internal IEnumerable<string> NonMergeSelectors { get; set; }
    }
}
