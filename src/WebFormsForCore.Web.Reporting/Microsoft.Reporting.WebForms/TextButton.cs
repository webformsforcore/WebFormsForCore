using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;

namespace Microsoft.Reporting.WebForms;

internal sealed class TextButton : ScriptControl, IPostBackEventHandler
{
	public bool ShowDisabled = true;

	private string m_clientSideClickScript;

	private string m_tooltip;

	private string m_text;

	private IReportViewerStyles m_viewerStyle;

	public string ClientSideClickScript
	{
		get
		{
			return m_clientSideClickScript;
		}
		set
		{
			m_clientSideClickScript = value;
		}
	}

	public string ClientSideObjectName => string.Format(CultureInfo.InvariantCulture, "$get('{0}').control", JavaScriptHelper.StringEscapeSingleQuote(ClientID));

	public event EventHandler Click;

	public TextButton(string text, string tooltip, IReportViewerStyles viewerStyle)
	{
		m_text = text;
		m_tooltip = tooltip;
		m_viewerStyle = viewerStyle;
		base.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
	}

	public void RaisePostBackEvent(string eventArgument)
	{
		if (this.Click != null)
		{
			this.Click(this, null);
		}
	}

	public string SetEnabledStateScript(string boolShouldEnableScript)
	{
		return string.Format(CultureInfo.InvariantCulture, "if ($get('{0}') != null && $get('{0}').control != null){1}.SetActive({2});", JavaScriptHelper.StringEscapeSingleQuote(ClientID), ClientSideObjectName, boolShouldEnableScript);
	}

	public string SetEnabledStateScript(bool shouldEnable)
	{
		return SetEnabledStateScript(shouldEnable ? "true" : "false");
	}

	protected override void Render(HtmlTextWriter writer)
	{
		EnsureChildControls();
		ScriptManager.GetCurrent(Page)?.RegisterScriptDescriptors(this);
		AddAttributesToRender(writer);
		AddAttributesForInitialState(writer);
		writer.AddAttribute(HtmlTextWriterAttribute.Title, m_tooltip, fEncode: true);
		writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
		writer.RenderBeginTag(HtmlTextWriterTag.A);
		writer.WriteEncodedText(m_text);
		writer.RenderEndTag();
	}

	private void AddAttributesForInitialState(HtmlTextWriter writer)
	{
		if (m_viewerStyle.LinkActive != null)
		{
			if (ShowDisabled)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, m_viewerStyle.LinkDisabled);
			}
			else
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, m_viewerStyle.LinkActive);
			}
			return;
		}
		if (ShowDisabled)
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(m_viewerStyle.LinkDisabledColor));
		}
		else
		{
			writer.AddStyleAttribute(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(m_viewerStyle.LinkActiveColor));
		}
		writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "none");
	}

	protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
	{
		ScriptControlDescriptor scriptControlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._TextButton", ClientID);
		scriptControlDescriptor.AddProperty("IsActive", !ShowDisabled);
		if (m_viewerStyle.LinkActive != null)
		{
			scriptControlDescriptor.AddProperty("ActiveLinkStyle", m_viewerStyle.LinkActive);
			scriptControlDescriptor.AddProperty("DisabledLinkStyle", m_viewerStyle.LinkDisabled);
		}
		else
		{
			scriptControlDescriptor.AddProperty("ActiveLinkColor", ColorTranslator.ToHtml(m_viewerStyle.LinkActiveColor));
			scriptControlDescriptor.AddProperty("DisabledLinkColor", ColorTranslator.ToHtml(m_viewerStyle.LinkDisabledColor));
			scriptControlDescriptor.AddProperty("ActiveHoverLinkColor", ColorTranslator.ToHtml(m_viewerStyle.LinkActiveHoverColor));
		}
		string text = m_clientSideClickScript ?? Page.ClientScript.GetPostBackEventReference(this, null);
		string script = string.Format(CultureInfo.InvariantCulture, "function(){{{0};}}", text);
		scriptControlDescriptor.AddScriptProperty("OnClickScript", script);
		return new ScriptDescriptor[1] { scriptControlDescriptor };
	}

	protected override IEnumerable<ScriptReference> GetScriptReferences()
	{
		ScriptReference scriptReference = new ScriptReference();
		scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
		return new ScriptReference[1] { scriptReference };
	}
}
