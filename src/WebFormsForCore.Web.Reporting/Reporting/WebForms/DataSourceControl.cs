// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.DataSourceControl
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class DataSourceControl : CompositeControl, IScriptControl
  {
    private ReportDataSourceInfo m_dsInfo;
    private IReportViewerStyles m_styles;
    private DataSourceControl.CredentialRenderer m_renderer;
    private SafeLiteralControl m_dsPromptControl;
    private SafeLiteralControl m_userPromptControl;
    private SafeLiteralControl m_pwdPromptControl;
    private TextBox m_userControl;
    private TextBox m_pwdControl;

    public DataSourceControl(
      ReportDataSourceInfo dsInfo,
      IReportViewerStyles styles,
      DataSourceControl.CredentialRenderer renderer)
    {
      this.m_dsInfo = dsInfo;
      this.m_styles = styles;
      this.m_renderer = renderer;
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      if (this.m_dsInfo.Prompt != null && this.m_dsInfo.Prompt.Length > 0)
      {
        this.m_dsPromptControl = new SafeLiteralControl(this.m_dsInfo.Prompt);
        this.Controls.Add((Control) this.m_dsPromptControl);
      }
      this.m_userPromptControl = new SafeLiteralControl(LocalizationHelper.Current.UserNamePrompt);
      this.Controls.Add((Control) this.m_userPromptControl);
      this.m_pwdPromptControl = new SafeLiteralControl(LocalizationHelper.Current.PasswordPrompt);
      this.Controls.Add((Control) this.m_pwdPromptControl);
      this.m_userControl = this.CreateTextBox();
      this.m_userControl.Attributes.Add("autocomplete", "off");
      this.m_userControl.TextChanged += new EventHandler(this.OnChanged);
      this.Controls.Add((Control) this.m_userControl);
      this.m_pwdControl = this.CreateTextBox();
      this.m_pwdControl.TextChanged += new EventHandler(this.OnChanged);
      this.m_pwdControl.TextMode = TextBoxMode.Password;
      this.Controls.Add((Control) this.m_pwdControl);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptControl<DataSourceControl>(this);
      this.m_userControl.Text = "";
      this.m_pwdControl.Text = "";
    }

    protected override void Render(HtmlTextWriter writer)
    {
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      this.m_renderer(this, writer);
    }

    private TextBox CreateTextBox()
    {
      TextBox textBox = (TextBox) new TextBoxWithClientID();
      textBox.Columns = 30;
      textBox.MaxLength = 256;
      textBox.CssClass = this.m_styles.ParameterTextBox;
      return textBox;
    }

    public ReportDataSourceInfo DataSourceInfo => this.m_dsInfo;

    public event EventHandler ValueChanged;

    private void OnChanged(object sender, EventArgs e)
    {
      if (this.ValueChanged == null)
        return;
      this.ValueChanged((object) this, EventArgs.Empty);
    }

    public void ApplyFont(FontInfo font)
    {
      this.EnsureChildControls();
      if (this.m_dsPromptControl != null)
        this.m_dsPromptControl.Font.CopyFrom(font);
      this.m_userPromptControl.Font.CopyFrom(font);
      this.m_userControl.Font.CopyFrom(font);
      this.m_pwdPromptControl.Font.CopyFrom(font);
      this.m_pwdControl.Font.CopyFrom(font);
    }

    public Control DataSourcePrompt
    {
      get
      {
        this.EnsureChildControls();
        return (Control) this.m_dsPromptControl;
      }
    }

    public Control UserPrompt
    {
      get
      {
        this.EnsureChildControls();
        return (Control) this.m_userPromptControl;
      }
    }

    public TextBox UserInput
    {
      get
      {
        this.EnsureChildControls();
        return this.m_userControl;
      }
    }

    public Control PasswordPrompt
    {
      get
      {
        this.EnsureChildControls();
        return (Control) this.m_pwdPromptControl;
      }
    }

    public TextBox PasswordInput
    {
      get
      {
        this.EnsureChildControls();
        return this.m_pwdControl;
      }
    }

    public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      this.EnsureChildControls();
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient.DataSourceCredential", this.ClientID);
      controlDescriptor.AddProperty("UserNameId", (object) this.m_userControl.ClientID);
      controlDescriptor.AddProperty("PasswordId", (object) this.m_pwdControl.ClientID);
      string dataSourcePrompt = this.m_dsInfo.Prompt;
      if (string.IsNullOrEmpty(dataSourcePrompt))
        dataSourcePrompt = this.m_dsInfo.Name;
      string str = LocalizationHelper.Current.CredentialMissingUserNameError(dataSourcePrompt);
      controlDescriptor.AddProperty("ValidationMessage", (object) str);
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

    internal delegate void CredentialRenderer(DataSourceControl control, HtmlTextWriter writer);
  }
}
