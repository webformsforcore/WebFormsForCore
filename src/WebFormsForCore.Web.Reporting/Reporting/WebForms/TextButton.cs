// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.TextButton
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class TextButton : ScriptControl, IPostBackEventHandler
  {
    public bool ShowDisabled = true;
    private string m_clientSideClickScript;
    private string m_tooltip;
    private string m_text;
    private IReportViewerStyles m_viewerStyle;

    public TextButton(string text, string tooltip, IReportViewerStyles viewerStyle)
    {
      this.m_text = text;
      this.m_tooltip = tooltip;
      this.m_viewerStyle = viewerStyle;
      this.Style.Add(HtmlTextWriterStyle.WhiteSpace, "nowrap");
    }

    public event EventHandler Click;

    public void RaisePostBackEvent(string eventArgument)
    {
      if (this.Click == null)
        return;
      this.Click((object) this, (EventArgs) null);
    }

    public string ClientSideClickScript
    {
      get => this.m_clientSideClickScript;
      set => this.m_clientSideClickScript = value;
    }

    public string ClientSideObjectName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$get('{0}').control", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID));
      }
    }

    public string SetEnabledStateScript(string boolShouldEnableScript)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if ($get('{0}') != null && $get('{0}').control != null){1}.SetActive({2});", (object) JavaScriptHelper.StringEscapeSingleQuote(this.ClientID), (object) this.ClientSideObjectName, (object) boolShouldEnableScript);
    }

    public string SetEnabledStateScript(bool shouldEnable)
    {
      return this.SetEnabledStateScript(shouldEnable ? "true" : "false");
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      ScriptManager.GetCurrent(this.Page)?.RegisterScriptDescriptors((IScriptControl) this);
      this.AddAttributesToRender(writer);
      this.AddAttributesForInitialState(writer);
      writer.AddAttribute(HtmlTextWriterAttribute.Title, this.m_tooltip, true);
      writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
      writer.RenderBeginTag(HtmlTextWriterTag.A);
      writer.WriteEncodedText(this.m_text);
      writer.RenderEndTag();
    }

    private void AddAttributesForInitialState(HtmlTextWriter writer)
    {
      if (this.m_viewerStyle.LinkActive != null)
      {
        if (this.ShowDisabled)
          writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_viewerStyle.LinkDisabled);
        else
          writer.AddAttribute(HtmlTextWriterAttribute.Class, this.m_viewerStyle.LinkActive);
      }
      else
      {
        if (this.ShowDisabled)
          writer.AddStyleAttribute(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(this.m_viewerStyle.LinkDisabledColor));
        else
          writer.AddStyleAttribute(HtmlTextWriterStyle.Color, ColorTranslator.ToHtml(this.m_viewerStyle.LinkActiveColor));
        writer.AddStyleAttribute(HtmlTextWriterStyle.TextDecoration, "none");
      }
    }

    protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._TextButton", this.ClientID);
      controlDescriptor.AddProperty("IsActive", (object) !this.ShowDisabled);
      if (this.m_viewerStyle.LinkActive != null)
      {
        controlDescriptor.AddProperty("ActiveLinkStyle", (object) this.m_viewerStyle.LinkActive);
        controlDescriptor.AddProperty("DisabledLinkStyle", (object) this.m_viewerStyle.LinkDisabled);
      }
      else
      {
        controlDescriptor.AddProperty("ActiveLinkColor", (object) ColorTranslator.ToHtml(this.m_viewerStyle.LinkActiveColor));
        controlDescriptor.AddProperty("DisabledLinkColor", (object) ColorTranslator.ToHtml(this.m_viewerStyle.LinkDisabledColor));
        controlDescriptor.AddProperty("ActiveHoverLinkColor", (object) ColorTranslator.ToHtml(this.m_viewerStyle.LinkActiveHoverColor));
      }
      string script = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "function(){{{0};}}", (object) (this.m_clientSideClickScript ?? this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null)));
      controlDescriptor.AddScriptProperty("OnClickScript", script);
      return (IEnumerable<ScriptDescriptor>) new ScriptDescriptor[1]
      {
        (ScriptDescriptor) controlDescriptor
      };
    }

    protected override IEnumerable<ScriptReference> GetScriptReferences()
    {
      ScriptReference scriptReference = new ScriptReference();
      scriptReference.Path = EmbeddedResourceOperation.CreateUrlForScriptFile();
      return (IEnumerable<ScriptReference>) new ScriptReference[1]
      {
        scriptReference
      };
    }
  }
}
