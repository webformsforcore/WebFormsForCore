
using System.ComponentModel;
using System.Configuration;
using System.Net.Security;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the default settings used to create connections to a remote computer. This class cannot be inherited.</summary>
	public sealed class ServicePointManagerElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty checkCertificateName = new ConfigurationProperty(nameof(checkCertificateName), typeof(bool), (object)true, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty checkCertificateRevocationList = new ConfigurationProperty(nameof(checkCertificateRevocationList), typeof(bool), (object)false, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty dnsRefreshTimeout = new ConfigurationProperty(nameof(dnsRefreshTimeout), typeof(int), (object)120000, (TypeConverter)null, (ConfigurationValidatorBase)new TimeoutValidator(true), ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty enableDnsRoundRobin = new ConfigurationProperty(nameof(enableDnsRoundRobin), typeof(bool), (object)false, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty encryptionPolicy = new ConfigurationProperty(nameof(encryptionPolicy), typeof(EncryptionPolicy), (object)EncryptionPolicy.RequireEncryption, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty expect100Continue = new ConfigurationProperty(nameof(expect100Continue), typeof(bool), (object)true, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty useNagleAlgorithm = new ConfigurationProperty(nameof(useNagleAlgorithm), typeof(bool), (object)true, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.ServicePointManagerElement" /> class.</summary>
		public ServicePointManagerElement()
		{
			this.properties.Add(this.checkCertificateName);
			this.properties.Add(this.checkCertificateRevocationList);
			this.properties.Add(this.dnsRefreshTimeout);
			this.properties.Add(this.enableDnsRoundRobin);
			this.properties.Add(this.encryptionPolicy);
			this.properties.Add(this.expect100Continue);
			this.properties.Add(this.useNagleAlgorithm);
		}

		protected override void PostDeserialize()
		{
			if (this.EvaluationContext.IsMachineLevel)
				return;
			PropertyInformation[] propertyInformationArray = new PropertyInformation[2]
			{
		this.ElementInformation.Properties["checkCertificateName"],
		this.ElementInformation.Properties["checkCertificateRevocationList"]
			};
			foreach (PropertyInformation propertyInformation in propertyInformationArray)
			{
				if (propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere)
				{
					try
					{
						ExceptionHelper.UnmanagedPermission.Demand();
					}
					catch (Exception ex)
					{
						throw new ConfigurationErrorsException(SR.GetString("net_config_property_permission", (object)propertyInformation.Name), ex);
					}
				}
			}
		}

		/// <summary>Gets or sets a Boolean value that controls checking host name information in an X509 certificate.</summary>
		/// <returns>
		/// <see langword="true" /> to specify host name checking; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("checkCertificateName", DefaultValue = true)]
		public bool CheckCertificateName
		{
			get => (bool)this[this.checkCertificateName];
			set => this[this.checkCertificateName] = (object)value;
		}

		/// <summary>Gets or sets a Boolean value that indicates whether the certificate is checked against the certificate authority revocation list.</summary>
		/// <returns>
		/// <see langword="true" /> if the certificate revocation list is checked; otherwise, <see langword="false" />.The default value is <see langword="false" />.</returns>
		[ConfigurationProperty("checkCertificateRevocationList", DefaultValue = false)]
		public bool CheckCertificateRevocationList
		{
			get => (bool)this[this.checkCertificateRevocationList];
			set => this[this.checkCertificateRevocationList] = (object)value;
		}

		/// <summary>Gets or sets the amount of time after which address information is refreshed.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> that specifies when addresses are resolved using DNS.</returns>
		[ConfigurationProperty("dnsRefreshTimeout", DefaultValue = 120000)]
		public int DnsRefreshTimeout
		{
			get => (int)this[this.dnsRefreshTimeout];
			set => this[this.dnsRefreshTimeout] = (object)value;
		}

		/// <summary>Gets or sets a Boolean value that controls using different IP addresses on connections to the same server.</summary>
		/// <returns>
		/// <see langword="true" /> to enable DNS round-robin behavior; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enableDnsRoundRobin", DefaultValue = false)]
		public bool EnableDnsRoundRobin
		{
			get => (bool)this[this.enableDnsRoundRobin];
			set => this[this.enableDnsRoundRobin] = (object)value;
		}

		/// <summary>Gets or sets the <see cref="T:System.Net.Security.EncryptionPolicy" /> to use.</summary>
		/// <returns>The encryption policy to use for a <see cref="T:System.Net.ServicePointManager" /> instance.</returns>
		[ConfigurationProperty("encryptionPolicy", DefaultValue = EncryptionPolicy.RequireEncryption)]
		public EncryptionPolicy EncryptionPolicy
		{
			get => (EncryptionPolicy)this[this.encryptionPolicy];
			set => this[this.encryptionPolicy] = (object)value;
		}

		/// <summary>Gets or sets a Boolean value that determines whether 100-Continue behavior is used.</summary>
		/// <returns>
		/// <see langword="true" /> to expect 100-Continue responses for <see langword="POST" /> requests; otherwise, <see langword="false" />. The default value is <see langword="true" />.</returns>
		[ConfigurationProperty("expect100Continue", DefaultValue = true)]
		public bool Expect100Continue
		{
			get => (bool)this[this.expect100Continue];
			set => this[this.expect100Continue] = (object)value;
		}

		/// <summary>Gets or sets a Boolean value that determines whether the Nagle algorithm is used.</summary>
		/// <returns>
		/// <see langword="true" /> to use the Nagle algorithm; otherwise, <see langword="false" />. The default value is <see langword="true" />.</returns>
		[ConfigurationProperty("useNagleAlgorithm", DefaultValue = true)]
		public bool UseNagleAlgorithm
		{
			get => (bool)this[this.useNagleAlgorithm];
			set => this[this.useNagleAlgorithm] = (object)value;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
	}
}
