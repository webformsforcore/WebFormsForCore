
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class GTRegexFactory10 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new GTRegexRunner10();
  }
}
#endif