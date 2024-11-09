// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.MapTileServerConfiguration
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

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
