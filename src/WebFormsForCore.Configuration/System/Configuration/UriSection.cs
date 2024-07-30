
#nullable disable
namespace System.Configuration
{
  /// <summary>Represents the Uri section within a configuration file.</summary>
  public sealed class UriSection : ConfigurationSection
  {
    private static readonly ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
    private static readonly ConfigurationProperty idn = new ConfigurationProperty(nameof (idn), typeof (IdnElement), (object) null, ConfigurationPropertyOptions.None);
    private static readonly ConfigurationProperty iriParsing = new ConfigurationProperty(nameof (iriParsing), typeof (IriParsingElement), (object) null, ConfigurationPropertyOptions.None);
    private static readonly ConfigurationProperty schemeSettings = new ConfigurationProperty(nameof (schemeSettings), typeof (SchemeSettingElementCollection), (object) null, ConfigurationPropertyOptions.None);

    static UriSection()
    {
      UriSection.properties.Add(UriSection.idn);
      UriSection.properties.Add(UriSection.iriParsing);
      UriSection.properties.Add(UriSection.schemeSettings);
    }

    /// <summary>Gets an <see cref="T:System.Configuration.IdnElement" /> object that contains the configuration setting for International Domain Name (IDN) processing in the <see cref="T:System.Uri" /> class.</summary>
    /// <returns>The configuration setting for International Domain Name (IDN) processing in the <see cref="T:System.Uri" /> class.</returns>
    [ConfigurationProperty("idn")]
    public IdnElement Idn => (IdnElement) this[UriSection.idn];

    /// <summary>Gets an <see cref="T:System.Configuration.IriParsingElement" /> object that contains the configuration setting for International Resource Identifiers (IRI) parsing in the <see cref="T:System.Uri" /> class.</summary>
    /// <returns>The configuration setting for International Resource Identifiers (IRI) parsing in the <see cref="T:System.Uri" /> class.</returns>
    [ConfigurationProperty("iriParsing")]
    public IriParsingElement IriParsing => (IriParsingElement) this[UriSection.iriParsing];

    /// <summary>Gets a <see cref="T:System.Configuration.SchemeSettingElementCollection" /> object that contains the configuration settings for scheme parsing in the <see cref="T:System.Uri" /> class.</summary>
    /// <returns>The configuration settings for scheme parsing in the <see cref="T:System.Uri" /> class</returns>
    [ConfigurationProperty("schemeSettings")]
    public SchemeSettingElementCollection SchemeSettings
    {
      get => (SchemeSettingElementCollection) this[UriSection.schemeSettings];
    }

    protected internal override ConfigurationPropertyCollection Properties => UriSection.properties;
  }
}
