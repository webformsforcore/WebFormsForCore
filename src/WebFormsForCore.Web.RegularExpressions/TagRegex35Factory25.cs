
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class TagRegex35Factory25 : RegexRunnerFactory
  {
    protected override RegexRunner CreateInstance() => (RegexRunner) new TagRegex35Runner25();
  }
}
#endif