using System;
using System.Collections.Generic;
using System.Text;
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

		public TaskItem[] Files { get; set; } = new TaskItem[0];
		public TaskItem Assembly { get; set; }
		public TaskItem Directory { get; set; }
		public string TargetFramework { get; set; }
		public bool LogToConsole { get; set; } = false;

		public bool IsTargetNetFX => TargetFramework.CompareTo("net4999") < 0;
		public bool IsTargetNetCore => TargetFramework.CompareTo("net5") >= 0;
		public bool IsTargetNetStandard => TargetFramework.StartsWith("netstandard");


#if !NETSTANDARD
		public class MSBuildCompileContext: ICompileContext
		{
			public CreateAspDesignerFiles Task { get; set; }
			public int FilenameCount { get; set; }
			public int VerboseNesting { get; set; }
			public bool HasErrors { get; private set; } = false;
			public MSBuildCompileContext(CreateAspDesignerFiles task) { Task = task; }
			public void BeginTask(int filenameCount) => FilenameCount = filenameCount;
			public void BeginFile(string filename) => LogMessage(MessageImportance.Low, $"Creating designer file for {filename}");
			public void EndFile(string filename, bool succeeded) {
				if (succeeded) LogMessage(MessageImportance.High, $"Successfully created designer file for {filename}");
				else LogMessage(MessageImportance.High, $"Failed creating designer file for {filename}");
			}
			public void Verbose(string format, params object[] args) => LogMessage(MessageImportance.Low, format, args);
			public void Warning(string format, params object[] args) => LogWarning(format, args);
			public void Error(string format, params object[] args) {
				HasErrors = true;
				LogError(format, args);
			}

			public void LogMessage(MessageImportance importance, string format, params object[] args) {
				if (Task.LogToConsole) Console.WriteLine(string.Format(format, args));
				else Task.Log.LogMessage(importance, format, args);
			}
			public void LogMessage(string format, params object[] args) {
				if (Task.LogToConsole) Console.WriteLine(string.Format(format, args));
				else Task.Log.LogMessage(format, args);
			}
			public void LogWarning(string format, params object[] args) {
				if (Task.LogToConsole) Console.WriteLine(string.Format(format, args));
				else Task.Log.LogWarning(format, args);
			}
			public void LogError(string format, params object[] args) {
				if (Task.LogToConsole) Console.Error.WriteLine(string.Format(format, args));
				else Task.Log.LogError(format, args);
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
					var designerFile = $"{file}.Designer{codeExt}";

					return !File.Exists(designerFile) ||
						File.GetLastWriteTimeUtc(designerFile) < info.LastWriteTimeUtc;
						// designer file is out of date
					
				}
			}
			return false;
		}

		public bool GenerateDesignerFiles(string dll, string WebRootPath, IEnumerable<string> aspFiles)
		{
			aspFiles = aspFiles.Where(ShouldCreateCodeFromAsp);
			// Begin doing the actual compiling task.
#if NETSTANDARD
			if (IsNetFX && IsTargetNetFX || IsCore && IsTargetNetCore ||
				IsCore && IsTargetNetFX)
			{

				Assembly buildAssembly;
				var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				if (IsCore) buildAssembly = System.Reflection.Assembly.LoadFrom(Path.GetFullPath(Path.Combine(path, "..\\net8.0\\EstrellasDeEsperanza.WebFormsForCore.Build.NetCore.dll")));
				else buildAssembly = System.Reflection.Assembly.LoadFrom(Path.GetFullPath(Path.Combine(path, "..\\net48\\EstrellasDeEsperanza.WebFormsForCore.Build.NetFX.dll")));

				var contextType = buildAssembly.GetType("EstrellasDeEsperanza.WebFormsForCore.Build.CreateAspDesignerFiles.MSBuildCompileContext");
				var context = Activator.CreateInstance(contextType, this);

				var common = buildAssembly.GetType("Redesigner.Library.Common");
				var generateDesignerFiles = common.GetMethod("GenerateDesignerFiles");
				generateDesignerFiles.Invoke(null, new object[] { context, aspFiles, WebRootPath, dll });

				var hasErrors = contextType.GetProperty("HasErrors");
				return (bool)hasErrors.GetValue(context);
			} else if (IsNetFX && IsTargetNetCore)
			{
				//LogMessage($"Starting dotnet...");
				var files = string.Join(";", aspFiles);

				bool hasErrors = false;
				var startInfo = new ProcessStartInfo("dotnet.exe", $"createdesignerfiles \"{dll}\"  \"{WebRootPath}\" \"{files}\"");
				startInfo.CreateNoWindow = true;
				startInfo.UseShellExecute = false;
				startInfo.RedirectStandardError = true;
				startInfo.RedirectStandardOutput = true;
				var p = new Process();
				p.StartInfo = startInfo;
				p.OutputDataReceived += (sender, args) =>
				{
					if (args.Data != null) Log.LogMessage(args.Data);
				};
				p.ErrorDataReceived += (sender, args) =>
				{
					if (args.Data != null)
					{
						hasErrors = true;
						Log.LogError(args.Data);
					}
				};
				p.EnableRaisingEvents = true;
				p.Start();
				p.BeginOutputReadLine();
				p.BeginErrorReadLine();
				p.WaitForExit(30000);
				return hasErrors;
			}
			return true;
#else
			var context = new MSBuildCompileContext(this);
			Common.GenerateDesignerFiles(context, aspFiles, WebRootPath, dll);
			return !context.HasErrors;
#endif
		}

		public override bool Execute()
		{
			return !Files.Any() ||
				GenerateDesignerFiles(Assembly.ItemSpec, Directory.ItemSpec, Files.Select(file => file.ItemSpec));
		}
	}
}
