
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal abstract class HandlerOperation : IDisposable
  {
    public virtual bool IsCacheable => false;

    public virtual void Dispose() => GC.SuppressFinalize((object) this);

    public abstract void PerformOperation(NameValueCollection urlQuery, HttpResponse response);

    protected static string GetAndEnsureParam(NameValueCollection urlQuery, string paramName)
    {
      // ISSUE: reference to a compiler-generated method
      return urlQuery[paramName] ?? throw new HttpHandlerInputException(Errors.MissingUrlParameter(paramName));
    }

    protected static int ParseRequiredInt(NameValueCollection urlQuery, string paramName)
    {
      int result;
      if (int.TryParse(HandlerOperation.GetAndEnsureParam(urlQuery, paramName), NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return result;
      throw new HttpHandlerInputException((Exception) new FormatException());
    }

    protected static int ParseOptionalInt(string paramValueStr)
    {
      if (paramValueStr == null)
        return 0;
      int result;
      if (int.TryParse(paramValueStr, out result))
        return result;
      throw new HttpHandlerInputException((Exception) new FormatException());
    }

    protected static bool ParseRequiredBool(NameValueCollection urlQuery, string paramName)
    {
      bool result;
      if (bool.TryParse(HandlerOperation.GetAndEnsureParam(urlQuery, paramName), out result))
        return result;
      throw new HttpHandlerInputException((Exception) new FormatException());
    }

    protected static object ParseRequiredEnum(
      NameValueCollection urlQuery,
      string paramName,
      Type enumType)
    {
      string andEnsureParam = HandlerOperation.GetAndEnsureParam(urlQuery, paramName);
      try
      {
        return Enum.Parse(enumType, andEnsureParam);
      }
      catch (Exception ex)
      {
        throw new HttpHandlerInputException(ex);
      }
    }
  }
}
