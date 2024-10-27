SET PackageVersion=1.1.0-beta
SET /p ApiKey=<NugetApiKey.txt

cd  src
dotnet build
cd WebFormsForCore.Build
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Configuration
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Drawing
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Serialization.Formatters
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web
dotnet pack WebFormsForCore.Web.csproj --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web.ApplicationServices
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web.Extensions
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web.Infrastructure
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web.Optimization
dotnet pack WebFormsForCore.Web.Optimization.csproj --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web.Optimization.WebForms
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web.RegularExpressions
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.Web.Services
dotnet pack --include-symbols -o ..\..\nupkg --no-build
cd ..\WebFormsForCore.WebGrease
dotnet pack WebFormsForCore.WebGrease.csproj --include-symbols -o ..\..\nupkg --no-build
cd ..\..

cd nuget

for /r %i in (*.nupkg) do dotnet nuget push %i --api-key %ApiKey%
for /r %i in (*.snupkg) do dotnet nuget push %i --api-key %ApiKey% 
