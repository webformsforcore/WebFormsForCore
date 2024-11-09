
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class EndTagRegexFactory3 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new EndTagRegexRunner3();
  }
}
#endif