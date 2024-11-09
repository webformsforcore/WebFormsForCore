#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class CommentRegexFactory7 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new CommentRegexRunner7();
  }
}
#endif