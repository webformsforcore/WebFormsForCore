// Decompiled with JetBrains decompiler
// Type: System.Web.RegularExpressions.ExpressionBuilderRegex
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
  internal class ExpressionBuilderRegex : Regex
  {
    public ExpressionBuilderRegex()
    {
      this.pattern = "\\G\\s*<%\\s*\\$\\s*(?<code>.*)?%>\\s*\\z";
      this.roptions = RegexOptions.Multiline | RegexOptions.Singleline;
      this.internalMatchTimeout = TimeSpan.FromTicks(-10000L);
      this.factory = (RegexRunnerFactory) new ExpressionBuilderRegexFactory16();
      this.capnames = new Hashtable();
      this.capnames.Add((object) "0", (object) 0);
      this.capnames.Add((object) "code", (object) 1);
      this.capslist = new string[2];
      this.capslist[0] = "0";
      this.capslist[1] = "code";
      this.capsize = 2;
      this.InitializeReferences();
    }

    public ExpressionBuilderRegex(TimeSpan A_1)
      : this()
    {
      Regex.ValidateMatchTimeout(A_1);
      this.internalMatchTimeout = A_1;
    }
  }
}
#endif