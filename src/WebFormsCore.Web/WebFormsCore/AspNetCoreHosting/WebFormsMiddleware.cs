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

		public async Task Invoke(Core.HttpContext context)
		{
			if (HandleExtensions.Any(ext => context.Request.Path.ToString().EndsWith(ext)))
			{
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