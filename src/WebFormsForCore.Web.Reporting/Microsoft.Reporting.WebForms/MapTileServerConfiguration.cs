using System;
using System.ComponentModel;

namespace Microsoft.Reporting.WebForms;

[TypeConverter(typeof(TypeNameHidingExpandableObjectConverter))]
public sealed class MapTileServerConfiguration
{
	private LocalProcessingHostMapTileServerConfiguration m_underlyingConfiguration;

	[NotifyParentProperty(true)]
	[SRDescription("MapTileServerConfigurationMaxConnectionsDesc")]
	[DefaultValue(2)]
	public int MaxConnections
	{
		get
		{
			return m_underlyingConfiguration.MaxConnections;
		}
		set
		{
			m_underlyingConfiguration.MaxConnections = value;
		}
	}

	[DefaultValue(10)]
	[NotifyParentProperty(true)]
	[SRDescription("MapTileServerConfigurationTimeoutDesc")]
	public int Timeout
	{
		get
		{
			return m_underlyingConfiguration.Timeout;
		}
		set
		{
			m_underlyingConfiguration.Timeout = value;
		}
	}

	[DefaultValue("(Default)")]
	[SRDescription("MapTileServerConfigurationAppIDDesc")]
	[NotifyParentProperty(true)]
	public string AppID
	{
		get
		{
			return m_underlyingConfiguration.AppID;
		}
		set
		{
			m_underlyingConfiguration.AppID = value;
		}
	}

	internal MapTileServerConfiguration(LocalProcessingHostMapTileServerConfiguration underlyingConfiguration)
	{
		if (underlyingConfiguration == null)
		{
			throw new ArgumentNullException("underlyingConfiguration");
		}
		m_underlyingConfiguration = underlyingConfiguration;
	}
}
