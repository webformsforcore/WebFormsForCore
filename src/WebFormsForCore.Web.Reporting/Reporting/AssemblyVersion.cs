// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.AssemblyVersion
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Microsoft.Reporting
{
  internal static class AssemblyVersion
  {
    private static string m_informationalVersion;

    public static string InformationalVersion
    {
      get
      {
        if (AssemblyVersion.m_informationalVersion == null)
        {
          foreach (object customAttribute in Assembly.GetExecutingAssembly().GetCustomAttributes(true))
          {
            if (customAttribute is AssemblyInformationalVersionAttribute versionAttribute)
            {
              AssemblyVersion.m_informationalVersion = versionAttribute.InformationalVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              break;
            }
          }
        }
        return AssemblyVersion.m_informationalVersion != null ? AssemblyVersion.m_informationalVersion : throw new Exception("Internal error: unknown assembly version");
      }
    }
  }
}
