
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the HttpListener element in the configuration file. This class cannot be inherited.</summary>
	public sealed class HttpListenerElement : ConfigurationElement
	{
		internal const bool UnescapeRequestUrlDefaultValue = true;
		private static ConfigurationPropertyCollection properties;
		private static readonly ConfigurationProperty unescapeRequestUrl = new ConfigurationProperty(nameof(unescapeRequestUrl), typeof(bool), (object)true, ConfigurationPropertyOptions.None);
		private static readonly ConfigurationProperty timeouts = new ConfigurationProperty(nameof(timeouts), typeof(HttpListenerTimeoutsElement), (object)null, ConfigurationPropertyOptions.None);

		static HttpListenerElement()
		{
			HttpListenerElement.properties = new ConfigurationPropertyCollection();
			HttpListenerElement.properties.Add(HttpListenerElement.unescapeRequestUrl);
			HttpListenerElement.properties.Add(HttpListenerElement.timeouts);
		}

		/// <summary>Gets a value that indicates if <see cref="T:System.Net.HttpListener" /> uses the raw unescaped URI instead of the converted URI.</summary>
		/// <returns>A Boolean value that indicates if <see cref="T:System.Net.HttpListener" /> uses the raw unescaped URI, rather than the converted URI.</returns>
		[ConfigurationProperty("unescapeRequestUrl", DefaultValue = true, IsRequired = false)]
		public bool UnescapeRequestUrl => (bool)this[HttpListenerElement.unescapeRequestUrl];

		/// <summary>Gets the default timeout elements used for an <see cref="T:System.Net.HttpListener" /> object.</summary>
		/// <returns>The timeout elements used for an <see cref="T:System.Net.HttpListener" /> object.</returns>
		[ConfigurationProperty("timeouts")]
		public HttpListenerTimeoutsElement Timeouts
		{
			get => (HttpListenerTimeoutsElement)this[HttpListenerElement.timeouts];
		}

		protected internal override ConfigurationPropertyCollection Properties => properties;
	}
}
