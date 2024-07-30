
#nullable disable
namespace System.Net.Configuration
{
  internal sealed class SmtpNetworkElementInternal
  {
    private string targetname;
    private string host;
    private string clientDomain;
    private int port;
    private NetworkCredential credential;
    private bool enableSsl;

    internal SmtpNetworkElementInternal(SmtpNetworkElement element)
    {
      this.host = element.Host;
      this.port = element.Port;
      this.targetname = element.TargetName;
      this.clientDomain = element.ClientDomain;
      this.enableSsl = element.EnableSsl;
      if (element.DefaultCredentials)
      {
        this.credential = (NetworkCredential) CredentialCache.DefaultCredentials;
      }
      else
      {
        if (element.UserName == null || element.UserName.Length <= 0)
          return;
        this.credential = new NetworkCredential(element.UserName, element.Password);
      }
    }

    internal NetworkCredential Credential => this.credential;

    internal string Host => this.host;

    internal string ClientDomain => this.clientDomain;

    internal int Port => this.port;

    internal string TargetName => this.targetname;

    internal bool EnableSsl => this.enableSsl;
  }
}
