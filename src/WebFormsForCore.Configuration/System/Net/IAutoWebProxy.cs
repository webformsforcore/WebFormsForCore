
#nullable disable
namespace System.Net
{
	internal interface IAutoWebProxy : IWebProxy
	{
		ProxyChain GetProxies(Uri destination);
	}
}
