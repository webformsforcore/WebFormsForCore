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
