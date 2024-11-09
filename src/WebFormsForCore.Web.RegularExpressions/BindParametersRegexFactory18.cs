#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class BindParametersRegexFactory18 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new BindParametersRegexRunner18();
  }
}

#endif