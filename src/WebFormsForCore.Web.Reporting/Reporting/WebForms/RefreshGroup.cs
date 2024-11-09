// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.RefreshGroup
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class RefreshGroup : ToolbarGroup
  {
    private ScriptSwitchImage m_refreshButton;

    public RefreshGroup(ReportViewer viewer)
      : base(viewer)
    {
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarRefresh;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_refreshButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.Refresh.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.RefreshDisabled.gif"), true, LocalizationHelper.Current.RefreshButtonToolTip, this.m_viewer);
      this.Controls.Add((Control) this.m_refreshButton);
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("RefreshButton", this.m_refreshButton.ClientID);
    }

    public override string LeadingSpace => ToolbarGroup.ReducedLeadingSpace;
  }
}
