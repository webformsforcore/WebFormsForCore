// Decompiled with JetBrains decompiler
// Type: System.Configuration.IdnElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;
using System.Globalization;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides the configuration setting for International Domain Name (IDN) processing in the <see cref="T:System.Uri" /> class.</summary>
  public sealed class IdnElement : ConfigurationElement
  {
    internal const UriIdnScope EnabledDefaultValue = UriIdnScope.None;
    private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty enabled = new ConfigurationProperty(nameof (enabled), typeof (UriIdnScope), (object) UriIdnScope.None, (TypeConverter) new IdnElement.UriIdnScopeTypeConverter(), (ConfigurationValidatorBase) null, ConfigurationPropertyOptions.None);

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.IdnElement" /> class.</summary>
    public IdnElement() => this.properties.Add(this.enabled);

    protected internal override ConfigurationPropertyCollection Properties => this.properties;

    /// <summary>Gets or sets the value of the <see cref="T:System.Configuration.IdnElement" /> configuration setting.</summary>
    /// <returns>A <see cref="T:System.UriIdnScope" /> that contains the current configuration setting for IDN processing.</returns>
    [ConfigurationProperty("enabled", DefaultValue = UriIdnScope.None)]
    public UriIdnScope Enabled
    {
      get => (UriIdnScope) this[this.enabled];
      set => this[this.enabled] = (object) value;
    }

    private class UriIdnScopeTypeConverter : TypeConverter
    {
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
        return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
      }

      public override object ConvertFrom(
        ITypeDescriptorContext context,
        CultureInfo culture,
        object value)
      {
        if (value is string str)
        {
          switch (str.ToLower(CultureInfo.InvariantCulture))
          {
            case "all":
              return (object) UriIdnScope.All;
            case "none":
              return (object) UriIdnScope.None;
            case "allexceptintranet":
              return (object) UriIdnScope.AllExceptIntranet;
          }
        }
        return base.ConvertFrom(context, culture, value);
      }
    }
  }
}
