
using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [TypeConverter(typeof (TypeNameHidingExpandableObjectConverter))]
  public sealed class MapTileServerConfiguration
  {
    private LocalProcessingHostMapTileServerConfiguration m_underlyingConfiguration;

    internal MapTileServerConfiguration(
      LocalProcessingHostMapTileServerConfiguration underlyingConfiguration)
    {
      this.m_underlyingConfiguration = underlyingConfiguration != null ? underlyingConfiguration : throw new ArgumentNullException(nameof (underlyingConfiguration));
    }

    [NotifyParentProperty(true)]
    [Microsoft.Reporting.SRDescription("MapTileServerConfigurationMaxConnectionsDesc")]
    [DefaultValue(2)]
    public int MaxConnections
    {
      get => this.m_underlyingConfiguration.MaxConnections;
      set => this.m_underlyingConfiguration.MaxConnections = value;
    }

    [DefaultValue(10)]
    [NotifyParentProperty(true)]
    [Microsoft.Reporting.SRDescription("MapTileServerConfigurationTimeoutDesc")]
    public int Timeout
    {
      get => this.m_underlyingConfiguration.Timeout;
      set => this.m_underlyingConfiguration.Timeout = value;
    }

    [DefaultValue("(Default)")]
    [Microsoft.Reporting.SRDescription("MapTileServerConfigurationAppIDDesc")]
    [NotifyParentProperty(true)]
    public string AppID
    {
      get => this.m_underlyingConfiguration.AppID;
      set => this.m_underlyingConfiguration.AppID = value;
    }
  }
}
