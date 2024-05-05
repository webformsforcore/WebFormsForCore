// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.AuthenticationModulesSection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for authentication modules. This class cannot be inherited.</summary>
	public sealed class AuthenticationModulesSection : ConfigurationSection
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty authenticationModules = new ConfigurationProperty((string)null, typeof(AuthenticationModuleElementCollection), (object)null, ConfigurationPropertyOptions.IsDefaultCollection);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.AuthenticationModulesSection" /> class.</summary>
		public AuthenticationModulesSection() => this.properties.Add(this.authenticationModules);

		protected override void PostDeserialize()
		{
			if (this.EvaluationContext.IsMachineLevel)
				return;
			try
			{
				ExceptionHelper.UnmanagedPermission.Demand();
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(SR.GetString("net_config_section_permission", (object)"authenticationModules"), ex);
			}
		}

		/// <summary>Gets the collection of authentication modules in the section.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.AuthenticationModuleElementCollection" /> that contains the registered authentication modules.</returns>
		[ConfigurationProperty("", IsDefaultCollection = true)]
		public AuthenticationModuleElementCollection AuthenticationModules
		{
			get => (AuthenticationModuleElementCollection)this[this.authenticationModules];
		}

		protected internal override void InitializeDefault()
		{
#if !WebFormsCore
			this.AuthenticationModules.Add(new AuthenticationModuleElement(typeof(NegotiateClient).AssemblyQualifiedName));
			this.AuthenticationModules.Add(new AuthenticationModuleElement(typeof(KerberosClient).AssemblyQualifiedName));
			this.AuthenticationModules.Add(new AuthenticationModuleElement(typeof(NtlmClient).AssemblyQualifiedName));
			this.AuthenticationModules.Add(new AuthenticationModuleElement(typeof(DigestClient).AssemblyQualifiedName));
			this.AuthenticationModules.Add(new AuthenticationModuleElement(typeof(BasicClient).AssemblyQualifiedName));
#endif 
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
	}
}
