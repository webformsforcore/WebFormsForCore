// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.InvalidConfigFileTypeException
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Configuration;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class InvalidConfigFileTypeException : ConfigurationErrorsException
  {
    internal InvalidConfigFileTypeException(string typeName)
      : base(Microsoft.Reporting.WebForms.Errors.CantFindConfigFileType(typeName))
    {
      // ISSUE: reference to a compiler-generated method (out of statement scope)
    }

    internal InvalidConfigFileTypeException(string typeName, string expectedTypeName)
      : base(Microsoft.Reporting.WebForms.Errors.MissingInterfaceOnConfigFileType(typeName, expectedTypeName))
    {
      // ISSUE: reference to a compiler-generated method (out of statement scope)
    }

    private InvalidConfigFileTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
