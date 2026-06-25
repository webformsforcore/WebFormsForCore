## WebFormsForCore AspNetCore Compiler

This is the WebFormsForCore version of the known aspnet_compiler.exe. It can be used to precompile WebFormsForCore
and ordinary plain old ASP.NET WebForms apps targeting .NET Framework. In comparison to aspnet_compiler.exe,
There is one additional argument `-t [TargetFramework]` that can be used to specify the target framework for compilation,
and thus overwriting the value in web.config compilation section.

## Installation
You can install aspnetcore_compiler either as a dotnet tool, by executing the command
```
dotnet tool install -g WebFormsForCore.AspNetCompiler
```
adn then run the tool with `aspnetcore_compiler ...`.

Alternatively, you can use AspNetCore compiler by referencing the `AspNetCoreCompiler.Build` nuget package in your project and then using the `<AspNetCoreCompiler>`
task in your project file like so:
```
<Target Name="PrecompileWeb">
  <AspNetCoreCompiler
      VirtualPath="/MyWebSite"
      MetabasePath="/"
      PhysicalPath="$(ProjectDir)"
      TargetPath="$(OutputPath)\Precompiled"
      TargetFramework="net10.0"
      TargetFrameworkMoniker="net10.0"
      BinFolder="bin"
      Updateable="true"
      Force="true"
      Debug="false"
      Clean="true"
      FixedNames="false"
      KeyFile=".."
      KeyContainer=".."
      DelaySign="false"
      AllowPartiallyTrustedCallers="false"
      />
</Target>
```
This task supports the same attributes as the `AspNetCompiler` task and also the `TargetFramework` attribute.

# Multi targeting
If you have a project targeting .NET Framework and .NET Core at the same time, you can precompile it by 
specifying the comma or semicolon separated bin directories like `-b bin,bin_dotnet` and the corresponding 
target frameworks like `-t net48,net10.0`.