#if NETCOREAPP

using System;
using System.Globalization;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Loader;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Core = Microsoft.AspNetCore.Http;

namespace System.Web.Hosting
{
    public class AspNetCoreHost : MarshalByRefObject, IRegisteredObject
    {
        private bool disableDirectoryListing;

        private string installPath;

        private string lowerCasedClientScriptPathWithTrailingSlash;

        private string lowerCasedVirtualPath;

        private string lowerCasedVirtualPathWithTrailingSlash;

        private volatile int pendingCallsCount;

        private string physicalClientScriptPath;

        private string physicalPath;

        private int port;

        private bool requireAuthentication;

		private IntPtr processToken;

		private string processUser;

        private string virtualPath;
        public bool HandleAllRequestsWithWebForms { get; set; } = false;
		public string[] HandleExtensions { get; set; } = new string[] { ".aspx", ".ashx", ".asmx", ".axd" };
        public string[] ProhibitedExtensions { get; set; } = new string[] { ".razor", ".cshtml", ".vbhtml", ".cs", ".vb", ".resx", ".resource", ".master", ".asax", ".ascx", "web.config", "appsettings.json" };
        public string[] DefaultDocuments { get; set; } = new string[] { "index.htm", "index.html", "Index.html", "Index.htm", "Default.htm", "iistart.htm", "Default.aspx", "default.aspx" };

        public Compilation.ClientBuildManager ClientBuildManager { get; set; } 
		public AppDomain AppDomain
        {
            get { return AppDomain.CurrentDomain; }
        }
		public AssemblyLoadContext LoadContext
		{
			get { return ApplicationManager.GetLoadContext(Assembly.GetCallingAssembly()); }
		}

		public AspNetCoreHost()
        {
            HostingEnvironment.RegisterObject(this);

        }

        public bool DisableDirectoryListing
        {
            get { return disableDirectoryListing; }
        }

        public string InstallPath
        {
            get { return installPath; }
        }

        public string NormalizedClientScriptPath
        {
            get { return lowerCasedClientScriptPathWithTrailingSlash; }
        }

        public string NormalizedVirtualPath
        {
            get { return lowerCasedVirtualPathWithTrailingSlash; }
        }

        public string PhysicalClientScriptPath
        {
            get { return physicalClientScriptPath; }
        }

        public string PhysicalPath
        {
            get { return physicalPath; }
        }

        public int Port
        {
            get { return port; }
        }

        public bool RequireAuthentication
        {
            get { return requireAuthentication; }
        }

        public string VirtualPath
        {
            get { return virtualPath; }
        }

        public HostingEnvironment HostingEnvironment { get; set; }

        #region IRegisteredObject Members

        void IRegisteredObject.Stop(bool immediate)
        {
            // Make sure all the pending calls complete before this Object is unregistered.
            WaitForPendingCallsToFinish();

            HostingEnvironment.UnregisterObject(this);

            Thread.Sleep(100);
            HttpRuntime.Close();
            Thread.Sleep(100);
        }

        #endregion

        public void Configure(string virtualPath, string physicalPath,
                              bool requireAuthentication)
        {
            Configure(virtualPath, physicalPath, requireAuthentication, false);
        }

        public void Configure(string virtualPath, string physicalPath)
        {
            Configure(virtualPath, physicalPath, false, false);
        }

        public void Configure(string virtualPath, string physicalPath,
                              bool requireAuthentication, bool disableDirectoryListing)
        {
            installPath = null;
            this.virtualPath = virtualPath;
            this.requireAuthentication = requireAuthentication;
            this.disableDirectoryListing = disableDirectoryListing;
            lowerCasedVirtualPath = CultureInfo.InvariantCulture.TextInfo.ToLower(virtualPath);
            lowerCasedVirtualPathWithTrailingSlash = virtualPath.EndsWith("/", StringComparison.Ordinal)
                                                          ? virtualPath
                                                          : virtualPath + "/";
            lowerCasedVirtualPathWithTrailingSlash =
                CultureInfo.InvariantCulture.TextInfo.ToLower(lowerCasedVirtualPathWithTrailingSlash);
            this.physicalPath = physicalPath;

			var assembly = GetType().Assembly;
            ApplicationManager.SetLoadContextData(".appPath", physicalPath, assembly);
			ApplicationManager.SetLoadContextData(".appVPath", virtualPath, assembly);

			physicalClientScriptPath = HttpRuntime.AspClientScriptPhysicalPath + "\\";
            lowerCasedClientScriptPathWithTrailingSlash =
                CultureInfo.InvariantCulture.TextInfo.ToLower(HttpRuntime.AspClientScriptVirtualPath + "/");

        }

		private void ObtainProcessToken()
		{
			/*
			if (Interop.ImpersonateSelf(2))
			{
				Interop.OpenThreadToken(Interop.GetCurrentThread(), 0xf01ff, true, ref processToken);
				Interop.RevertToSelf();
				// ReSharper disable PossibleNullReferenceException
				// ReSharper restore PossibleNullReferenceException
			} */
		    processUser = WindowsIdentity.GetCurrent().Name;
		}


		public SecurityIdentifier GetProcessSid() => WindowsIdentity.GetCurrent().User;
        public IntPtr GetProcessToken() => WindowsIdentity.GetCurrent().Token;
        public string GetProcessUser() => WindowsIdentity.GetCurrent().Name;

        public override object InitializeLifetimeService()
        {
            // never expire the license
            return null;
        }

        public bool IsVirtualPathAppPath(string path)
        {
            if (path == null)
            {
                return false;
            }
            path = CultureInfo.InvariantCulture.TextInfo.ToLower(path);
            return (path == lowerCasedVirtualPath || path == lowerCasedVirtualPathWithTrailingSlash);
        }

        public bool IsVirtualPathInApp(string path, out bool isClientScriptPath)
        {
            isClientScriptPath = false;

            if (path == null)
            {
                return false;
            }

            if (virtualPath == "/" && path.StartsWith("/", StringComparison.Ordinal))
            {
                if (path.StartsWith(lowerCasedClientScriptPathWithTrailingSlash, StringComparison.Ordinal))
                {
                    isClientScriptPath = true;
                }
                return true;
            }

            path = CultureInfo.InvariantCulture.TextInfo.ToLower(path);

            if (path.StartsWith(lowerCasedVirtualPathWithTrailingSlash, StringComparison.Ordinal))
            {
                return true;
            }

            if (path == lowerCasedVirtualPath)
            {
                return true;
            }

            if (path.StartsWith(lowerCasedClientScriptPathWithTrailingSlash, StringComparison.Ordinal))
            {
                isClientScriptPath = true;
                return true;
            }

            return false;
        }

        public string MapPath(string path) => HostingEnvironment.MapPath(path);
        public bool IsLegacyRequest(Core.HttpContext context)
        {
            var path = context.Request.Path.Value;
            string pathWithoutSlash = path;
            if (path.EndsWith("/")) pathWithoutSlash = path.Substring(0, path.Length - 1);
            if (path == "") path = "/";
            if (path != "/") path = pathWithoutSlash;

			if (HandleExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) return true;

            if (ProhibitedExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))) return false;

            var mappedPath = MapPath(path);
            if (Directory.Exists(mappedPath))
            {
				var defaultDoc = DefaultDocuments
                    .Select(doc => new { Mapped = Path.Combine(mappedPath, doc), Virtual = $"{pathWithoutSlash}/{doc}" })
                    .Where(doc => File.Exists(doc.Mapped))
                    .FirstOrDefault();
                if (defaultDoc != null)
                {
                    context.Request.Path = defaultDoc.Virtual;
                    return true;
                }
            }

			return HandleAllRequestsWithWebForms || 
                File.Exists(mappedPath) &&
                !(Path.GetDirectoryName(mappedPath) == AppDomain.CurrentDomain.BaseDirectory);
		}

		public bool IsVirtualPathInApp(String path)
        {
            bool isClientScriptPath;
            return IsVirtualPathInApp(path, out isClientScriptPath);
        }

        public async Task ProcessRequest(Core.HttpContext context)
        {
            AddPendingCall();

            await new AspNetCoreWorkerRequest(this, context).Process();

            RemovePendingCall();
        }

        [SecurityPermission(SecurityAction.Assert, Unrestricted = true)]
        public void Shutdown()
        {
            HostingEnvironment.InitiateShutdown();
        }

        private void AddPendingCall()
        {
            //TODO: investigate this issue - ref var not volitile
#pragma warning disable 0420
            Interlocked.Increment(ref pendingCallsCount);
#pragma warning restore 0420
        }

        private void RemovePendingCall()
        {
            //TODO: investigate this issue - ref var not volitile
#pragma warning disable 0420
            Interlocked.Decrement(ref pendingCallsCount);
#pragma warning restore 0420
        }

        private void WaitForPendingCallsToFinish()
        {
            for (; ; )
            {
                if (pendingCallsCount <= 0)
                {
                    break;
                }

                Thread.Sleep(250);
            }
        }
    }
}

#endif