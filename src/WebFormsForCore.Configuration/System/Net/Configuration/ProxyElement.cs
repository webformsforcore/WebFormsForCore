
using System.ComponentModel;
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Identifies the configuration settings for Web proxy server. This class cannot be inherited.</summary>
  public sealed class ProxyElement : ConfigurationElement
  {
    private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty autoDetect = new ConfigurationProperty(nameof (autoDetect), typeof (ProxyElement.AutoDetectValues), (object) ProxyElement.AutoDetectValues.Unspecified, (TypeConverter) new EnumConverter(typeof (ProxyElement.AutoDetectValues)), (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None);
    private readonly ConfigurationProperty scriptLocation = new ConfigurationProperty(nameof (scriptLocation), typeof (Uri), (object) null, (TypeConverter) new UriTypeConverter(), (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None);
    private readonly ConfigurationProperty bypassonlocal = new ConfigurationProperty(nameof (bypassonlocal), typeof (ProxyElement.BypassOnLocalValues), (object) ProxyElement.BypassOnLocalValues.Unspecified, (TypeConverter) new EnumConverter(typeof (ProxyElement.BypassOnLocalValues)), (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None);
    private readonly ConfigurationProperty proxyaddress = new ConfigurationProperty(nameof (proxyaddress), typeof (Uri), (object) null, (TypeConverter) new UriTypeConverter(), (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None);
    private readonly ConfigurationProperty usesystemdefault = new ConfigurationProperty(nameof (usesystemdefault), typeof (ProxyElement.UseSystemDefaultValues), (object) ProxyElement.UseSystemDefaultValues.Unspecified, (TypeConverter) new EnumConverter(typeof (ProxyElement.UseSystemDefaultValues)), (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None);

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.ProxyElement" /> class.</summary>
    public ProxyElement()
    {
      this.properties.Add(this.autoDetect);
      this.properties.Add(this.scriptLocation);
      this.properties.Add(this.bypassonlocal);
      this.properties.Add(this.proxyaddress);
      this.properties.Add(this.usesystemdefault);
    }

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets an <see cref="T:System.Net.Configuration.ProxyElement.AutoDetectValues" /> value that controls whether the Web proxy is automatically detected.</summary>
		/// <returns>
		/// <see cref="F:System.Net.Configuration.ProxyElement.AutoDetectValues.True" /> if the <see cref="T:System.Net.WebProxy" /> is automatically detected; <see cref="F:System.Net.Configuration.ProxyElement.AutoDetectValues.False" /> if the <see cref="T:System.Net.WebProxy" /> is not automatically detected; or <see cref="F:System.Net.Configuration.ProxyElement.AutoDetectValues.Unspecified" />.</returns>
		[ConfigurationProperty("autoDetect", DefaultValue = ProxyElement.AutoDetectValues.Unspecified)]
    public ProxyElement.AutoDetectValues AutoDetect
    {
      get => (ProxyElement.AutoDetectValues) this[this.autoDetect];
      set => this[this.autoDetect] = (object) value;
    }

    /// <summary>Gets or sets an <see cref="T:System.Uri" /> value that specifies the location of the automatic proxy detection script.</summary>
    /// <returns>A <see cref="T:System.Uri" /> specifying the location of the automatic proxy detection script.</returns>
    [ConfigurationProperty("scriptLocation")]
    public Uri ScriptLocation
    {
      get => (Uri) this[this.scriptLocation];
      set => this[this.scriptLocation] = (object) value;
    }

    /// <summary>Gets or sets a value that indicates whether local resources are retrieved by using a Web proxy server.</summary>
    /// <returns>A value that indicates whether local resources are retrieved by using a Web proxy server.</returns>
    [ConfigurationProperty("bypassonlocal", DefaultValue = ProxyElement.BypassOnLocalValues.Unspecified)]
    public ProxyElement.BypassOnLocalValues BypassOnLocal
    {
      get => (ProxyElement.BypassOnLocalValues) this[this.bypassonlocal];
      set => this[this.bypassonlocal] = (object) value;
    }

    /// <summary>Gets or sets the URI that identifies the Web proxy server to use.</summary>
    /// <returns>The URI that identifies the Web proxy server to use.</returns>
    [ConfigurationProperty("proxyaddress")]
    public Uri ProxyAddress
    {
      get => (Uri) this[this.proxyaddress];
      set => this[this.proxyaddress] = (object) value;
    }

    /// <summary>Gets or sets a <see cref="T:System.Boolean" /> value that controls whether the Internet Explorer Web proxy settings are used.</summary>
    /// <returns>
    /// <see langword="true" /> if the Internet Explorer LAN settings are used to detect and configure the default <see cref="T:System.Net.WebProxy" /> used for requests; otherwise, <see langword="false" />.</returns>
    [ConfigurationProperty("usesystemdefault", DefaultValue = ProxyElement.UseSystemDefaultValues.Unspecified)]
    public ProxyElement.UseSystemDefaultValues UseSystemDefault
    {
      get => (ProxyElement.UseSystemDefaultValues) this[this.usesystemdefault];
      set => this[this.usesystemdefault] = (object) value;
    }

    /// <summary>Specifies whether the proxy is bypassed for local resources.</summary>
    public enum BypassOnLocalValues
    {
      /// <summary>Unspecified.</summary>
      Unspecified = -1, // 0xFFFFFFFF
      /// <summary>All requests for local resources should go through the proxy</summary>
      False = 0,
      /// <summary>Access local resources directly.</summary>
      True = 1,
    }

    /// <summary>Specifies whether to use the local system proxy settings to determine whether the proxy is bypassed for local resources.</summary>
    public enum UseSystemDefaultValues
    {
      /// <summary>The system default proxy setting is unspecified.</summary>
      Unspecified = -1, // 0xFFFFFFFF
      /// <summary>Do not use system default proxy setting values</summary>
      False = 0,
      /// <summary>Use system default proxy setting values.</summary>
      True = 1,
    }

    /// <summary>Specifies whether the proxy is automatically detected.</summary>
    public enum AutoDetectValues
    {
      /// <summary>Unspecified.</summary>
      Unspecified = -1, // 0xFFFFFFFF
      /// <summary>The proxy is not automatically detected.</summary>
      False = 0,
      /// <summary>The proxy is automatically detected.</summary>
      True = 1,
    }
  }
}
