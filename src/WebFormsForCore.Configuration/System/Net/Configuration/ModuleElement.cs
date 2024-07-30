
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the type information for a custom <see cref="T:System.Net.IWebProxy" /> module. This class cannot be inherited.</summary>
	public sealed class ModuleElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty type = new ConfigurationProperty(nameof(type), typeof(string), (object)null, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.ModuleElement" /> class.</summary>
		public ModuleElement() => this.properties.Add(this.type);
		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the type and assembly information for the current instance.</summary>
		/// <returns>A string that identifies a type that implements the <see cref="T:System.Net.IWebProxy" /> interface or <see langword="null" /> if no value has been specified.</returns>
		[ConfigurationProperty("type")]
		public string Type
		{
			get => (string)this[this.type];
			set => this[this.type] = (object)value;
		}
	}
}
