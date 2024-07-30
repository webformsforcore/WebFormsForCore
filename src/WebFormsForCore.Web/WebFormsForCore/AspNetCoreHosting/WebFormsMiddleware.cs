// ASP.NET Core middleware

#if NETCOREAPP

using Microsoft.AspNetCore.Builder;
using Core = Microsoft.AspNetCore.Http;
using System;
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

		private readonly Core.RequestDelegate next;

		public readonly ApplicationManager ApplicationManager = new ApplicationManager();

		AspNetCoreHost host = null;
		public AspNetCoreHost Host
		{
			get
			{
				if (host == null)
				{
					AppId = ApplicationManager.CreateApplicationId(VirtualPath, PhysicalPath);
					host = ApplicationManager.CreateInstanceInNewWorkerLoadContext(typeof(AspNetCoreHost), AppId, System.Web.VirtualPath.Create(VirtualPath), PhysicalPath, UseSeparateAssemblyLoadContext) as AspNetCoreHost;
				}
				return host;
			}
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

		public WebFormsMiddleware(Core.RequestDelegate next)
		{
			this.next = next;
			var path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
			if (path.EndsWith(Path.DirectorySeparatorChar.ToString())) path = path.Substring(0, path.Length - 1);
			PhysicalPath = Path.GetDirectoryName(path);
			//PhysicalPath = Path.GetDirectoryName(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath));
			VirtualPath = "/";

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

				host.ProcessRequest(context);
			} else {
				await next.Invoke(context);
			}
		}

		public virtual bool IsLegacyRequest(Core.HttpContext context) => host.IsLegacyRequest(context);
	}
	public static class WebFormsMiddlewareExtensions
	{
		public static IApplicationBuilder UseWebForms(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<WebFormsMiddleware>();
		}
	}
}

#endif