
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class TagRegexFactory1 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new TagRegexRunner1();
  }
}
#endif