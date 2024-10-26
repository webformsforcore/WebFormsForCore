SET PackageVersion=1.1.0-beta

cd  src
cd WebFormsForCore.Build
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Configuration
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Drawing
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Serialization.Formatters
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web
dotnet pack WebFormsForCore.Web.csproj --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.ApplicationServices
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.Extensions
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.Infrastructure
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.Optimization
dotnet pack WebFormsForCore.Web.Optimization.csproj --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.Optimization.WebForms
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.RegularExpressions
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.Services
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.WebGrease
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\..



