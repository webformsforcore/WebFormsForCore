// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.MailSettingsSectionGroup
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.MailSettingsSectionGroup" /> class.</summary>
  public sealed class MailSettingsSectionGroup : ConfigurationSectionGroup
  {
    /// <summary>Gets the SMTP settings for the local computer.</summary>
    /// <returns>A <see cref="T:System.Net.Configuration.SmtpSection" /> object that contains configuration information for the local computer.</returns>
    public SmtpSection Smtp => (SmtpSection) this.Sections["smtp"];
  }
}
