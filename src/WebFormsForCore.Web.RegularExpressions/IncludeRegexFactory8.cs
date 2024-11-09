
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class IncludeRegexFactory8 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new IncludeRegexRunner8();
  }
}
#endif