// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.SmtpSection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Net.Mail;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the SMTP section in the <see langword="System.Net" /> configuration file.</summary>
	public sealed class SmtpSection : ConfigurationSection
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty from = new ConfigurationProperty(nameof(from), typeof(string), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty network = new ConfigurationProperty(nameof(network), typeof(SmtpNetworkElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty specifiedPickupDirectory = new ConfigurationProperty(nameof(specifiedPickupDirectory), typeof(SmtpSpecifiedPickupDirectoryElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty deliveryMethod = new ConfigurationProperty(nameof(deliveryMethod), typeof(SmtpDeliveryMethod), (object)SmtpDeliveryMethod.Network, (TypeConverter)new SmtpSection.SmtpDeliveryMethodTypeConverter(), (ConfigurationValidatorBase)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty deliveryFormat = new ConfigurationProperty(nameof(deliveryFormat), typeof(SmtpDeliveryFormat), (object)SmtpDeliveryFormat.SevenBit, (TypeConverter)new SmtpSection.SmtpDeliveryFormatTypeConverter(), (ConfigurationValidatorBase)null, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.SmtpSection" /> class.</summary>
		public SmtpSection()
		{
			this.properties.Add(this.deliveryMethod);
			this.properties.Add(this.deliveryFormat);
			this.properties.Add(this.from);
			this.properties.Add(this.network);
			this.properties.Add(this.specifiedPickupDirectory);
		}

		/// <summary>Gets or sets the Simple Mail Transport Protocol (SMTP) delivery method. The default delivery method is <see cref="F:System.Net.Mail.SmtpDeliveryMethod.Network" />.</summary>
		/// <returns>A string that represents the SMTP delivery method.</returns>
		[ConfigurationProperty("deliveryMethod", DefaultValue = SmtpDeliveryMethod.Network)]
		public SmtpDeliveryMethod DeliveryMethod
		{
			get => (SmtpDeliveryMethod)this[this.deliveryMethod];
			set => this[this.deliveryMethod] = (object)value;
		}

		/// <summary>Gets or sets the delivery format to use for sending outgoing email using the Simple Mail Transport Protocol (SMTP).</summary>
		/// <returns>Returns <see cref="T:System.Net.Mail.SmtpDeliveryFormat" />.
		/// The delivery format to use for sending outgoing email using SMTP.</returns>
		[ConfigurationProperty("deliveryFormat", DefaultValue = SmtpDeliveryFormat.SevenBit)]
		public SmtpDeliveryFormat DeliveryFormat
		{
			get => (SmtpDeliveryFormat)this[this.deliveryFormat];
			set => this[this.deliveryFormat] = (object)value;
		}

		/// <summary>Gets or sets the default value that indicates who the email message is from.</summary>
		/// <returns>A string that represents the default value indicating who a mail message is from.</returns>
		[ConfigurationProperty("from")]
		public string From
		{
			get => (string)this[this.from];
			set => this[this.from] = (object)value;
		}

		/// <summary>Gets the configuration element that controls the network settings used by the Simple Mail Transport Protocol (SMTP). file.<see cref="T:System.Net.Configuration.SmtpNetworkElement" />.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.SmtpNetworkElement" /> object.
		/// The configuration element that controls the network settings used by SMTP.</returns>
		[ConfigurationProperty("network")]
		public SmtpNetworkElement Network => (SmtpNetworkElement)this[this.network];

		/// <summary>Gets the pickup directory that will be used by the SMPT client.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.SmtpSpecifiedPickupDirectoryElement" /> object that specifies the pickup directory folder.</returns>
		[ConfigurationProperty("specifiedPickupDirectory")]
		public SmtpSpecifiedPickupDirectoryElement SpecifiedPickupDirectory
		{
			get => (SmtpSpecifiedPickupDirectoryElement)this[this.specifiedPickupDirectory];
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		private class SmtpDeliveryMethodTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(
			  ITypeDescriptorContext context,
			  CultureInfo culture,
			  object value)
			{
				if (value is string str)
				{
					switch (str.ToLower(CultureInfo.InvariantCulture))
					{
						case "network":
							return (object)SmtpDeliveryMethod.Network;
						case "specifiedpickupdirectory":
							return (object)SmtpDeliveryMethod.SpecifiedPickupDirectory;
						case "pickupdirectoryfromiis":
							return (object)SmtpDeliveryMethod.PickupDirectoryFromIis;
					}
				}
				return base.ConvertFrom(context, culture, value);
			}
		}

		private class SmtpDeliveryFormatTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(
			  ITypeDescriptorContext context,
			  CultureInfo culture,
			  object value)
			{
				if (value is string str)
				{
					switch (str.ToLower(CultureInfo.InvariantCulture))
					{
						case "sevenbit":
							return (object)SmtpDeliveryFormat.SevenBit;
						case "international":
							return (object)SmtpDeliveryFormat.International;
					}
				}
				return base.ConvertFrom(context, culture, value);
			}
		}
	}
}
