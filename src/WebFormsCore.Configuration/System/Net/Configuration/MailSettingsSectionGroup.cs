
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
