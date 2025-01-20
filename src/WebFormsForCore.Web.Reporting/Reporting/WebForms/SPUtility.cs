
using System.Text.RegularExpressions;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class SPUtility
  {
    public static bool IsValidStringInput(string regexp, string newValue)
    {
      Regex regex = new Regex(regexp);
      return !string.IsNullOrEmpty(newValue) && regex.IsMatch(newValue);
    }
  }
}
