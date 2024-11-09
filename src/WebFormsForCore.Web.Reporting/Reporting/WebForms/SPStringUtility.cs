// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.SPStringUtility
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
