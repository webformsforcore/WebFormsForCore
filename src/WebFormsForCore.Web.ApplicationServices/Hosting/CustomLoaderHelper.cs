using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Versioning;
using System.Security.Policy;

#nullable disable
namespace System.Web.Hosting
{
	internal sealed class CustomLoaderHelper : MarshalByRefObject
	{
		private static readonly string _customLoaderTargetFrameworkName = new FrameworkName(".NETFramework", new Version(4, 5, 1)).ToString();
		private static readonly string _customLoaderAssemblyName = typeof(CustomLoaderHelper).Assembly.FullName;
		private static readonly string _customLoaderTypeName = typeof(CustomLoaderHelper).FullName;
		private static readonly Guid IID_ICustomLoader = new Guid("50A3CE65-2F9F-44E9-9094-32C6C928F966");

		private CustomLoaderHelper()
		{
		}

#if NETFRAMEWORK
		internal static IObjectHandle GetCustomLoader(
		  ICustomLoaderHelperFunctions helperFunctions,
		  string appConfigMetabasePath,
		  string configFilePath,
		  string customLoaderPhysicalPath,
		  out AppDomain newlyCreatedAppDomain)
		{
			bool? customLoaderIsEnabled = helperFunctions.CustomLoaderIsEnabled;
			if (customLoaderIsEnabled.HasValue)
			{
				if (!customLoaderIsEnabled.Value)
					throw new NotSupportedException(ApplicationServicesStrings.CustomLoader_ForbiddenByHost);
			}
			else if (!CustomLoaderHelper.IsFullyTrusted(helperFunctions, appConfigMetabasePath))
				throw new NotSupportedException(ApplicationServicesStrings.CustomLoader_NotInFullTrust);
			string str = helperFunctions.MapPath("/bin/");
			AppDomainSetup info = new AppDomainSetup()
			{
				PrivateBinPathProbe = "*",
				PrivateBinPath = str,
				ApplicationBase = helperFunctions.AppPhysicalPath,
				TargetFrameworkName = CustomLoaderHelper._customLoaderTargetFrameworkName
			};
			if (configFilePath != null)
				info.ConfigurationFile = configFilePath;
			AppDomain domain = AppDomain.CreateDomain("aspnet-custom-loader-" + Guid.NewGuid().ToString(), (Evidence)null, info);
			try
			{
				ObjectHandle customLoaderImpl = ((CustomLoaderHelper)domain.CreateInstanceAndUnwrap(CustomLoaderHelper._customLoaderAssemblyName, CustomLoaderHelper._customLoaderTypeName, false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder)null, (object[])null, (CultureInfo)null, (object[])null)).GetCustomLoaderImpl(customLoaderPhysicalPath);
				newlyCreatedAppDomain = domain;
				return (IObjectHandle)customLoaderImpl;
			}
			catch
			{
				AppDomain.Unload(domain);
				throw;
			}
		}
#else
		internal static ObjectHandle GetCustomLoader(
		  ICustomLoaderHelperFunctions helperFunctions,
		  string appConfigMetabasePath,
		  string configFilePath,
		  string customLoaderPhysicalPath,
		  out AppDomain newlyCreatedAppDomain)
		{
			bool? customLoaderIsEnabled = helperFunctions.CustomLoaderIsEnabled;
			if (customLoaderIsEnabled.HasValue)
			{
				if (!customLoaderIsEnabled.Value)
					throw new NotSupportedException(ApplicationServicesStrings.CustomLoader_ForbiddenByHost);
			}
			else if (!CustomLoaderHelper.IsFullyTrusted(helperFunctions, appConfigMetabasePath))
				throw new NotSupportedException(ApplicationServicesStrings.CustomLoader_NotInFullTrust);
			string str = helperFunctions.MapPath("/bin/");

			newlyCreatedAppDomain = AppDomain.CurrentDomain;

			return new CustomLoaderHelper().GetCustomLoaderImpl(customLoaderPhysicalPath);
		}
#endif
		private ObjectHandle GetCustomLoaderImpl(string customLoaderPhysicalPath)
		{
			AssemblyName assemblyName = AssemblyName.GetAssemblyName(customLoaderPhysicalPath);
			object instance = Activator.CreateInstance((Assembly.Load(assemblyName).GetCustomAttribute<CustomLoaderAttribute>() ?? throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ApplicationServicesStrings.CustomLoader_NoAttributeFound, new object[1]
			{
		(object) assemblyName
			}))).CustomLoaderType);
			return CustomLoaderHelper.ObjectImplementsComInterface(instance, CustomLoaderHelper.IID_ICustomLoader) ? new ObjectHandle(instance) : throw new InvalidOperationException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, ApplicationServicesStrings.CustomLoader_MustImplementICustomLoader, new object[1]
			{
		(object) instance.GetType()
			}));
		}

		private static bool IsFullyTrusted(
		  ICustomLoaderHelperFunctions helperFunctions,
		  string appConfigMetabasePath)
		{
			try
			{
				return string.Equals("Full", helperFunctions.GetTrustLevel(appConfigMetabasePath), StringComparison.Ordinal);
			}
			catch
			{
				return false;
			}
		}

		private static bool ObjectImplementsComInterface(object o, Guid iid)
		{
			IntPtr pUnk = IntPtr.Zero;
			IntPtr ppv = IntPtr.Zero;
			try
			{
				pUnk = Marshal.GetIUnknownForObject(o);
				return Marshal.QueryInterface(pUnk, ref iid, out ppv) == 0 && ppv != IntPtr.Zero;
			}
			finally
			{
				if (pUnk != IntPtr.Zero)
					Marshal.Release(pUnk);
				if (ppv != IntPtr.Zero)
					Marshal.Release(ppv);
			}
		}
	}
}
