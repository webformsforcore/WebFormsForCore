using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal class ReportAreaVisibilityState : CompositeControl
{
	private HiddenField m_persistedState;

	private ReportAreaContent? m_newState = null;

	private ReportArea m_reportArea;

	protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

	public ReportAreaContent NewClientState
	{
		get
		{
			if (m_newState.HasValue)
			{
				return m_newState.Value;
			}
			return CurrentClientState;
		}
		set
		{
			m_newState = value;
		}
	}

	public ReportAreaContent CurrentClientState
	{
		get
		{
			EnsureChildControls();
			return (ReportAreaContent)Enum.Parse(typeof(ReportAreaContent), m_persistedState.Value);
		}
	}

	public ReportAreaVisibilityState(ReportArea reportArea)
	{
		m_reportArea = reportArea;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		m_persistedState = new HiddenField();
		m_persistedState.EnableViewState = false;
		m_persistedState.Value = ReportAreaContent.None.ToString();
		Controls.Add(m_persistedState);
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.Style[HtmlTextWriterStyle.Visibility] = "none";
		base.OnPreRender(e);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		m_persistedState.Value = NewClientState.ToString();
		writer.AddAttribute("NewContentType", "Microsoft.Reporting.WebFormsClient.ReportAreaContent." + NewClientState);
		writer.AddAttribute("ForNonReportContentArea", ReportArea.IsDisplayedInNonReportContentPanel(NewClientState) ? "true" : "false");
		base.Render(writer);
	}
}
