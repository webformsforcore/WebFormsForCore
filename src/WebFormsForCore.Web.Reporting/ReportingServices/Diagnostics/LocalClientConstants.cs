﻿
#nullable disable
namespace Microsoft.ReportingServices.Diagnostics
{
  internal static class LocalClientConstants
  {
    private static readonly string m_clientNotLocalHeaderName = "RSClientNotLocalHeader";

    public static string ClientNotLocalHeaderName
    {
      get => LocalClientConstants.m_clientNotLocalHeaderName;
    }
  }
}
