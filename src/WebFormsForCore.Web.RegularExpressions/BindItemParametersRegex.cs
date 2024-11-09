#if NETFRAMEWORK

using System.Collections;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindItemParametersRegex : Regex
  {
    public BindItemParametersRegex()
    {
      this.pattern = "(?<fieldName>([\\w\\.]+))\\s*\\z";
      this.roptions = RegexOptions.Multiline | RegexOptions.Singleline;
      this.internalMatchTimeout = TimeSpan.FromTicks(-10000L);
      this.factory = (RegexRunnerFactory) new BindItemParametersRegexFactory27();
      this.capnames = new Hashtable();
      this.capnames.Add((object) "fieldName", (object) 2);
      this.capnames.Add((object) "0", (object) 0);
      this.capnames.Add((object) "1", (object) 1);
      this.capslist = new string[3];
      this.capslist[0] = "0";
      this.capslist[1] = "1";
      this.capslist[2] = "fieldName";
      this.capsize = 3;
      this.InitializeReferences();
    }

    public BindItemParametersRegex(TimeSpan A_1)
      : this()
    {
      Regex.ValidateMatchTimeout(A_1);
      this.internalMatchTimeout = A_1;
    }
  }
}

#endif