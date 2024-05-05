// Decompiled with JetBrains decompiler
// Type: System.Configuration.ConfigurationBuilderCollection
// Assembly: System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F80D3B8-83DB-4C4E-BE29-E92F4607776E
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Configuration\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Configuration.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Configuration.xml

using System.Configuration.Provider;

#nullable disable
namespace System.Configuration
{
  /// <summary>Maintains a collection of <see cref="T:System.Configuration.ConfigurationBuilder" /> objects by name.</summary>
  public class ConfigurationBuilderCollection : ProviderCollection
  {
    /// <summary>Adds a <see cref="T:System.Configuration.ConfigurationBuilder" /> object to the <see cref="T:System.Configuration.ConfigurationBuilderCollection" /> object.</summary>
    /// <param name="builder">The <see cref="T:System.Configuration.ConfigurationBuilder" /> object to add to the collection.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="builder" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentException">The configuration provider in <paramref name="builder" /> must implement the class <see cref="T:System.Configuration.ConfigurationBuilder" />.</exception>
    public override void Add(ProviderBase builder)
    {
      if (builder == null)
        throw new ArgumentNullException(nameof (builder));
      if (!(builder is ConfigurationBuilder))
        throw new ArgumentException(SR.GetString("Config_provider_must_implement_type", (object) typeof (ConfigurationBuilder).ToString()), nameof (builder));
      base.Add(builder);
    }

    /// <summary>Gets the <see cref="T:System.Configuration.ConfigurationBuilder" /> object from the <see cref="T:System.Configuration.ConfigurationBuilderCollection" /> that is configured with the provided name.</summary>
    /// <param name="name">A configuration builder name.</param>
    /// <returns>The <see cref="T:System.Configuration.ConfigurationBuilder" /> object that is configured with the provided <paramref name="name" />.</returns>
    public ConfigurationBuilder this[string name] => (ConfigurationBuilder) base[name];
  }
}
