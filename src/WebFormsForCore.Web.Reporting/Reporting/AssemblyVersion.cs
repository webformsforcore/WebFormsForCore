
using System;
using System.Globalization;
using System.Reflection;

#nullable disable
namespace Microsoft.Reporting
{
  internal static class AssemblyVersion
  {
    private static string m_informationalVersion;

    public static string InformationalVersion
    {
      get
      {
        if (AssemblyVersion.m_informationalVersion == null)
        {
          foreach (object customAttribute in Assembly.GetExecutingAssembly().GetCustomAttributes(true))
          {
            if (customAttribute is AssemblyInformationalVersionAttribute versionAttribute)
            {
              AssemblyVersion.m_informationalVersion = versionAttribute.InformationalVersion.ToString((IFormatProvider) CultureInfo.InvariantCulture);
              break;
            }
          }
        }
        return AssemblyVersion.m_informationalVersion != null ? AssemblyVersion.m_informationalVersion : throw new Exception("Internal error: unknown assembly version");
      }
    }
  }
}
