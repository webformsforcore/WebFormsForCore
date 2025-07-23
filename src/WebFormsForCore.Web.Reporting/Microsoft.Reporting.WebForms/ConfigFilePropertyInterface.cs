using System;
using System.Configuration;

namespace Microsoft.Reporting.WebForms;

internal sealed class ConfigFilePropertyInterface<InterfaceType> where InterfaceType : class
{
	private string m_propertyName;

	private string m_interfaceTypeName;

	private bool m_propertyLoaded;

	private Type m_propertyType;

	public ConfigFilePropertyInterface(string propertyName, string interfaceTypeName)
	{
		m_propertyName = propertyName;
		m_interfaceTypeName = interfaceTypeName;
	}

	public InterfaceType GetInstance()
	{
		EnsurePropertyLoaded();
		if (m_propertyType == null)
		{
			return null;
		}
		return (InterfaceType)Activator.CreateInstance(m_propertyType);
	}

	private void EnsurePropertyLoaded()
	{
		if (m_propertyLoaded || m_propertyName == null)
		{
			return;
		}
		string text = ConfigurationManager.AppSettings[m_propertyName];
		if (!string.IsNullOrEmpty(text))
		{
			Type type = Type.GetType(text);
			if (type == null)
			{
				throw new InvalidConfigFileTypeException(text);
			}
			if (!typeof(InterfaceType).IsAssignableFrom(type))
			{
				throw new InvalidConfigFileTypeException(text, m_interfaceTypeName);
			}
			m_propertyType = type;
		}
		m_propertyLoaded = true;
	}
}
