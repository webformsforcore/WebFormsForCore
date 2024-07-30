
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents an SMTP pickup directory configuration element.</summary>
	public sealed class SmtpSpecifiedPickupDirectoryElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty pickupDirectoryLocation = new ConfigurationProperty(nameof(pickupDirectoryLocation), typeof(string), (object)null, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.SmtpSpecifiedPickupDirectoryElement" /> class.</summary>
		public SmtpSpecifiedPickupDirectoryElement()
		{
			this.properties.Add(this.pickupDirectoryLocation);
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the folder where applications save mail messages to be processed by the SMTP server.</summary>
		/// <returns>A string that specifies the pickup directory for email messages.</returns>
		[ConfigurationProperty("pickupDirectoryLocation")]
		public string PickupDirectoryLocation
		{
			get => (string)this[this.pickupDirectoryLocation];
			set => this[this.pickupDirectoryLocation] = (object)value;
		}
	}
}
