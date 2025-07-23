using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class PageRequestManagerErrorHandler : CompositeControl
{
	private Label m_errorMessageView;

	protected override void CreateChildControls()
	{
		Controls.Clear();
		base.CreateChildControls();
		m_errorMessageView = new Label();
		m_errorMessageView.Style.Add(HtmlTextWriterStyle.Display, "none");
		Controls.Add(m_errorMessageView);
	}

	protected override void RenderChildren(HtmlTextWriter writer)
	{
		string format = "\r\n$addHandler(window, 'beforeunload', function() {{Sys.WebForms.PageRequestManager.getInstance().abortPostBack();}});\r\n\r\nSys.WebForms.PageRequestManager.getInstance().add_endRequest(function(sender, args) {{\r\n    if (args.get_error() !== null) {{\r\n        var label = $get('{0}');\r\n        label.style.display = '';\r\n        label.innerText = args.get_error().message;\r\n        label.textContent = label.innerText;\r\n    }}\r\n}});\r\n\r\nSys.WebForms.PageRequestManager.getInstance().add_beginRequest(function(sender, args) {{$get('{0}').style.display = 'none';}});\r\n";
		string value = string.Format(CultureInfo.InvariantCulture, format, JavaScriptHelper.StringEscapeSingleQuote(m_errorMessageView.ClientID));
		base.RenderChildren(writer);
		writer.RenderBeginTag(HtmlTextWriterTag.Script);
		writer.Write(value);
		writer.RenderEndTag();
	}
}
