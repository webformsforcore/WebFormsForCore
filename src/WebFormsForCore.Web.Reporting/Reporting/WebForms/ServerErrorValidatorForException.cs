
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
