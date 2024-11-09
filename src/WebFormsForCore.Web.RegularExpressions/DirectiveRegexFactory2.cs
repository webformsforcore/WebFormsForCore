
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class DirectiveRegexFactory2 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new DirectiveRegexRunner2();
  }
}
#endif