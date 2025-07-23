using System;
using System.Globalization;
using System.Reflection;

namespace Microsoft.Reporting;

internal static class AssemblyVersion
{
	private static string m_informationalVersion;

	public static string InformationalVersion
	{
		get
		{
			if (m_informationalVersion == null)
			{
				object[] customAttributes = Assembly.GetExecutingAssembly().GetCustomAttributes(inherit: true);
				object[] array = customAttributes;
				foreach (object obj in array)
				{
					if (obj is AssemblyInformationalVersionAttribute assemblyInformationalVersionAttribute)
					{
						m_informationalVersion = assemblyInformationalVersionAttribute.InformationalVersion.ToString(CultureInfo.InvariantCulture);
						break;
					}
				}
			}
			if (m_informationalVersion == null)
			{
				throw new Exception("Internal error: unknown assembly version");
			}
			return m_informationalVersion;
		}
	}
}
