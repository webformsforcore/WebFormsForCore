// Decompiled with JetBrains decompiler
// Type: System.Net.NclUtilities
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

#nullable disable
namespace System.Net
{
	internal static class NclUtilities
	{

#if !WebFormsCore 
		private static volatile ContextCallback s_ContextRelativeDemandCallback;
		private static volatile IPAddress[] _LocalAddresses;
		private static object _LocalAddressesLock;
		private static volatile NetworkAddressChangePolled s_AddressChange;

		internal static bool IsThreadPoolLow()
		{
			if (ComNetOS.IsAspNetServer)
				return false;
			int workerThreads;
			int completionPortThreads;
			ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
			return workerThreads < 2 || completionPortThreads < 2;
		}

		internal static bool HasShutdownStarted
		{
			get => Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload();
		}

		internal static bool IsCredentialFailure(SecurityStatus error)
		{
			return error == SecurityStatus.LogonDenied || error == SecurityStatus.UnknownCredentials || error == SecurityStatus.NoImpersonation || error == SecurityStatus.NoAuthenticatingAuthority || error == SecurityStatus.UntrustedRoot || error == SecurityStatus.CertExpired || error == SecurityStatus.SmartcardLogonRequired || error == SecurityStatus.BadBinding;
		}

		internal static bool IsClientFault(SecurityStatus error)
		{
			return error == SecurityStatus.InvalidToken || error == SecurityStatus.CannotPack || error == SecurityStatus.QopNotSupported || error == SecurityStatus.NoCredentials || error == SecurityStatus.MessageAltered || error == SecurityStatus.OutOfSequence || error == SecurityStatus.IncompleteMessage || error == SecurityStatus.IncompleteCredentials || error == SecurityStatus.WrongPrincipal || error == SecurityStatus.TimeSkew || error == SecurityStatus.IllegalMessage || error == SecurityStatus.CertUnknown || error == SecurityStatus.AlgorithmMismatch || error == SecurityStatus.SecurityQosFailed || error == SecurityStatus.UnsupportedPreauth;
		}

		internal static ContextCallback ContextRelativeDemandCallback
		{
			get
			{
				if (NclUtilities.s_ContextRelativeDemandCallback == null)
					NclUtilities.s_ContextRelativeDemandCallback = new ContextCallback(NclUtilities.DemandCallback);
				return NclUtilities.s_ContextRelativeDemandCallback;
			}
		}

		private static void DemandCallback(object state) => ((CodeAccessPermission)state).Demand();

		internal static bool GuessWhetherHostIsLoopback(string host)
		{
			string lowerInvariant1 = host.ToLowerInvariant();
			if (lowerInvariant1 == "localhost" || lowerInvariant1 == "loopback")
				return true;
			IPGlobalProperties globalProperties = IPGlobalProperties.InternalGetIPGlobalProperties();
			string lowerInvariant2 = globalProperties.HostName.ToLowerInvariant();
			return lowerInvariant1 == lowerInvariant2 || lowerInvariant1 == lowerInvariant2 + "." + globalProperties.DomainName.ToLowerInvariant();
		}
#endif
		internal static bool IsFatal(Exception exception)
		{
			switch (exception)
			{
				case null:
					return false;
				case OutOfMemoryException _:
				case StackOverflowException _:
					return true;
				default:
					return exception is ThreadAbortException;
			}
		}
#if !WebFormsCore
		internal static IPAddress[] LocalAddresses
		{
			get
			{
				if (NclUtilities.s_AddressChange != null && NclUtilities.s_AddressChange.CheckAndReset())
					return NclUtilities._LocalAddresses = NclUtilities.GetLocalAddresses();
				if (NclUtilities._LocalAddresses != null)
					return NclUtilities._LocalAddresses;
				lock (NclUtilities.LocalAddressesLock)
				{
					if (NclUtilities._LocalAddresses != null)
						return NclUtilities._LocalAddresses;
					NclUtilities.s_AddressChange = new NetworkAddressChangePolled();
					return NclUtilities._LocalAddresses = NclUtilities.GetLocalAddresses();
				}
			}
		}

		private static IPAddress[] GetLocalAddresses()
		{
			ArrayList arrayList = new ArrayList(16);
			int length = 0;
			SafeLocalFree adapterAddresses = (SafeLocalFree)null;
			GetAdaptersAddressesFlags flags = GetAdaptersAddressesFlags.SkipAnycast | GetAdaptersAddressesFlags.SkipMulticast | GetAdaptersAddressesFlags.SkipDnsServer | GetAdaptersAddressesFlags.SkipFriendlyName;
			uint outBufLen = 0;
			uint adaptersAddresses = UnsafeNetInfoNativeMethods.GetAdaptersAddresses(AddressFamily.Unspecified, (uint)flags, IntPtr.Zero, SafeLocalFree.Zero, ref outBufLen);
			while (adaptersAddresses == 111U)
			{
				try
				{
					adapterAddresses = SafeLocalFree.LocalAlloc((int)outBufLen);
					adaptersAddresses = UnsafeNetInfoNativeMethods.GetAdaptersAddresses(AddressFamily.Unspecified, (uint)flags, IntPtr.Zero, adapterAddresses, ref outBufLen);
					IpAdapterAddresses structure;
					if (adaptersAddresses == 0U)
					{
						for (IntPtr ptr = adapterAddresses.DangerousGetHandle(); ptr != IntPtr.Zero; ptr = structure.next)
						{
							structure = (IpAdapterAddresses)Marshal.PtrToStructure(ptr, typeof(IpAdapterAddresses));
							if (structure.firstUnicastAddress != IntPtr.Zero)
							{
								UnicastIPAddressInformationCollection informationCollection = SystemUnicastIPAddressInformation.MarshalUnicastIpAddressInformationCollection(structure.firstUnicastAddress);
								length += informationCollection.Count;
								arrayList.Add((object)informationCollection);
							}
						}
					}
				}
				finally
				{
					adapterAddresses?.Close();
					adapterAddresses = (SafeLocalFree)null;
				}
			}
			if (adaptersAddresses != 0U && adaptersAddresses != 232U)
				throw new NetworkInformationException((int)adaptersAddresses);
			IPAddress[] localAddresses = new IPAddress[length];
			uint num = 0;
			foreach (UnicastIPAddressInformationCollection informationCollection in arrayList)
			{
				foreach (IPAddressInformation addressInformation in informationCollection)
					localAddresses[(int)num++] = addressInformation.Address;
			}
			return localAddresses;
		}

		internal static bool IsAddressLocal(IPAddress ipAddress)
		{
			foreach (object localAddress in NclUtilities.LocalAddresses)
			{
				if (ipAddress.Equals(localAddress, false))
					return true;
			}
			return false;
		}

		private static object LocalAddressesLock
		{
			get
			{
				if (NclUtilities._LocalAddressesLock == null)
					Interlocked.CompareExchange(ref NclUtilities._LocalAddressesLock, new object(), (object)null);
				return NclUtilities._LocalAddressesLock;
			}
		}
#endif
	}
}
