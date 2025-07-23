using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class FindGroup : ToolbarGroup
{
	private TextBox m_findText;

	private TextButton m_findButton;

	private TextButton m_nextButton;

	private SafeLiteralControl m_sep;

	private bool m_showTextSeparator;

	public override string GroupCssClassName => m_viewer.ViewerStyle.ToolbarFind;

	public FindGroup(ReportViewer viewer, bool showTextSeparator)
		: base(viewer)
	{
		m_showTextSeparator = showTextSeparator;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_findText = new TextBox();
		m_findText.MaxLength = 255;
		m_findText.Columns = 10;
		m_findText.ToolTip = LocalizationHelper.Current.SearchTextBoxToolTip;
		m_findText.CssClass = m_viewer.ViewerStyle.ToolbarTextBox;
		m_findText.Enabled = false;
		Controls.Add(m_findText);
		m_findButton = new TextButton(LocalizationHelper.Current.FindButtonText, LocalizationHelper.Current.FindButtonToolTip, m_viewer.ViewerStyle);
		m_findButton.ShowDisabled = true;
		Controls.Add(m_findButton);
		m_sep = new SafeLiteralControl();
		m_sep.Text = "|";
		m_sep.Visible = m_showTextSeparator;
		Controls.Add(m_sep);
		m_nextButton = new TextButton(LocalizationHelper.Current.FindNextButtonText, LocalizationHelper.Current.FindNextButtonToolTip, m_viewer.ViewerStyle);
		m_nextButton.ShowDisabled = true;
		Controls.Add(m_nextButton);
	}

	protected override void OnPreRender(EventArgs e)
	{
		EnsureChildControls();
		base.OnPreRender(e);
		SearchState searchState = m_viewer.SearchState;
		if (searchState != null)
		{
			m_findText.Text = searchState.Text;
		}
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_findText.Font.CopyFrom(Font);
		m_findButton.Font.CopyFrom(Font);
		m_nextButton.Font.CopyFrom(Font);
		m_sep.Font.CopyFrom(Font);
		base.Render(writer);
	}

	public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
	{
		EnsureChildControls();
		toolbarDesc.AddElementProperty("FindTextBox", m_findText.ClientID);
		toolbarDesc.AddElementProperty("FindButton", m_findButton.ClientID);
		toolbarDesc.AddElementProperty("FindNextButton", m_nextButton.ClientID);
		toolbarDesc.AddProperty("CanFindNext", m_viewer.SearchState != null);
	}
}
