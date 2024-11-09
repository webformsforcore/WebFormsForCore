// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ExportGroup
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Globalization;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ExportGroup : ToolbarGroup
  {
    private DropDownMenu m_exportButton;

    public ExportGroup(ReportViewer viewer)
      : base(viewer)
    {
      this.m_viewer = viewer;
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarExport;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_exportButton = new DropDownMenu(new ButtonImageInfo()
      {
        EnabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.Export.gif"),
        DisabledUrl = EmbeddedResourceOperation.CreateUrl("Microsoft.Reporting.WebForms.Icons.ExportDisabled.gif")
      }, LocalizationHelper.Current.ExportButtonToolTip, this.m_viewer.ViewerStyle);
      this.m_exportButton.ShowEnabled = false;
      this.Controls.Add((Control) this.m_exportButton);
    }

    protected override void OnPreRender(EventArgs e)
    {
      this.EnsureChildControls();
      this.PopulateDropDown();
      base.OnPreRender(e);
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      this.m_exportButton.Font.CopyFrom(this.Font);
      base.Render(writer);
    }

    private void PopulateDropDown()
    {
      this.m_exportButton.Items.Clear();
      this.m_exportButton.Font.CopyFrom(this.Font);
      LocalizationHelper current = (LocalizationHelper) LocalizationHelper.Current;
      foreach (RenderingExtension renderingExtension in this.GetClientSupportedRenderingExtensions())
      {
        if (renderingExtension.Visible)
          this.m_exportButton.Items.Add(new MenuInfo()
          {
            Text = current.GetLocalizedNameForRenderingExtension(renderingExtension),
            ClientScript = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$find('{0}').exportReport('{1}');", (object) JavaScriptHelper.JavaScriptStringEscape(this.m_viewer.ClientID, '\''), (object) JavaScriptHelper.JavaScriptStringEscape(renderingExtension.Name, '\''))
          });
      }
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("ExportButton", this.m_exportButton.ClientID);
    }
  }
}
