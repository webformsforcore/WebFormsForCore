using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace WebFormsForCore.Compilers
{
	public class Compiler
	{
		private static ImmutableDictionary<string, Assembly> Dlls = ImmutableDictionary<string, Assembly>.Empty;
		public static bool Run(string cmd, string args, string workingDirectory, TextWriter output, out int exitCode)
		{
			var arguments = CommandLineParser.SplitCommandLineIntoArguments(args, false)
				.Where(arg => !arg.StartsWith("/shared") && !arg.StartsWith("/keepalive"))
				.ToArray();
			cmd = arguments.FirstOrDefault() ?? "";
			arguments = arguments
				.Skip(1)
				.ToArray();
			var file = Path.GetFileName(cmd);
			if (file.Equals("csc.dll", StringComparison.OrdinalIgnoreCase) ||
				file.Equals("vbc.dll", StringComparison.OrdinalIgnoreCase))
			{
				Assembly? assembly = null;
				if (!Dlls.TryGetValue(cmd, out assembly))
				{
					assembly = Assembly.LoadFrom(cmd);
					Interlocked.Exchange(ref Dlls, Dlls.Add(cmd, assembly));
				}

				var buildClientType = assembly.GetType("Microsoft.CodeAnalysis.CommandLine.BuildClient");
				var sdkPath = buildClientType.GetMethod("GetSystemSdkDirectory").Invoke(null, new object[0]) as string;
				var clientPath = buildClientType.GetMethod("GetClientDirectory").Invoke(null, new object[0]) as string;
				var buildServerConnectionType = assembly.GetType("Microsoft.CodeAnalysis.CommandLine.BuildServerConnection");
				var tempPath = buildServerConnectionType.GetMethod("GetTempPath", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { workingDirectory }) as string;

				var loader = Activator.CreateInstance(Type.GetType("Microsoft.CodeAnalysis.DefaultAnalyzerAssemblyLoader, Microsoft.CodeAnalysis"), true);
				var buildPaths = Activator.CreateInstance(Type.GetType("Microsoft.CodeAnalysis.BuildPaths, Microsoft.CodeAnalysis"),
						BindingFlags.NonPublic | BindingFlags.Instance,
						null,
						new object[] { clientPath, workingDirectory, sdkPath, tempPath },
						null);

				if (file.Equals("csc.dll", StringComparison.OrdinalIgnoreCase)) {
					var cscType = assembly.GetType("Microsoft.CodeAnalysis.CSharp.CommandLine.Csc");
					exitCode = (int)cscType.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static)
						.Invoke(null, new object[] {
							arguments,
							buildPaths,
							output,
							loader
						});
					return true;
				} else
				{
					var vbcType = assembly.GetType("Microsoft.CodeAnalysis.VisualBasic.CommandLine.Vbc");
					exitCode = (int)vbcType.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static)
						.Invoke(null, new object[] {
							arguments,
							buildPaths,
							output,
							loader
						});
					return true;
				}
			} else {
				exitCode = 0;
				return false;
			}
		}
	}
}