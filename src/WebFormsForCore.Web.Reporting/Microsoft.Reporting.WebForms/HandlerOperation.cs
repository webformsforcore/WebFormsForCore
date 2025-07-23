using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace Microsoft.Reporting.WebForms;

internal abstract class HandlerOperation : IDisposable
{
	public virtual bool IsCacheable => false;

	public virtual void Dispose()
	{
		GC.SuppressFinalize(this);
	}

	public abstract void PerformOperation(NameValueCollection urlQuery, HttpResponse response);

	protected static string GetAndEnsureParam(NameValueCollection urlQuery, string paramName)
	{
		string text = urlQuery[paramName];
		if (text == null)
		{
			throw new HttpHandlerInputException(Errors.MissingUrlParameter(paramName));
		}
		return text;
	}

	protected static int ParseRequiredInt(NameValueCollection urlQuery, string paramName)
	{
		string andEnsureParam = GetAndEnsureParam(urlQuery, paramName);
		if (int.TryParse(andEnsureParam, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		throw new HttpHandlerInputException(new FormatException());
	}

	protected static int ParseOptionalInt(string paramValueStr)
	{
		if (paramValueStr == null)
		{
			return 0;
		}
		if (int.TryParse(paramValueStr, out var result))
		{
			return result;
		}
		throw new HttpHandlerInputException(new FormatException());
	}

	protected static bool ParseRequiredBool(NameValueCollection urlQuery, string paramName)
	{
		string andEnsureParam = GetAndEnsureParam(urlQuery, paramName);
		if (bool.TryParse(andEnsureParam, out var result))
		{
			return result;
		}
		throw new HttpHandlerInputException(new FormatException());
	}

	protected static object ParseRequiredEnum(NameValueCollection urlQuery, string paramName, Type enumType)
	{
		string andEnsureParam = GetAndEnsureParam(urlQuery, paramName);
		try
		{
			return Enum.Parse(enumType, andEnsureParam);
		}
		catch (Exception e)
		{
			throw new HttpHandlerInputException(e);
		}
	}
}
