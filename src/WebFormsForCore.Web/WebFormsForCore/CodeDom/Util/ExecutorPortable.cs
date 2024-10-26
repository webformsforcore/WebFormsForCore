// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace System.CodeDom.Compiler
{
	public static class ExecutorPortable
	{
		private const int ProcessTimeOut = 600000;

		private static FileStream CreateInheritedFile(string file) =>
			new FileStream(file, FileMode.CreateNew, FileAccess.Write, FileShare.Read | FileShare.Inheritable);

		public static void ExecWait(string cmd, string args, TempFileCollection tempFiles)
		{
			string outputName = null, errorName = null;
			ExecWaitWithCapture(cmd, args, tempFiles, ref outputName, ref errorName);
		}

		public static int ExecWaitWithCapture(IntPtr userToken, string cmd, string args, TempFileCollection tempFiles, ref string outputName, ref string errorName) =>
			ExecWaitWithCapture(userToken, cmd, args, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName);

		public static int ExecWaitWithCapture(string cmd, string args, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName) =>
			ExecWaitWithCapture(IntPtr.Zero, cmd, args, currentDir, tempFiles, ref outputName, ref errorName);

		public static int ExecWaitWithCapture(string cmd, string args, TempFileCollection tempFiles, ref string outputName, ref string errorName) =>
			ExecWaitWithCapture(IntPtr.Zero, cmd, args, Environment.CurrentDirectory, tempFiles, ref outputName, ref errorName);

		public static int ExecWaitWithCapture(IntPtr userToken, string cmd, string args, string currentDir, TempFileCollection tempFiles, ref string outputName, ref string errorName)
		{
			if (userToken != IntPtr.Zero)
			{
				throw new PlatformNotSupportedException();
			}

			if (string.IsNullOrEmpty(outputName))
			{
				outputName = tempFiles.AddExtension("out");
			}

			if (string.IsNullOrEmpty(errorName))
			{
				errorName = tempFiles.AddExtension("err");
			}

			using (var outputWriter = new StreamWriter(CreateInheritedFile(outputName), Encoding.UTF8))
			using (var errorWriter = new StreamWriter(CreateInheritedFile(errorName), Encoding.UTF8))
			{
				// Output the command line...
				outputWriter.Write(currentDir);
				outputWriter.Write("> ");
				outputWriter.Write(cmd);
				outputWriter.Write(" ");
				outputWriter.WriteLine(args);
				outputWriter.WriteLine();
				outputWriter.WriteLine();

				var psi = new ProcessStartInfo(cmd, args)
				{
					WorkingDirectory = currentDir,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};

				using (Process p = Process.Start(psi))
				{
					p.OutputDataReceived += (s, e) =>
					{
						if (e.Data != null)
						{
							outputWriter.WriteLine(e.Data);
						}
					};
					p.ErrorDataReceived += (s, e) =>
					{
						if (e.Data != null)
						{
							errorWriter.WriteLine(e.Data);
						}
					};

					p.BeginOutputReadLine();
					p.BeginErrorReadLine();

					if (!p.WaitForExit(ProcessTimeOut))
					{
						throw new ExternalException("Timeout error.");
					}

					p.WaitForExit();

					return p.ExitCode;
				}
			}
		}
	}
}
