#if NETCOREAPP

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Web;
using System.Threading;

namespace EstrellasDeEsperanza.WebFormsForCore.Test
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var app = builder.Build();

			Thread.Sleep(15000);

			app.UseWebForms(options => options.HandleAllRequestsWithWebForms());
			
			app.Run();

		}
	}
}

#endif