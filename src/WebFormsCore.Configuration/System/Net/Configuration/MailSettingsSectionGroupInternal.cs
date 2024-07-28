
#nullable disable
namespace System.Net.Configuration
{
  internal sealed class MailSettingsSectionGroupInternal
  {
    private SmtpSectionInternal smtp;

    internal MailSettingsSectionGroupInternal() => this.smtp = SmtpSectionInternal.GetSection();

    internal SmtpSectionInternal Smtp => this.smtp;

    internal static MailSettingsSectionGroupInternal GetSection()
    {
      return new MailSettingsSectionGroupInternal();
    }
  }
}
