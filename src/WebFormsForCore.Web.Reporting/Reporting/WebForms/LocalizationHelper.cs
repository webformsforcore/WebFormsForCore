// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.LocalizationHelper
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Globalization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class LocalizationHelper : 
    IReportViewerMessages3,
    IReportViewerMessages2,
    IReportViewerMessages
  {
    private static LocalizationHelper m_theInstance;
    private static object m_lockObject = new object();

    private LocalizationHelper()
    {
    }

    public static IReportViewerMessages3 Current
    {
      get
      {
        lock (LocalizationHelper.m_lockObject)
        {
          if (LocalizationHelper.m_theInstance == null)
            LocalizationHelper.m_theInstance = new LocalizationHelper();
          return (IReportViewerMessages3) LocalizationHelper.m_theInstance;
        }
      }
    }

    private static string GetLocalizedString(string builtinString, string customString)
    {
      return customString == null ? builtinString : customString;
    }

    private IReportViewerMessages2 ReportViewerMessages2
    {
      get => WebConfigReader.Current.ViewerMessages as IReportViewerMessages2;
    }

    private IReportViewerMessages3 ReportViewerMessages3
    {
      get => WebConfigReader.Current.ViewerMessages as IReportViewerMessages3;
    }

    string IReportViewerMessages.DocumentMapButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ShowHideDocMapTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.DocumentMapButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.ParameterAreaButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ShowHideParametersTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ParameterAreaButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.FirstPageButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.FirstPageTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.FirstPageButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.PreviousPageButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.PreviousPageTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.PreviousPageButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.CurrentPageTextBoxToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.CurrentPageTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.CurrentPageTextBoxToolTip : (string) null);
      }
    }

    string IReportViewerMessages.PageOf
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.Of, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.PageOf : (string) null);
      }
    }

    string IReportViewerMessages.NextPageButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.NextPageTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.NextPageButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.LastPageButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.LastPageTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.LastPageButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.BackButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.BackTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.BackButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.RefreshButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.RefreshTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.RefreshButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.PrintButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.PrintTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.PrintButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.ExportButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ExportButtonTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ExportButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.ZoomControlToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ZoomTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ZoomControlToolTip : (string) null);
      }
    }

    string IReportViewerMessages.SearchTextBoxToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.SearchTextBoxToolTip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.SearchTextBoxToolTip : (string) null);
      }
    }

    string IReportViewerMessages.FindButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.FindTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.FindButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.FindNextButtonToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.FindNextTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.FindNextButtonToolTip : (string) null);
      }
    }

    string IReportViewerMessages.ZoomToPageWidth
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.PageWidth, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ZoomToPageWidth : (string) null);
      }
    }

    string IReportViewerMessages.ZoomToWholePage
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.WholePage, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ZoomToWholePage : (string) null);
      }
    }

    string IReportViewerMessages.FindButtonText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.FindFirst, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.FindButtonText : (string) null);
      }
    }

    string IReportViewerMessages.FindNextButtonText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.FindNext, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.FindNextButtonText : (string) null);
      }
    }

    string IReportViewerMessages.ViewReportButtonText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ViewReport, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ViewReportButtonText : (string) null);
      }
    }

    string IReportViewerMessages.ProgressText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(CommonStrings.AsyncProgressText, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ProgressText : (string) null);
      }
    }

    string IReportViewerMessages.TextNotFound
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Errors.SearchNotFound, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.TextNotFound : (string) null);
      }
    }

    string IReportViewerMessages.NoMoreMatches
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Errors.SearchNextNotFound, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.NoMoreMatches : (string) null);
      }
    }

    string IReportViewerMessages.ChangeCredentialsText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ChangeCredentials, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ChangeCredentialsText : (string) null);
      }
    }

    string IReportViewerMessages.NullCheckBoxText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.NullCheckBox, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.NullCheckBoxText : (string) null);
      }
    }

    string IReportViewerMessages.NullValueText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.NullValue, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.NullValueText : (string) null);
      }
    }

    string IReportViewerMessages.TrueValueText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.True, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.TrueValueText : (string) null);
      }
    }

    string IReportViewerMessages.FalseValueText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.False, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.FalseValueText : (string) null);
      }
    }

    string IReportViewerMessages.SelectAValue
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.SelectValidValue, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.SelectAValue : (string) null);
      }
    }

    string IReportViewerMessages.UserNamePrompt
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.UserName, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.UserNamePrompt : (string) null);
      }
    }

    string IReportViewerMessages.PasswordPrompt
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.Password, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.PasswordPrompt : (string) null);
      }
    }

    string IReportViewerMessages.SelectAll
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.SelectAll, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.SelectAll : (string) null);
      }
    }

    string IReportViewerMessages.TodayIs
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.TodayIs, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.TodayIs : (string) null);
      }
    }

    string IReportViewerMessages.ExportFormatsToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ExportFormatsTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ExportFormatsToolTip : (string) null);
      }
    }

    string IReportViewerMessages.ExportButtonText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ExportButton, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ExportButtonText : (string) null);
      }
    }

    string IReportViewerMessages.SelectFormat
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.SelectFormat, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.SelectFormat : (string) null);
      }
    }

    string IReportViewerMessages.DocumentMap
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.DocumentMap, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.DocumentMap : (string) null);
      }
    }

    string IReportViewerMessages.InvalidPageNumber
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Errors.PageNumberInvalid, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.InvalidPageNumber : (string) null);
      }
    }

    string IReportViewerMessages.ChangeCredentialsToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.ChangeCredentialsTooltip, WebConfigReader.Current.ViewerMessages != null ? WebConfigReader.Current.ViewerMessages.ChangeCredentialsToolTip : (string) null);
      }
    }

    string IReportViewerMessages2.GetLocalizedNameForRenderingExtension(string format)
    {
      throw new NotImplementedException();
    }

    public string GetLocalizedNameForRenderingExtension(RenderingExtension ext)
    {
      return LocalizationHelper.GetLocalizedString(ext.LocalizedName, this.ReportViewerMessages2 != null ? this.ReportViewerMessages2.GetLocalizedNameForRenderingExtension(ext.Name) : (string) null);
    }

    public string ParameterMissingSelectionError(string parameterPrompt)
    {
      // ISSUE: reference to a compiler-generated method
      return LocalizationHelper.GetLocalizedString(Errors.MissingSelectionClientError(parameterPrompt), this.ReportViewerMessages2 != null ? this.ReportViewerMessages2.ParameterMissingSelectionError(parameterPrompt) : (string) null);
    }

    public string ParameterMissingValueError(string parameterPrompt)
    {
      // ISSUE: reference to a compiler-generated method
      return LocalizationHelper.GetLocalizedString(Errors.MissingValueClientError(parameterPrompt), this.ReportViewerMessages2 != null ? this.ReportViewerMessages2.ParameterMissingValueError(parameterPrompt) : (string) null);
    }

    public string CredentialMissingUserNameError(string dataSourcePrompt)
    {
      return LocalizationHelper.GetLocalizedString(Errors.MissingCredentials, this.ReportViewerMessages2 != null ? this.ReportViewerMessages2.CredentialMissingUserNameError(dataSourcePrompt) : (string) null);
    }

    public string ClientNoScript
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.NoScript, this.ReportViewerMessages2 != null ? this.ReportViewerMessages2.ClientNoScript : (string) null);
      }
    }

    public string ClientPrintControlLoadFailed
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.CantLoadPrintControl, this.ReportViewerMessages2 != null ? this.ReportViewerMessages2.ClientPrintControlLoadFailed : (string) null);
      }
    }

    public string ParameterDropDownToolTip
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(ParameterInputControlStrings.DropDownTooltip, this.ReportViewerMessages2 != null ? this.ReportViewerMessages2.ParameterDropDownToolTip : (string) null);
      }
    }

    string IReportViewerMessages3.TotalPages(int pageCount, PageCountMode pageCountMode)
    {
      // ISSUE: reference to a compiler-generated method
      return LocalizationHelper.GetLocalizedString(pageCountMode == PageCountMode.Actual || pageCount <= 0 ? pageCount.ToString((IFormatProvider) CultureInfo.CurrentCulture) : CommonStrings.EstimateTotalPages(pageCount), this.ReportViewerMessages3 != null ? this.ReportViewerMessages3.TotalPages(pageCount, pageCountMode) : (string) null);
    }

    public string CancelLinkText
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(CommonStrings.CancelLinkText, this.ReportViewerMessages3 != null ? this.ReportViewerMessages3.CancelLinkText : (string) null);
      }
    }

    public string CalendarLoading
    {
      get
      {
        return LocalizationHelper.GetLocalizedString(Strings.Loading, this.ReportViewerMessages3 != null ? this.ReportViewerMessages3.CalendarLoading : (string) null);
      }
    }
  }
}
