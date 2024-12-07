// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Configuration;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Runtime.InteropServices;

#if WebFormsForCore && NETCOREAPP
using W = EstrellasDeEsperanza.WebFormsForCore.CodeDom.Compiler;
#else
using W = System.CodeDom.Compiler;
#endif

namespace EstrellasDeEsperanza.WebFormsForCore.CodeDom.Compiler
{
	internal abstract class Compiler : ICodeCompiler
	{
		private readonly W.CodeDomProvider _codeDomProvider;
		protected readonly IProviderOptions _providerOptions;
		private string _compilerFullPath = null;
		private const string CLR_PROFILING_SETTING = "COR_ENABLE_PROFILING";
		private const string DISABLE_PROFILING = "0";

		public Compiler(W.CodeDomProvider codeDomProvider, IProviderOptions providerOptions)
		{
			this._codeDomProvider = codeDomProvider;
			this._providerOptions = providerOptions;
		}

		public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (compilationUnit == null)
			{
				throw new ArgumentNullException("compilationUnit");
			}

			return CompileAssemblyFromDomBatch(options, new CodeCompileUnit[] { compilationUnit });
		}

		public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] compilationUnits)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (compilationUnits == null)
			{
				throw new ArgumentNullException("compilationUnits");
			}

			try
			{
				var sources = compilationUnits.Select(c =>
				{
					var writer = new StringWriter();
					_codeDomProvider.GenerateCodeFromCompileUnit(c, writer, new CodeGeneratorOptions());
					return writer.ToString();
				});

				return FromSourceBatch(options, sources.ToArray());
			}
			finally
			{
				options.TempFiles.Delete();
			}
		}

		public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}

			return CompileAssemblyFromFileBatch(options, new string[] { fileName });
		}

		public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (fileNames == null)
			{
				throw new ArgumentNullException("fileNames");
			}

			try
			{
				// Try opening the files to make sure they exists.  This will throw an exception
				// if it doesn't
				foreach (var fileName in fileNames)
				{
					using (var str = File.OpenRead(fileName)) { }
				}

				return FromFileBatch(options, fileNames);
			}
			finally
			{
				options.TempFiles.Delete();
			}
		}

		public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return CompileAssemblyFromSourceBatch(options, new string[] { source });
		}

		public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}

			try
			{
				return FromSourceBatch(options, sources);
			}
			finally
			{
				options.TempFiles.Delete();
			}
		}

		protected abstract string FileExtension
		{
			get;
		}

		protected virtual string CompilerName
		{
			get
			{
				if (null == _compilerFullPath)
				{
					_compilerFullPath = _providerOptions.CompilerFullPath;

					// Try opening the file to make sure the compiler exist.  This will throw an exception
					// if it doesn't
					using (var str = File.OpenRead(_compilerFullPath)) { }
				}

				return _compilerFullPath;
			}
		}

		protected abstract void ProcessCompilerOutputLine(CompilerResults results, string line);

		protected abstract string CmdArgsFromParameters(CompilerParameters options);

		protected abstract string FullPathsOption
		{
			get;
		}

		protected virtual void FixUpCompilerParameters(CompilerParameters options)
		{
			FixTreatWarningsAsErrors(options);
		}

		private string GetCompilationArgumentString(CompilerParameters options)
		{
			FixUpCompilerParameters(options);

			return CmdArgsFromParameters(options);
		}

		// CodeDom sets TreatWarningAsErrors to true whenever warningLevel is non-zero.
		// However, TreatWarningAsErrors should be false by default.
		// And users should be able to set the value by set the value of option "WarnAsError".
		// ASP.Net does fix this option in a like named function, but only for old CodeDom providers (CSharp/VB).
		// The old ASP.Net fix was to set TreatWarningAsErrors to false anytime '/warnaserror' was
		// detected in the compiler command line options, thus allowing the user-specified
		// option to prevail. In these CodeDom providers though, users have control through
		// the 'WarnAsError' provider option as well as manual control over the command
		// line args. 'WarnAsError' will default to false but can be set by the user.
		// So just go with the 'WarnAsError' provider option here.
		private void FixTreatWarningsAsErrors(CompilerParameters parameters)
		{
			parameters.TreatWarningsAsErrors = _providerOptions.WarnAsError;
		}

		private CompilerResults FromSourceBatch(CompilerParameters options, string[] sources)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}

			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

			var filenames = new string[sources.Length];
			CompilerResults results = null;

			// the extra try-catch is here to mitigate exception filter injection attacks.
			try
			{
#if NETFRAMEWORK
                WindowsImpersonationContext impersonation = RevertImpersonation();
#endif
				try
				{
					for (int i = 0; i < sources.Length; i++)
					{
						string name = options.TempFiles.AddExtension(i + FileExtension);
						var temp = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.Read);
						try
						{
							using (var sw = new StreamWriter(temp, Encoding.UTF8))
							{
								sw.Write(sources[i]);
								sw.Flush();
							}
						}
						finally
						{
							temp.Close();
						}
						filenames[i] = name;
					}

					results = FromFileBatch(options, filenames);
				}
				finally
				{
#if NETFRAMEWORK
                    ReImpersonate(impersonation);
#endif
				}
			}
			catch
			{
				throw;
			}

			return results;
		}

		private CompilerResults FromFileBatch(CompilerParameters options, string[] fileNames)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (fileNames == null)
			{
				throw new ArgumentNullException("fileNames");
			}

			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

			string outputFile = null;
			int retValue = 0;
			var results = new CompilerResults(options.TempFiles);
			var perm1 = new SecurityPermission(SecurityPermissionFlag.ControlEvidence);
			perm1.Assert();
			try
			{

#pragma warning disable 618
#if NETFRAMEWORK
                results.Evidence = options.Evidence;
#endif
#pragma warning restore 618

			}
			finally
			{
				SecurityPermission.RevertAssert();
			}

			bool createdEmptyAssembly = false;
			if (options.OutputAssembly == null || options.OutputAssembly.Length == 0)
			{
				string extension = (options.GenerateExecutable) ? "exe" : "dll";
				options.OutputAssembly = results.TempFiles.AddExtension(extension, !options.GenerateInMemory);

				// Create an empty assembly.  This is so that the file will have permissions that
				// we can later access with our current credential. If we don't do this, the compiler
				// could end up creating an assembly that we cannot open.
				new FileStream(options.OutputAssembly, FileMode.Create, FileAccess.ReadWrite).Close();
				createdEmptyAssembly = true;
			}

			var pdbname = "pdb";

			// Don't delete pdbs when debug=false but they have specified pdbonly.
			if (options.CompilerOptions != null
					&& -1 != CultureInfo.InvariantCulture.CompareInfo.IndexOf(options.CompilerOptions, "/debug:pdbonly", CompareOptions.IgnoreCase))
			{
				results.TempFiles.AddExtension(pdbname, true);
			}
			else
			{
				results.TempFiles.AddExtension(pdbname);
			}

			string args = GetCompilationArgumentString(options) + " " + JoinStringArray(fileNames, " ");

			// Use a response file if the compiler supports it
			string responseFileArgs = GetResponseFileCmdArgs(options, args);
			string trueArgs = null;
			if (responseFileArgs != null)
			{
				trueArgs = args;
				args = responseFileArgs;
			}

			// Appending TTL to the command line arguments.
			if (_providerOptions.CompilerServerTimeToLive > 0)
			{
				args = string.Format("/shared /keepalive:\"{0}\" {1}", _providerOptions.CompilerServerTimeToLive, args);
			}

			Compile(options,
				CompilerName,
				args,
				ref outputFile,
				ref retValue);

			results.NativeCompilerReturnValue = retValue;

			// only look for errors/warnings if the compile failed or the caller set the warning level
			if (retValue != 0 || options.WarningLevel > 0)
			{

				// The output of the compiler is in UTF8
				string[] lines = ReadAllLines(outputFile, Encoding.UTF8, FileShare.ReadWrite);
				bool replacedArgs = false;
				foreach (string line in lines)
				{
					if (!replacedArgs && trueArgs != null && line.Contains(args))
					{
						replacedArgs = true;
						var outputLine = string.Format("{0}>{1} {2}",
							Environment.CurrentDirectory,
							CompilerName,
							trueArgs);
						results.Output.Add(outputLine);
					}
					else
					{
						results.Output.Add(line);
					}

					ProcessCompilerOutputLine(results, line);
				}

				// Delete the empty assembly if we created one
				if (retValue != 0 && createdEmptyAssembly)
				{
					File.Delete(options.OutputAssembly);
				}
			}

			if (retValue != 0 || results.Errors.HasErrors || !options.GenerateInMemory)
			{

				results.PathToAssembly = options.OutputAssembly;
				return results;
			}

			// Read assembly into memory:
			byte[] assemblyBuff = File.ReadAllBytes(options.OutputAssembly);

			// Read symbol file into memory and ignore any errors that may be encountered:
			// (This functionality was added in NetFx 4.5, errors must be ignored to ensure compatibility)
			byte[] symbolsBuff = null;
			try
			{

				string symbFileName = options.TempFiles.BasePath + "." + pdbname;

				if (File.Exists(symbFileName))
				{
					symbolsBuff = File.ReadAllBytes(symbFileName);
				}
			}
			catch
			{
				symbolsBuff = null;
			}

			// Now get permissions and load assembly from buffer into the CLR:
			var perm = new SecurityPermission(SecurityPermissionFlag.ControlEvidence);
			perm.Assert();

			try
			{

#pragma warning disable 618 // Load with evidence is obsolete - this warning is passed on via the options.Evidence property
#if NETFRAMEWORK
                results.CompiledAssembly = Assembly.Load(assemblyBuff, symbolsBuff, options.Evidence);
#else
				results.CompiledAssembly = Assembly.Load(assemblyBuff, symbolsBuff);
#endif
#pragma warning restore 618

			}
			finally
			{
				SecurityPermission.RevertAssert();
			}

			return results;
		}

#if NETFRAMEWORK
        private static void ReImpersonate(WindowsImpersonationContext impersonation) {
            impersonation.Undo();
        }

        [PermissionSet(SecurityAction.LinkDemand, Unrestricted = true), SecurityPermission(SecurityAction.Assert, ControlPrincipal = true, UnmanagedCode = true)]
        private static WindowsImpersonationContext RevertImpersonation() {
            return WindowsIdentity.Impersonate(new IntPtr(0));
        }
#endif
		private static string[] ReadAllLines(String file, Encoding encoding, FileShare share)
		{
			using (var stream = File.Open(file, FileMode.Open, FileAccess.Read, share))
			{
				String line;
				var lines = new List<String>();

				using (var sr = new StreamReader(stream, encoding))
				{
					while ((line = sr.ReadLine()) != null)
					{
						lines.Add(line);
					}
				}

				return lines.ToArray();
			}
		}

		public string[] ParseArguments(string commandLine)
		{
			char[] parmChars = commandLine.ToCharArray();
			bool inQuote = false;
			for (int index = 0; index < parmChars.Length; index++)
			{
				if (parmChars[index] == '"')
					inQuote = !inQuote;
				if (!inQuote && parmChars[index] == ' ')
					parmChars[index] = '\n';
			}
			return (new string(parmChars)).Split('\n');
		}

		public string MapPath(string path) => System.Web.Hosting.HostingEnvironment.MapPath(path);

		private void CompileCSharp(string arguments)
		{
#if NETCOREAPP
			var args = ParseArguments(arguments);
			Microsoft.CodeAnalysis.CSharp.CommandLine.Program.Main(args);
#endif
		}

		private void CompileVB(string arguments)
		{
#if NETCOREAPP
			var args = ParseArguments(arguments);
			Microsoft.CodeAnalysis.VisualBasic.CommandLine.Program.Main(args);
#endif
		}

		private string Find(string command)
		{
			if (command.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
				command = command.Substring(0, command.Length - 4);
			var commandWithExe = command + ".exe";

			IEnumerable<string> paths;
			if (OSInfo.IsWindows)
			{
				paths = new[] { "", Environment.CurrentDirectory,
					Environment.GetFolderPath(Environment.SpecialFolder.System),
					Environment.GetFolderPath(Environment.SpecialFolder.SystemX86) }
					.Concat((Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine) ?? "")
						.Split(Path.PathSeparator))
					.Concat((Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User) ?? "")
						.Split(Path.PathSeparator))
					.Where(path => !string.IsNullOrEmpty(path));
			} else
			{
				paths = new[] { "", Environment.CurrentDirectory }
					.Concat(Environment.GetEnvironmentVariable("PATH")
						.Split(Path.PathSeparator));
			}
			return paths
				.SelectMany(path => new[] {
					Path.Combine(path, command),
					Path.Combine(path, commandWithExe) })
				.FirstOrDefault(path => File.Exists(path));
		}
		private void Compile(CompilerParameters options, string compilerFullPath, string arguments,
							  ref string outputFile, ref int nativeReturnValue)
		{
			string errorFile = null;
			outputFile = options.TempFiles.AddExtension("out");
			System.Diagnostics.Debug.WriteLine($"Compile {outputFile} ...");

			bool profilingSettingIsUpdated = false;
			string originalClrProfilingSetting = null;
			if (AppSettings.DisableProfilingDuringCompilation)
			{
				// if CLR_PROFILING_SETTING is not set in environment variables, this returns null
				originalClrProfilingSetting = Environment.GetEnvironmentVariable(CLR_PROFILING_SETTING, EnvironmentVariableTarget.Process);
				// if CLR profiling is already disabled, don't bother to set it again
				if (originalClrProfilingSetting != DISABLE_PROFILING)
				{
					Environment.SetEnvironmentVariable(CLR_PROFILING_SETTING, DISABLE_PROFILING, EnvironmentVariableTarget.Process);
					profilingSettingIsUpdated = true;
				}
			}

			string[] args;
			string cmdLine = null;

/*#if NETCOREAPP
			var appBinPath = AppDomain.CurrentDomain.BaseDirectory;
			if (compilerFullPath == Path.Combine(appBinPath, "csc.dll"))
			{
				CompileCSharp(arguments);
			}
			else if (compilerFullPath == Path.Combine(appBinPath, "vbc.dll"))
			{
				CompileVB(arguments);
			}
			else
#endif */
			if (compilerFullPath.EndsWith(".dll"))
			{
				var dotnetExe = Find("dotnet");
				cmdLine = $"\"{compilerFullPath}\" {arguments}";
				nativeReturnValue = ExecutorPortable.ExecWaitWithCapture(
					options.UserToken,
					dotnetExe, cmdLine,
					Environment.CurrentDirectory,
					options.TempFiles,
					ref outputFile,
					ref errorFile);
			}
			else
			{
				cmdLine = "\"" + compilerFullPath + "\" " + arguments; ;
				nativeReturnValue = Executor.ExecWaitWithCapture(
					options.UserToken,
					cmdLine,
					Environment.CurrentDirectory,
					options.TempFiles,
					ref outputFile,
					ref errorFile);
			}

			if (profilingSettingIsUpdated)
			{
				Environment.SetEnvironmentVariable(CLR_PROFILING_SETTING, originalClrProfilingSetting, EnvironmentVariableTarget.Process);
			}
		}

		private string GetResponseFileCmdArgs(CompilerParameters options, string cmdArgs)
		{

			string responseFileName = options.TempFiles.AddExtension("cmdline");
			var responseFileStream = new FileStream(responseFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
			try
			{
				using (var sw = new StreamWriter(responseFileStream, Encoding.UTF8))
				{
					sw.Write(cmdArgs);
					sw.Flush();
				}
			}
			finally
			{
				responseFileStream.Close();
			}

			// Always specify the /noconfig flag (outside of the response file)
			return "/noconfig " + FullPathsOption + "@\"" + responseFileName + "\"";
		}

		private static string JoinStringArray(string[] sa, string separator)
		{
			if (sa == null || sa.Length == 0)
			{
				return String.Empty;
			}

			if (sa.Length == 1)
			{
				return "\"" + sa[0] + "\"";
			}

			var sb = new StringBuilder();
			for (int i = 0; i < sa.Length - 1; i++)
			{
				sb.Append("\"");
				sb.Append(sa[i]);
				sb.Append("\"");
				sb.Append(separator);
			}

			sb.Append("\"");
			sb.Append(sa[sa.Length - 1]);
			sb.Append("\"");

			return sb.ToString();
		}
	}
}
