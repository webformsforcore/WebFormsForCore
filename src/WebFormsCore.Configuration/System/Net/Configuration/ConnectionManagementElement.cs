// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.ConnectionManagementElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the maximum number of connections to a remote computer. This class cannot be inherited.</summary>
	public sealed class ConnectionManagementElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty address = new ConfigurationProperty(nameof(address), typeof(string), (object)null, ConfigurationPropertyOptions.IsKey);
		private readonly ConfigurationProperty maxconnection = new ConfigurationProperty(nameof(maxconnection), typeof(int), (object)1, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.ConnectionManagementElement" /> class.</summary>
		public ConnectionManagementElement()
		{
			this.properties.Add(this.address);
			this.properties.Add(this.maxconnection);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.ConnectionManagementElement" /> class with the specified address and connection limit information.</summary>
		/// <param name="address">A string that identifies the address of a remote computer.</param>
		/// <param name="maxConnection">An integer that identifies the maximum number of connections allowed to <paramref name="address" /> from the local computer.</param>
		public ConnectionManagementElement(string address, int maxConnection)
		  : this()
		{
			this.Address = address;
			this.MaxConnection = maxConnection;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the address for remote computers.</summary>
		/// <returns>A string that contains a regular expression describing an IP address or DNS name.</returns>
		[ConfigurationProperty("address", IsRequired = true, IsKey = true)]
		public string Address
		{
			get => (string)this[this.address];
			set => this[this.address] = (object)value;
		}

		/// <summary>Gets or sets the maximum number of connections that can be made to a remote computer.</summary>
		/// <returns>An integer that specifies the maximum number of connections.</returns>
		[ConfigurationProperty("maxconnection", IsRequired = true, DefaultValue = 1)]
		public int MaxConnection
		{
			get => (int)this[this.maxconnection];
			set => this[this.maxconnection] = (object)value;
		}

		internal string Key => this.Address;
	}
}
