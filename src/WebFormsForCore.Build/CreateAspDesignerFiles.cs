using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
#if !NETSTANDARD
using Redesigner.Library;
using Redesigner.CommandLine;
#endif
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace EstrellasDeEsperanza.WebFormsForCore.Build
{
	public class CreateAspDesignerFiles: Task
	{
		public static bool IsCore => !(IsNetFX || IsNetNative);
		public static bool IsNetFX => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
		public static bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);

		public ITaskItem[] Files { get; set; } = new TaskItem[0];
		public ITaskItem Assembly { get; set; }
		public ITaskItem Directory { get; set; }
		public string TargetFramework { get; set; }
		public bool LogToConsole { get; set; } = false;
		public bool SeparateProcess { get; set; } = true;

		public bool IsTargetNetFX => TargetFramework.CompareTo("net4999") < 0;
		public bool IsTargetNetCore => TargetFramework.CompareTo("net5") >= 0;
		public bool IsTargetNetStandard => TargetFramework.StartsWith("netstandard");


#if !NETSTANDARD
		public class MSBuildCompileContext: ICompileContext
		{
			private string filename;
			public Task Task { get; set; }
			public bool LogToConsole => (bool)Task.GetType().GetProperty("LogToConsole").GetValue(Task);
			public int FilenameCount { get; set; }
			public int VerboseNesting { get; set; }
			public bool HasErrors { get; private set; } = false;
			public MSBuildCompileContext(Task task) { Task = task; }
			public void BeginTask(int filenameCount) => FilenameCount = filenameCount;
			public void BeginFile(string filename)
			{
				this.filename = filename;
				LogMessage(MessageImportance.Low, $"Creating designer file for {filename}");
			}
			public void EndFile(string filename, bool succeeded) {
				if (succeeded) LogMessage(MessageImportance.High, $"Successfully created designer file for {filename}");
				else LogMessage(MessageImportance.High, $"Failed creating designer file for {filename}");
				filename = "";
			}
			public void Verbose(string format, params object[] args) => LogMessage(MessageImportance.Normal, format, args);
			public void Warning(string format, params object[] args) => LogWarning(format, args);
			public void Error(string format, params object[] args) {
				HasErrors = true;
				LogError(format, args);
			}

			public int LineNumber(string format, params object[] args)
			{
				var msg = string.Format(format, args);
				var lineNo = Regex.Match(msg, "Line (?<line>[0-9]+):");
				if (lineNo.Success) return int.Parse(lineNo.Groups["line"].Value);
				return 0;
			}

			public void LogMessage(MessageImportance importance, string format, params object[] args) {
				var line = LineNumber(format, args);
				if (LogToConsole) Console.WriteLine($"{filename} ({line}): {string.Format(format, args)}");
				else Task.Log.LogMessage("", "", "", filename, line, 0, line, 1, importance, format, args);
			}
			public void LogMessage(string format, params object[] args) {
				var line = LineNumber(format, args);
				if (LogToConsole) Console.WriteLine($"{filename} ({line}): {string.Format(format, args)}");
				else Task.Log.LogMessage("", "", "", filename, line, 0, line, 1, format, args);
			}
			public void LogWarning(string format, params object[] args) {
				var line = LineNumber(format, args);
				if (LogToConsole) Console.WriteLine($"{filename} ({line}): warning: {string.Format(format, args)}");
				else Task.Log.LogWarning("", "", "", filename, line, 0, line, 1, format, args);
			}
			public void LogError(string format, params object[] args) {
				var line = LineNumber(format, args);
				if (LogToConsole) Console.Error.WriteLine($"{filename} ({line}): {string.Format(format, args)}");
				else Task.Log.LogWarning("", "", "", filename, line, 0, line, 1, format, args);
			}

		}
#endif

		public bool ShouldCreateCodeFromAsp(string file)
		{
			var info = new FileInfo(file);
			if (info.Exists)
			{
				var ext = Path.GetExtension(file);
				var csFile = file + ".cs";
				var vbFile = file + ".vb";
				var isCS = File.Exists(csFile);
				var isVB = File.Exists(vbFile);
				if (isCS || isVB)
				{

					if (isVB) return false;

					var codeExt = isCS ? ".cs" : ".vb";
					var codeFile = file + codeExt;
					var designerFile = $"{file}.designer{codeExt}";

					return !File.Exists(designerFile) ||
						File.GetLastWriteTimeUtc(designerFile) < info.LastWriteTimeUtc;
						// designer file is out of date
					
				}
			}
			return false;
		}

		public void ParseMessage(string msg, out string file, out int line, out bool warning, out string message)
		{
			var m = Regex.Match(msg, @"(?<file>.*?)\s+\((?<line>[0-9]+)\):\s*(?<warning>[Ww]arning:\s*)?(?<msg>.*$)");
			if (m.Success)
			{
				file = m.Groups["file"].Value;
				line = int.Parse(m.Groups["line"].Value);
				warning = m.Groups["warning"].Success;
				message = m.Groups["msg"].Value;
			} else
			{
				file = "";
				line = 0;
				warning = false;
				message = msg;
			}
		}

		public bool RunCommand(string cmd, string args)
		{
			var startInfo = new ProcessStartInfo(cmd, args);
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardError = true;
			startInfo.RedirectStandardOutput = true;
			var p = new Process();
			p.StartInfo = startInfo;
			p.OutputDataReceived += (sender, args) =>
			{
				if (args.Data != null)
				{
					bool warnign;
					int line;
					string file, msg;
					ParseMessage(args.Data, out file, out line, out warnign, out msg);
					if (warnign) Log.LogWarning("", "", "", file, line, 0, line, 1, msg);
					else
					{
						MessageImportance importance;
						if (msg.Contains("Successfully created designer file for") || msg.Contains("Failed creating designer file for"))
						{
							importance = MessageImportance.High;
						}
						else importance = MessageImportance.Normal;

						Log.LogMessage("", "", "", file, line, 0, line, 1, importance, msg);
					}
				}
			};
			bool hasErrors = false;
			p.ErrorDataReceived += (sender, args) =>
			{
				if (args.Data != null)
				{
					hasErrors = true;
					bool warnign;
					int line;
					string file, msg;
					ParseMessage(args.Data, out file, out line, out warnign, out msg);
					Log.LogWarning("", "", "", file, line, 0, line, 1, msg);
				}
			};
			p.EnableRaisingEvents = true;
			p.Start();
			p.BeginOutputReadLine();
			p.BeginErrorReadLine();
			return p.WaitForExit(30000) && !hasErrors;
		}

		public bool RunProcess(string dll, string WebRootPath, IEnumerable<string> aspFiles)
		{
			if (IsTargetNetCore)
			{
				//LogMessage($"Starting dotnet...");
				var files = string.Join(";", aspFiles);

				var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				var assembly = Path.GetFullPath(Path.Combine(path, "..\\net8.0\\EstrellasDeEsperanza.WebFormsForCore.Build.NetCore.dll"));

				return RunCommand("dotnet.exe", $"\"{assembly}\" createdesignerfiles \"{dll}\" \"{WebRootPath}\" \"{files}\"");

			}
			else if (IsTargetNetFX)
			{
				var files = string.Join(";", aspFiles);

				var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				var assembly = Path.GetFullPath(Path.Combine(path, "..\\net48\\EstrellasDeEsperanza.WebFormsForCore.Build.NetFX.exe"));

				return RunCommand(assembly, $"createdesignerfiles \"{dll}\" \"{WebRootPath}\" \"{files}\"");
			}
			return true;
		}

		ResolveEventHandler ResolveAssembly = null;

		public bool GenerateDesignerFiles(string dll, string WebRootPath, IEnumerable<string> aspFiles)
		{
			aspFiles = aspFiles
				.Where(ShouldCreateCodeFromAsp)
				.ToArray();

			if (!aspFiles.Any()) return true;

			// Begin doing the actual compiling task.
#if NETSTANDARD
			if (!SeparateProcess && (IsNetFX && IsTargetNetFX || IsCore && IsTargetNetCore))
			{
				var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				ResolveAssembly = (sender, args) =>
				{
					string assemblyFile;
					if (IsCore) assemblyFile = Path.GetFullPath(Path.Combine(path, $"..\\net8.0\\{args.Name}.dll"));
					else assemblyFile = Path.GetFullPath(Path.Combine(path, $"..\\net48\\{args.Name}.dll"));
				
					if (File.Exists(assemblyFile)) return System.Reflection.Assembly.LoadFrom(assemblyFile);
					else
					{
						Log.LogError($"Assembly {args.Name} not found in {assemblyFile}");
						return null;
					}
				};
				AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
				Assembly buildAssembly;
				if (IsCore) buildAssembly = System.Reflection.Assembly.LoadFrom(Path.GetFullPath(Path.Combine(path, "..\\net8.0\\EstrellasDeEsperanza.WebFormsForCore.Build.NetCore.dll")));
				else buildAssembly = System.Reflection.Assembly.LoadFrom(Path.GetFullPath(Path.Combine(path, "..\\net48\\EstrellasDeEsperanza.WebFormsForCore.Build.NetFX.exe")));

				var contextType = buildAssembly.GetType("EstrellasDeEsperanza.WebFormsForCore.Build.CreateAspDesignerFiles+MSBuildCompileContext");
				var context = Activator.CreateInstance(contextType, (Task)this);

				var common = buildAssembly.GetType("Redesigner.Library.Common");
				var generateDesignerFiles = common.GetMethod("GenerateDesignerFiles");
				generateDesignerFiles.Invoke(null, new object[] { context, aspFiles, WebRootPath, dll });

				var hasErrors = contextType.GetProperty("HasErrors");
				return (bool)hasErrors.GetValue(context);
			} else return RunProcess(dll, WebRootPath, aspFiles);
#else
			if (!SeparateProcess)
			{
				var context = new MSBuildCompileContext(this);
				Common.GenerateDesignerFiles(context, aspFiles, WebRootPath, dll);
				return !context.HasErrors;
			} else return RunProcess(dll, WebRootPath, aspFiles);
#endif
		}

		public override bool Execute()
		{
			if (Files.Any()) GenerateDesignerFiles(Assembly.ItemSpec.Replace("\\\\", "\\"), Directory.ItemSpec, Files.Select(file => file.ItemSpec));
			
			if (ResolveAssembly != null)
			{
				AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;
				ResolveAssembly = null;
			}
			return true;
		}
	}
}
