using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class DataSourceControl : CompositeControl, IScriptControl
{
	internal delegate void CredentialRenderer(DataSourceControl control, HtmlTextWriter writer);

	private ReportDataSourceInfo m_dsInfo;

	private IReportViewerStyles m_styles;

	private CredentialRenderer m_renderer;

	private SafeLiteralControl m_dsPromptControl;

	private SafeLiteralControl m_userPromptControl;

	private SafeLiteralControl m_pwdPromptControl;

	private TextBox m_userControl;

	private TextBox m_pwdControl;

	public ReportDataSourceInfo DataSourceInfo => m_dsInfo;

	public Control DataSourcePrompt
	{
		get
		{
			EnsureChildControls();
			return m_dsPromptControl;
		}
	}

	public Control UserPrompt
	{
		get
		{
			EnsureChildControls();
			return m_userPromptControl;
		}
	}

	public TextBox UserInput
	{
		get
		{
			EnsureChildControls();
			return m_userControl;
		}
	}

	public Control PasswordPrompt
	{
		get
		{
			EnsureChildControls();
			return m_pwdPromptControl;
		}
	}

	public TextBox PasswordInput
	{
		get
		{
			EnsureChildControls();
			return m_pwdControl;
		}
	}

	public event EventHandler ValueChanged;

	public DataSourceControl(ReportDataSourceInfo dsInfo, IReportViewerStyles styles, CredentialRenderer renderer)
	{
		m_dsInfo = dsInfo;
		m_styles = styles;
		m_renderer = renderer;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		if (m_dsInfo.Prompt != null && m_dsInfo.Prompt.Length > 0)
		{
			m_dsPromptControl = new SafeLiteralControl(m_dsInfo.Prompt);
			Controls.Add(m_dsPromptControl);
		}
		m_userPromptControl = new SafeLiteralControl(LocalizationHelper.Current.UserNamePrompt);
		Controls.Add(m_userPromptControl);
		m_pwdPromptControl = new SafeLiteralControl(LocalizationHelper.Current.PasswordPrompt);
		Controls.Add(m_pwdPromptControl);
		m_userControl = CreateTextBox();
		m_userControl.Attributes.Add("autocomplete", "off");
		m_userControl.TextChanged += OnChanged;
		Controls.Add(m_userControl);
		m_pwdControl = CreateTextBox();
		m_pwdControl.TextChanged += OnChanged;
		m_pwdControl.TextMode = TextBoxMode.Password;
		Controls.Add(m_pwdControl);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptControl(this);
		m_userControl.Text = "";
		m_pwdControl.Text = "";
	}

	protected override void Render(HtmlTextWriter writer)
	{
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		m_renderer(this, writer);
	}

	private TextBox CreateTextBox()
	{
		TextBox textBox = new TextBoxWithClientID();
		textBox.Columns = 30;
		textBox.MaxLength = 256;
		textBox.CssClass = m_styles.ParameterTextBox;
		return textBox;
	}

	private void OnChanged(object sender, EventArgs e)
	{
		if (this.ValueChanged != null)
		{
			this.ValueChanged(this, EventArgs.Empty);
		}
	}

	public void ApplyFont(FontInfo font)
	{
		EnsureChildControls();
		if (m_dsPromptControl != null)
		{
			m_dsPromptControl.Font.CopyFrom(font);
		}
		m_userPromptControl.Font.CopyFrom(font);
		m_userControl.Font.CopyFrom(font);
		m_pwdPromptControl.Font.CopyFrom(font);
		m_pwdControl.Font.CopyFrom(font);
	}

	public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		EnsureChildControls();
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient.DataSourceCredential", ClientID);
		scriptControlDescriptor.AddProperty("UserNameId", m_userControl.ClientID);
		scriptControlDescriptor.AddProperty("PasswordId", m_pwdControl.ClientID);
		string text = m_dsInfo.Prompt;
		if (string.IsNullOrEmpty(text))
		{
			text = m_dsInfo.Name;
		}
		string value = LocalizationHelper.Current.CredentialMissingUserNameError(text);
		scriptControlDescriptor.AddProperty("ValidationMessage", value);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	public IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
