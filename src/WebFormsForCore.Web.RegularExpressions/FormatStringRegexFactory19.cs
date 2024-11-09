
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class FormatStringRegexFactory19 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new FormatStringRegexRunner19();
  }
}
#endif