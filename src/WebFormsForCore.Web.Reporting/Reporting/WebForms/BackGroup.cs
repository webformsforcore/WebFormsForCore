﻿
using System;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class BackGroup : ToolbarGroup
  {
    private ScriptSwitchImage m_backButton;

    public BackGroup(ReportViewer viewer)
      : base(viewer)
    {
    }

    public override string GroupCssClassName => (string) null;

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      this.m_backButton = new ScriptSwitchImage(new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.BackEnabled.gif", "Microsoft.Reporting.WebForms.Icons.BackRTLEnabled.gif"), new ToolbarImageInfo("Microsoft.Reporting.WebForms.Icons.BackDisabled.gif", "Microsoft.Reporting.WebForms.Icons.BackRTLDisabled.gif"), true, LocalizationHelper.Current.BackButtonToolTip, this.m_viewer);
      this.m_backButton.ClickImage1 += new EventHandler(this.BackButton_Click);
      this.Controls.Add((Control) this.m_backButton);
    }

    protected override void OnPreRender(EventArgs e)
    {
      base.OnPreRender(e);
      if (this.m_viewer.InteractivityPostBackMode == InteractivityPostBackMode.AlwaysAsynchronous)
        return;
      ScriptManager.GetCurrent(this.Page).RegisterPostBackControl((Control) this.m_backButton);
    }

    private void BackButton_Click(object sender, EventArgs e)
    {
      this.OnReportAction(new ReportActionEventArgs("Back", (string) null));
    }

    public override void AddScriptDescriptorProperties(ScriptControlDescriptor toolbarDesc)
    {
      this.EnsureChildControls();
      toolbarDesc.AddProperty("IsDrillthroughReport", (object) this.m_viewer.Report.IsDrillthroughReport);
      toolbarDesc.AddElementProperty("DrillBackButton", this.m_backButton.ClientID);
    }
  }
}
