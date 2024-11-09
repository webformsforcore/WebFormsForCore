
#if NETFRAMEWORK

using System.Text.RegularExpressions;

#nullable disable
namespace System.Web.RegularExpressions
{
  internal class FormatStringRegex : Regex
  {
    public FormatStringRegex()
    {
      this.pattern = "^(([^\"]*(\"\")?)*)$";
      this.roptions = RegexOptions.Multiline | RegexOptions.Singleline;
      this.internalMatchTimeout = TimeSpan.FromTicks(-10000L);
      this.factory = (RegexRunnerFactory) new FormatStringRegexFactory19();
      this.capsize = 4;
      this.InitializeReferences();
    }

    public FormatStringRegex(TimeSpan A_1)
      : this()
    {
      Regex.ValidateMatchTimeout(A_1);
      this.internalMatchTimeout = A_1;
    }
  }
}
#endif