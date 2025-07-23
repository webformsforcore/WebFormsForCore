using System.Text.RegularExpressions;

namespace Microsoft.Reporting.WebForms;

internal static class SPUtility
{
	public static bool IsValidStringInput(string regexp, string newValue)
	{
		Regex regex = new Regex(regexp);
		if (string.IsNullOrEmpty(newValue) || !regex.IsMatch(newValue))
		{
			return false;
		}
		return true;
	}
}
