// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Strings.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// <summary>
//   Represents the various strings
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Represents the various strings </summary>
    internal static class Strings
    {
        /// <summary>
        /// Gets the CSS.
        /// </summary>
        /// <value>The CSS string.</value>
        internal const string Css = "css";

        /// <summary>The dpi resource pivot key.</summary>
        internal const string DpiResourcePivotKey = "dpi";

        /// <summary>The themes resource pivot key.</summary>
        internal const string ThemesResourcePivotKey = "themes";

        /// <summary>The locales resource pivot key.</summary>
        internal const string LocalesResourcePivotKey = "locales";

        /// <summary>
        /// Gets the CSS filter.
        /// </summary>
        /// <value>The CSS filter.</value>
        internal const string CssFilter = "*.css";

        /// <summary>
        /// Gets the JS filter.
        /// </summary>
        /// <value>The JS filter.</value>
        internal const string JsFilter = "*.js";

        /// <summary>
        /// Js String constant
        /// </summary>
        internal const string JS = "js";

        /// <summary>
        /// Measurement units
        /// </summary>
        internal const string Px = "px";

        /// <summary>
        /// The scan log extension
        /// </summary>
        internal const string ScanLogExtension = ".scan.xml";

        /// <summary>
        /// Resource filter string constant
        /// </summary>
        internal const string ResxExtension = ".resx";

        /// <summary>
        /// Gets the semicolon.
        /// </summary>
        /// <value>The semicolon.</value>
        internal const string Semicolon = ";";

        /// <summary>
        /// Gets the default locale.
        /// </summary>
        /// <value>The default locale.</value>
        internal const string DefaultLocale = "generic-generic";

        /// <summary>
        /// Gets the default locale resx.
        /// </summary>
        /// <value>The default locale resx.</value>
        internal const string DefaultResx = "generic-generic.resx";

        /// <summary>
        /// argument name of the globals to ingore for js minification
        /// </summary>
        internal const string GlobalsToIgnoreArg = "/global:";

        /// <summary>
        /// default globals ignored.
        /// </summary>
        internal const string DefaultGlobalsToIgnore = "jQuery";

        /// <summary>
        /// Default set of minify args.
        /// </summary>
        internal const string DefaultMinifyArgs = "";

        /// <summary>
        /// Set of args passed to jscruch for analyze/minify.
        /// </summary>
        internal const string DefaultAnalyzeArgs = "-analyze -WARN:4";

        /// <summary>The css localized output.</summary>
        internal const string CssLocalizedOutput = "CssLocalizedOutput";

        /// <summary>The js localized output.</summary>
        internal const string JsLocalizedOutput = "JsLocalizedOutput";

        /// <summary>file name for image hash logs</summary>
        internal const string ImagesLogFile = "images_log.xml";

        /// <summary>file name for css logs</summary>
        internal const string CssLogFile = "css_log.xml";

        /// <summary>file name for js logs</summary>
        internal const string JsLogFile = "js_log.xml";

        /// <summary>
        /// File Filter Separator.
        /// </summary>
        internal static readonly char[] FileFilterSeparator = ",".ToCharArray();

        /// <summary>default list of image extensions.</summary>
        internal static readonly List<string> DefaultImageExtensions = new[] { "png", "jpg", "jpeg", "gif" }.ToList();

        /// <summary>Gets the semicolon separator field.</summary>
        private static readonly char[] SemicolonSeparatorField = new[] { ';' };

        /// <summary>Gets the semicolon separator.</summary>
        internal static char[] SemicolonSeparator
        {
            get { return SemicolonSeparatorField; }
        }
    }
}
