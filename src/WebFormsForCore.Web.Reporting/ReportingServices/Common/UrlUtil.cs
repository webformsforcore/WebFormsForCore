﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.ReportingServices.Common.UrlUtil
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;

#nullable disable
namespace Microsoft.ReportingServices.Common
{
  internal static class UrlUtil
  {
    public static string UrlEncode(string input)
    {
      return input == null ? (string) null : Uri.EscapeDataString(input);
    }

    public static string UrlDecode(string input)
    {
      if (input == null)
        return (string) null;
      input = input.Replace("+", " ");
      return Uri.UnescapeDataString(input);
    }
  }
}
