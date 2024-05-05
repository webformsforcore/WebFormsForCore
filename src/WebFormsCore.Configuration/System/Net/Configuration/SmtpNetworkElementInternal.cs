// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.SmtpNetworkElementInternal
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
