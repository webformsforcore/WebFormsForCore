#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class AspCodeRegexFactory4 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new AspCodeRegexRunner4();
  }
}
#endif