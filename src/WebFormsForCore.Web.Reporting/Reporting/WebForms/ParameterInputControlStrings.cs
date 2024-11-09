// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ParameterInputControlStrings
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Globalization;
using System.Runtime.CompilerServices;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [CompilerGenerated]
  internal class ParameterInputControlStrings
  {
    protected ParameterInputControlStrings()
    {
    }

    public static CultureInfo Culture
    {
      get => ParameterInputControlStrings.Keys.Culture;
      set => ParameterInputControlStrings.Keys.Culture = value;
    }

    public static string True => ParameterInputControlStrings.Keys.GetString(nameof (True));

    public static string False => ParameterInputControlStrings.Keys.GetString(nameof (False));

    public static string NullCheckBox
    {
      get => ParameterInputControlStrings.Keys.GetString(nameof (NullCheckBox));
    }

    public static string NullValue
    {
      get => ParameterInputControlStrings.Keys.GetString(nameof (NullValue));
    }

    public static string SelectValidValue
    {
      get => ParameterInputControlStrings.Keys.GetString(nameof (SelectValidValue));
    }

    public static string TodayIs => ParameterInputControlStrings.Keys.GetString(nameof (TodayIs));

    public static string NextMonthToolTip
    {
      get => ParameterInputControlStrings.Keys.GetString(nameof (NextMonthToolTip));
    }

    public static string PreviousMonthToolTip
    {
      get => ParameterInputControlStrings.Keys.GetString(nameof (PreviousMonthToolTip));
    }

    public static string SelectAll
    {
      get => ParameterInputControlStrings.Keys.GetString(nameof (SelectAll));
    }

    public static string DropDownTooltip
    {
      get => ParameterInputControlStrings.Keys.GetString(nameof (DropDownTooltip));
    }
  }
}
