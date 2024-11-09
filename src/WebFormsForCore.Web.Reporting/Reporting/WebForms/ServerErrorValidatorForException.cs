// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ServerErrorValidatorForException
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ServerErrorValidatorForException : ServerErrorValidator, INamingContainer
  {
    private ErrorControl m_errorControl;

    public void SetException(Exception e)
    {
      this.EnsureChildControls();
      this.IsValid = e == null;
      this.m_errorControl.SetException(e);
    }

    protected override void CreateChildControls()
    {
      this.Controls.Clear();
      base.CreateChildControls();
      this.m_errorControl = new ErrorControl(false);
      this.Controls.Add((Control) this.m_errorControl);
    }

    public override ControlCollection Controls
    {
      get
      {
        this.EnsureChildControls();
        return base.Controls;
      }
    }

    protected override void Render(HtmlTextWriter writer)
    {
      this.EnsureChildControls();
      if (!this.m_errorControl.HasException)
        return;
      base.Render(writer);
    }
  }
}
