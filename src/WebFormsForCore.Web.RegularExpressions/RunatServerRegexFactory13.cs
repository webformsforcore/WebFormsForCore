
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class RunatServerRegexFactory13 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new RunatServerRegexRunner13();
  }
}
#endif