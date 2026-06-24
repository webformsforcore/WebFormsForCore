SET PackageVersion=1.6.0
SET Configuration=Debug

REM msbuild /p:Configuration=%Configuration%

cd  src
cd WebFormsForCore.Compilers
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Build
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration%;Packaging=True -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Configuration
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Drawing
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Serialization.Formatters
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web
dotnet pack WebFormsForCore.Web.csproj --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.ApplicationServices
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.Extensions
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.Infrastructure
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.Optimization
dotnet pack WebFormsForCore.Web.Optimization.csproj --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.Optimization.WebForms
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.Mobile
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.RegularExpressions
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.Web.Services
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.WebGrease
dotnet pack WebFormsForCore.WebGrease.csproj --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\WebFormsForCore.AjaxControlToolkit\AjaxControlToolkit
dotnet pack --include-symbols -o ..\..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\AjaxControlToolkit.HtmlEditor.Sanitizer
dotnet pack --include-symbols -o ..\..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\AjaxControlToolkit.StaticResources
dotnet pack --include-symbols -o ..\..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
cd ..\..\WebFormsForCore.AspNetCompiler
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion%
dotnet pack --include-symbols -o ..\..\nupkg --no-build -c %Configuration% -p:Version=%PackageVersion% -p:FileVersion=%PackageVersion% -p:PackAsTool=false

cd ..\..\..