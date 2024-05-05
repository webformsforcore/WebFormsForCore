// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.SmtpNetworkElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;
using System.Configuration;
using System.Net.Mail;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the network element in the SMTP configuration file. This class cannot be inherited.</summary>
	public sealed class SmtpNetworkElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty defaultCredentials = new ConfigurationProperty(nameof(defaultCredentials), typeof(bool), (object)false, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty host = new ConfigurationProperty(nameof(host), typeof(string), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty clientDomain = new ConfigurationProperty(nameof(clientDomain), typeof(string), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty password = new ConfigurationProperty(nameof(password), typeof(string), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty port = new ConfigurationProperty(nameof(port), typeof(int), (object)25, (TypeConverter)null, (ConfigurationValidatorBase)new IntegerValidator(1, (int)ushort.MaxValue), ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty userName = new ConfigurationProperty(nameof(userName), typeof(string), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty targetName = new ConfigurationProperty(nameof(targetName), typeof(string), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty enableSsl = new ConfigurationProperty(nameof(enableSsl), typeof(bool), (object)false, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.SmtpNetworkElement" /> class.</summary>
		public SmtpNetworkElement()
		{
			this.properties.Add(this.defaultCredentials);
			this.properties.Add(this.host);
			this.properties.Add(this.clientDomain);
			this.properties.Add(this.password);
			this.properties.Add(this.port);
			this.properties.Add(this.userName);
			this.properties.Add(this.targetName);
			this.properties.Add(this.enableSsl);
		}

		protected override void PostDeserialize()
		{
			if (this.EvaluationContext.IsMachineLevel)
				return;
			PropertyInformation property = this.ElementInformation.Properties["port"];
			if (property.ValueOrigin != PropertyValueOrigin.SetHere)
				return;
			if ((int)property.Value == (int)property.DefaultValue)
				return;
			try
			{
				new SmtpPermission(SmtpAccess.ConnectToUnrestrictedPort).Demand();
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(SR.GetString("net_config_property_permission", (object)property.Name), ex);
			}
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Determines whether or not default user credentials are used to access an SMTP server. The default value is <see langword="false" />.</summary>
		/// <returns>
		/// <see langword="true" /> indicates that default user credentials will be used to access the SMTP server; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("defaultCredentials", DefaultValue = false)]
		public bool DefaultCredentials
		{
			get => (bool)this[this.defaultCredentials];
			set => this[this.defaultCredentials] = (object)value;
		}

		/// <summary>Gets or sets the name of the SMTP server.</summary>
		/// <returns>A string that represents the name of the SMTP server to connect to.</returns>
		[ConfigurationProperty("host")]
		public string Host
		{
			get => (string)this[this.host];
			set => this[this.host] = (object)value;
		}

		/// <summary>Gets or sets the Service Provider Name (SPN) to use for authentication when using extended protection to connect to an SMTP mail server.</summary>
		/// <returns>A string that represents the SPN to use for authentication when using extended protection to connect to an SMTP mail server.</returns>
		[ConfigurationProperty("targetName")]
		public string TargetName
		{
			get => (string)this[this.targetName];
			set => this[this.targetName] = (object)value;
		}

		/// <summary>Gets or sets the client domain name used in the initial SMTP protocol request to connect to an SMTP mail server.</summary>
		/// <returns>A string that represents the client domain name used in the initial SMTP protocol request to connect to an SMTP mail server.</returns>
		[ConfigurationProperty("clientDomain")]
		public string ClientDomain
		{
			get => (string)this[this.clientDomain];
			set => this[this.clientDomain] = (object)value;
		}

		/// <summary>Gets or sets the user password to use to connect to an SMTP mail server.</summary>
		/// <returns>A string that represents the password to use to connect to an SMTP mail server.</returns>
		[ConfigurationProperty("password")]
		public string Password
		{
			get => (string)this[this.password];
			set => this[this.password] = (object)value;
		}

		/// <summary>Gets or sets the port that SMTP clients use to connect to an SMTP mail server. The default value is 25.</summary>
		/// <returns>A string that represents the port to connect to an SMTP mail server.</returns>
		[ConfigurationProperty("port", DefaultValue = 25)]
		public int Port
		{
			get => (int)this[this.port];
			set => this[this.port] = (object)value;
		}

		/// <summary>Gets or sets the user name to connect to an SMTP mail server.</summary>
		/// <returns>A string that represents the user name to connect to an SMTP mail server.</returns>
		[ConfigurationProperty("userName")]
		public string UserName
		{
			get => (string)this[this.userName];
			set => this[this.userName] = (object)value;
		}

		/// <summary>Gets or sets whether SSL is used to access an SMTP mail server. The default value is <see langword="false" />.</summary>
		/// <returns>
		/// <see langword="true" /> indicates that SSL will be used to access the SMTP mail server; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enableSsl", DefaultValue = false)]
		public bool EnableSsl
		{
			get => (bool)this[this.enableSsl];
			set => this[this.enableSsl] = (object)value;
		}
	}
}
