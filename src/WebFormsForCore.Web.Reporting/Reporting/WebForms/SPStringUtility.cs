
using System.Text.RegularExpressions;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class SPStringUtility
  {
    public static string RemoveNonAlphaNumericChars(string value)
    {
      return value == null || value.Length == 0 ? value : Regex.Replace(value, "[^a-zA-Z_0-9]+", string.Empty, RegexOptions.Compiled);
    }
  }
}
