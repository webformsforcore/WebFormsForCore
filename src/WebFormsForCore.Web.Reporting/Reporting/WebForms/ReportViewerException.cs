// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ReportViewerException
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  [StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
  public abstract class ReportViewerException : Exception
  {
    protected ReportViewerException(string message)
      : base(message)
    {
    }

    protected ReportViewerException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ReportViewerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
