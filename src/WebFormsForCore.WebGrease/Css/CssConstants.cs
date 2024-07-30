// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CssConstants.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   CssConstants used by ASTs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css
{
    /// <summary>CssConstants used by ASTs</summary>
    internal static class CssConstants
    {
        /// <summary>
        /// Gets the Double Dot string constant
        /// </summary>
        /// <value>Double Dot string constant</value>
        public const string DoubleDot = "..";

        /// <summary>The and string.</summary>
        public const string And = "and";

        /// <summary>The namespace string.</summary>
        public const string Namespace = "@namespace";

        /// <summary>The not string.</summary>
        public const string Not = "not";

        /// <summary>The prefix match.</summary>
        public const string PrefixMatch = "^=";

        /// <summary>The suffix match.</summary>
        public const string SuffixMatch = "$=";

        /// <summary>The sub string match.</summary>
        public const string SubstringMatch = "*=";

        /// <summary>The universal character.</summary>
        public const string Star = "*";

        /// <summary>
        /// Charset symbol
        /// </summary>
        public const string Charset = "@charset ";

        /// <summary>
        /// Import symbol
        /// </summary>
        public const string Import = "@import ";

        /// <summary>
        /// Media symbol
        /// </summary>
        public const string Media = "@media ";

        /// <summary>
        /// Page symbol
        /// </summary>
        public const string Page = "@page";

        /// <summary>The important priority text.</summary>
        public const string Important = "!important";

        /// <summary>
        /// Url symbol
        /// </summary>
        public const string Url = "url";

        /// <summary>
        /// Single space
        /// </summary>
        public const char SingleSpace = ' ';

        /// <summary>
        /// Comma Character
        /// </summary>
        public const char Comma = ',';

        /// <summary>
        /// Semi colon
        /// </summary>
        public const char Semicolon = ';';

        /// <summary>
        /// Round bracket open
        /// </summary>
        public const char OpenRoundBracket = '(';

        /// <summary>
        /// Round bracket close
        /// </summary>
        public const char CloseRoundBracket = ')';

        /// <summary>
        /// Curly bracket open
        /// </summary>
        public const char OpenCurlyBracket = '{';

        /// <summary>
        /// Curly bracket close
        /// </summary>
        public const char CloseCurlyBracket = '}';

        /// <summary>
        /// Curly square open
        /// </summary>
        public const char OpenSquareBracket = '[';

        /// <summary>
        /// Curly square close
        /// </summary>
        public const char CloseSquareBracket = ']';

        /// <summary>
        /// Dot character
        /// </summary>
        public const char Dot = '.';

        /// <summary>
        /// Hash character
        /// </summary>
        public const char Hash = '#';

        /// <summary>
        /// Colon character
        /// </summary>
        public const char Colon = ':';

        /// <summary>
        /// Equal character
        /// </summary>
        public const string Equal = "=";

        /// <summary>
        /// Plus character
        /// </summary>
        public const string Plus = "+";

        /// <summary>
        /// Greater character
        /// </summary>
        public const string Greater = ">";

        /// <summary>
        /// Tilde character
        /// </summary>
        public const string Tilde = "~";

        /// <summary>
        /// Whitespace string
        /// </summary>
        public const string Whitespace = "WHITESPACE";

        /// <summary>
        /// Includes string
        /// </summary>
        public const string Includes = "~=";

        /// <summary>
        /// Dash match
        /// </summary>
        public const string DashMatch = "|=";

        /// <summary>
        /// Rgb string
        /// </summary>
        public const string Rgb = "rgb";

        /// <summary>The escaped new line.</summary>
        public const string EscapedNewLine = "\\\n";

        /// <summary>The escaped carriage return and new line CRLF.</summary>
        public const string EscapedCarriageReturnNewLine = "\\\r\n";

        /// <summary>The escaped form feed.</summary>
        public const string EscapedFormFeed = "\\\f";

        /// <summary>The pipe string.</summary>
        public const string Pipe = "|";
    }
}
