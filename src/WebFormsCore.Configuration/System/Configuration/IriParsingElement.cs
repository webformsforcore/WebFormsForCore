// Decompiled with JetBrains decompiler
// Type: System.Configuration.IriParsingElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides the configuration setting for International Resource Identifier (IRI) processing in the <see cref="T:System.Uri" /> class.</summary>
  public sealed class IriParsingElement : ConfigurationElement
  {
    internal const bool EnabledDefaultValue = false;
    private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty enabled = new ConfigurationProperty(nameof (enabled), typeof (bool), (object) false, ConfigurationPropertyOptions.None);

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.IriParsingElement" /> class.</summary>
    public IriParsingElement() => this.properties.Add(this.enabled);

    protected internal override ConfigurationPropertyCollection Properties => this.properties;

    /// <summary>Gets or sets the value of the <see cref="T:System.Configuration.IriParsingElement" /> configuration setting.</summary>
    /// <returns>A Boolean that indicates if International Resource Identifier (IRI) processing is enabled.</returns>
    [ConfigurationProperty("enabled", DefaultValue = false)]
    public bool Enabled
    {
      get => (bool) this[this.enabled];
      set => this[this.enabled] = (object) value;
    }
  }
}
