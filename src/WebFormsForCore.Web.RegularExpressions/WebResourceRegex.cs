
#if NETFRAMEWORK

using System.Collections;
using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class WebResourceRegex : Regex
  {
    public WebResourceRegex()
    {
      this.pattern = "<%\\s*=\\s*WebResource\\(\"(?<resourceName>[^\"]*)\"\\)\\s*%>";
      this.roptions = RegexOptions.Multiline | RegexOptions.Singleline;
      this.internalMatchTimeout = TimeSpan.FromTicks(-10000L);
      this.factory = (RegexRunnerFactory) new WebResourceRegexFactory20();
      this.capnames = new Hashtable();
      this.capnames.Add((object) "resourceName", (object) 1);
      this.capnames.Add((object) "0", (object) 0);
      this.capslist = new string[2];
      this.capslist[0] = "0";
      this.capslist[1] = "resourceName";
      this.capsize = 2;
      this.InitializeReferences();
    }

    public WebResourceRegex(TimeSpan A_1)
      : this()
    {
      Regex.ValidateMatchTimeout(A_1);
      this.internalMatchTimeout = A_1;
    }
  }
}
#endif