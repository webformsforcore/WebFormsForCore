// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ParametersArea
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ParametersArea : CompositeControl, IScriptControl
  {
    private const int ParameterColumns = 2;
    protected const string ParameterRowAttribute = "IsParameterRow";
    public bool ShowCredentialPrompts = true;
    public bool ShowParameterPrompts = true;
    private ReportDataSourceInfoCollection m_dsInfos;
    private ReportParameterInfoCollection m_paramsInfo;
    private bool m_allCredentialsSatisfied;
    private ParameterControlCollection m_paramControls;
    private DataSourceControlCollection m_dsControls;
    private Dictionary<BaseParameterInputControl, SafeLiteralControl> m_paramPrompts = new Dictionary<BaseParameterInputControl, SafeLiteralControl>();
    private Button m_viewReportButton;
    private TextButton m_changeCredentials;
    private bool m_credentialsChanged;
    private string m_autoPostBackTarget;
    private bool m_showHiddenParameters;
    private bool m_positioningMode = true;
    private string m_innerCssClass;
    private ReportViewer m_viewer;
    private IReportViewerStyles m_styles;
    private IParameterSupplier m_parameterSupplier;

    public ParametersArea(IReportViewerStyles styles) => this.m_styles = styles;

    public ParametersArea(
      IReportViewerStyles styles,
      bool showHiddenParameters,
      bool positioningMode)
      : this(styles)
    {
      this.m_showHiddenParameters = showHiddenParameters;
      this.m_positioningMode = positioningMode;
    }

    public ParametersArea(ReportViewer viewer, bool showHiddenParameters, bool positioningMode)
      : this(viewer)
    {
      this.m_showHiddenParameters = showHiddenParameters;
      this.m_positioningMode = positioningMode;
    }

    public ParametersArea(ReportViewer viewer)
      : this(viewer.ViewerStyle)
    {
      this.m_viewer = viewer;
      this.Error += new ReportErrorEventHandler(this.m_viewer.OnError);
    }

    public event EventHandler ViewReportClick;

    public event EventHandler ParameterValuesChanged;

    public event EventHandler AutoPostBackOccurred;

    public event ReportErrorEventHandler Error;

    public event ReportCredentialsEventHandler SubmittingDataSourceCredentials;

    public event ReportParametersEventHandler SubmittingParameterValues;

    protected virtual IParameterSupplier GetParameterSupplier()
    {
      return this.m_viewer != null ? (IParameterSupplier) new ReportParameterSupplier(this.m_viewer.Report) : throw new NotImplementedException();
    }

    protected bool OnError(Exception e)
    {
      if (this.Error == null)
        return false;
      this.Error((object) this, new ReportErrorEventArgs(e));
      return true;
    }

    protected override void CreateChildControls()
    {
      try
      {
        this.Controls.Clear();
        this.m_paramControls = (ParameterControlCollection) null;
        this.m_dsControls = (DataSourceControlCollection) null;
        this.m_paramPrompts.Clear();
        this.m_viewReportButton = (Button) new ViewReportButton();
        this.m_viewReportButton.Text = LocalizationHelper.Current.ViewReportButtonText;
        this.m_viewReportButton.Click += new EventHandler(this.OnViewReport);
        this.Controls.Add((Control) this.m_viewReportButton);
        this.m_changeCredentials = new TextButton(LocalizationHelper.Current.ChangeCredentialsText, LocalizationHelper.Current.ChangeCredentialsToolTip, this.m_styles);
        this.m_changeCredentials.ShowDisabled = false;
        this.Controls.Add((Control) this.m_changeCredentials);
        if (Global.IsDesignTime)
          return;
        if (this.m_paramsInfo == null)
          this.RefreshControlsFromReportMetadata();
        if (this.m_dsInfos != null && this.m_dsInfos.Count > 0)
        {
          this.m_dsControls = DataSourceControlCollection.Create(this.m_dsInfos, new DataSourceControl.CredentialRenderer(this.RenderOneDataSource), this.m_styles);
          foreach (DataSourceControl dsControl in (List<DataSourceControl>) this.m_dsControls)
          {
            dsControl.ValueChanged += new EventHandler(this.OnCredentialsChanged);
            this.Controls.Add((Control) dsControl);
          }
        }
        if (this.m_paramsInfo == null || this.m_paramsInfo.Count <= 0)
          return;
        this.m_paramControls = ParameterControlCollection.Create(this.m_paramsInfo, this.m_parameterSupplier.IsQueryExecutionAllowed, this.m_styles, this.m_showHiddenParameters, this.m_positioningMode);
        foreach (BaseParameterInputControl parameterControl in this.ParameterControls)
        {
          SafeLiteralControl child = new SafeLiteralControl(parameterControl.Prompt, parameterControl.Disabled);
          this.Controls.Add((Control) child);
          this.m_paramPrompts.Add(parameterControl, child);
          parameterControl.ValueChanged += new EventHandler(this.OnParametersChanged);
          parameterControl.AutoPostBackOccurred += new EventHandler(this.OnAutoPostBackOccurred);
          this.CreateRenderedParameterControl(parameterControl);
        }
      }
      catch (Exception ex)
      {
        if (this.OnError(ex))
          return;
        throw;
      }
    }

    protected virtual void CreateRenderedParameterControl(BaseParameterInputControl parameterControl)
    {
      this.Controls.Add((Control) parameterControl);
    }

    protected void ClearChildControls(bool clearParameterMetaData)
    {
      if (clearParameterMetaData)
      {
        this.m_paramsInfo = (ReportParameterInfoCollection) null;
        this.m_dsInfos = (ReportDataSourceInfoCollection) null;
        this.m_allCredentialsSatisfied = false;
        this.m_parameterSupplier = (IParameterSupplier) null;
      }
      if (!this.ChildControlsCreated)
        return;
      this.m_paramControls = (ParameterControlCollection) null;
      this.m_dsControls = (DataSourceControlCollection) null;
      this.ClearChildState();
      this.ChildControlsCreated = false;
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      if (this.m_viewer != null && !this.m_viewer.ViewerStyle.GetFontFromCss)
        this.Font.CopyFrom(this.m_viewer.ViewerStyle.Font);
      if (this.m_viewer != null)
      {
        this.ShowParameterPrompts = this.m_viewer.ShowParameterPrompts;
        this.ShowCredentialPrompts = this.m_viewer.ShowCredentialPrompts;
      }
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<ParametersArea>(this);
      this.m_changeCredentials.ClientSideClickScript = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if ($get('{0}') != null) $get('{0}').control.OnChangeCredentialsClick();", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
    }

    public void RenderCloseDropDownAttributes(HtmlTextWriter writer)
    {
      writer.AddAttribute("onclick", this.CloseDropDownJavaFunctionCall);
      writer.AddAttribute("onactivate", this.CloseDropDownJavaFunctionCall);
    }

    public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._PromptArea", this.ClientID);
      if (this.m_viewer != null)
        controlDescriptor.AddProperty("ReportViewerId", (object) this.m_viewer.ClientID);
      controlDescriptor.AddProperty("CredentialsLinkId", (object) this.m_changeCredentials.ClientID);
      controlDescriptor.AddProperty("ParametersGridID", (object) this.ParametersGridID);
      controlDescriptor.AddProperty("ViewReportButtonId", (object) this.m_viewReportButton.ClientID);
      if (this.m_paramControls != null && this.RenderParameters)
      {
        List<string> stringList = new List<string>();
        foreach (BaseParameterInputControl parameterInputControl in this.m_paramControls.Values)
        {
          if (!parameterInputControl.Disabled)
            stringList.Add(parameterInputControl.ClientID);
        }
        controlDescriptor.AddProperty("ParameterIdList", (object) stringList.ToArray());
      }
      if (this.m_dsControls != null && this.ShouldRenderCredentialsIfSupported)
        controlDescriptor.AddProperty("CredentialIdList", (object) this.m_dsControls.DataSourceClientIds);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }

    public IEnumerable<ScriptReference> GetScriptReferences()
    {
      ScriptReference scriptReference = new ScriptReference();
      scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        scriptReference
      };
    }

    public string InnerCssClass
    {
      get => this.m_innerCssClass;
      set => this.m_innerCssClass = value;
    }

    private string CloseDropDownJavaFunctionCall
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if ($get('{0}') != null && $get('{0}').control != null) $get('{0}').control.HideActiveDropDown();", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
      }
    }

    public void AddCloseDropDownAttributes(HtmlControl control)
    {
      control.Attributes.Add("onclick", this.CloseDropDownJavaFunctionCall);
      control.Attributes.Add("onactivate", this.CloseDropDownJavaFunctionCall);
    }

    protected virtual string ParametersGridID => "ParametersGrid" + this.ClientID;

    protected string ParameterTableID => "ParameterTable_" + this.ClientID;

    protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      this.AddAttributesToRender(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      if (!string.IsNullOrEmpty(this.InnerCssClass))
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.InnerCssClass);
      this.RenderCloseDropDownAttributes(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Div);
      this.RenderChildren(writer);
      if (BrowserDetection.Current.IsIE)
        writer.Write("<input type=\"text\" style=\"visibility:hidden;display:none\" disabled=\"disabled\"></input>");
      writer.RenderEndTag();
      writer.RenderEndTag();
      this.RenderAbsolutePositionedChildren(writer);
    }

    protected override void RenderChildren(HtmlTextWriter writer)
    {
      writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
      writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
      writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
      writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ParameterTableID);
      writer.AddAttribute(HtmlTextWriterAttribute.Name, this.ParameterTableID);
      if (this.m_styles.ParameterContainer != null)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.ParameterContainer);
      }
      else
      {
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "5px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "5px");
        writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "10px");
        if (!this.m_styles.BackColor.IsEmpty)
          writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(this.m_styles.BackColor));
        this.m_styles.AddInternalBorderAttributes(writer, "border-bottom");
      }
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
      writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      this.RenderParameterControls(writer);
      writer.RenderEndTag();
      writer.AddAttribute(HtmlTextWriterAttribute.Width, "6px");
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      writer.RenderEndTag();
      if (this.m_styles.ViewReportContainer != null)
      {
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.ViewReportContainer);
      }
      else
      {
        this.m_styles.AddInternalBorderAttributes(writer, "border-left");
        writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
        writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "10px");
      }
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      this.RenderViewReportCell(writer);
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    private void RenderAbsolutePositionedChildren(HtmlTextWriter writer)
    {
      if (!this.RenderParameters || this.m_paramControls == null || !this.m_positioningMode)
        return;
      foreach (BaseParameterInputControl parameterControl in this.ParameterControls)
      {
        if (parameterControl is GenericDropDownInputControl downInputControl)
          downInputControl.RenderAbsolutePositionedControls(writer);
      }
    }

    protected string PromptValidationScript
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$get('{0}').control.ValidateInputs()", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
      }
    }

    private void RenderViewReportCell(HtmlTextWriter writer)
    {
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      this.m_viewReportButton.Font.CopyFrom(this.Font);
      this.m_viewReportButton.RenderControl(writer);
      writer.RenderEndTag();
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    protected override void LoadViewState(object savedState)
    {
      try
      {
        base.LoadViewState(savedState);
        this.PostLoadViewState();
      }
      catch (Exception ex)
      {
        if (this.OnError(ex))
          return;
        throw;
      }
    }

    protected virtual void PostLoadViewState()
    {
      if (this.ViewState["HasControls"] == null || !(bool) this.ViewState["HasControls"])
        return;
      this.RefreshControlsFromReportMetadata();
    }

    protected override object SaveViewState()
    {
      if (this.Visible)
      {
        this.EnsureChildControls();
        this.ViewState["HasControls"] = (object) (bool) (this.m_dsControls != null ? 1 : (this.m_paramControls != null ? 1 : 0));
      }
      return base.SaveViewState();
    }

    private void OnViewReport(object sender, EventArgs e)
    {
      if (this.ViewReportClick == null)
        return;
      this.ViewReportClick((object) this, e);
    }

    protected bool ShouldRenderCredentialsIfSupported
    {
      get => this.ShowCredentialPrompts && this.HasCredentials;
    }

    protected bool RenderCredentialsHidden
    {
      get => this.ShouldRenderCredentialsIfSupported && this.m_allCredentialsSatisfied;
    }

    protected bool RenderParameters
    {
      get
      {
        return (!this.ShouldRenderCredentialsIfSupported || this.RenderCredentialsHidden) && this.HasVisibleParameters && this.ShowParameterPrompts;
      }
    }

    public bool HasVisibleParameters
    {
      get
      {
        this.EnsureChildControls();
        return this.m_paramControls != null && this.m_paramControls.Count > 0;
      }
    }

    public bool HasCredentials
    {
      get
      {
        this.EnsureChildControls();
        return this.m_dsControls != null && this.m_dsControls.Count > 0;
      }
    }

    public void EnsureReportMetaData()
    {
      if (this.ChildControlsCreated)
        return;
      this.GetMetaDataFromReport();
    }

    public void RefreshControlsFromReportMetadata()
    {
      this.ClearChildControls(true);
      this.GetMetaDataFromReport();
    }

    private void GetMetaDataFromReport()
    {
      this.m_parameterSupplier = this.GetParameterSupplier();
      if (!this.m_parameterSupplier.IsReadyForConnection)
        return;
      if (this.m_paramsInfo == null)
        this.m_paramsInfo = this.m_parameterSupplier.GetParameters();
      this.m_dsInfos = this.m_parameterSupplier.GetDataSources(out this.m_allCredentialsSatisfied);
    }

    public bool SaveControlValuesToReport()
    {
      this.EnsureChildControls();
      bool report = true;
      if (!this.SaveCredentialsToReport())
      {
        ReportParameterCollection paramValues;
        IEnumerable<string> autoSubmitParamNames;
        report = this.GetParameterValuesToSubmit(out paramValues, out autoSubmitParamNames);
        bool flag = true;
        if (this.SubmittingParameterValues != null)
        {
          ReportParametersEventArgs e = new ReportParametersEventArgs(paramValues, this.AutoPostBackTarget != null);
          this.SubmittingParameterValues((object) this, e);
          if (e.Cancel)
            flag = false;
        }
        if (paramValues.Count > 0 && flag)
          this.SaveParametersToReport((IEnumerable<ReportParameter>) paramValues, autoSubmitParamNames);
      }
      this.ClearChildControls(true);
      return report;
    }

    protected string AutoPostBackTarget => this.m_autoPostBackTarget;

    protected ReportParameterInfoCollection SaveParametersToReport(
      IEnumerable<ReportParameter> parameters,
      IEnumerable<string> autoSubmitParameterNames)
    {
      this.m_parameterSupplier.SetParameters(parameters);
      ReportParameterInfoCollection parameters1 = this.m_parameterSupplier.GetParameters();
      if (autoSubmitParameterNames != null)
      {
        foreach (string submitParameterName in autoSubmitParameterNames)
        {
          if (parameters1[submitParameterName].HasUnsatisfiedDownstreamParametersWithDefaults)
          {
            this.m_parameterSupplier.SetParameters((IEnumerable<ReportParameter>) new ReportParameter[0]);
            break;
          }
        }
      }
      return parameters1;
    }

    protected bool GetParameterValuesToSubmit(
      out ReportParameterCollection paramValues,
      out IEnumerable<string> autoSubmitParamNames)
    {
      paramValues = new ReportParameterCollection();
      autoSubmitParamNames = (IEnumerable<string>) new string[0];
      bool parameterValuesToSubmit = true;
      if (this.ParameterControls != null)
      {
        foreach (BaseParameterInputControl parameterControl in this.ParameterControls)
        {
          if (!parameterControl.Disabled && parameterControl.CurrentValue != null)
          {
            ReportParameter reportParameter = new ReportParameter();
            reportParameter.Name = parameterControl.ReportParameter.Name;
            reportParameter.Values.AddRange(parameterControl.CurrentValue);
            paramValues.Add(reportParameter);
          }
          else
            parameterValuesToSubmit = false;
        }
        if (this.AutoPostBackTarget != null)
          autoSubmitParamNames = (IEnumerable<string>) new string[1]
          {
            this.AutoPostBackTarget
          };
      }
      return parameterValuesToSubmit;
    }

    protected bool SaveCredentialsToReport()
    {
      if (this.m_dsControls == null || !this.m_credentialsChanged)
        return false;
      DataSourceCredentialsCollection credentials = new DataSourceCredentialsCollection();
      foreach (DataSourceControl dsControl in (List<DataSourceControl>) this.m_dsControls)
        credentials.Add(new DataSourceCredentials()
        {
          Name = dsControl.DataSourceInfo.Name,
          UserId = dsControl.UserInput.Text,
          Password = dsControl.PasswordInput.Text
        });
      if (this.SubmittingDataSourceCredentials != null)
      {
        ReportCredentialsEventArgs e = new ReportCredentialsEventArgs(credentials);
        this.SubmittingDataSourceCredentials((object) this, e);
        if (e.Cancel || e.Credentials.Count == 0)
          return true;
      }
      this.m_parameterSupplier.SetDataSourceCredentials(credentials);
      return true;
    }

    private void RenderParameterControls(HtmlTextWriter writer)
    {
      writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ParametersGridID);
      writer.RenderBeginTag(HtmlTextWriterTag.Table);
      if (this.RenderCredentialsHidden)
      {
        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
        writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "5");
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        this.ChangeCredentialsLink.Font.CopyFrom(this.Font);
        this.ChangeCredentialsLink.RenderControl(writer);
        writer.RenderEndTag();
        writer.RenderEndTag();
      }
      if (this.ShouldRenderCredentialsIfSupported)
      {
        foreach (Control dsControl in (List<DataSourceControl>) this.m_dsControls)
          dsControl.RenderControl(writer);
      }
      if (this.RenderParameters)
      {
        int num = 0;
        bool flag = false;
        foreach (BaseParameterInputControl parameterControl in this.ParameterControls)
        {
          if (num == 0)
          {
            writer.AddAttribute("IsParameterRow", "true");
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            flag = true;
          }
          else
            this.AddSpacerCell(writer);
          this.RenderOneParameter(writer, parameterControl);
          num = (num + 1) % 2;
          if (num == 0)
          {
            writer.RenderEndTag();
            flag = false;
          }
        }
        if (flag)
          writer.RenderEndTag();
      }
      writer.RenderEndTag();
    }

    public void ValidateNonVisibleReportInputsSatisfied()
    {
      this.EnsureChildControls();
      if (!this.m_allCredentialsSatisfied)
      {
        if (!this.ShouldRenderCredentialsIfSupported)
          throw new MissingDataSourceCredentialsException();
      }
      else
      {
        if (this.m_paramControls == null)
          return;
        if (this.RenderParameters)
        {
          if (this.m_paramControls.HiddenUnsatisfiedParameter != null && !this.m_paramControls.VisibleParameterNeedsValue)
            throw new MissingParameterException(this.m_paramControls.HiddenUnsatisfiedParameter);
        }
        else if (this.m_paramControls.AnyUnsatisfiedParameter != null)
          throw new MissingParameterException(this.m_paramControls.AnyUnsatisfiedParameter);
      }
    }

    public void ValidateAllReportInputsSatisfied()
    {
      this.EnsureChildControls();
      if (!this.m_allCredentialsSatisfied)
        throw new MissingDataSourceCredentialsException();
      if (this.m_paramControls != null && this.m_paramControls.AnyUnsatisfiedParameter != null)
        throw new MissingParameterException(this.m_paramControls.AnyUnsatisfiedParameter);
    }

    private void ApplyParameterStyles(BaseParameterInputControl control)
    {
      control.CheckBoxCssClass = this.m_styles.CheckBox;
      control.TextBoxCssClass = this.m_styles.ParameterTextBox;
      control.TextBoxDisabledCssClass = this.m_styles.ParameterDisabledTextBox;
      control.TextBoxDisabledColor = this.m_styles.BackColor;
      if (control is ValidValuesParameterInputControl parameterInputControl)
        parameterInputControl.EmptyDropDownCssClass = this.m_styles.EmptyDropDown;
      if (!(control is MultiValueValidValuesInputControl valuesInputControl))
        return;
      valuesInputControl.DropDownCssClass = this.m_styles.MultiValueValidValueDropDown;
    }

    private void RenderOneParameter(HtmlTextWriter writer, BaseParameterInputControl control)
    {
      this.ApplyParameterStyles(control);
      this.AddLabelCellAttributes(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      writer.AddAttribute(HtmlTextWriterAttribute.For, control.PrimaryFormElementId);
      writer.RenderBeginTag(HtmlTextWriterTag.Label);
      SafeLiteralControl promptForInputControl = this.GetParameterPromptForInputControl(control);
      promptForInputControl.Font.CopyFrom(this.Font);
      promptForInputControl.RenderControl(writer);
      writer.RenderEndTag();
      writer.RenderEndTag();
      this.AddInputCellAttributes(writer);
      writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "0px");
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      control.Font.CopyFrom(this.Font);
      control.Disabled = true;
      control.RenderControl(writer);
      writer.RenderEndTag();
    }

    protected virtual void RenderOneDataSource(DataSourceControl control, HtmlTextWriter writer)
    {
      bool credentialsHidden = this.RenderCredentialsHidden;
      control.ApplyFont(this.Font);
      if (control.DataSourcePrompt != null)
      {
        if (credentialsHidden)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
        writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "5");
        this.AddLabelCellAttributes(writer);
        writer.RenderBeginTag(HtmlTextWriterTag.Td);
        control.DataSourcePrompt.RenderControl(writer);
        writer.RenderEndTag();
        writer.RenderEndTag();
      }
      if (credentialsHidden)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
      writer.AddAttribute(HtmlTextWriterAttribute.Id, control.ClientID);
      writer.RenderBeginTag(HtmlTextWriterTag.Tr);
      this.AddLabelCellAttributes(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      control.UserPrompt.RenderControl(writer);
      writer.RenderEndTag();
      this.AddInputCellAttributes(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      control.UserInput.RenderControl(writer);
      writer.RenderEndTag();
      this.AddSpacerCell(writer);
      this.AddLabelCellAttributes(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      control.PasswordPrompt.RenderControl(writer);
      writer.RenderEndTag();
      this.AddInputCellAttributes(writer);
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      control.PasswordInput.RenderControl(writer);
      writer.RenderEndTag();
      writer.RenderEndTag();
    }

    private void AddLabelCellAttributes(HtmlTextWriter writer)
    {
      if (this.m_styles.ParameterLabel != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.ParameterLabel);
      else
        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
    }

    private void AddInputCellAttributes(HtmlTextWriter writer)
    {
      if (this.m_styles.ParameterInput != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.ParameterInput);
      else
        writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
    }

    private void AddSpacerCell(HtmlTextWriter writer)
    {
      if (this.m_styles.ParameterColumnSpacer != null)
        writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_styles.ParameterColumnSpacer);
      else
        writer.AddAttribute(HtmlTextWriterAttribute.Width, "22px");
      writer.RenderBeginTag(HtmlTextWriterTag.Td);
      writer.RenderEndTag();
    }

    private void OnCredentialsChanged(object sender, EventArgs e)
    {
      this.m_allCredentialsSatisfied = true;
      this.m_credentialsChanged = true;
      this.OnControlValueChanged(sender, e);
    }

    private void OnParametersChanged(object sender, EventArgs e)
    {
      this.OnControlValueChanged(sender, e);
    }

    protected virtual void OnAutoPostBackOccurred(object sender, EventArgs e)
    {
      this.m_autoPostBackTarget = ((BaseParameterInputControl) sender).ReportParameter.Name;
      if (this.AutoPostBackOccurred == null)
        return;
      this.AutoPostBackOccurred((object) this, EventArgs.Empty);
    }

    private void OnControlValueChanged(object sender, EventArgs e)
    {
      if (this.ParameterValuesChanged == null)
        return;
      this.ParameterValuesChanged((object) this, (EventArgs) null);
    }

    protected SafeLiteralControl GetParameterPromptForInputControl(BaseParameterInputControl control)
    {
      return this.m_paramPrompts[control];
    }

    protected IEnumerable<BaseParameterInputControl> ParameterControls
    {
      get
      {
        return this.m_paramControls != null ? (IEnumerable<BaseParameterInputControl>) this.m_paramControls.Values : (IEnumerable<BaseParameterInputControl>) null;
      }
    }

    protected IEnumerable<DataSourceControl> CredentialControls
    {
      get => (IEnumerable<DataSourceControl>) this.m_dsControls;
    }

    protected TextButton ChangeCredentialsLink => this.m_changeCredentials;

    protected IReportViewerStyles ViewerStyles => this.m_styles;
  }
}
