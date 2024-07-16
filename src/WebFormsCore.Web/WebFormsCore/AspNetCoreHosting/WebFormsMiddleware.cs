// ASP.NET Core middleware

#if NETCOREAPP

using Microsoft.AspNetCore.Builder;
using Core = Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Hosting;

namespace Microsoft.AspNetCore.Builder
{
	public class WebFormsMiddleware
	{
		string[] HandleExtensions = new string[] { ".aspx", ".ashx", ".asmx", ".asax" };

		private readonly Core.RequestDelegate next;

		AspNetCoreHost host = new AspNetCoreHost();

		public WebFormsMiddleware(Core.RequestDelegate next)
		{
			this.next = next;
			var path = Path.GetDirectoryName(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath));

			host.Configure("/", path);
		}

		public async Task AllowSynchronousIO(Core.HttpContext context)
		{
			var syncIoFeature = context.Features.Get<Core.Features.IHttpBodyControlFeature>();
			if (syncIoFeature != null)
			{
				syncIoFeature.AllowSynchronousIO = true;
			}
		}

		public async Task Invoke(Core.HttpContext context)
		{
			if (HandleExtensions.Any(ext => context.Request.Path.ToString().EndsWith(ext)))
			{
				AllowSynchronousIO(context);

				host.ProcessRequest(context);
			} else {
				await next.Invoke(context);
			}
		}
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