// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.DefaultProxySectionInternal
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;

#nullable disable
namespace System.Net.Configuration
{
	internal sealed class DefaultProxySectionInternal
	{
		private IWebProxy webProxy;
		private static object classSyncObject;

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlPrincipal)]
		internal DefaultProxySectionInternal(DefaultProxySection section)
		{
			if (!section.Enabled)
				return;
			if (section.Proxy.AutoDetect == ProxyElement.AutoDetectValues.Unspecified && section.Proxy.ScriptLocation == (Uri)null && string.IsNullOrEmpty(section.Module.Type) && section.Proxy.UseSystemDefault != ProxyElement.UseSystemDefaultValues.True && section.Proxy.ProxyAddress == (Uri)null && section.Proxy.BypassOnLocal == ProxyElement.BypassOnLocalValues.Unspecified && section.BypassList.Count == 0)
			{
				if (section.Proxy.UseSystemDefault == ProxyElement.UseSystemDefaultValues.False)
				{
					this.webProxy = (IWebProxy)new EmptyWebProxy();
					return;
				}
				try
				{
					new SecurityPermission(SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.ControlPrincipal).Assert();
#if !WebFormsCore
					using (WindowsIdentity.Impersonate(IntPtr.Zero))
					{
						CodeAccessPermission.RevertAssert();
#endif
						var proxy = (WebProxy)Activator.CreateInstance(typeof(System.Net.WebProxy), BindingFlags.NonPublic, new object[] { true }, CultureInfo.InvariantCulture);
						this.webProxy = (IWebProxy)new WebProxyWrapper(proxy);
#if !WebFormsCore
					}
#endif
				}
				catch
				{
					throw;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(section.Module.Type))
				{
					Type type = Type.GetType(section.Module.Type, true, true);
					if ((type.Attributes & TypeAttributes.VisibilityMask) != TypeAttributes.Public)
						throw new ConfigurationErrorsException(SR.GetString("net_config_proxy_module_not_public"));
					this.webProxy = typeof(IWebProxy).IsAssignableFrom(type) ? (IWebProxy)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder)null, new object[0], CultureInfo.InvariantCulture) : throw new InvalidCastException(SR.GetString("net_invalid_cast", (object)type.FullName, (object)"IWebProxy"));
				}
				else
				{
					if (section.Proxy.UseSystemDefault == ProxyElement.UseSystemDefaultValues.True && section.Proxy.AutoDetect == ProxyElement.AutoDetectValues.Unspecified)
					{
						if (section.Proxy.ScriptLocation == (Uri)null)
						{
							try
							{
								new SecurityPermission(SecurityPermissionFlag.UnmanagedCode | SecurityPermissionFlag.ControlPrincipal).Assert();
#if !WebFormsCore
								using (WindowsIdentity.Impersonate(IntPtr.Zero))
								{
									CodeAccessPermission.RevertAssert();
#endif
									var proxy = (WebProxy)Activator.CreateInstance(typeof(System.Net.WebProxy), BindingFlags.NonPublic, new object[] { true }, CultureInfo.InvariantCulture);
									this.webProxy = proxy;
									goto label_27;
#if !WebFormsCore
								}
#endif
							}
							catch
							{
								throw;
							}
						}
					}
					this.webProxy = (IWebProxy)new System.Net.WebProxy();
				}
			label_27:
				if (this.webProxy is System.Net.WebProxy webProxy)
				{
#if !WebFormsCore
					if (section.Proxy.AutoDetect != ProxyElement.AutoDetectValues.Unspecified)
						webProxy.AutoDetect = section.Proxy.AutoDetect == ProxyElement.AutoDetectValues.True;
					if (section.Proxy.ScriptLocation != (Uri)null)
						webProxy.ScriptLocation = section.Proxy.ScriptLocation;
#endif
					if (section.Proxy.BypassOnLocal != ProxyElement.BypassOnLocalValues.Unspecified)
						webProxy.BypassProxyOnLocal = section.Proxy.BypassOnLocal == ProxyElement.BypassOnLocalValues.True;
					if (section.Proxy.ProxyAddress != (Uri)null)
						webProxy.Address = section.Proxy.ProxyAddress;
					int count = section.BypassList.Count;
					if (count > 0)
					{
						string[] strArray = new string[section.BypassList.Count];
						for (int index = 0; index < count; ++index)
							strArray[index] = section.BypassList[index].Address;
						webProxy.BypassList = strArray;
					}
					if (section.Module.Type == null)
						this.webProxy = (IWebProxy)new WebProxyWrapper(webProxy);
				}
			}
			if (this.webProxy == null || !section.UseDefaultCredentials)
				return;
			this.webProxy.Credentials = (ICredentials)SystemNetworkCredential.defaultCredential;
		}

		internal static object ClassSyncObject
		{
			get
			{
				if (DefaultProxySectionInternal.classSyncObject == null)
				{
					object obj = new object();
					Interlocked.CompareExchange(ref DefaultProxySectionInternal.classSyncObject, obj, (object)null);
				}
				return DefaultProxySectionInternal.classSyncObject;
			}
		}

		internal static DefaultProxySectionInternal GetSection()
		{
			lock (DefaultProxySectionInternal.ClassSyncObject)
			{
				if (!(PrivilegedConfigurationManager.GetSection(ConfigurationStrings.DefaultProxySectionPath) is DefaultProxySection section))
					return (DefaultProxySectionInternal)null;
				try
				{
					return new DefaultProxySectionInternal(section);
				}
				catch (Exception ex)
				{
					if (!NclUtilities.IsFatal(ex))
						throw new ConfigurationErrorsException(SR.GetString("net_config_proxy"), ex);
					throw;
				}
			}
		}

		internal IWebProxy WebProxy => this.webProxy;
	}
}
