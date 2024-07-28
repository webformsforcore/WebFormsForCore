//------------------------------------------------------------------------------
// <copyright file="XmlConvert.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Xml {
    using System.IO;
    using System.Text;
    using System.Globalization;
    using System.Xml.Schema;
    using System.Diagnostics;
    using System.Collections;
    using System.Text.RegularExpressions;
    using System.Web.Services;

    /// <include file='doc\XmlConvert.uex' path='docs/doc[@for="XmlConvert"]/*' />
    /// <devdoc>
    ///    Encodes and decodes XML names according to
    ///    the "Encoding of arbitrary Unicode Characters in XML Names" specification.
    /// </devdoc>
    public class XmlConvertInternal {

		// XML whitespace characters, <spec>http://www.w3.org/TR/REC-xml#NT-S</spec>
		internal static readonly char[] WhitespaceChars = new char[] { ' ', '\t', '\n', '\r' };

		// Trim a string using XML whitespace characters
		internal static string TrimString(string value)
		{
			return value.Trim(WhitespaceChars);
		}

		internal static Uri ToUri(string s) {
            if (s != null && s.Length > 0) { //string.Empty is a valid uri but not "   "
                s = TrimString(s);
                if (s.Length == 0 || s.IndexOf("##", StringComparison.Ordinal) != -1) {
                    throw new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Uri"));
                }
            }
            Uri uri;
            if (!Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out uri)) {
                throw new FormatException(Res.GetString(Res.XmlConvert_BadFormat, s, "Uri"));
            }
            return uri;
        }

        // Split a string into a whitespace-separated list of tokens
        internal static string[] SplitString(string value) {
            return value.Split(WhitespaceChars, StringSplitOptions.RemoveEmptyEntries);
        }

        internal static string[] SplitString(string value, StringSplitOptions splitStringOptions) {
            return value.Split(WhitespaceChars, splitStringOptions);
        }
    }
}
