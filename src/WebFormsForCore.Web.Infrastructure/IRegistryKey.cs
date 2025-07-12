using System;
using System.Security;

namespace Microsoft.Web.Infrastructure;

[SecurityCritical]
internal interface IRegistryKey : IDisposable
{
	string[] GetSubKeyNames();

	object GetValue(string name);

	IRegistryKey OpenSubKey(string name);
}
