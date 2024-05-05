// Decompiled with JetBrains decompiler
// Type: System.Net.EmptyWebProxy
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Net
{
  [Serializable]
  internal sealed class EmptyWebProxy : IAutoWebProxy, IWebProxy
  {
    [NonSerialized]
    private ICredentials m_credentials;

    public Uri GetProxy(Uri uri) => uri;

    public bool IsBypassed(Uri uri) => true;

    public ICredentials Credentials
    {
      get => this.m_credentials;
      set => this.m_credentials = value;
    }

    ProxyChain IAutoWebProxy.GetProxies(Uri destination)
    {
      return (ProxyChain) new DirectProxy(destination);
    }
  }
}
