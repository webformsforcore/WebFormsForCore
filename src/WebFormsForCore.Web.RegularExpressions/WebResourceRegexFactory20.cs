
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class WebResourceRegexFactory20 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new WebResourceRegexRunner20();
  }
}
#endif