// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.FindGroup
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Web.UI;
using System.Web.UI.WebControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class FindGroup : ToolbarGroup
  {
    private TextBox m_findText;
    private TextButton m_findButton;
    private TextButton m_nextButton;
    private SafeLiteralControl m_sep;
    private bool m_showTextSeparator;

    public FindGroup(ReportViewer viewer, bool showTextSeparator)
      : base(viewer)
    {
      this.m_showTextSeparator = showTextSeparator;
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarFind;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_findText = new TextBox();
      this.m_findText.MaxLength = (int) byte.MaxValue;
      this.m_findText.Columns = 10;
      this.m_findText.ToolTip = LocalizationHelper.Current.SearchTextBoxToolTip;
      this.m_findText.CssClass = this.m_viewer.ViewerStyle.ToolbarTextBox;
      this.m_findText.Enabled = false;
      this.Controls.Add((Control) this.m_findText);
      this.m_findButton = new TextButton(LocalizationHelper.Current.FindButtonText, LocalizationHelper.Current.FindButtonToolTip, this.m_viewer.ViewerStyle);
      this.m_findButton.ShowDisabled = true;
      this.Controls.Add((Control) this.m_findButton);
      this.m_sep = new SafeLiteralControl();
      this.m_sep.Text = "|";
      this.m_sep.Visible = this.m_showTextSeparator;
      this.Controls.Add((Control) this.m_sep);
      this.m_nextButton = new TextButton(LocalizationHelper.Current.FindNextButtonText, LocalizationHelper.Current.FindNextButtonToolTip, this.m_viewer.ViewerStyle);
      this.m_nextButton.ShowDisabled = true;
      this.Controls.Add((Control) this.m_nextButton);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      base.OnPreRender(e);
      SearchState searchState = this.m_viewer.SearchState;
      if (searchState == null)
        return;
      this.m_findText.Text = searchState.Text;
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_findText.Font.CopyFrom(this.Font);
      this.m_findButton.Font.CopyFrom(this.Font);
      this.m_nextButton.Font.CopyFrom(this.Font);
      this.m_sep.Font.CopyFrom(this.Font);
      base.Render(writer);
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("FindTextBox", this.m_findText.ClientID);
      toolbarDesc.AddElementProperty("FindButton", this.m_findButton.ClientID);
      toolbarDesc.AddElementProperty("FindNextButton", this.m_nextButton.ClientID);
      toolbarDesc.AddProperty("CanFindNext", (object) (this.m_viewer.SearchState != null));
    }
  }
}
