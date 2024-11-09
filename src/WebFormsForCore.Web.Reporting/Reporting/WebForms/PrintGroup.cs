// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.PrintGroup
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class PrintGroup : ToolbarGroup
  {
    private ScriptSwitchImage m_printButton;

    public PrintGroup(ReportViewer viewer)
      : base(viewer)
    {
    }

    public override string GroupCssClassName => this.m_viewer.ViewerStyle.ToolbarPrint;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_printButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.Print.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.PrintDisabled.gif"), true, LocalizationHelper.Current.PrintButtonToolTip, this.m_viewer);
      this.m_printButton.ShowImage2 = true;
      this.Controls.Add((Control) this.m_printButton);
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddElementProperty("PrintButton", this.m_printButton.ClientID);
    }

    public override string LeadingSpace => ToolbarGroup.ReducedLeadingSpace;
  }
}
