#if NETCOREAPP

using System.Reflection;
using System.Runtime.CompilerServices;
//using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System;

//
// Summary:
//     Provides a managed equivalent of an unmanaged host.
[SecurityCritical]
[ComVisible(true)]
[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.Infrastructure)]
public class AppDomainManager : MarshalByRefObject
{
	//private AppDomainManagerInitializationOptions m_flags;

	//private ApplicationActivator m_appActivator;

	private Assembly m_entryAssembly;

	//
	// Summary:
	//     Gets the initialization flags for custom application domain managers.
	//
	// Returns:
	//     A bitwise combination of the enumeration values that describe the initialization
	//     action to perform. The default is System.AppDomainManagerInitializationOptions.None.
	/* public AppDomainManagerInitializationOptions InitializationFlags
	{
		get
		{
			return m_flags;
		}
		set
		{
			m_flags = value;
		}
	}

	//
	// Summary:
	//     Gets the application activator that handles the activation of add-ins and manifest-based
	//     applications for the domain.
	//
	// Returns:
	//     The application activator.
	public virtual ApplicationActivator ApplicationActivator
	{
		get
		{
			if (m_appActivator == null)
			{
				m_appActivator = new ApplicationActivator();
			}

			return m_appActivator;
		}
	}*/

	//
	// Summary:
	//     Gets the host security manager that participates in security decisions for the
	//     application domain.
	//
	// Returns:
	//     The host security manager.
	public virtual HostSecurityManager HostSecurityManager => null;

	//
	// Summary:
	//     Gets the host execution context manager that manages the flow of the execution
	//     context.
	//
	// Returns:
	//     The host execution context manager.
	public virtual HostExecutionContextManager HostExecutionContextManager => throw new NotSupportedException(); // HostExecutionContextManager.GetInternalHostExecutionContextManager();

	//
	// Summary:
	//     Gets the entry assembly for an application.
	//
	// Returns:
	//     The entry assembly for the application.
	public virtual Assembly EntryAssembly
	{
		[SecurityCritical]
		get
		{
			if (m_entryAssembly == null)
			{
				AppDomain currentDomain = AppDomain.CurrentDomain;
				/*
				if (currentDomain.IsDefaultAppDomain() && currentDomain.ActivationContext != null)
				{
					ManifestRunner manifestRunner = new ManifestRunner(currentDomain, currentDomain.ActivationContext);
					m_entryAssembly = manifestRunner.EntryAssembly;
				}
				else
				{
					RuntimeAssembly o = null;
					GetEntryAssembly(JitHelpers.GetObjectHandleOnStack(ref o));
					m_entryAssembly = o;
				} */
				m_entryAssembly = Assembly.GetEntryAssembly();
			}

			return m_entryAssembly;
		}
	}

	internal static AppDomainManager CurrentAppDomainManager
	{
		[SecurityCritical]
		get
		{
			throw new NotSupportedException();
			//AppDomain.CurrentDomain.DomainManager;
		}
	}

	//
	// Summary:
	//     Initializes a new instance of the System.AppDomainManager class.
	public AppDomainManager()
	{
	}

	//
	// Summary:
	//     Returns a new or existing application domain.
	//
	// Parameters:
	//   friendlyName:
	//     The friendly name of the domain.
	//
	//   securityInfo:
	//     An object that contains evidence mapped through the security policy to establish
	//     a top-of-stack permission set.
	//
	//   appDomainInfo:
	//     An object that contains application domain initialization information.
	//
	// Returns:
	//     A new or existing application domain.
	[SecurityCritical]
	public virtual AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
	{
		return CreateDomainHelper(friendlyName, securityInfo, appDomainInfo);
	}

	//
	// Summary:
	//     Provides a helper method to create an application domain.
	//
	// Parameters:
	//   friendlyName:
	//     The friendly name of the domain.
	//
	//   securityInfo:
	//     An object that contains evidence mapped through the security policy to establish
	//     a top-of-stack permission set.
	//
	//   appDomainInfo:
	//     An object that contains application domain initialization information.
	//
	// Returns:
	//     A newly created application domain.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     friendlyName is null.
	[SecurityCritical]
	[SecurityPermission(SecurityAction.Demand, ControlAppDomain = true)]
	protected static AppDomain CreateDomainHelper(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
	{
		/*
		if (friendlyName == null)
		{
			throw new ArgumentNullException();
		}

		if (securityInfo != null)
		{
			new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			AppDomain.CheckDomainCreationEvidence(appDomainInfo, securityInfo);
		}

		if (appDomainInfo == null)
		{
			appDomainInfo = new AppDomainSetup();
		}

		if (appDomainInfo.AppDomainManagerAssembly == null || appDomainInfo.AppDomainManagerType == null)
		{
			AppDomain.CurrentDomain.GetAppDomainManagerType(out var assembly, out var type);
			if (appDomainInfo.AppDomainManagerAssembly == null)
			{
				appDomainInfo.AppDomainManagerAssembly = assembly;
			}

			if (appDomainInfo.AppDomainManagerType == null)
			{
				appDomainInfo.AppDomainManagerType = type;
			}
		}

		if (appDomainInfo.TargetFrameworkName == null)
		{
			appDomainInfo.TargetFrameworkName = AppDomain.CurrentDomain.GetTargetFrameworkName();
		}

		return AppDomain.CreateDomain(friendlyName, appDomainInfo, securityInfo, (securityInfo == null) ? AppDomain.CurrentDomain.InternalEvidence : null, AppDomain.CurrentDomain.GetSecurityDescriptor());
		*/

		return AppDomain.CurrentDomain;
	}

	//
	// Summary:
	//     Initializes the new application domain.
	//
	// Parameters:
	//   appDomainInfo:
	//     An object that contains application domain initialization information.
	[SecurityCritical]
	public virtual void InitializeNewDomain(AppDomainSetup appDomainInfo)
	{
	}

	/*
	[DllImport("QCall", CharSet = CharSet.Unicode)]
	[SuppressUnmanagedCodeSecurity]
	private static extern void GetEntryAssembly(ObjectHandleOnStack retAssembly);
	*/

	//
	// Summary:
	//     Indicates whether the specified operation is allowed in the application domain.
	//
	//
	// Parameters:
	//   state:
	//     A subclass of System.Security.SecurityState that identifies the operation whose
	//     security status is requested.
	//
	// Returns:
	//     true if the host allows the operation specified by state to be performed in the
	//     application domain; otherwise, false.
	public virtual bool CheckSecuritySettings(SecurityState state)
	{
		return false;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasHost();

	/*[DllImport("QCall", CharSet = CharSet.Unicode)]
	[SecurityCritical]
	[SuppressUnmanagedCodeSecurity]
	private static extern void RegisterWithHost(IntPtr appDomainManager);
	*/
	internal void RegisterWithHost()
	{ /*
		if (!HasHost())
		{
			return;
		}

		IntPtr intPtr = IntPtr.Zero;
		RuntimeHelpers.PrepareConstrainedRegions();
		try
		{
			intPtr = Marshal.GetIUnknownForObject(this);
			RegisterWithHost(intPtr);
		}
		finally
		{
			if (!intPtr.IsNull())
			{
				Marshal.Release(intPtr);
			}
		} */
	}
}
#endif