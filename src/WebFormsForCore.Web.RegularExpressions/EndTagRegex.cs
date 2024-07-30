// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.EndTagRegex
// Assembly: System.Web.RegularExpressions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2D011335-7300-40B5-8CDC-1E3EA0A75C61
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.RegularExpressions\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.RegularExpressions.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.RegularExpressions.xml

#if NETFRAMEWORK

using System.Collections;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  /// <summary>Provides a regular expression to parse an end tag of an HTML element, an XML element, or an ASP.NET web server control tag.</summary>
  public class EndTagRegex : Regex
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Web.RegularExpressions.EndTagRegex" /> class.</summary>
    public EndTagRegex()
    {
      this.pattern = "\\G</(?<tagname>[\\w:\\.]+)\\s*>";
      this.roptions = RegexOptions.Multiline | RegexOptions.Singleline;
      this.internalMatchTimeout = TimeSpan.FromTicks(-10000L);
      this.factory = (RegexRunnerFactory) new EndTagRegexFactory3();
      this.capnames = new Hashtable();
      this.capnames.Add((object) "0", (object) 0);
      this.capnames.Add((object) "tagname", (object) 1);
      this.capslist = new string[2];
      this.capslist[0] = "0";
      this.capslist[1] = "tagname";
      this.capsize = 2;
      this.InitializeReferences();
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Web.RegularExpressions.EndTagRegex" /> class with a specified time-out value.</summary>
    /// <param name="A_1">A time-out interval, or <see cref="F:System.Text.RegularExpressions.Regex.InfiniteMatchTimeout" /> if matching operations should not time out.</param>
    public EndTagRegex(TimeSpan A_1)
      : this()
    {
      Regex.ValidateMatchTimeout(A_1);
      this.internalMatchTimeout = A_1;
    }
  }
}
#endif