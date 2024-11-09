
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class EvalExpressionRegexFactory22 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new EvalExpressionRegexRunner22();
  }
}
#endif