using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Permissions;
using System.Web;

namespace Microsoft.Web.Infrastructure;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class HttpContextHelper
{
	[SecuritySafeCritical]
	public static void ExecuteInNullContext(Action action)
	{
		HttpContext current = HttpContext.Current;
		try
		{
			HttpContextHelper.SetCurrentContext((HttpContext)null);
			action();
		}
		finally
		{
			HttpContextHelper.SetCurrentContext(current);
		}
	}

	[SecurityCritical]
	[SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts", Justification = "This is fully critical, and we carefully control the callers.")]
	[PermissionSet(SecurityAction.Assert, Unrestricted = true)]
	private static void SetCurrentContext(HttpContext context) => HttpContext.Current = context;
}
