
#if NETFRAMEWORK

using System.Collections;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  /// <summary>Provides a regular expression to parse the opening tag of an HTML element, an XML element, or an ASP.NET Web server control tag.</summary>
  public class TagRegex : Regex
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Web.RegularExpressions.TagRegex" /> class.</summary>
    public TagRegex()
    {
      this.pattern = "\\G<(?<tagname>[\\w:\\.]+)(\\s+(?<attrname>\\w[-\\w:]*)(\\s*=\\s*\"(?<attrval>[^\"]*)\"|\\s*=\\s*'(?<attrval>[^']*)'|\\s*=\\s*(?<attrval><%#.*?%>)|\\s*=\\s*(?<attrval>[^\\s=\"'/>]*)|(?<attrval>\\s*?)))*\\s*(?<empty>/)?>";
      this.roptions = RegexOptions.Multiline | RegexOptions.Singleline;
      this.internalMatchTimeout = TimeSpan.FromTicks(-10000L);
      this.factory = (RegexRunnerFactory) new TagRegexFactory1();
      this.capnames = new Hashtable();
      this.capnames.Add((object) "empty", (object) 6);
      this.capnames.Add((object) "attrval", (object) 5);
      this.capnames.Add((object) "tagname", (object) 3);
      this.capnames.Add((object) "attrname", (object) 4);
      this.capnames.Add((object) "2", (object) 2);
      this.capnames.Add((object) "1", (object) 1);
      this.capnames.Add((object) "0", (object) 0);
      this.capslist = new string[7];
      this.capslist[0] = "0";
      this.capslist[1] = "1";
      this.capslist[2] = "2";
      this.capslist[3] = "tagname";
      this.capslist[4] = "attrname";
      this.capslist[5] = "attrval";
      this.capslist[6] = "empty";
      this.capsize = 7;
      this.InitializeReferences();
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.RegularExpressions.TagRegex" /> class with a specified time-out value.</summary>
    /// <param name="A_1">A time-out interval, or <see cref="F:System.Text.RegularExpressions.Regex.InfiniteMatchTimeout" /> if matching operations should not time out.</param>
    public TagRegex(TimeSpan A_1)
      : this()
    {
      Regex.ValidateMatchTimeout(A_1);
      this.internalMatchTimeout = A_1;
    }
  }
}
#endif