using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Compilation;
using System.Web.Hosting;

#nullable disable
namespace System.Web.Compilation;

internal class Start
{
    public static int Main(string[] args)
    {
        return RunMain(args);
    }

    static int RunMain(string[] args)
    {
        return Precompiler.Main(args);
    }
}

// Flags that drive the behavior of precompilation
[Flags]
public enum PrecompileFlags
{

    Default = 0x00000000,

    // determines whether the deployed app will be updatable
    Updatable = 0x00000001,

    // determines whether the target directory can be overwritten
    OverwriteTarget = 0x00000002,

    // determines whether the compiler will emit debug information
    ForceDebug = 0x00000004,

    // determines whether the application is built clean
    Clean = 0x00000008,

    // determines whether the /define:CodeAnalysis flag needs to be added
    // as compilation symbol
    CodeAnalysis = 0x00000010,

    // determines whether to generate APTCA attribute.
    AllowPartiallyTrustedCallers = 0x00000020,

    // determines whether to delaySign the generate assemblies.
    DelaySign = 0x00000040,

    // determines whether to use fixed assembly names
    FixedNames = 0x00000080,

    // determines whether to skip BadImageFormatException
    IgnoreBadImageFormatException = 0x00000100,
}

public class Precompiler
{
    private static string _sourcePhysicalDir;
    private static string _metabasePath;
    private static string _sourceVirtualDir;
    private static string _targetPhysicalDir;
    private static string _keyFile;
    private static string _keyContainer;
    private static string _binFolder;
    private static PrecompileFlags _precompilationFlags;
    private static bool _showErrorStack;
    private static List<string> _excludedVirtualPaths;
    private static string _targetFramework;
    private static int maxLineLength = 80 /*0x50*/;
    private const int leftMargin = 14;
    private static readonly char[] invalidVirtualPathChars = new char[2] { '*', '?' };
    public static bool Silent = false;
    public static int Main(string[] args)
    {
        Precompiler._excludedVirtualPaths = new List<string>();
        bool flag = false;
        try
        {
            Precompiler.maxLineLength = Console.BufferWidth;
        }
        catch
        {
        }
        Precompiler.SetThreadUICulture();
        for (int index = 0; index < args.Length; ++index)
        {
            string lower = args[index].ToLower(CultureInfo.InvariantCulture);
            if (lower == "-nologo" || lower == "/nologo")
                flag = true;
        }
        if (!flag)
        {
            Console.WriteLine(string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.brand_text, new object[1]
            {
        (object) "10.0.8"
            }));
            Console.WriteLine(CompilerResources.header_text);
            Console.WriteLine(CompilerResources.copyright);
            Console.WriteLine();
        }
        if (args.Length == 0)
        {
            Console.WriteLine(CompilerResources.short_usage_text);
            return 1;
        }
        if (!Precompiler.ValidateArgs(args))
            return 1;
        try
        {

            if (Precompiler._sourceVirtualDir == null)
                Precompiler._sourceVirtualDir = Precompiler._metabasePath;
            var parameter = new PrecompileParameter();
            parameter.PrecompilationFlags = Precompiler._precompilationFlags;
            parameter.StrongNameKeyFile = Precompiler._keyFile;
            parameter.StrongNameKeyContainer = Precompiler._keyContainer;
            parameter.BinFolder = Precompiler._binFolder;
            parameter.TargetFramework = Precompiler._targetFramework;
            parameter.ExcludedVirtualPaths.AddRange((IEnumerable<string>)Precompiler._excludedVirtualPaths);
            var targetDir = Precompiler._targetPhysicalDir;

            return Precompile(Precompiler._sourceVirtualDir, Precompiler._sourcePhysicalDir, targetDir, parameter);
        }
        catch (FileLoadException ex)
        {
            if ((Precompiler._precompilationFlags & PrecompileFlags.DelaySign) != PrecompileFlags.Default && (int)typeof(FileLoadException).GetProperty("HResult", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true).Invoke((object)ex, (object[])null) == -2146233318)
            {
                Precompiler.DumpErrors((Exception)new FileLoadException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.Strongname_failure, new object[1]
                {
          (object) ex.FileName
                }), ex.FileName, (Exception)ex));
                return 1;
            }
            Precompiler.DumpErrors((Exception)ex);
        }
        catch (Exception ex)
        {
            Precompiler.DumpErrors(ex);
        }
        return 1;
    }
    public static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = true)
    {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

        DirectoryInfo[] dirs = dir.GetDirectories();

        // Create destination if it doesn't exist
        Directory.CreateDirectory(destinationDir);

        // Copy files
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, overwrite);
        }

        // Copy subdirectories recursively
        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir, overwrite);
        }
    }

    static int pass = 0;
    static int reentrant = 0;
    public static int Precompile(string sourceVirtualDir, string sourcePhysicalDir, string targetDir, PrecompileParameter parameter, bool forceCleanBuild = false)
    {
        if (Interlocked.Increment(ref reentrant) == 0) HasErrors = false;
        var binFolders = parameter.BinFolder?.Split(new char[] { ',', ';' }, StringSplitOptions.TrimEntries) ?? new string[] { null };
        var targetFrameworks = parameter.TargetFramework?.Split(new char[] { ',', ';' }, StringSplitOptions.TrimEntries) ?? new string[] { null };
        if (binFolders.Length <= 1 && targetFrameworks.Length <= 1)
        {
            var self = Assembly.GetExecutingAssembly().Location;
            var alc = new AssemblyLoadContext($"PrecompileContext.{++pass}", true);
            var selfpath = Path.GetDirectoryName(self);
            var selfwebdll = Path.Combine(selfpath, "System.Web.dll");
            var selfconfigdll = Path.Combine(selfpath, "System.Configuration.ConfigurationManager.dll");
            var path = Path.Combine(sourcePhysicalDir, binFolders[0] ?? "bin");
            //AssemblyLoaderNetCore.Init(path, alc);

            //var webdll = Path.Combine(path, "System.Web.dll");
            //var configdll = Path.Combine(path, "System.Configuration.ConfigurationManager.dll");
            //AssemblyLoaderNetCore.UseNetFXGAC = !File.Exists(webdll) ||
            //    Regex.IsMatch(targetFrameworks[0], "^(net)?[234][0-9.]+$", RegexOptions.Singleline);
            alc.LoadFromAssemblyPath(selfconfigdll);
            alc.LoadFromAssemblyPath(selfwebdll);
            var selfAssembly = alc.LoadFromAssemblyPath(self);

            var precompileType = selfAssembly.GetType("System.Web.Compilation.Precompiler");
            var precompileMethod = precompileType.GetMethod("PrecompileInternal");

            try
            {
                precompileMethod.Invoke(null, new object[] { sourceVirtualDir, sourcePhysicalDir, targetDir,
                    parameter.StrongNameKeyFile, parameter.StrongNameKeyContainer,
                    parameter.PrecompilationFlags, parameter.ExcludedVirtualPaths,
                    parameter.BinFolder, parameter.TargetFramework,
                    forceCleanBuild });
                return HasErrors ? -1 : 0;
            }
            catch (Exception ex)
            {
                DumpErrors(ex);
                return -1;
            }
            finally
            {
                alc.Unload();
                Interlocked.Decrement(ref reentrant);
            }
        }
        else
        {
            if (binFolders.Length != targetFrameworks.Length)
            {
                Console.WriteLine(CompilerResources.unequalBinAndFramework);
                Interlocked.Decrement(ref reentrant);
                return -401;
            }

            int i;
            for (i = 0; i < binFolders.Length; i++)
            {
                var par = new PrecompileParameter()
                {
                    PrecompilationFlags = parameter.PrecompilationFlags,
                    StrongNameKeyContainer = parameter.StrongNameKeyContainer,
                    StrongNameKeyFile = parameter.StrongNameKeyFile,
                    TargetFramework = targetFrameworks[i],
                    BinFolder = binFolders[i]
                };
                par.ExcludedVirtualPaths.AddRange(parameter.ExcludedVirtualPaths);

                var tempTargetDir = targetDir;
                if (i >= 1) tempTargetDir = Path.Combine(targetDir, binFolders[i], "_AspNetCompiler");
                Directory.CreateDirectory(tempTargetDir);

                Precompile(sourceVirtualDir, sourcePhysicalDir, tempTargetDir, par, i >= 1);

                if (i >= 1)
                {
                    var targetBin = Path.Combine(targetDir, binFolders[i]);
                    var tempTargetBin = Path.Combine(tempTargetDir, binFolders[i]);
                    if (Directory.Exists(tempTargetBin)) CopyDirectory(tempTargetBin, targetBin, true);
                    if (Directory.Exists(tempTargetDir)) Directory.Delete(tempTargetDir, true);
                }
            }
        }

        Interlocked.Decrement(ref reentrant);
        return HasErrors ? -1 : 0;
    }

    public static int PrecompileInternal(string sourceVirtualDir, string sourcePhysicalDir, string targetDir,
        string strongNameKeyFile,
        string strongNameKeyContainer,
        PrecompileFlags precompilationFlags,
        List<string> excludedVirtualPaths,
        string binFolder,
        string targetFramework,
        bool forceCleanBuild = false)
    {
        var par = new ClientBuildManagerParameter()
        {
            TargetFramework = targetFramework,
            BinFolder = binFolder,
            PrecompilationFlags = (System.Web.Compilation.PrecompilationFlags)precompilationFlags,
            StrongNameKeyFile = strongNameKeyFile,
            StrongNameKeyContainer = strongNameKeyContainer
        };
        par.ExcludedVirtualPaths.AddRange(excludedVirtualPaths);
        var path = Path.Combine(sourcePhysicalDir, binFolder ?? "bin");
        var webdll = Path.Combine(path, "System.Web.dll");
        var configdll = Path.Combine(path, "System.Configuration.ConfigurationManager.dll");
        //AssemblyLoaderNetCore.UseNetFXGAC = !File.Exists(webdll) ||
        //  Regex.IsMatch(targetFramework, "^(net)?[234][0-9.]+$", RegexOptions.Singleline);

        using var manager = new ClientBuildManager(sourceVirtualDir, sourcePhysicalDir, targetDir, par);
        AssemblyLoaderNetCore.AdditionalPaths.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        manager.PrecompileApplication(new CBMCallback(), forceCleanBuild);

        AssemblyLoaderNetCore.Dispose();
        return HasErrors ? -1 : 0;
    }

    private static void SetThreadUICulture()
    {
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture.GetConsoleFallbackUICulture();
        if (Console.OutputEncoding.CodePage == 65001 || Console.OutputEncoding.CodePage == Thread.CurrentThread.CurrentUICulture.TextInfo.OEMCodePage || Console.OutputEncoding.CodePage == Thread.CurrentThread.CurrentUICulture.TextInfo.ANSICodePage)
            return;
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
    }

    private static void DisplayUsage()
    {
        Console.WriteLine(CompilerResources.usage);
        Console.WriteLine("aspnet_compiler [-?] [-m metabasePath | -v virtualPath [-p physicalDir]]");
        Console.WriteLine("                [[-u] [-f] [-d] [-fixednames] targetDir] [-c]");
        Console.WriteLine("                [-x excludeVirtualPath [...]]");
        Console.WriteLine("                [-b binFolder]");
        Console.WriteLine("                [[-keyfile file | -keycontainer container]");
        Console.WriteLine("                     [-aptca] [-delaySign]]");
        Console.WriteLine("                [-errorstack]");
        Console.WriteLine("                [[-t] targetFramework]");
        Console.WriteLine();
        Precompiler.DisplaySwitchWithHelp("-?", CompilerResources.questionmark_help);
        Precompiler.DisplaySwitchWithHelp("-m", CompilerResources.m_help);
        Precompiler.DisplaySwitchWithHelp("-v", CompilerResources.v_help);
        Precompiler.DisplaySwitchWithHelp("-p", CompilerResources.p_help);
        Precompiler.DisplaySwitchWithHelp("-u", CompilerResources.u_help);
        Precompiler.DisplaySwitchWithHelp("-f", CompilerResources.f_help);
        Precompiler.DisplaySwitchWithHelp("-d", CompilerResources.d_help);
        Precompiler.DisplaySwitchWithHelp("-t", CompilerResources.t_help);
        Precompiler.DisplaySwitchWithHelp("targetDir", CompilerResources.targetDir_help);
        Precompiler.DisplaySwitchWithHelp("-c", CompilerResources.c_help);
        Precompiler.DisplaySwitchWithHelp("-x", CompilerResources.x_help);
        Precompiler.DisplaySwitchWithHelp("-b", CompilerResources.b_help);
        Precompiler.DisplaySwitchWithHelp("-keyfile", CompilerResources.keyfile_help);
        Precompiler.DisplaySwitchWithHelp("-keycontainer", CompilerResources.keycontainer_help);
        Precompiler.DisplaySwitchWithHelp("-aptca", CompilerResources.aptca_help);
        Precompiler.DisplaySwitchWithHelp("-delaysign", CompilerResources.delaysign_help);
        Precompiler.DisplaySwitchWithHelp("-fixednames", CompilerResources.fixednames_help);
        Precompiler.DisplaySwitchWithHelp("-nologo", CompilerResources.nologo_help);
        Precompiler.DisplaySwitchWithHelp("-errorstack", CompilerResources.errorstack_help);
        Console.WriteLine();
        Console.WriteLine(CompilerResources.examples);
        Console.WriteLine();
        Precompiler.DisplayWordWrappedString(CompilerResources.example1);
        Console.WriteLine("    aspnet_compiler -m /LM/W3SVC/1/Root/MyApp c:\\MyTarget");
        Console.WriteLine("    aspnet_compiler -v /MyApp c:\\MyTarget");
        Console.WriteLine();
        Precompiler.DisplayWordWrappedString(CompilerResources.example2);
        Console.WriteLine("    aspnet_compiler -v /MyApp");
        Console.WriteLine();
        Precompiler.DisplayWordWrappedString(CompilerResources.example3);
        Console.WriteLine("    aspnet_compiler -v /MyApp -p c:\\myapp c:\\MyTarget");
        Console.WriteLine();
    }

    private static void DisplaySwitchWithHelp(string switchString, string stringHelpString)
    {
        Console.Write(switchString);
        Precompiler.DisplayWordWrappedString(stringHelpString, switchString.Length, 14);
    }

    private static void DisplayWordWrappedString(string s)
    {
        Precompiler.DisplayWordWrappedString(s, 0, 0);
    }

    private static void DisplayWordWrappedString(string s, int currentOffset, int leftMargin)
    {
        string[] strArray = s.Split(' ');
        bool flag = true;
        foreach (string str in strArray)
        {
            int length = str.Length;
            if (!flag)
                ++length;
            if (currentOffset + length >= Precompiler.maxLineLength)
            {
                Console.WriteLine();
                currentOffset = 0;
                flag = true;
            }
            if (flag)
            {
                for (; currentOffset < leftMargin; ++currentOffset)
                    Console.Write(' ');
            }
            else
            {
                Console.Write(' ');
                ++currentOffset;
            }
            Console.Write(str);
            currentOffset += str.Length;
            flag = false;
        }
        Console.WriteLine();
    }

    private static string GetNextArgument(string[] args, ref int index)
    {
        if (index != args.Length - 1)
            return args[++index];
        Console.WriteLine(string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.missing_arg, new object[1]
        {
      (object) args[index]
        }));
        return (string)null;
    }

    private static bool ValidateArgs(string[] args)
    {
        if (args.Length == 0)
            return false;
        for (int index = 0; index < args.Length; ++index)
        {
            string str = args[index];
            if (str[0] != '/' && str[0] != '-')
            {
                if (Precompiler._targetPhysicalDir == null)
                {
                    Precompiler._targetPhysicalDir = str;
                    Precompiler._targetPhysicalDir = Precompiler.GetFullPath(Precompiler._targetPhysicalDir);
                    if (Precompiler._targetPhysicalDir == null)
                        return false;
                }
                else
                {
                    Precompiler.DumpError("1001", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.unexpected_param, new object[1]
                    {
            (object) str
                    }));
                    return false;
                }
            }
            else
            {
                switch (str.Substring(1).ToLower(CultureInfo.InvariantCulture))
                {
                    case "?":
                        Precompiler.DisplayUsage();
                        return false;
                    case "aptca":
                        Precompiler._precompilationFlags |= PrecompileFlags.AllowPartiallyTrustedCallers;
                        continue;
                    case "c":
                        Precompiler._precompilationFlags |= PrecompileFlags.Clean;
                        continue;
                    case "d":
                        Precompiler._precompilationFlags |= PrecompileFlags.ForceDebug;
                        continue;
                    case "b":
                        Precompiler._binFolder = Precompiler.GetNextArgument(args, ref index);
                        if (Precompiler._binFolder == null)
                            return false; ;
                        continue;
                    case "delaysign":
                        Precompiler._precompilationFlags |= PrecompileFlags.DelaySign;
                        continue;
                    case "errorstack":
                        Precompiler._showErrorStack = true;
                        continue;
                    case "f":
                        Precompiler._precompilationFlags |= PrecompileFlags.OverwriteTarget;
                        continue;
                    case "fixednames":
                        Precompiler._precompilationFlags |= PrecompileFlags.FixedNames;
                        continue;
                    case "keycontainer":
                        Precompiler._keyContainer = Precompiler.GetNextArgument(args, ref index);
                        if (Precompiler._keyContainer == null)
                            return false;
                        continue;
                    case "keyfile":
                        Precompiler._keyFile = Precompiler.GetNextArgument(args, ref index);
                        if (Precompiler._keyFile == null)
                            return false;
                        if (!File.Exists(Precompiler._keyFile))
                        {
                            Precompiler.DumpError("1012", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.invalid_keyfile, new object[1]
                            {
                (object) Precompiler._keyFile
                            }));
                            return false;
                        }
                        Precompiler._keyFile = Path.GetFullPath(Precompiler._keyFile);
                        continue;
                    case "m":
                        Precompiler._metabasePath = Precompiler.GetNextArgument(args, ref index);
                        if (Precompiler._metabasePath == null)
                            return false;
                        continue;
                    case "nologo":
                        continue;
                    case "p":
                        Precompiler._sourcePhysicalDir = Precompiler.GetNextArgument(args, ref index);
                        if (Precompiler._sourcePhysicalDir == null)
                            return false;
                        Precompiler._sourcePhysicalDir = Precompiler.GetFullPath(Precompiler._sourcePhysicalDir);
                        if (Precompiler._sourcePhysicalDir == null)
                            return false;
                        if (!Directory.Exists(Precompiler._sourcePhysicalDir))
                        {
                            Precompiler.DumpError("1003", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.dir_not_exist, new object[1]
                            {
                (object) Precompiler._sourcePhysicalDir
                            }));
                            return false;
                        }
                        continue;
                    case "u":
                        Precompiler._precompilationFlags |= PrecompileFlags.Updatable;
                        continue;
                    case "v":
                        Precompiler._sourceVirtualDir = Precompiler.GetNextArgument(args, ref index);
                        if (Precompiler._sourceVirtualDir == null)
                            return false;
                        if (!Precompiler.IsValidVirtualPath(Precompiler._sourceVirtualDir))
                        {
                            Precompiler.DumpError("1011", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.invalid_vpath, new object[1]
                            {
                (object) Precompiler._sourceVirtualDir
                            }));
                            return false;
                        }
                        continue;
                    case "x":
                        string nextArgument = Precompiler.GetNextArgument(args, ref index);
                        if (nextArgument == null)
                            return false;
                        Precompiler._excludedVirtualPaths.Add(nextArgument);
                        continue;
                    case "t":
                        string nextArg = Precompiler.GetNextArgument(args, ref index);
                        if (nextArg == null)
                            return false;
                        Precompiler._targetFramework = nextArg;
                        continue;
                    default:
                        Precompiler.DumpError("1004", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.unknown_switch, new object[1]
                        {
              (object) str
                        }));
                        return false;
                }
            }
        }
        if (Precompiler._sourceVirtualDir == null == (Precompiler._metabasePath == null))
        {
            Precompiler.DumpError("1005", CompilerResources.need_m_or_v);
            return false;
        }
        if (Precompiler._sourcePhysicalDir != null && Precompiler._metabasePath != null)
        {
            Precompiler.DumpError("1006", CompilerResources.no_m_and_p);
            return false;
        }
        if ((Precompiler._precompilationFlags & PrecompileFlags.Updatable) != PrecompileFlags.Default && Precompiler._targetPhysicalDir == null)
        {
            Precompiler.DumpError("1007", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
            {
        (object) "u"
            }));
            return false;
        }
        if ((Precompiler._precompilationFlags & PrecompileFlags.OverwriteTarget) != PrecompileFlags.Default && Precompiler._targetPhysicalDir == null)
        {
            Precompiler.DumpError("1008", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
            {
        (object) "f"
            }));
            return false;
        }
        if ((Precompiler._precompilationFlags & PrecompileFlags.ForceDebug) != PrecompileFlags.Default && Precompiler._targetPhysicalDir == null)
        {
            Precompiler.DumpError("1009", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
            {
        (object) "d"
            }));
            return false;
        }
        if (Precompiler._keyFile != null && Precompiler._targetPhysicalDir == null)
        {
            Precompiler.DumpError("1017", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
            {
        (object) "keyfile"
            }));
            return false;
        }
        if (Precompiler._keyContainer != null && Precompiler._targetPhysicalDir == null)
        {
            Precompiler.DumpError("1018", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
            {
        (object) "keycontainer"
            }));
            return false;
        }
        if ((Precompiler._precompilationFlags & PrecompileFlags.FixedNames) != PrecompileFlags.Default && Precompiler._targetPhysicalDir == null)
        {
            Precompiler.DumpError("1019", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
            {
        (object) "fixednames"
            }));
            return false;
        }
        if ((Precompiler._precompilationFlags & PrecompileFlags.DelaySign) != PrecompileFlags.Default)
        {
            if (Precompiler._keyFile == null && Precompiler._keyContainer == null)
            {
                Precompiler.DumpError("1013", CompilerResources.invalid_delaysign);
                return false;
            }
            if (Precompiler._targetPhysicalDir == null)
            {
                Precompiler.DumpError("1015", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
                {
          (object) "delaysign"
                }));
                return false;
            }
        }
        if ((Precompiler._precompilationFlags & PrecompileFlags.AllowPartiallyTrustedCallers) != PrecompileFlags.Default)
        {
            if (Precompiler._keyFile == null && Precompiler._keyContainer == null)
            {
                Precompiler.DumpError("1014", CompilerResources.invalid_aptca);
                return false;
            }
            if (Precompiler._targetPhysicalDir == null)
            {
                Precompiler.DumpError("1016", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.flag_requires_target, new object[1]
                {
          (object) "aptca"
                }));
                return false;
            }
        }
        return true;
    }

    private static bool IsValidVirtualPath(string virtualPath)
    {
        return virtualPath != null && virtualPath.IndexOfAny(Precompiler.invalidVirtualPathChars) < 0;
    }

    private static string GetFullPath(string path)
    {
        try
        {
            return Path.GetFullPath(path);
        }
        catch
        {
            Precompiler.DumpError("1010", string.Format((IFormatProvider)CultureInfo.CurrentCulture, CompilerResources.invalid_path, new object[1]
            {
        (object) path
            }));
            return (string)null;
        }
    }

    public static void DumpErrors(Exception exception)
    {
        Exception formattableException = Precompiler.GetFormattableException(exception);
        if (formattableException != null)
            exception = formattableException;
        else
        {
            var name = exception.GetType().Name;
            while (name != "HttpCompileException" && name != "HttpParseException" &&
                name != "ConfigurationException" && exception.InnerException != null)
            {
                exception = exception.InnerException;
                name = exception.GetType().Name;
            }
        }

        switch (exception.GetType().Name)
        {
            case "HttpCompileException":
            case "HttpParseException":
            label_5:
                if (!Precompiler._showErrorStack)
                    break;
                Precompiler.DumpExceptionStack(exception);
                break;
            case "ConfigurationException":
                var filename = exception.GetType().GetProperty("Filename").GetValue(exception) as string;
                var line = (int)exception.GetType().GetProperty("Line").GetValue(exception);
                var message = exception.GetType().GetProperty("BareMessage").GetValue(exception) as string;
                Precompiler.DumpError(filename, line, false, "ASPCONFIG", message);
                goto label_5;
            default:
                Precompiler.DumpError((string)null, 0, false, "ASPRUNTIME", exception.Message);
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
                return innerException == null ? (Exception)null : Precompiler.GetFormattableException(innerException);
        }
    }

    private static void DumpCompileError(CompilerError error)
    {
        Precompiler.DumpError(error.FileName, error.Line, error.IsWarning, error.ErrorNumber, error.ErrorText);
    }

    public static Action<Exception> OnException { get; set; } = null;
    private static void DumpExceptionStack(Exception e)
    {
        if (OnException != null) OnException.Invoke(e);
        else if (!Silent)
        {
            Exception innerException = e.InnerException;
            if (innerException != null)
                Precompiler.DumpExceptionStack(innerException);
            string str = $"[{e.GetType().Name}]";
            if (e.Message != null && e.Message.Length > 0)
                str = $"{str}: {e.Message}";
            Console.WriteLine();
            Console.WriteLine(str);
            if (e.StackTrace == null)
                return;
            Console.WriteLine(e.StackTrace);
        }
    }

    private static void DumpError(string errorNumber, string message)
    {
        Precompiler.DumpError((string)null, 0, false, errorNumber, message);
    }

    public static Action<string, int, bool, string, string> OnError { get; set; } = null;
    public static bool HasErrors { get; set; } = false;
    private static void DumpError(
      string filename,
      int line,
      bool warning,
      string errorNumber,
      string message)
    {
        HasErrors = true;
        if (OnError != null) OnError?.Invoke(filename, line, warning, errorNumber, message);
        else if (!Silent)
        {
            if (filename != null)
            {
                Console.Write(filename);
                Console.Write($"({line.ToString()}): ");
            }
            if (warning)
                Console.Write("warning ");
            else
                Console.Write("error ");
            Console.Write(errorNumber + ": ");
            Console.WriteLine(message);
        }
    }

    private class CBMCallback : ClientBuildManagerCallback
    {
        public override void ReportCompilerError(CompilerError error)
        {
            Precompiler.DumpCompileError(error);
        }

        public override void ReportParseError(ParserError error)
        {
            Precompiler.DumpError(error.VirtualPath, error.Line, false, "ASPPARSE", error.ErrorText);
        }
        public override void ReportProgress(string message) { }
        public override object InitializeLifetimeService() => (object)null;

    }
}

[Serializable]
public class PrecompileParameter
{
    private string _strongNameKeyFile;
    private string _strongNameKeyContainer;
    private PrecompileFlags _precompilationFlags = PrecompileFlags.Default;
    private List<string> _excludedVirtualPaths;
    private string _binFolder;

    public string TargetFramework { get; set; }

    public List<string> ExcludedVirtualPaths
    {
        get
        {
            if (_excludedVirtualPaths == null)
            {
                _excludedVirtualPaths = new List<string>();
            }
            return _excludedVirtualPaths;
        }
    }

    // Determines the behavior of the precompilation
    public PrecompileFlags PrecompilationFlags
    {
        get { return _precompilationFlags; }
        set { _precompilationFlags = value; }
    }

    public string StrongNameKeyFile
    {
        get { return _strongNameKeyFile; }
        set { _strongNameKeyFile = value; }
    }

    public string StrongNameKeyContainer
    {
        get { return _strongNameKeyContainer; }
        set { _strongNameKeyContainer = value; }
    }

    public string BinFolder
    {
        get => _binFolder;
        set => _binFolder = value;
    }
}