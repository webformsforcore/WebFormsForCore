// Decompiled with JetBrains decompiler
// Type: System.Configuration.Internal.IInternalConfigurationBuilderHost
// Assembly: System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F80D3B8-83DB-4C4E-BE29-E92F4607776E
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Configuration\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Configuration.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Configuration.xml

using System.Runtime.InteropServices;
using System.Xml;

#nullable disable
namespace System.Configuration.Internal
{
	/// <summary>Defines the supplemental interface to <see cref="T:System.Configuration.Internal.IInternalConfigHost" /> for configuration hosts that wish to support the application of <see cref="T:System.Configuration.ConfigurationBuilder" /> objects.</summary>
	[ComVisible(false)]
	public interface IInternalConfigurationBuilderHost
	{
		/// <summary>Processes the markup of a configuration section using the provided <see cref="T:System.Configuration.ConfigurationBuilder" />.</summary>
		/// <param name="rawXml">The <see cref="T:System.Xml.XmlNode" /> to process.</param>
		/// <param name="builder">
		/// <see cref="T:System.Configuration.ConfigurationBuilder" /> to use to process the <paramref name="rawXml" />.</param>
		/// <returns>The processed <see cref="T:System.Xml.XmlNode" />.</returns>
		XmlNode ProcessRawXml(XmlNode rawXml, ConfigurationBuilder builder);

		/// <summary>Processes a <see cref="T:System.Configuration.ConfigurationSection" /> object using the provided <see cref="T:System.Configuration.ConfigurationBuilder" />.</summary>
		/// <param name="configSection">The <see cref="T:System.Configuration.ConfigurationSection" /> to process.</param>
		/// <param name="builder">
		/// <see cref="T:System.Configuration.ConfigurationBuilder" /> to use to process the <paramref name="configSection" />.</param>
		/// <returns>The processed <see cref="T:System.Configuration.ConfigurationSection" />.</returns>
		ConfigurationSection ProcessConfigurationSection(
		  ConfigurationSection configSection,
		  ConfigurationBuilder builder);
	}
}
