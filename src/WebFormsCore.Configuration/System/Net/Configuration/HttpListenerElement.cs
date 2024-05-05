// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.HttpListenerElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

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
