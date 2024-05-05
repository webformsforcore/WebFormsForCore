using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Configuration
{
	internal class WebProxyWrapperOpaque : IAutoWebProxy, IWebProxy
	{
		protected readonly WebProxy webProxy;
		internal WebProxyWrapperOpaque(WebProxy webProxy) => this.webProxy = webProxy;

		public Uri GetProxy(Uri destination) => this.webProxy.GetProxy(destination);

		public bool IsBypassed(Uri host) => this.webProxy.IsBypassed(host);

		public ICredentials Credentials
		{
			get => this.webProxy.Credentials;
			set => this.webProxy.Credentials = value;
		}

		public ProxyChain GetProxies(Uri destination)
		{
			return ((IAutoWebProxy)this.webProxy).GetProxies(destination);
		}
	}

	internal class WebProxyWrapper : WebProxyWrapperOpaque
	{
		internal WebProxyWrapper(WebProxy webProxy)
		  : base(webProxy)
		{
		}

		internal WebProxy WebProxy => this.webProxy;
	}
}