#if NETCOREAPP

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Web;
using System.Threading;

namespace WebFormsForCore.Test
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession();

			var app = builder.Build();

			app.UseSession();

			app.UseAspNetCoreSessionProvider();
			app.UseWebForms(options => options.HandleAllRequestsWithWebForms());

			app.Run();

		}
	}
}

#endif