// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.PerformanceCountersElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the performance counter element in the <see langword="System.Net" /> configuration file that determines whether networking performance counters are enabled. This class cannot be inherited.</summary>
	public sealed class PerformanceCountersElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty enabled = new ConfigurationProperty(nameof(enabled), typeof(bool), (object)false, ConfigurationPropertyOptions.None);

		/// <summary>Instantiates a <see cref="T:System.Net.Configuration.PerformanceCountersElement" /> object.</summary>
		public PerformanceCountersElement() => this.properties.Add(this.enabled);

		/// <summary>Gets or sets whether performance counters are enabled.</summary>
		/// <returns>
		/// <see langword="true" /> if performance counters are enabled; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enabled", DefaultValue = false)]
		public bool Enabled
		{
			get => (bool)this[this.enabled];
			set => this[this.enabled] = (object)value;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
	}
}
