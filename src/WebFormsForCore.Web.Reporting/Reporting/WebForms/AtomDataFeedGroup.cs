// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.AtomDataFeedGroup
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class AtomDataFeedGroup : ToolbarGroup
  {
    private ScriptSwitchImage m_atomDataFeedButton;

    public AtomDataFeedGroup(ReportViewer viewer)
      : base(viewer)
    {
    }

    public bool IsSupported
    {
      get
      {
        foreach (RenderingExtension renderingExtension in this.GetClientSupportedRenderingExtensions())
        {
          if (renderingExtension.Name.Equals("ATOM", StringComparison.OrdinalIgnoreCase))
            return true;
        }
        return false;
      }
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarAtomDataFeed;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_atomDataFeedButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.AtomDataFeed.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.AtomDataFeedDisabled.gif"), true, Strings.AtomDataFeedTooltip, this.m_viewer);
      this.m_atomDataFeedButton.ShowImage2 = true;
      this.Controls.Add((Control) this.m_atomDataFeedButton);
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("AtomDataFeedButton", this.m_atomDataFeedButton.ClientID);
    }

    public override string LeadingSpace => ToolbarGroup.ReducedLeadingSpace;
  }
}
