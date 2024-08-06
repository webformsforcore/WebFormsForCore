# WebFormsForCore

A library to run WebForms apps on ASP.NET Core. This library provides a port
of the System.Web libraries of .NET Framework to .NET 8. With this library,
you can run WebForms sites directly in ASP.NET Core. 

# Usage

If you have a WebForms project you want to convert to NET Core, proceed as follows:

First convert you Project to a SDK Project. This can be done easiest by using a converter like the migrate-2019 tool. To use install that tool run `dotnet tool install --global Project2015To2017.Migrate2019.Tool`. Then go to the directory of your solution and run `dotnet migrate-2019 wizard` to convert your solution to an SDK project.
After conversion change the target framework of your project to `net8.0`. You might also keep net48, in order to dual run your project with NET Framework & NET Core. Then, for `net8.0`, import the WebFormsForCore packages like so:
```
<ItemGroup Condition="'$(TargetFramework)' == 'net8.0'">
    <PackageReference Include="EstrellasDeEsperanza.WebFormsForCore.Web" Version="1.0.0" />
</ItemGroup>
```
If your project also needs `System.Web.Extensions` or `System.Web.Optimization` import the corresponding packages also, like `EstrellasDeEsperanza.WebFormsForCore.Web.Extensions` or `EstrellasDeEsperanza.WebFormsForCore.Web.Optimization` etc.

Finally configure ASP-NET Core to use WebForms in the initialization code Program.cs like so:
```
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Program
{

	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorPages();
		builder.Services.AddControllersWithViews();

		var app = builder.Build();

		app.UseStaticFiles();

		//app.UseAuthorization();

		app.UseWebForms();

		app.MapDefaultControllerRoute();
			
		app.Run();

	}
}
```
