using Microsoft.Win32;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Microsoft.Web.Infrastructure;

[SecurityCritical]
internal sealed class RegistryKeyWrapper : IRegistryKey, IDisposable
{
	private readonly RegistryKey _registryKey;

	public RegistryKeyWrapper(RegistryKey registryKey) => this._registryKey = registryKey;

	[SecuritySafeCritical]
	public void Dispose() => this._registryKey.Dispose();

	[SecurityCritical]
	public string[] GetSubKeyNames() => this._registryKey.GetSubKeyNames();

	[SecurityCritical]
	public object GetValue(string name) => this._registryKey.GetValue(name);

	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The caller is responsible for calling Dispose.")]
	[SecurityCritical]
	public IRegistryKey OpenSubKey(string name)
	{
		RegistryKey registryKey = this._registryKey.OpenSubKey(name);
		return registryKey == null ? (IRegistryKey)null : (IRegistryKey)new RegistryKeyWrapper(registryKey);
	}

	[SecuritySafeCritical]
	public override string ToString() => this._registryKey.ToString();
}
