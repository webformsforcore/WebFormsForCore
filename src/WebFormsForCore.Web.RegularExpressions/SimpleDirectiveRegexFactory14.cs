
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class SimpleDirectiveRegexFactory14 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance()
    {
      return (RegexRunner) new SimpleDirectiveRegexRunner14();
    }
  }
}
#endif