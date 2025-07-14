using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Runtime.InteropServices;

namespace Microsoft.Web.Infrastructure;

[SecurityCritical]
internal sealed class KillBitHelper : IDisposable
{
	private const string _killBitRegKeyName = "SOFTWARE\\Microsoft\\ASP.NET\\Security\\DisableLoadList\\";
	private const string _killBitFwdLink = "http://go.microsoft.com/fwlink/?LinkID=204759";
	private static readonly Version _thisAssemblyFileVersion = KillBitHelper.GetThisAssemblyFileVersion();
	private readonly IRegistryKey _localMachine;

	[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposing of this instance will dispose of the appropriate fields.")]
	public KillBitHelper()
	  : this((IRegistryKey)new RegistryKeyWrapper(Registry.LocalMachine))
	{
	}

	internal KillBitHelper(IRegistryKey localMachine) => this._localMachine = localMachine;

	[SecuritySafeCritical]
	public void Dispose() => this._localMachine.Dispose();

	private List<Version> GetKillBittedVersions()
	{
		List<Version> killBittedVersions = new List<Version>();
		new RegistryPermission(RegistryPermissionAccess.Read, this._localMachine.ToString() + "\\SOFTWARE\\Microsoft\\ASP.NET\\Security\\DisableLoadList\\").Assert();
		try
		{
			IRegistryKey registryKey1 = this._localMachine.OpenSubKey("SOFTWARE\\Microsoft\\ASP.NET\\Security\\DisableLoadList\\");
			if (registryKey1 != null)
			{
				using (registryKey1)
				{
					foreach (string subKeyName in registryKey1.GetSubKeyNames())
					{
						if (subKeyName.StartsWith("Microsoft.Web.Infrastructure", StringComparison.OrdinalIgnoreCase))
						{
							IRegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName);
							using (registryKey2)
							{
								if (registryKey2.GetValue("Flags") is int num)
								{
									if (num != 0)
									{
										string input = (string)registryKey2.GetValue("FileVersion");
										if (!string.IsNullOrEmpty(input))
											killBittedVersions.Add(Version.Parse(input));
									}
								}
							}
						}
					}
				}
			}
		}
		finally
		{
			CodeAccessPermission.RevertAssert();
		}
		return killBittedVersions;
	}

	private static Version GetThisAssemblyFileVersion()
	{
		return Version.Parse(CommonAssemblies.MicrosoftWebInfrastructure.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false).Cast<AssemblyFileVersionAttribute>().Single<AssemblyFileVersionAttribute>().Version);
	}

	internal bool IsThisAssemblyKillBitted()
	{
		return this.GetKillBittedVersions().Any<Version>(new Func<Version, bool>(KillBitHelper.KillBitMatchesThisAssemblyVersion));
	}

	private static bool KillBitMatchesThisAssemblyVersion(Version killBittedVersion)
	{
		return KillBitHelper._thisAssemblyFileVersion.Major == killBittedVersion.Major && KillBitHelper._thisAssemblyFileVersion.Minor == killBittedVersion.Minor && KillBitHelper._thisAssemblyFileVersion.Build <= killBittedVersion.Build;
	}

	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "LinkID", Justification = "Represents a URL segment.")]
	[SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "fwlink", Justification = "Represents a URL segment.")]
	public void ThrowIfKillBitIsSet()
	{
		if (this.IsThisAssemblyKillBitted())
			throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.InvariantCulture, "Kill bit was set. Please see {0} for more information.", new object[1]
			{
		(object) "http://go.microsoft.com/fwlink/?LinkID=204759"
			}));
	}
}
