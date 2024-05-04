#if NET8_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.Web.RegularExpressions
{
	public partial class NetCoreRegExes
	{

		const string directiveRegexString = @"<%\s*@" +
			@"(" +
			@"\s*(?<attrname>\w[\w:]*(?=\W))(" +            // Attribute name
			@"\s*(?<equal>=)\s*""(?<attrval>[^""]*)""|" +   // ="bar" attribute value
			@"\s*(?<equal>=)\s*'(?<attrval>[^']*)'|" +      // ='bar' attribute value
			@"\s*(?<equal>=)\s*(?<attrval>[^\s""'%>]*)|" +  // =bar attribute value
			@"(?<equal>)(?<attrval>\s*?)" +                 // no attrib value (with no '=')
			@")" +
			@")*" +
			@"\s*?%>";
		const RegexOptions regexOptions = RegexOptions.Singleline | RegexOptions.Multiline;

        [GeneratedRegex(@"\G<(?<tagname>[\w:\.]+)" +
		    @"(" +
		    @"\s+(?<attrname>\w[-\w:]*)(" +       // Attribute name
		    @"\s*=\s*""(?<attrval>[^""]*)""|" +   // ="bar" attribute value
		    @"\s*=\s*'(?<attrval>[^']*)'|" +      // ='bar' attribute value
		    @"\s*=\s*(?<attrval><%#.*?%>)|" +     // =<%#expr%> attribute value
		    @"\s*=\s*(?<attrval>[^\s=""'/>]*)|" + // =bar attribute value
		    @"(?<attrval>\s*?)" +                 // no attrib value (with no '=')
		    @")" +
		    @")*" +
		    @"\s*(?<empty>/)?>", regexOptions)]
        public partial Regex TagRegex();
        [GeneratedRegex(@"\G" + directiveRegexString, regexOptions)]
        public partial Regex DirectiveRegex();

        [GeneratedRegex(@"\G</(?<tagname>[\w:\.]+)\s*>", regexOptions)]
        public partial Regex EndTagRegex();

        [GeneratedRegex(@"\G<%(?!@)(?<code>.*?)%>", regexOptions)]
        public partial Regex AspCodeRegex();

        [GeneratedRegex(@"\G<%\s*?=(?<code>.*?)?%>", regexOptions)]
        public partial Regex AspExprRegex();

        [GeneratedRegex(@"\G<%#(?<encode>:)?(?<code>.*?)?%>", regexOptions)]
        public partial Regex DatabindExprRegex();

        [GeneratedRegex(@"\G<%--(([^-]*)-)*?-%>", regexOptions)]
        public partial Regex CommentRegex();

        [GeneratedRegex(@"\G<!--\s*#(?i:include)\s*(?<pathtype>[\w]+)\s*=\s*[""']?(?<filename>[^\""']*?)[""']?\s*-->", regexOptions)]
        public partial Regex IncludeRegex();

        [GeneratedRegex(@"\G[^<]+", regexOptions)]
        public partial Regex TextRegex();

        [GeneratedRegex("[^%]>", regexOptions)]
        public partial Regex GTRegex();

        [GeneratedRegex("<[^%]", regexOptions)]
        public partial Regex LTRegex();

        [GeneratedRegex("<%(?![#$])(([^%]*)%)*?>", regexOptions)]
        public partial Regex ServerTagsRegex();

        [GeneratedRegex(@"runat\W*server", regexOptions | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        public partial Regex RunatServerRegex();

        [GeneratedRegex(directiveRegexString, regexOptions)]
        public partial Regex SimpleDirectiveRegex();

        [GeneratedRegex(@"\G\s*<%\s*?#(?<encode>:)?(?<code>.*?)?%>\s*\z", regexOptions)]
        public partial Regex DataBindRegex();

        [GeneratedRegex(@"\G\s*<%\s*\$\s*(?<code>.*)?%>\s*\z", regexOptions)]
        public partial Regex ExpressionBuilderRegex();

        [GeneratedRegex(@"^\s*bind\s*\((?<params>.*)\)\s*\z", regexOptions | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        public partial Regex BindExpressionRegex();

        [GeneratedRegex(@"\s*((""(?<fieldName>(([\w\.]+)|(\[.+\])))"")|('(?<fieldName>(([\w\.]+)|(\[.+\])))'))\s*(,\s*((""(?<formatString>.*)"")|('(?<formatString>.*)'))\s*)?\s*\z", regexOptions)]
        public partial Regex BindParametersRegex();

        [GeneratedRegex(@"^(([^""]*("""")?)*)$", regexOptions)]
        public partial Regex FormatStringRegex();

        [GeneratedRegex(@"<%\s*=\s*WebResource\(""(?<resourceName>[^""]*)""\)\s*%>", regexOptions)]
        public partial Regex WebResourceRegex();

        [GeneratedRegex(@"\W", regexOptions)]
        public partial Regex NonWordRegex();

        [GeneratedRegex(@"^\s*eval\s*\((?<params>.*)\)\s*\z", regexOptions | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        public partial Regex EvalExpressionRegex();
        
        [GeneratedRegex(@"\$(?:\{(?<name>\w+)\})", regexOptions)]
        public partial Regex BrowserCapsRefRegex();

        [GeneratedRegex(@"\G<%:(?<code>.*?)?%>", regexOptions)]
        public partial Regex AspEncodedExprRegex();

		// Note that this string is slightly different from TagRegex above
		// at the "=bar attribute" line. 
		// The 3.5 regex is used only when targeting 2.0/3.5 for 
		// backward compatibility (Dev10 bug 830783).
        [GeneratedRegex(@"\G<(?<tagname>[\w:\.]+)" +
                                                  @"(" +
                                                  @"\s+(?<attrname>\w[-\w:]*)(" +       // Attribute name
                                                  @"\s*=\s*""(?<attrval>[^""]*)""|" +   // ="bar" attribute value
                                                  @"\s*=\s*'(?<attrval>[^']*)'|" +      // ='bar' attribute value
                                                  @"\s*=\s*(?<attrval><%#.*?%>)|" +     // =<%#expr%> attribute value
                                                  @"\s*=\s*(?<attrval>[^\s=/>]*)|" + // =bar attribute value
                                                  @"(?<attrval>\s*?)" +                 // no attrib value (with no '=')
                                                  @")" +
                                                  @")*" +
                                                  @"\s*(?<empty>/)?>",
                                            regexOptions)]
        public partial Regex TagRegex35();

        [GeneratedRegex(@"^\s*BindItem\.(?<params>.*)\s*\z", regexOptions | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
        public partial Regex BindItemExpressionRegex();

        [GeneratedRegex(@"(?<fieldName>([\w\.]+))\s*\z", regexOptions)]
        public partial Regex BindItemParametersRegex();

	}
}
#endif