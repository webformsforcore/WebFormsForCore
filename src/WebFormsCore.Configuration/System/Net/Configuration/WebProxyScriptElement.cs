// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.WebProxyScriptElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents information used to configure Web proxy scripts. This class cannot be inherited.</summary>
	public sealed class WebProxyScriptElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty autoConfigUrlRetryInterval = new ConfigurationProperty(nameof(autoConfigUrlRetryInterval), typeof(int), (object)600, (TypeConverter)null, (ConfigurationValidatorBase)new WebProxyScriptElement.RetryIntervalValidator(), ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty downloadTimeout = new ConfigurationProperty(nameof(downloadTimeout), typeof(TimeSpan), (object)TimeSpan.FromMinutes(1.0), (TypeConverter)null, (ConfigurationValidatorBase)new System.Configuration.TimeSpanValidator(new TimeSpan(0, 0, 0), TimeSpan.MaxValue, false), ConfigurationPropertyOptions.None);

		/// <summary>Initializes an instance of the <see cref="T:System.Net.Configuration.WebProxyScriptElement" /> class.</summary>
		public WebProxyScriptElement()
		{
			this.properties.Add(this.autoConfigUrlRetryInterval);
			this.properties.Add(this.downloadTimeout);
		}

		protected override void PostDeserialize()
		{
			if (this.EvaluationContext.IsMachineLevel)
				return;
			try
			{
				ExceptionHelper.WebPermissionUnrestricted.Demand();
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(SR.GetString("net_config_element_permission", (object)"webProxyScript"), ex);
			}
		}

		/// <summary>Gets or sets a value that defines the frequency (in seconds) that the WinHttpAutoProxySvc service attempts to retry the download of an AutoConfigUrl script.</summary>
		/// <returns>the frequency (in seconds) that the WinHttpAutoProxySvc service attempts to retry the download of an AutoConfigUrl script.</returns>
		[ConfigurationProperty("autoConfigUrlRetryInterval", DefaultValue = 600)]
		public int AutoConfigUrlRetryInterval
		{
			get => (int)this[this.autoConfigUrlRetryInterval];
			set => this[this.autoConfigUrlRetryInterval] = (object)value;
		}

		/// <summary>Gets or sets the Web proxy script download timeout using the format hours:minutes:seconds.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> object that contains the timeout value. The default download timeout is one minute.</returns>
		[ConfigurationProperty("downloadTimeout", DefaultValue = "00:01:00")]
		public TimeSpan DownloadTimeout
		{
			get => (TimeSpan)this[this.downloadTimeout];
			set => this[this.downloadTimeout] = (object)value;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		private class RetryIntervalValidator : ConfigurationValidatorBase
		{
			public override bool CanValidate(Type type) => type == typeof(int);

			public override void Validate(object value)
			{
				int actualValue = (int)value;
				if (actualValue < 0)
					throw new ArgumentOutOfRangeException(nameof(value), (object)actualValue, SR.GetString("ArgumentOutOfRange_Bounds_Lower_Upper", (object)0, (object)int.MaxValue));
			}
		}
	}
}
