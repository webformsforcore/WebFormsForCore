
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class TextRegexFactory9 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new TextRegexRunner9();
  }
}
#endif