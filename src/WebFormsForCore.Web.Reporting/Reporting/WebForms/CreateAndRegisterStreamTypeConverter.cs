﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.CreateAndRegisterStreamTypeConverter
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using Microsoft.ReportingServices.Interfaces;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal static class CreateAndRegisterStreamTypeConverter
  {
    internal static CreateAndRegisterStream ToInnerType(this CreateAndRegisterStream callback)
    {
      return (CreateAndRegisterStream) ((name, extension, encoding, mimeType, willSeek, operation) => callback.Invoke(name, extension, encoding, mimeType, willSeek, operation));
    }

    internal static CreateAndRegisterStream ToOuterType(this CreateAndRegisterStream callback)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: method pointer
      return new CreateAndRegisterStream((object) new CreateAndRegisterStreamTypeConverter.\u003C\u003Ec__DisplayClass4()
      {
        callback = callback
      }, __methodptr(\u003CToOuterType\u003Eb__3));
    }
  }
}