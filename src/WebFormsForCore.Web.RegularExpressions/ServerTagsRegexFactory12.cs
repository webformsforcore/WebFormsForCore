
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class ServerTagsRegexFactory12 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new ServerTagsRegexRunner12();
  }
}
#endif