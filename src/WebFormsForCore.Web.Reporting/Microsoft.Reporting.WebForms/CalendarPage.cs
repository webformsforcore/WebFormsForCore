using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Microsoft.Reporting.WebForms;

internal class CalendarPage : Page
{
	private CalendarPageControl m_calendarPageControl;

	public CalendarPage()
	{
		EnableViewState = false;
	}

	protected override void CreateChildControls()
	{
		Controls.Clear();
		HtmlForm htmlForm = new HtmlForm();
		Controls.Add(htmlForm);
		m_calendarPageControl = new CalendarPageControl();
		m_calendarPageControl.CalendarScriptUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.datepicker.js");
		m_calendarPageControl.CssUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.calendar.css");
		m_calendarPageControl.ImageUrl = EmbeddedResourceOperation.CreateUrl(null);
		htmlForm.Controls.Add(m_calendarPageControl);
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		writer.RenderBeginTag(HtmlTextWriterTag.Html);
		writer.RenderBeginTag(HtmlTextWriterTag.Head);
		writer.RenderEndTag();
		writer.AddAttribute("onload", m_calendarPageControl.OnPageLoadScript, fEndode: true);
		writer.AddAttribute("onkeydown", "OnKeyDown(this);");
		writer.RenderBeginTag(HtmlTextWriterTag.Body);
		base.Render(writer);
		writer.RenderEndTag();
		writer.RenderEndTag();
	}
}
