
using System;

#nullable disable
namespace Microsoft.ReportingServices.Common
{
  internal static class UrlUtil
  {
    public static string UrlEncode(string input)
    {
      return input == null ? (string) null : Uri.EscapeDataString(input);
    }

    public static string UrlDecode(string input)
    {
      if (input == null)
        return (string) null;
      input = input.Replace("+", " ");
      return Uri.UnescapeDataString(input);
    }
  }
}
