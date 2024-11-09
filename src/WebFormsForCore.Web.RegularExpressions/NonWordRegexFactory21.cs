
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class NonWordRegexFactory21 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new NonWordRegexRunner21();
  }
}
#endif