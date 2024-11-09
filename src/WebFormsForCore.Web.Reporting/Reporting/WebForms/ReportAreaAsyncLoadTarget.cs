// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportAreaAsyncLoadTarget
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal class ReportAreaAsyncLoadTarget : ScriptControl, IPostBackEventHandler
  {
    private bool m_causePostBack;

    public event EventHandler PostBackTarget;

    public void TriggerImmediatePostBack() => this.m_causePostBack = true;

    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
      if (this.PostBackTarget == null)
        return;
      this.PostBackTarget((object) this, EventArgs.Empty);
    }

    protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
    {
      if (!this.m_causePostBack)
        return (IEnumerable<ScriptDescriptor>) null;
      ScriptControlDescriptor controlDescriptor = new ScriptControlDescriptor("Microsoft.Reporting.WebFormsClient._ReportAreaAsyncLoadTarget", this.ClientID);
      string script = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "function() {{{0};}}", (object) this.Page.ClientScript.GetPostBackEventReference((Control) this, (string) null));
      controlDescriptor.AddScriptProperty("PostBackForAsyncLoad", script);
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
