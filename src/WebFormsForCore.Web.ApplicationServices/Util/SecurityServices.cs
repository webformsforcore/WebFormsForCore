// Decompiled with JetBrains decompiler
// Type: System.Web.Util.SecurityServices
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

using System.Globalization;

#nullable disable
namespace System.Web.Util
{
  internal static class SecurityServices
  {
    internal static void CheckPasswordParameter(string param, string paramName)
    {
      if (param == null)
        throw new ArgumentNullException(paramName);
      SecurityServices.CheckForEmptyParameter(param, paramName);
    }

    internal static void CheckForEmptyOrWhiteSpaceParameter(ref string param, string paramName)
    {
      if (param == null)
        return;
      param = param.Trim();
      SecurityServices.CheckForEmptyParameter(param, paramName);
    }

    internal static void CheckForEmptyParameter(string param, string paramName)
    {
      if (param.Length < 1)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ApplicationServicesStrings.Parameter_can_not_be_empty, new object[1]
        {
          (object) paramName
        }), paramName);
    }
  }
}
