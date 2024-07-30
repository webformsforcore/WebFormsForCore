
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
