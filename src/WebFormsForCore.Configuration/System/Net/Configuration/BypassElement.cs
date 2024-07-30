
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the address information for resources that are not retrieved using a proxy server. This class cannot be inherited.</summary>
	public sealed class BypassElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty address = new ConfigurationProperty(nameof(address), typeof(string), (object)null, ConfigurationPropertyOptions.IsKey);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.BypassElement" /> class.</summary>
		public BypassElement() => this.properties.Add(this.address);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.BypassElement" /> class with the specified type information.</summary>
		/// <param name="address">A string that identifies the address of a resource.</param>
		public BypassElement(string address)
		  : this()
		{
			this.Address = address;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the addresses of resources that bypass the proxy server.</summary>
		/// <returns>A string that identifies a resource.</returns>
		[ConfigurationProperty("address", IsRequired = true, IsKey = true)]
		public string Address
		{
			get => (string)this[this.address];
			set => this[this.address] = (object)value;
		}

		internal string Key => this.Address;
	}
}
