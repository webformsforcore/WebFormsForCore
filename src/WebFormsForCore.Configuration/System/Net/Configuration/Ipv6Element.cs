
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Determines whether Internet Protocol version 6 is enabled on the local computer. This class cannot be inherited.</summary>
	public sealed class Ipv6Element : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty enabled = new ConfigurationProperty(nameof(enabled), typeof(bool), (object)false, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.Ipv6Element" /> class.</summary>
		public Ipv6Element() => this.properties.Add(this.enabled);

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets a Boolean value that indicates whether Internet Protocol version 6 is enabled on the local computer.</summary>
		/// <returns>
		/// <see langword="true" /> if IPv6 is enabled; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enabled", DefaultValue = false)]
		public bool Enabled
		{
			get => (bool)this[this.enabled];
			set => this[this.enabled] = (object)value;
		}
	}
}
