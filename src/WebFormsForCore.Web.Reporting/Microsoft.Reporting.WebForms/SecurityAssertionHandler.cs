using System;
using System.Globalization;
using System.Security;

namespace Microsoft.Reporting.WebForms;

internal static class SecurityAssertionHandler
{
	private static bool m_assumeFullTrust = true;

	[SecurityCritical]
	[SecurityTreatAsSafe]
	internal static void RunWithSecurityAssert(CodeAccessPermission permission, Action action)
	{
		if (m_assumeFullTrust)
		{
			try
			{
				permission.Assert();
			}
			catch (InvalidOperationException ex)
			{
				string.Format(CultureInfo.InvariantCulture, "Security Assertions disabled due to exception: {0}", ex);
				m_assumeFullTrust = false;
			}
		}
		action();
	}

	internal static TResult RunWithSecurityAssert<TResult>(CodeAccessPermission permission, Func<TResult> action)
	{
		TResult ret = default(TResult);
		RunWithSecurityAssert(permission, delegate
		{
			ret = action();
		});
		return ret;
	}
}
