
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the maximum length for response headers. This class cannot be inherited.</summary>
	public sealed class HttpWebRequestElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty maximumResponseHeadersLength = new ConfigurationProperty(nameof(maximumResponseHeadersLength), typeof(int), (object)64, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty maximumErrorResponseLength = new ConfigurationProperty(nameof(maximumErrorResponseLength), typeof(int), (object)64, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty maximumUnauthorizedUploadLength = new ConfigurationProperty(nameof(maximumUnauthorizedUploadLength), typeof(int), (object)-1, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty useUnsafeHeaderParsing = new ConfigurationProperty(nameof(useUnsafeHeaderParsing), typeof(bool), (object)false, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.HttpWebRequestElement" /> class.</summary>
		public HttpWebRequestElement()
		{
			this.properties.Add(this.maximumResponseHeadersLength);
			this.properties.Add(this.maximumErrorResponseLength);
			this.properties.Add(this.maximumUnauthorizedUploadLength);
			this.properties.Add(this.useUnsafeHeaderParsing);
		}

		protected override void PostDeserialize()
		{
			if (this.EvaluationContext.IsMachineLevel)
				return;
			PropertyInformation[] propertyInformationArray = new PropertyInformation[2]
			{
		this.ElementInformation.Properties["maximumResponseHeadersLength"],
		this.ElementInformation.Properties["maximumErrorResponseLength"]
			};
			foreach (PropertyInformation propertyInformation in propertyInformationArray)
			{
				if (propertyInformation.ValueOrigin == PropertyValueOrigin.SetHere)
				{
					try
					{
						ExceptionHelper.WebPermissionUnrestricted.Demand();
					}
					catch (Exception ex)
					{
						throw new ConfigurationErrorsException(SR.GetString("net_config_property_permission", (object)propertyInformation.Name), ex);
					}
				}
			}
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the maximum length of an upload in response to an unauthorized error code.</summary>
		/// <returns>A 32-bit signed integer containing the maximum length (in multiple of 1,024 byte units) of an upload in response to an unauthorized error code. A value of -1 indicates that no size limit will be imposed on the upload. Setting the <see cref="P:System.Net.Configuration.HttpWebRequestElement.MaximumUnauthorizedUploadLength" /> property to any other value will only send the request body if it is smaller than the number of bytes specified. So a value of 1 would indicate to only send the request body if it is smaller than 1,024 bytes.
		/// The default value for this property is -1.</returns>
		[ConfigurationProperty("maximumUnauthorizedUploadLength", DefaultValue = -1)]
		public int MaximumUnauthorizedUploadLength
		{
			get => (int)this[this.maximumUnauthorizedUploadLength];
			set => this[this.maximumUnauthorizedUploadLength] = (object)value;
		}

		/// <summary>Gets or sets the maximum allowed length of an error response.</summary>
		/// <returns>A 32-bit signed integer containing the maximum length in kilobytes (1024 bytes) of the error response. The default value is 64.</returns>
		[ConfigurationProperty("maximumErrorResponseLength", DefaultValue = 64)]
		public int MaximumErrorResponseLength
		{
			get => (int)this[this.maximumErrorResponseLength];
			set => this[this.maximumErrorResponseLength] = (object)value;
		}

		/// <summary>Gets or sets the maximum allowed length of the response headers.</summary>
		/// <returns>A 32-bit signed integer containing the maximum length in kilobytes (1024 bytes) of the response headers. The default value is 64.</returns>
		[ConfigurationProperty("maximumResponseHeadersLength", DefaultValue = 64)]
		public int MaximumResponseHeadersLength
		{
			get => (int)this[this.maximumResponseHeadersLength];
			set => this[this.maximumResponseHeadersLength] = (object)value;
		}

		/// <summary>Setting this property ignores validation errors that occur during HTTP parsing.</summary>
		/// <returns>Boolean that indicates whether this property has been set.</returns>
		[ConfigurationProperty("useUnsafeHeaderParsing", DefaultValue = false)]
		public bool UseUnsafeHeaderParsing
		{
			get => (bool)this[this.useUnsafeHeaderParsing];
			set => this[this.useUnsafeHeaderParsing] = (object)value;
		}
	}
}
