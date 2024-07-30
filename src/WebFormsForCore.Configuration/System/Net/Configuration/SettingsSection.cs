
using System.Configuration;
using System.Net.Cache;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for sockets, IPv6, response headers, and service points. This class cannot be inherited.</summary>
	public sealed class SettingsSection : ConfigurationSection
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty httpWebRequest = new ConfigurationProperty(nameof(httpWebRequest), typeof(HttpWebRequestElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty ipv6 = new ConfigurationProperty(nameof(ipv6), typeof(Ipv6Element), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty servicePointManager = new ConfigurationProperty(nameof(servicePointManager), typeof(ServicePointManagerElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty socket = new ConfigurationProperty(nameof(socket), typeof(SocketElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty webProxyScript = new ConfigurationProperty(nameof(webProxyScript), typeof(WebProxyScriptElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty performanceCounters = new ConfigurationProperty(nameof(performanceCounters), typeof(PerformanceCountersElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty httpListener = new ConfigurationProperty(nameof(httpListener), typeof(HttpListenerElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty webUtility = new ConfigurationProperty(nameof(webUtility), typeof(WebUtilityElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty windowsAuthentication = new ConfigurationProperty(nameof(windowsAuthentication), typeof(WindowsAuthenticationElement), (object)null, ConfigurationPropertyOptions.None);

		internal static void EnsureConfigLoaded()
		{
			try
			{
				object obj;
#if !WebFormsForCore
				AuthenticationManager.EnsureConfigLoaded();
				obj = (object)RequestCacheManager.IsCachingEnabled;
#endif
				obj = (object)System.Net.ServicePointManager.DefaultConnectionLimit;
				obj = (object)System.Net.ServicePointManager.Expect100Continue;
#if !WebFormsForCore
				obj = (object)WebRequest.PrefixList;
				obj = (object)WebRequest.InternalDefaultWebProxy;
#endif
			}
			catch
			{
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.ConnectionManagementSection" /> class.</summary>
		public SettingsSection()
		{
			this.properties.Add(this.httpWebRequest);
			this.properties.Add(this.ipv6);
			this.properties.Add(this.servicePointManager);
			this.properties.Add(this.socket);
			this.properties.Add(this.webProxyScript);
			this.properties.Add(this.performanceCounters);
			this.properties.Add(this.httpListener);
			this.properties.Add(this.webUtility);
			this.properties.Add(this.windowsAuthentication);
		}

		/// <summary>Gets the configuration element that controls the settings used by an <see cref="T:System.Net.HttpWebRequest" /> object.</summary>
		/// <returns>The configuration element that controls the maximum response header length and other settings used by an <see cref="T:System.Net.HttpWebRequest" /> object.</returns>
		[ConfigurationProperty("httpWebRequest")]
		public HttpWebRequestElement HttpWebRequest
		{
			get => (HttpWebRequestElement)this[this.httpWebRequest];
		}

		/// <summary>Gets the configuration element that enables Internet Protocol version 6 (IPv6).</summary>
		/// <returns>The configuration element that controls the setting used by IPv6.</returns>
		[ConfigurationProperty("ipv6")]
		public Ipv6Element Ipv6 => (Ipv6Element)this[this.ipv6];

		/// <summary>Gets the configuration element that controls settings for connections to remote host computers.</summary>
		/// <returns>The configuration element that controls settings for connections to remote host computers.</returns>
		[ConfigurationProperty("servicePointManager")]
		public ServicePointManagerElement ServicePointManager
		{
			get => (ServicePointManagerElement)this[this.servicePointManager];
		}

		/// <summary>Gets the configuration element that controls settings for sockets.</summary>
		/// <returns>The configuration element that controls settings for sockets.</returns>
		[ConfigurationProperty("socket")]
		public SocketElement Socket => (SocketElement)this[this.socket];

		/// <summary>Gets the configuration element that controls the execution timeout and download timeout of Web proxy scripts.</summary>
		/// <returns>The configuration element that controls settings for the execution timeout and download timeout used by the Web proxy scripts.</returns>
		[ConfigurationProperty("webProxyScript")]
		public WebProxyScriptElement WebProxyScript
		{
			get => (WebProxyScriptElement)this[this.webProxyScript];
		}

		/// <summary>Gets the configuration element that controls whether network performance counters are enabled.</summary>
		/// <returns>The configuration element that controls usage of network performance counters.</returns>
		[ConfigurationProperty("performanceCounters")]
		public PerformanceCountersElement PerformanceCounters
		{
			get => (PerformanceCountersElement)this[this.performanceCounters];
		}

		/// <summary>Gets the configuration element that controls the settings used by an <see cref="T:System.Net.HttpListener" /> object.</summary>
		/// <returns>An <see cref="T:System.Net.Configuration.HttpListenerElement" /> object.
		/// The configuration element that controls the settings used by an <see cref="T:System.Net.HttpListener" /> object.</returns>
		[ConfigurationProperty("httpListener")]
		public HttpListenerElement HttpListener => (HttpListenerElement)this[this.httpListener];

		/// <summary>Gets the configuration element that controls the settings used by an <see cref="T:System.Net.WebUtility" /> object.</summary>
		/// <returns>The configuration element that controls the settings used by a <see cref="T:System.Net.WebUtility" /> object.</returns>
		[ConfigurationProperty("webUtility")]
		public WebUtilityElement WebUtility => (WebUtilityElement)this[this.webUtility];

		/// <summary>Gets the configuration element that controls the number of handles for default network credentials.</summary>
		/// <returns>The configuration element that controls the number of handles for default network credentials.</returns>
		[ConfigurationProperty("windowsAuthentication")]
		public WindowsAuthenticationElement WindowsAuthentication
		{
			get => (WindowsAuthenticationElement)this[this.windowsAuthentication];
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
	}
}
