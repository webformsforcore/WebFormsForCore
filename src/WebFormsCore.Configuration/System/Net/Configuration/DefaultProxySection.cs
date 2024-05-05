// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.DefaultProxySection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for Web proxy server usage. This class cannot be inherited.</summary>
	public sealed class DefaultProxySection : ConfigurationSection
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty bypasslist = new ConfigurationProperty(nameof(bypasslist), typeof(BypassElementCollection), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty module = new ConfigurationProperty(nameof(module), typeof(ModuleElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty proxy = new ConfigurationProperty(nameof(proxy), typeof(ProxyElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty enabled = new ConfigurationProperty(nameof(enabled), typeof(bool), (object)true, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty useDefaultCredentials = new ConfigurationProperty(nameof(useDefaultCredentials), typeof(bool), (object)false, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.DefaultProxySection" /> class.</summary>
		public DefaultProxySection()
		{
			this.properties.Add(this.bypasslist);
			this.properties.Add(this.module);
			this.properties.Add(this.proxy);
			this.properties.Add(this.enabled);
			this.properties.Add(this.useDefaultCredentials);
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
				throw new ConfigurationErrorsException(SR.GetString("net_config_section_permission", (object)"defaultProxy"), ex);
			}
		}

		/// <summary>Gets the collection of resources that are not obtained using the Web proxy server.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.BypassElementCollection" /> that contains the addresses of resources that bypass the Web proxy server.</returns>
		[ConfigurationProperty("bypasslist")]
		public BypassElementCollection BypassList => (BypassElementCollection)this[this.bypasslist];

		/// <summary>Gets the type information for a custom Web proxy implementation.</summary>
		/// <returns>The type information for a custom Web proxy implementation.</returns>
		[ConfigurationProperty("module")]
		public ModuleElement Module => (ModuleElement)this[this.module];

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets the URI that identifies the Web proxy server to use.</summary>
		/// <returns>The URI that identifies the Web proxy server.</returns>
		[ConfigurationProperty("proxy")]
		public ProxyElement Proxy => (ProxyElement)this[this.proxy];

		/// <summary>Gets or sets whether a Web proxy is used.</summary>
		/// <returns>
		/// <see langword="true" /> if a Web proxy will be used; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("enabled", DefaultValue = true)]
		public bool Enabled
		{
			get => (bool)this[this.enabled];
			set => this[this.enabled] = (object)value;
		}

		/// <summary>Gets or sets whether default credentials are to be used to access a Web proxy server.</summary>
		/// <returns>
		/// <see langword="true" /> if default credentials are to be used; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("useDefaultCredentials", DefaultValue = false)]
		public bool UseDefaultCredentials
		{
			get => (bool)this[this.useDefaultCredentials];
			set => this[this.useDefaultCredentials] = (object)value;
		}

		protected internal override void Reset(ConfigurationElement parentElement)
		{
			DefaultProxySection parentElement1 = new DefaultProxySection();
			parentElement1.InitializeDefault();
			base.Reset((ConfigurationElement)parentElement1);
		}
	}
}
