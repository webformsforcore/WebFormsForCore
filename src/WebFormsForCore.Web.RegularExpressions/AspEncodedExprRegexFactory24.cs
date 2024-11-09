#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class AspEncodedExprRegexFactory24 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new AspEncodedExprRegexRunner24();
  }
}

#endif