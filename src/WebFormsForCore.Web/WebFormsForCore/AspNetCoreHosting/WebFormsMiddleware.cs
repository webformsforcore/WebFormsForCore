// ASP.NET Core middleware

#if NETCOREAPP

using Microsoft.AspNetCore.Builder;
using Core = Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Runtime.Loader;

namespace Microsoft.AspNetCore.Builder
{
	public class WebFormsMiddleware
	{
		const bool UseSeparateAssemblyLoadContext = false;

		public string VirtualPath { get; set; }
		public string PhysicalPath { get; set; }
		public string AppId { get; set; }
		public bool HandleAllRequestsWithWebForms = false;
		public bool UseClassicMode = true;
		public IEnumerable<string> HandleExtensions = null;
		public IEnumerable<string> AddHandleExtensions = Array.Empty<string>();
		public IEnumerable<string> DefaultDocuments = null;
		public IEnumerable<string> AddDefaultDocuments = Array.Empty<string>();


		private readonly Core.RequestDelegate next;

		public readonly ApplicationManager ApplicationManager = new ApplicationManager();

		AspNetCoreHost host = null;
		public AspNetCoreHost Host
		{
			get
			{
				if (host == null)
				{
					//AppId = ApplicationManager.CreateApplicationId(VirtualPath, PhysicalPath);
					//host = ApplicationManager.CreateInstanceInNewWorkerLoadContext(typeof(AspNetCoreHost), AppId, System.Web.VirtualPath.Create(VirtualPath), PhysicalPath, UseSeparateAssemblyLoadContext) as AspNetCoreHost;
					host = CreateWorkerLoadContextWithHost(VirtualPath, PhysicalPath, typeof(AspNetCoreHost)) as AspNetCoreHost;
					if (HandleExtensions != null) host.HandleExtensions = HandleExtensions.ToArray();
					if (AddHandleExtensions.Any()) host.HandleExtensions = host.HandleExtensions.Concat(AddHandleExtensions).ToArray();
					if (DefaultDocuments != null) host.DefaultDocuments = DefaultDocuments.ToArray();
					if (AddDefaultDocuments.Any()) host.DefaultDocuments = host.DefaultDocuments.Concat(AddDefaultDocuments).ToArray();
					host.HandleAllRequestsWithWebForms = HandleAllRequestsWithWebForms;
					host.UseClassicMode = UseClassicMode;
				}
				return host;
			}
		}

		private object CreateWorkerLoadContextWithHost(string virtualPath, string physicalPath, Type hostType)
		{
			// create BuildManagerHost in the worker app domain
			//ApplicationManager appManager = ApplicationManager.GetApplicationManager();
			Type buildManagerHostType = typeof(HttpRuntime).Assembly.GetType("System.Web.Compilation.BuildManagerHost");
			AppId = ApplicationManager.CreateApplicationId(VirtualPath, PhysicalPath);
			var vpath = System.Web.VirtualPath.Create(virtualPath);
			IRegisteredObject buildManagerHost = ApplicationManager.CreateInstanceInNewWorkerLoadContext(buildManagerHostType, AppId, vpath, physicalPath, false) as IRegisteredObject;

			// call BuildManagerHost.RegisterAssembly to make Host type loadable in the worker app domain
			buildManagerHostType.InvokeMember("RegisterAssembly",
											  BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic,
											  null,
											  buildManagerHost,
											  new object[] { hostType.Assembly.FullName, hostType.Assembly.Location });
			// create Host in the worker app domain
			return ApplicationManager.CreateObject(AppId, hostType, virtualPath, physicalPath, false);
		}

		public void RestartApplication()
		{
			AssemblyLoadContext context;
			lock (ApplicationManager)
			{
				context = ApplicationManager.GetAssemblyLoadContext(AppId);
				host = null;
				AppId = Guid.NewGuid().ToString();
			}
			if (context != AssemblyLoadContext.Default) context.Unload();
		}

		public WebFormsMiddleware(Core.RequestDelegate next, Action<WebFormsOptions> optionsBuilder)
		{
			this.next = next;
			var path = AppDomain.CurrentDomain.BaseDirectory;
			if (path.EndsWith(Path.DirectorySeparatorChar.ToString())) path = path.Substring(0, path.Length - 1);
			PhysicalPath = Path.GetDirectoryName(path);
			//PhysicalPath = Path.GetDirectoryName(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath));
			VirtualPath = "/";

			optionsBuilder?.Invoke(new WebFormsOptions(this));

			Host.Configure(VirtualPath, PhysicalPath);
		}

		public void AllowSynchronousIO(Core.HttpContext context)
		{
			var syncIoFeature = context.Features.Get<Core.Features.IHttpBodyControlFeature>();
			if (syncIoFeature != null)
			{
				syncIoFeature.AllowSynchronousIO = true;
			}
		}

		public async Task Invoke(Core.HttpContext context)
		{
			if (IsLegacyRequest(context))
			{
				AllowSynchronousIO(context);

				await host.ProcessRequest(context);
			} else {
				await next.Invoke(context);
			}
		}

		public virtual bool IsLegacyRequest(Core.HttpContext context) => host.IsLegacyRequest(context);
	}

	public class WebFormsOptions
	{
		WebFormsMiddleware Instance;

		public WebFormsOptions(WebFormsMiddleware instance) { Instance = instance; }
		public WebFormsOptions HandleExtensions(params string[] extensions) { Instance.HandleExtensions = extensions; return this; }
		public WebFormsOptions AddHandleExtensions(params string[] extensions) { Instance.AddHandleExtensions = Instance.AddHandleExtensions.Concat(extensions).ToArray(); return this; }
		public WebFormsOptions DefaultDocuments(params string[] docs) { Instance.DefaultDocuments = docs; return this; }
		public WebFormsOptions AddDefaultDocuments(params string[] docs) { Instance.AddDefaultDocuments = Instance.AddDefaultDocuments.Concat(docs).ToArray(); return this; }
		public WebFormsOptions VirtualPath(string path) {
			if (!path.StartsWith('/')) path = "/" + path;
			Instance.VirtualPath = path;
			return this;
		}
		public WebFormsOptions PhysicalPath(string path) { Instance.PhysicalPath = path; return this; }
		public WebFormsOptions HandleAllRequestsWithWebForms() { Instance.HandleAllRequestsWithWebForms = true; return this; }

        public WebFormsOptions UseClassicMode() { Instance.UseClassicMode = true; return this; }
        public WebFormsOptions UseIntegratedMode() { Instance.UseClassicMode = false; return this; }
    }

    public static class WebFormsMiddlewareExtensions
	{
		public static IApplicationBuilder UseWebForms(this IApplicationBuilder builder, Action<WebFormsOptions> optionsBuilder = null)
		{
			AssemblyLoaderNetCore.Init();
			if (optionsBuilder == null) optionsBuilder = options => { };
			return builder.UseMiddleware<WebFormsMiddleware>(optionsBuilder);
		}
	}
}

#endif