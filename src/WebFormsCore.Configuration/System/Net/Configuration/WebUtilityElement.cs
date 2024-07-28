
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the WebUtility element in the configuration file.</summary>
	public sealed class WebUtilityElement : ConfigurationElement
	{
		private readonly ConfigurationProperty unicodeDecodingConformance = new ConfigurationProperty(nameof(unicodeDecodingConformance), typeof(UnicodeDecodingConformance), (object)UnicodeDecodingConformance.Auto, (TypeConverter)new WebUtilityElement.EnumTypeConverter<UnicodeDecodingConformance>(), (ConfigurationValidatorBase)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty unicodeEncodingConformance = new ConfigurationProperty(nameof(unicodeEncodingConformance), typeof(UnicodeEncodingConformance), (object)UnicodeEncodingConformance.Auto, (TypeConverter)new WebUtilityElement.EnumTypeConverter<UnicodeEncodingConformance>(), (ConfigurationValidatorBase)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.WebUtilityElement" /> class.</summary>
		public WebUtilityElement()
		{
			this.properties.Add(this.unicodeDecodingConformance);
			this.properties.Add(this.unicodeEncodingConformance);
		}

		/// <summary>Gets the default Unicode decoding conformance behavior used for an <see cref="T:System.Net.WebUtility" /> object.</summary>
		/// <returns>Returns <see cref="T:System.Net.Configuration.UnicodeDecodingConformance" />.
		/// The default Unicode decoding behavior.</returns>
		[ConfigurationProperty("unicodeDecodingConformance", DefaultValue = UnicodeDecodingConformance.Auto)]
		public UnicodeDecodingConformance UnicodeDecodingConformance
		{
			get => (UnicodeDecodingConformance)this[this.unicodeDecodingConformance];
			set => this[this.unicodeDecodingConformance] = (object)value;
		}

		/// <summary>Gets the default Unicode encoding conformance behavior used for an <see cref="T:System.Net.WebUtility" /> object.</summary>
		/// <returns>Returns <see cref="T:System.Net.Configuration.UnicodeEncodingConformance" />.
		/// The default Unicode encoding behavior.</returns>
		[ConfigurationProperty("unicodeEncodingConformance", DefaultValue = UnicodeEncodingConformance.Auto)]
		public UnicodeEncodingConformance UnicodeEncodingConformance
		{
			get => (UnicodeEncodingConformance)this[this.unicodeEncodingConformance];
			set => this[this.unicodeEncodingConformance] = (object)value;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
		private class EnumTypeConverter<TEnum> : TypeConverter where TEnum : struct
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
				TEnum result;
				return value is string str && System.Enum.TryParse<TEnum>(str, true, out result) ? (object)result : base.ConvertFrom(context, culture, value);
			}
		}
	}
}
