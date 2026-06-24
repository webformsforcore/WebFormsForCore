using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

#if NET10_0_OR_GREATER
using System.Web.Compilation;
#endif

namespace WebFormsForCore.Build;

public class AspNetCoreCompiler : Task
{
    public ITaskItem VirtualPath { get; set; }
    public ITaskItem PhysicalPath { get; set; }
    public ITaskItem TargetPath { get; set; }
    public ITaskItem MetabasePath { get; set; }
    public ITaskItem[] ExcludeVirtualPaths { get; set; }

    private ITaskItem targetFramework = null;
    public ITaskItem TargetFramework
    {
        get => targetFramework ?? TargetFrameworkMoniker;
        set => targetFramework = value;
    }
    public ITaskItem TargetFrameworkMoniker { get; set; } = null;
    public ITaskItem BinFolder { get; set; }

    public bool Force { get; set; }
    public bool Debug { get; set; }
    public bool Clean { get; set; }
    public bool Updateable { get; set; }
    public bool FixedNames { get; set; }
    public bool ShowErrorStack { get; set; } = false;
    public ITaskItem KeyFile { get; set; }
    public ITaskItem KeyContainer { get; set; }
    public bool DelaySing { get; set; } = false;
    public bool AllowPartiallyTrustedCallers { get; set; } = true;
    public bool IsCore => !(IsNetFX || IsNetNative);
    public bool IsNetFX => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
    public bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);
    public bool IsNet10 => Environment.Version.Major >= 10;
    public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public bool LogToConsole { get; set; } = false;

    void LogError(string filename, int line, bool warning, string category, string msg)
    {
        if (Log != null && !LogToConsole) Log.LogError(null, category, null, filename, line, 0, line, 0, msg);
        else Console.Error.WriteLine($"# {filename} ({line}): {(warning ? "warning" : "error")} {category}: {msg}");
    }

    void LogMessage(string msg)
    {
        if (Log != null && !LogToConsole) Log.LogMessage(MessageImportance.High, msg);
        else Console.WriteLine(msg);
    }

    public void LogErrors(Exception exception)
    {
        Exception formattableException = GetFormattableException(exception);
        if (formattableException != null)
            exception = formattableException;
        switch (exception.GetType().Name)
        {
            case "HttpCompileException":
            case "HttpParseException":
            label_5:
                if (!ShowErrorStack) break;
                LogExceptionStack(exception);
                break;
            case "ConfigurationException":
                var filename = exception.GetType().GetProperty("Filename").GetValue(exception) as string;
                var line = (int)exception.GetType().GetProperty("Line").GetValue(exception);
                var message = exception.GetType().GetProperty("BareMessage").GetValue(exception) as string;
                LogError(filename, line, false, "ASPCONFIG", message);
                goto label_5;
            default:
                LogError((string)null, 0, false, "ASPRUNTIME", exception.Message);
                goto label_5;
        }
    }

    public static Exception GetFormattableException(Exception e)
    {
        switch (e.GetType().Name)
        {
            case "HttpCompileException":
            case "HttpParseException":
            case "ConfigurationException":
                return e;
            default:
                Exception innerException = e.InnerException;
                return innerException == null ? (Exception)null : GetFormattableException(innerException);
        }
    }

    private void LogExceptionStack(Exception e)
    {
        Exception innerException = e.InnerException;
        if (innerException != null)
            LogExceptionStack(innerException);
        string str = $"[{e.GetType().Name}]";
        if (e.Message != null && e.Message.Length > 0)
            str = $"{str}: {e.Message}";
        if (Log != null && !LogToConsole) Log.LogWarning(str + Environment.NewLine + e.StackTrace);
        else
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine(str);
            if (e.StackTrace == null)
                return;
            Console.Error.WriteLine(e.StackTrace);
        }
    }

    bool ExecuteCore()
    {
#if NET10_0_OR_GREATER
        var par = new PrecompileParameter()
        {
            BinFolder = BinFolder?.ItemSpec,
            TargetFramework = TargetFramework?.ItemSpec,
            StrongNameKeyFile = !string.IsNullOrEmpty(KeyFile?.ItemSpec) ? Path.GetFullPath(KeyFile.ItemSpec) : null,
            StrongNameKeyContainer = KeyContainer?.ItemSpec
        };
        par.ExcludedVirtualPaths.AddRange(ExcludeVirtualPaths
            .Select(item => !string.IsNullOrEmpty(item?.ItemSpec) ? Path.GetFullPath(item.ItemSpec) : null)
            .Where(path => !string.IsNullOrEmpty(path)));
        par.PrecompilationFlags = default;
        if (Clean) par.PrecompilationFlags |= PrecompileFlags.Clean;
        if (Force) par.PrecompilationFlags |= PrecompileFlags.OverwriteTarget;
        if (Updateable) par.PrecompilationFlags |= PrecompileFlags.Updatable;
        if (FixedNames) par.PrecompilationFlags |= PrecompileFlags.FixedNames;
        if (Debug) par.PrecompilationFlags |= PrecompileFlags.ForceDebug;
        if (DelaySing) par.PrecompilationFlags |= PrecompileFlags.DelaySign;
        if (AllowPartiallyTrustedCallers) par.PrecompilationFlags |= PrecompileFlags.AllowPartiallyTrustedCallers;

        Precompiler.OnException += LogErrors;
        Precompiler.OnError += LogError;

        var targetPath = TargetPath?.ItemSpec;
        if (!string.IsNullOrEmpty(targetPath)) targetPath = Path.GetFullPath(targetPath);
        else targetPath = null;

        var physicalPath = PhysicalPath?.ItemSpec;
        if (!string.IsNullOrEmpty(physicalPath)) physicalPath = Path.GetFullPath(physicalPath);
        else physicalPath = null;

        Precompiler.Precompile(VirtualPath?.ItemSpec, physicalPath, targetPath, par, Force);

        Precompiler.OnException -= LogErrors;
        Precompiler.OnError -= LogError;

        return !Precompiler.HasErrors;
#else
        throw new NotSupportedException("ExecuteCore can only run on .NET 10");
#endif
    }
    public override bool Execute()
    {
        Debugger.Launch();

        if (!IsNet10)
        {
            //LogMessage($"Starting dotnet...");
            var dll = Assembly.GetExecutingAssembly().Location;
            var net = "net10.0";
            dll = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(dll)!)!, net, "WebFormsForCore.Build.NetCore.dll");
            var iswin = IsWindows;
            var physicalPath = PhysicalPath?.ItemSpec;
            if (!string.IsNullOrEmpty(physicalPath)) physicalPath = Path.GetFullPath(physicalPath);
            else physicalPath = "";
            var targetPath = TargetPath?.ItemSpec;
            if (!string.IsNullOrEmpty(targetPath)) targetPath = Path.GetFullPath(targetPath);
            else targetPath = "";
            var keyFile = KeyFile?.ItemSpec;
            if (!string.IsNullOrEmpty(keyFile)) keyFile = Path.GetFullPath(keyFile);
            else keyFile = "";
            var startInfo = new ProcessStartInfo($"dotnet{(IsWindows ? ".exe" : "")}", $"\"{dll}\" aspnetcompile " +
                $"\"{physicalPath}\" \"{VirtualPath?.ItemSpec ?? ""}\" \"{MetabasePath?.ItemSpec ?? ""}\" " +
                $"\"{targetPath}\" \"{BinFolder?.ItemSpec ?? ""}\" \"{TargetFramework?.ItemSpec ?? ""}\" " +
                $"{Clean} {Force} {Updateable} {Debug} {DelaySing} {FixedNames} " +
                $"\"{keyFile}\" \"{KeyContainer?.ItemSpec ?? ""}\"");

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            var p = new Process();
            p.StartInfo = startInfo;
            p.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null) LogMessage(args.Data);
            };
            bool hasErrors = false;
            p.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    hasErrors = true;
                    var match = Regex.Match(args.Data, @"^#\s+(?<file>.*?)\((?<line>[0-9]+)\):\s+(?<warning>error|warning)\s+(?<category>.*?):\s*(?<msg>.*?)$");
                    if (match.Success) {
                        LogError(match.Groups["file"].Value, int.Parse(match.Groups["line"].Value), match.Groups["warning"].Value == "warning", match.Groups["category"].Value, match.Groups["msg"].Value);
                    } else LogMessage(args.Data);
                }
            };
            p.EnableRaisingEvents = true;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit(30000);
            return !hasErrors;
        } else
        {
            return ExecuteCore();
        }
    }
} 
