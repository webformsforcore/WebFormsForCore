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
cd ..\WebFormsForCore.ApplicationServices
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Extensions
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Infrastructure
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Optimization
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Optimization.WebForms
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.RegularExpressions
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.Web.Services
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\WebFormsForCore.WebGrease
dotnet pack --include-symbols -o ..\..\nupkg
cd ..\..



