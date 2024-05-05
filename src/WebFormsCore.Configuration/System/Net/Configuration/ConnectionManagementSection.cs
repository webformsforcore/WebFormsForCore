// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.ConnectionManagementSection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for connection management. This class cannot be inherited.</summary>
	public sealed class ConnectionManagementSection : ConfigurationSection
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty connectionManagement = new ConfigurationProperty((string)null, typeof(ConnectionManagementElementCollection), (object)null, ConfigurationPropertyOptions.IsDefaultCollection);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.ConnectionManagementSection" /> class.</summary>
		public ConnectionManagementSection() => this.properties.Add(this.connectionManagement);

		/// <summary>Gets the collection of connection management objects in the section.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.ConnectionManagementElementCollection" /> that contains the connection management information for the local computer.</returns>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public ConnectionManagementElementCollection ConnectionManagement
		{
			get => (ConnectionManagementElementCollection)this[this.connectionManagement];
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
	}
}
