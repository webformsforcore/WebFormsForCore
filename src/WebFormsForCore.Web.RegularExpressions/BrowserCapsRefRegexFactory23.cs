#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BrowserCapsRefRegexFactory23 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new BrowserCapsRefRegexRunner23();
  }
}

#endif