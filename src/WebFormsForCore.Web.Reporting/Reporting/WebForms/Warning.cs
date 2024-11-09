// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.Warning
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class Warning
  {
    internal Warning(
      string code,
      string message,
      string objectName,
      string objectType,
      string severity)
    {
      this.Code = code;
      this.Message = message;
      this.ObjectName = objectName;
      this.ObjectType = objectType;
      if (string.Compare(severity, nameof (Warning), StringComparison.OrdinalIgnoreCase) == 0)
        this.Severity = Severity.Warning;
      else
        this.Severity = Severity.Error;
    }

    internal static Warning[] FromSoapWarnings(Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.Warning[] soapWarnings)
    {
      if (soapWarnings == null)
        return new Warning[0];
      Warning[] warningArray = new Warning[soapWarnings.Length];
      for (int index = 0; index < soapWarnings.Length; ++index)
        warningArray[index] = new Warning(soapWarnings[index].Code, soapWarnings[index].Message, soapWarnings[index].ObjectName, soapWarnings[index].ObjectType, soapWarnings[index].Severity);
      return warningArray;
    }

    internal static Warning[] FromProcessingMessageList(ProcessingMessageList processingWarnings)
    {
      if (processingWarnings == null)
        return new Warning[0];
      Warning[] warningArray = new Warning[((ArrayList) processingWarnings).Count];
      for (int index = 0; index < ((ArrayList) processingWarnings).Count; ++index)
        warningArray[index] = new Warning(processingWarnings[index].Code.ToString(), processingWarnings[index].Message, processingWarnings[index].ObjectName, processingWarnings[index].ObjectType.ToString(), processingWarnings[index].Severity.ToString());
      return warningArray;
    }

    public string Code { get; private set; }

    public string Message { get; private set; }

    public string ObjectName { get; private set; }

    public string ObjectType { get; private set; }

    public Severity Severity { get; private set; }
  }
}
