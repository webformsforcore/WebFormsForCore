#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindExpressionRegexFactory17 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new BindExpressionRegexRunner17();
  }
}

#endif