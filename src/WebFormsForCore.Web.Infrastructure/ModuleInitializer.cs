using System.Security;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Web.Infrastructure;

internal static class ModuleInitializer
{
	[SecuritySafeCritical]
	[ModuleInitializer]
	public static void Initialize() => ModuleInitializer.CriticalInitializer.Initialize();

	[SecurityCritical]
	private static class CriticalInitializer
	{
		private static int _initializeCalled;

		public static void Initialize()
		{
			if (Interlocked.Exchange(ref ModuleInitializer.CriticalInitializer._initializeCalled, 1) != 0)
				return;
			ModuleInitializer.CriticalInitializer.CheckKillBit();
		}

		private static void CheckKillBit()
		{
			if (OSInfo.IsWindows)
			{
				using (KillBitHelper killBitHelper = new KillBitHelper())
					killBitHelper.ThrowIfKillBitIsSet();
			}
		}
	}
}
