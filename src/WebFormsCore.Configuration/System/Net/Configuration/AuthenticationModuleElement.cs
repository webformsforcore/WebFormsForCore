// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.AuthenticationModuleElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the type information for an authentication module. This class cannot be inherited.</summary>
	public sealed class AuthenticationModuleElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty type = new ConfigurationProperty(nameof(type), typeof(string), (object)null, ConfigurationPropertyOptions.IsKey);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> class.</summary>
		public AuthenticationModuleElement() => this.properties.Add(this.type);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.AuthenticationModuleElement" /> class with the specified type information.</summary>
		/// <param name="typeName">A string that identifies the type and the assembly that contains it.</param>
		public AuthenticationModuleElement(string typeName)
		  : this()
		{
			if (!(typeName != (string)this.type.DefaultValue))
				return;
			this.Type = typeName;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the type and assembly information for the current instance.</summary>
		/// <returns>A string that identifies a type that implements an authentication module or <see langword="null" /> if no value has been specified.</returns>
		[ConfigurationProperty("type", IsRequired = true, IsKey = true)]
		public string Type
		{
			get => (string)this[this.type];
			set => this[this.type] = (object)value;
		}

		internal string Key => this.Type;
	}
}
