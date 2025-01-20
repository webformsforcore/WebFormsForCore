
using System.Web.UI;
using System.Web.UI.HtmlControls;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class CalendarPage : Page
  {
    private CalendarPageControl m_calendarPageControl;

    public CalendarPage() => this.EnableViewState = false;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      HtmlForm child = new HtmlForm();
      this.Controls.Add((Control) child);
      this.m_calendarPageControl = new CalendarPageControl();
      this.m_calendarPageControl.CalendarScriptUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.datepicker.js");
      this.m_calendarPageControl.CssUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.calendar.css");
      this.m_calendarPageControl.ImageUrl = EmbeddedResourceOperation.CreateUrl((string) null);
      child.Controls.Add((Control) this.m_calendarPageControl);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      writer.RenderBeginTag(HtmlTextWriterTag.Html);
      writer.RenderBeginTag(HtmlTextWriterTag.Head);
      writer.RenderEndTag();
      writer.AddAttribute("onload", this.m_calendarPageControl.OnPageLoadScript, true);
      writer.AddAttribute("onkeydown", "OnKeyDown(this);");
      writer.RenderBeginTag(HtmlTextWriterTag.Body);
      base.Render(writer);
      writer.RenderEndTag();
      writer.RenderEndTag();
    }
  }
}
