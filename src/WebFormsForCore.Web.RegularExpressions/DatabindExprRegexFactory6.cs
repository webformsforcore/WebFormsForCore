#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class DatabindExprRegexFactory6 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new DatabindExprRegexRunner6();
  }
}
#endif