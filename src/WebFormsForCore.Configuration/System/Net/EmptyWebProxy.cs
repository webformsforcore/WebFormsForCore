
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
