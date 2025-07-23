using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

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

	public string InnerCssClass
	{
		get
		{
			return m_innerCssClass;
		}
		set
		{
			m_innerCssClass = value;
		}
	}

	private string CloseDropDownJavaFunctionCall => string.Format(CultureInfo.InvariantCulture, "if ($get('{0}') != null && $get('{0}').control != null) $get('{0}').control.HideActiveDropDown();", JavaScriptHelper.StringEscapeSingleQuote(ClientID));

	protected virtual string ParametersGridID => "ParametersGrid" + ClientID;

	protected string ParameterTableID => "ParameterTable_" + ClientID;

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	protected string PromptValidationScript => string.Format(CultureInfo.InvariantCulture, "$get('{0}').control.ValidateInputs()", JavaScriptHelper.StringEscapeSingleQuote(ClientID));

	protected bool ShouldRenderCredentialsIfSupported
	{
		get
		{
			if (ShowCredentialPrompts)
			{
				return HasCredentials;
			}
			return false;
		}
	}

	protected bool RenderCredentialsHidden
	{
		get
		{
			if (ShouldRenderCredentialsIfSupported)
			{
				return m_allCredentialsSatisfied;
			}
			return false;
		}
	}

	protected bool RenderParameters
	{
		get
		{
			if (ShouldRenderCredentialsIfSupported && !RenderCredentialsHidden)
			{
				return false;
			}
			if (HasVisibleParameters)
			{
				return ShowParameterPrompts;
			}
			return false;
		}
	}

	public bool HasVisibleParameters
	{
		get
		{
			EnsureChildControls();
			if (m_paramControls != null)
			{
				return m_paramControls.Count > 0;
			}
			return false;
		}
	}

	public bool HasCredentials
	{
		get
		{
			EnsureChildControls();
			if (m_dsControls != null)
			{
				return m_dsControls.Count > 0;
			}
			return false;
		}
	}

	protected string AutoPostBackTarget => m_autoPostBackTarget;

	protected IEnumerable<BaseParameterInputControl> ParameterControls
	{
		get
		{
			if (m_paramControls != null)
			{
				return m_paramControls.Values;
			}
			return null;
		}
	}

	protected IEnumerable<DataSourceControl> CredentialControls => m_dsControls;

	protected TextButton ChangeCredentialsLink => m_changeCredentials;

	protected IReportViewerStyles ViewerStyles => m_styles;

	public event EventHandler ViewReportClick;

	public event EventHandler ParameterValuesChanged;

	public event EventHandler AutoPostBackOccurred;

	public event ReportErrorEventHandler Error;

	public event ReportCredentialsEventHandler SubmittingDataSourceCredentials;

	public event ReportParametersEventHandler SubmittingParameterValues;

	public ParametersArea(IReportViewerStyles styles)
	{
		m_styles = styles;
	}

	public ParametersArea(IReportViewerStyles styles, bool showHiddenParameters, bool positioningMode)
		: this(styles)
	{
		m_showHiddenParameters = showHiddenParameters;
		m_positioningMode = positioningMode;
	}

	public ParametersArea(ReportViewer viewer, bool showHiddenParameters, bool positioningMode)
		: this(viewer)
	{
		m_showHiddenParameters = showHiddenParameters;
		m_positioningMode = positioningMode;
	}

	public ParametersArea(ReportViewer viewer)
		: this(viewer.ViewerStyle)
	{
		m_viewer = viewer;
		Error += m_viewer.OnError;
	}

	protected virtual IParameterSupplier GetParameterSupplier()
	{
		if (m_viewer != null)
		{
			return new ReportParameterSupplier(m_viewer.Report);
		}
		throw new NotImplementedException();
	}

	protected bool OnError(Exception e)
	{
		if (this.Error != null)
		{
			this.Error(this, new ReportErrorEventArgs(e));
			return true;
		}
		return false;
	}

	protected override void CreateChildControls()
	{
		try
		{
			Controls.Clear();
			m_paramControls = null;
			m_dsControls = null;
			m_paramPrompts.Clear();
			m_viewReportButton = new ViewReportButton();
			m_viewReportButton.Text = LocalizationHelper.Current.ViewReportButtonText;
			m_viewReportButton.Click += OnViewReport;
			Controls.Add(m_viewReportButton);
			m_changeCredentials = new TextButton(LocalizationHelper.Current.ChangeCredentialsText, LocalizationHelper.Current.ChangeCredentialsToolTip, m_styles);
			m_changeCredentials.ShowDisabled = false;
			Controls.Add(m_changeCredentials);
			if (Global.IsDesignTime)
			{
				return;
			}
			if (m_paramsInfo == null)
			{
				RefreshControlsFromReportMetadata();
			}
			if (m_dsInfos != null && m_dsInfos.Count > 0)
			{
				m_dsControls = DataSourceControlCollection.Create(m_dsInfos, RenderOneDataSource, m_styles);
				foreach (DataSourceControl dsControl in m_dsControls)
				{
					dsControl.ValueChanged += OnCredentialsChanged;
					Controls.Add(dsControl);
				}
			}
			if (m_paramsInfo == null || m_paramsInfo.Count <= 0)
			{
				return;
			}
			bool isQueryExecutionAllowed = m_parameterSupplier.IsQueryExecutionAllowed;
			m_paramControls = ParameterControlCollection.Create(m_paramsInfo, isQueryExecutionAllowed, m_styles, m_showHiddenParameters, m_positioningMode);
			foreach (BaseParameterInputControl parameterControl in ParameterControls)
			{
				SafeLiteralControl safeLiteralControl = new SafeLiteralControl(parameterControl.Prompt, parameterControl.Disabled);
				Controls.Add(safeLiteralControl);
				m_paramPrompts.Add(parameterControl, safeLiteralControl);
				parameterControl.ValueChanged += OnParametersChanged;
				parameterControl.AutoPostBackOccurred += OnAutoPostBackOccurred;
				CreateRenderedParameterControl(parameterControl);
			}
		}
		catch (Exception e)
		{
			if (!OnError(e))
			{
				throw;
			}
		}
	}

	protected virtual void CreateRenderedParameterControl(BaseParameterInputControl parameterControl)
	{
		Controls.Add(parameterControl);
	}

	protected void ClearChildControls(bool clearParameterMetaData)
	{
		if (clearParameterMetaData)
		{
			m_paramsInfo = null;
			m_dsInfos = null;
			m_allCredentialsSatisfied = false;
			m_parameterSupplier = null;
		}
		if (base.ChildControlsCreated)
		{
			m_paramControls = null;
			m_dsControls = null;
			ClearChildState();
			base.ChildControlsCreated = false;
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		if (m_viewer != null && !m_viewer.ViewerStyle.GetFontFromCss)
		{
			Font.CopyFrom(m_viewer.ViewerStyle.Font);
		}
		if (m_viewer != null)
		{
			ShowParameterPrompts = m_viewer.ShowParameterPrompts;
			ShowCredentialPrompts = m_viewer.ShowCredentialPrompts;
		}
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
		m_changeCredentials.ClientSideClickScript = string.Format(CultureInfo.InvariantCulture, "if ($get('{0}') != null) $get('{0}').control.OnChangeCredentialsClick();", JavaScriptHelper.StringEscapeSingleQuote(ClientID));
	}

	public void RenderCloseDropDownAttributes(HtmlTextWriter writer)
	{
		writer.AddAttribute("onclick", CloseDropDownJavaFunctionCall);
		writer.AddAttribute("onactivate", CloseDropDownJavaFunctionCall);
	}

	public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._PromptArea", ClientID);
		if (m_viewer != null)
		{
			scriptControlDescriptor.AddProperty("ReportViewerId", m_viewer.ClientID);
		}
		scriptControlDescriptor.AddProperty("CredentialsLinkId", m_changeCredentials.ClientID);
		scriptControlDescriptor.AddProperty("ParametersGridID", ParametersGridID);
		scriptControlDescriptor.AddProperty("ViewReportButtonId", m_viewReportButton.ClientID);
		if (m_paramControls != null && RenderParameters)
		{
			List<string> list = new List<string>();
			foreach (BaseParameterInputControl value in m_paramControls.Values)
			{
				if (!value.Disabled)
				{
					list.Add(value.ClientID);
				}
			}
			scriptControlDescriptor.AddProperty("ParameterIdList", list.ToArray());
		}
		if (m_dsControls != null && ShouldRenderCredentialsIfSupported)
		{
			scriptControlDescriptor.AddProperty("CredentialIdList", m_dsControls.DataSourceClientIds);
		}
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	public IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}

	public void AddCloseDropDownAttributes(HtmlControl control)
	{
		control.Attributes.Add("onclick", CloseDropDownJavaFunctionCall);
		control.Attributes.Add("onactivate", CloseDropDownJavaFunctionCall);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		AddAttributesToRender(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		if (!string.IsNullOrEmpty(InnerCssClass))
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, InnerCssClass);
		}
		RenderCloseDropDownAttributes(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Div);
		RenderChildren(writer);
		if (BrowserDetection.Current.IsIE)
		{
			writer.Write("<input type=\"text\" style=\"visibility:hidden;display:none\" disabled=\"disabled\"></input>");
		}
		writer.RenderEndTag();
		writer.RenderEndTag();
		RenderAbsolutePositionedChildren(writer);
	}

	protected override void RenderChildren(HtmlTextWriter writer)
	{
		writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
		writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
		writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
		writer.AddAttribute(HtmlTextWriterAttribute.Id, ParameterTableID);
		writer.AddAttribute(HtmlTextWriterAttribute.Name, ParameterTableID);
		if (m_styles.ParameterContainer != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.ParameterContainer);
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingTop, "5px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingLeft, "5px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "5px");
			writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingBottom, "10px");
			if (!m_styles.BackColor.IsEmpty)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.BackgroundColor, ColorTranslator.ToHtml(m_styles.BackColor));
			}
			m_styles.AddInternalBorderAttributes(writer, "border-bottom");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.AddAttribute(HtmlTextWriterAttribute.Width, "100%");
		writer.AddAttribute(HtmlTextWriterAttribute.Height, "100%");
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		RenderParameterControls(writer);
		writer.RenderEndTag();
		writer.AddAttribute(HtmlTextWriterAttribute.Width, "6px");
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		writer.RenderEndTag();
		if (m_styles.ViewReportContainer != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.ViewReportContainer);
		}
		else
		{
			m_styles.AddInternalBorderAttributes(writer, "border-left");
			writer.AddAttribute(HtmlTextWriterAttribute.Align, "center");
			writer.AddAttribute(HtmlTextWriterAttribute.Valign, "top");
			writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "10px");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		RenderViewReportCell(writer);
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
	}

	private void RenderAbsolutePositionedChildren(HtmlTextWriter writer)
	{
		if (!RenderParameters || m_paramControls == null || !m_positioningMode)
		{
			return;
		}
		foreach (BaseParameterInputControl parameterControl in ParameterControls)
		{
			if (parameterControl is GenericDropDownInputControl genericDropDownInputControl)
			{
				genericDropDownInputControl.RenderAbsolutePositionedControls(writer);
			}
		}
	}

	private void RenderViewReportCell(HtmlTextWriter writer)
	{
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		m_viewReportButton.Font.CopyFrom(Font);
		m_viewReportButton.RenderControl(writer);
		writer.RenderEndTag();
		writer.RenderEndTag();
		writer.RenderEndTag();
	}

	protected override void LoadViewState(object savedState)
	{
		try
		{
			base.LoadViewState(savedState);
			PostLoadViewState();
		}
		catch (Exception e)
		{
			if (!OnError(e))
			{
				throw;
			}
		}
	}

	protected virtual void PostLoadViewState()
	{
		if (ViewState["HasControls"] != null && (bool)ViewState["HasControls"])
		{
			RefreshControlsFromReportMetadata();
		}
	}

	protected override object SaveViewState()
	{
		if (Visible)
		{
			EnsureChildControls();
			ViewState["HasControls"] = m_dsControls != null || m_paramControls != null;
		}
		return base.SaveViewState();
	}

	private void OnViewReport(object sender, EventArgs e)
	{
		if (this.ViewReportClick != null)
		{
			this.ViewReportClick(this, e);
		}
	}

	public void EnsureReportMetaData()
	{
		if (!base.ChildControlsCreated)
		{
			GetMetaDataFromReport();
		}
	}

	public void RefreshControlsFromReportMetadata()
	{
		ClearChildControls(clearParameterMetaData: true);
		GetMetaDataFromReport();
	}

	private void GetMetaDataFromReport()
	{
		m_parameterSupplier = GetParameterSupplier();
		if (m_parameterSupplier.IsReadyForConnection)
		{
			if (m_paramsInfo == null)
			{
				m_paramsInfo = m_parameterSupplier.GetParameters();
			}
			m_dsInfos = m_parameterSupplier.GetDataSources(out m_allCredentialsSatisfied);
		}
	}

	public bool SaveControlValuesToReport()
	{
		EnsureChildControls();
		bool result = true;
		if (!SaveCredentialsToReport())
		{
			result = GetParameterValuesToSubmit(out var paramValues, out var autoSubmitParamNames);
			bool flag = true;
			if (this.SubmittingParameterValues != null)
			{
				ReportParametersEventArgs e = new ReportParametersEventArgs(paramValues, AutoPostBackTarget != null);
				this.SubmittingParameterValues(this, e);
				if (e.Cancel)
				{
					flag = false;
				}
			}
			if (paramValues.Count > 0 && flag)
			{
				SaveParametersToReport(paramValues, autoSubmitParamNames);
			}
		}
		ClearChildControls(clearParameterMetaData: true);
		return result;
	}

	protected ReportParameterInfoCollection SaveParametersToReport(IEnumerable<ReportParameter> parameters, IEnumerable<string> autoSubmitParameterNames)
	{
		m_parameterSupplier.SetParameters(parameters);
		ReportParameterInfoCollection parameters2 = m_parameterSupplier.GetParameters();
		if (autoSubmitParameterNames != null)
		{
			foreach (string autoSubmitParameterName in autoSubmitParameterNames)
			{
				if (parameters2[autoSubmitParameterName].HasUnsatisfiedDownstreamParametersWithDefaults)
				{
					m_parameterSupplier.SetParameters(new ReportParameter[0]);
					break;
				}
			}
		}
		return parameters2;
	}

	protected bool GetParameterValuesToSubmit(out ReportParameterCollection paramValues, out IEnumerable<string> autoSubmitParamNames)
	{
		paramValues = new ReportParameterCollection();
		autoSubmitParamNames = new string[0];
		bool result = true;
		if (ParameterControls != null)
		{
			foreach (BaseParameterInputControl parameterControl in ParameterControls)
			{
				if (!parameterControl.Disabled && parameterControl.CurrentValue != null)
				{
					ReportParameter reportParameter = new ReportParameter();
					reportParameter.Name = parameterControl.ReportParameter.Name;
					reportParameter.Values.AddRange(parameterControl.CurrentValue);
					paramValues.Add(reportParameter);
				}
				else
				{
					result = false;
				}
			}
			if (AutoPostBackTarget != null)
			{
				autoSubmitParamNames = new string[1] { AutoPostBackTarget };
			}
		}
		return result;
	}

	protected bool SaveCredentialsToReport()
	{
		if (m_dsControls != null && m_credentialsChanged)
		{
			DataSourceCredentialsCollection dataSourceCredentialsCollection = new DataSourceCredentialsCollection();
			foreach (DataSourceControl dsControl in m_dsControls)
			{
				DataSourceCredentials dataSourceCredentials = new DataSourceCredentials();
				dataSourceCredentials.Name = dsControl.DataSourceInfo.Name;
				dataSourceCredentials.UserId = dsControl.UserInput.Text;
				dataSourceCredentials.Password = dsControl.PasswordInput.Text;
				dataSourceCredentialsCollection.Add(dataSourceCredentials);
			}
			if (this.SubmittingDataSourceCredentials != null)
			{
				ReportCredentialsEventArgs e = new ReportCredentialsEventArgs(dataSourceCredentialsCollection);
				this.SubmittingDataSourceCredentials(this, e);
				if (e.Cancel || e.Credentials.Count == 0)
				{
					return true;
				}
			}
			m_parameterSupplier.SetDataSourceCredentials(dataSourceCredentialsCollection);
			return true;
		}
		return false;
	}

	private void RenderParameterControls(HtmlTextWriter writer)
	{
		writer.AddAttribute(HtmlTextWriterAttribute.Id, ParametersGridID);
		writer.RenderBeginTag(HtmlTextWriterTag.Table);
		if (RenderCredentialsHidden)
		{
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.AddAttribute(HtmlTextWriterAttribute.Align, "left");
			writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "5");
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			ChangeCredentialsLink.Font.CopyFrom(Font);
			ChangeCredentialsLink.RenderControl(writer);
			writer.RenderEndTag();
			writer.RenderEndTag();
		}
		if (ShouldRenderCredentialsIfSupported)
		{
			foreach (DataSourceControl dsControl in m_dsControls)
			{
				dsControl.RenderControl(writer);
			}
		}
		if (RenderParameters)
		{
			int num = 0;
			bool flag = false;
			foreach (BaseParameterInputControl parameterControl in ParameterControls)
			{
				if (num == 0)
				{
					writer.AddAttribute("IsParameterRow", "true");
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
					flag = true;
				}
				else
				{
					AddSpacerCell(writer);
				}
				RenderOneParameter(writer, parameterControl);
				num = (num + 1) % 2;
				if (num == 0)
				{
					writer.RenderEndTag();
					flag = false;
				}
			}
			if (flag)
			{
				writer.RenderEndTag();
			}
		}
		writer.RenderEndTag();
	}

	public void ValidateNonVisibleReportInputsSatisfied()
	{
		EnsureChildControls();
		if (!m_allCredentialsSatisfied)
		{
			if (!ShouldRenderCredentialsIfSupported)
			{
				throw new MissingDataSourceCredentialsException();
			}
		}
		else
		{
			if (m_paramControls == null)
			{
				return;
			}
			if (RenderParameters)
			{
				if (m_paramControls.HiddenUnsatisfiedParameter != null && !m_paramControls.VisibleParameterNeedsValue)
				{
					throw new MissingParameterException(m_paramControls.HiddenUnsatisfiedParameter);
				}
			}
			else if (m_paramControls.AnyUnsatisfiedParameter != null)
			{
				throw new MissingParameterException(m_paramControls.AnyUnsatisfiedParameter);
			}
		}
	}

	public void ValidateAllReportInputsSatisfied()
	{
		EnsureChildControls();
		if (!m_allCredentialsSatisfied)
		{
			throw new MissingDataSourceCredentialsException();
		}
		if (m_paramControls != null && m_paramControls.AnyUnsatisfiedParameter != null)
		{
			throw new MissingParameterException(m_paramControls.AnyUnsatisfiedParameter);
		}
	}

	private void ApplyParameterStyles(BaseParameterInputControl control)
	{
		control.CheckBoxCssClass = m_styles.CheckBox;
		control.TextBoxCssClass = m_styles.ParameterTextBox;
		control.TextBoxDisabledCssClass = m_styles.ParameterDisabledTextBox;
		control.TextBoxDisabledColor = m_styles.BackColor;
		if (control is ValidValuesParameterInputControl validValuesParameterInputControl)
		{
			validValuesParameterInputControl.EmptyDropDownCssClass = m_styles.EmptyDropDown;
		}
		if (control is MultiValueValidValuesInputControl multiValueValidValuesInputControl)
		{
			multiValueValidValuesInputControl.DropDownCssClass = m_styles.MultiValueValidValueDropDown;
		}
	}

	private void RenderOneParameter(HtmlTextWriter writer, BaseParameterInputControl control)
	{
		ApplyParameterStyles(control);
		AddLabelCellAttributes(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		writer.AddAttribute(HtmlTextWriterAttribute.For, control.PrimaryFormElementId);
		writer.RenderBeginTag(HtmlTextWriterTag.Label);
		SafeLiteralControl parameterPromptForInputControl = GetParameterPromptForInputControl(control);
		parameterPromptForInputControl.Font.CopyFrom(Font);
		parameterPromptForInputControl.RenderControl(writer);
		writer.RenderEndTag();
		writer.RenderEndTag();
		AddInputCellAttributes(writer);
		writer.AddStyleAttribute(HtmlTextWriterStyle.PaddingRight, "0px");
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		control.Font.CopyFrom(Font);
		control.Disabled = true;
		control.RenderControl(writer);
		writer.RenderEndTag();
	}

	protected virtual void RenderOneDataSource(DataSourceControl control, HtmlTextWriter writer)
	{
		bool renderCredentialsHidden = RenderCredentialsHidden;
		control.ApplyFont(Font);
		if (control.DataSourcePrompt != null)
		{
			if (renderCredentialsHidden)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
			}
			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "5");
			AddLabelCellAttributes(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Td);
			control.DataSourcePrompt.RenderControl(writer);
			writer.RenderEndTag();
			writer.RenderEndTag();
		}
		if (renderCredentialsHidden)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
		}
		writer.AddAttribute(HtmlTextWriterAttribute.Id, control.ClientID);
		writer.RenderBeginTag(HtmlTextWriterTag.Tr);
		AddLabelCellAttributes(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		control.UserPrompt.RenderControl(writer);
		writer.RenderEndTag();
		AddInputCellAttributes(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		control.UserInput.RenderControl(writer);
		writer.RenderEndTag();
		AddSpacerCell(writer);
		AddLabelCellAttributes(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		control.PasswordPrompt.RenderControl(writer);
		writer.RenderEndTag();
		AddInputCellAttributes(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		control.PasswordInput.RenderControl(writer);
		writer.RenderEndTag();
		writer.RenderEndTag();
	}

	private void AddLabelCellAttributes(HtmlTextWriter writer)
	{
		if (m_styles.ParameterLabel != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.ParameterLabel);
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
		}
	}

	private void AddInputCellAttributes(HtmlTextWriter writer)
	{
		if (m_styles.ParameterInput != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.ParameterInput);
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Padding, "5px");
		}
	}

	private void AddSpacerCell(HtmlTextWriter writer)
	{
		if (m_styles.ParameterColumnSpacer != null)
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Class, m_styles.ParameterColumnSpacer);
		}
		else
		{
			writer.AddAttribute(HtmlTextWriterAttribute.Width, "22px");
		}
		writer.RenderBeginTag(HtmlTextWriterTag.Td);
		writer.RenderEndTag();
	}

	private void OnCredentialsChanged(object sender, EventArgs e)
	{
		m_allCredentialsSatisfied = true;
		m_credentialsChanged = true;
		OnControlValueChanged(sender, e);
	}

	private void OnParametersChanged(object sender, EventArgs e)
	{
		OnControlValueChanged(sender, e);
	}

	protected virtual void OnAutoPostBackOccurred(object sender, EventArgs e)
	{
		m_autoPostBackTarget = ((BaseParameterInputControl)sender).ReportParameter.Name;
		if (this.AutoPostBackOccurred != null)
		{
			this.AutoPostBackOccurred(this, EventArgs.Empty);
		}
	}

	private void OnControlValueChanged(object sender, EventArgs e)
	{
		if (this.ParameterValuesChanged != null)
		{
			this.ParameterValuesChanged(this, null);
		}
	}

	protected SafeLiteralControl GetParameterPromptForInputControl(BaseParameterInputControl control)
	{
		return m_paramPrompts[control];
	}
}
