// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.WindowsAuthenticationElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the Windows authentication element in a configuration file. This class cannot be inherited.</summary>
	public sealed class WindowsAuthenticationElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties;
		private readonly ConfigurationProperty defaultCredentialsHandleCacheSize;

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.WindowsAuthenticationElement" /> class.</summary>
		public WindowsAuthenticationElement()
		{
			this.defaultCredentialsHandleCacheSize = new ConfigurationProperty(nameof(defaultCredentialsHandleCacheSize), typeof(int), (object)0, (TypeConverter)null, (ConfigurationValidatorBase)new WindowsAuthenticationElement.CacheSizeValidator(), ConfigurationPropertyOptions.None);
			this.properties = new ConfigurationPropertyCollection();
			this.properties.Add(this.defaultCredentialsHandleCacheSize);
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Defines the default size of the Windows credential handle cache.</summary>
		/// <returns>The default size of the Windows credential handle cache.</returns>
		[ConfigurationProperty("defaultCredentialsHandleCacheSize", DefaultValue = 0)]
		public int DefaultCredentialsHandleCacheSize
		{
			get => (int)this[this.defaultCredentialsHandleCacheSize];
			set => this[this.defaultCredentialsHandleCacheSize] = (object)value;
		}

		private class CacheSizeValidator : ConfigurationValidatorBase
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
