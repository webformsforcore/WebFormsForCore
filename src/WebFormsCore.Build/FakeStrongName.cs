using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Build;
using Microsoft.Build.Framework;
using Microsoft.Build.Shared;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using System.Runtime.InteropServices;

namespace EstrellasDeEsperanza.WebFormsCore.Build
{
	public class FakeStrongName : Task
	{
		public static bool IsCore => !(IsNetFX || IsNetNative);
		public static bool IsNetFX => RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
		public static bool IsNetNative => RuntimeInformation.FrameworkDescription.StartsWith(".NET Native", StringComparison.OrdinalIgnoreCase);
		public ITaskItem[]? Assemblies { get; set; }
		public ITaskItem? Source { get; set; }
		public ITaskItem? Key { get; set; }
		public string? PublicKey { get; set; }
		public string? PublicKeyToken { get; set; }
		public bool LogToConsole { get; set; } = false;
		byte[] StringToByteArray(string hex)
		{
			int length = hex.Length;
			byte[] bytes = new byte[length / 2];

			for (int i = 0; i < length; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return bytes;
		}

		void LogError(string msg)
		{
			if (Log != null && !LogToConsole) Log.LogError(msg);
			else Console.Error.WriteLine(msg);
		}

		void LogMessage(string msg)
		{
			if (Log != null && !LogToConsole) Log.LogMessage(MessageImportance.High, msg);
			else Console.WriteLine(msg);
		}

		public override bool Execute()
		{
			byte[]? publicKey = null;
			byte[]? publicKeyToken = null;

			if (string.IsNullOrEmpty(PublicKey) || string.IsNullOrEmpty(PublicKeyToken))
			{
				if (string.IsNullOrEmpty(Source?.ItemSpec))
				{
					LogError("Need to specify PublicKey if no Source specified.");
					return false;
				}

				if (!File.Exists(Source?.ItemSpec))
				{
					LogError("Need to specify a valid Source dll in Source.");
					return false;
				}
				var source = AssemblyDefinition.ReadAssembly(Source?.ItemSpec);
				publicKey = source.Name.PublicKey;
				publicKeyToken = source.Name.PublicKeyToken;
			}
			else if (!string.IsNullOrEmpty(PublicKey) && !string.IsNullOrEmpty(PublicKeyToken))
			{
				try
				{
					publicKey = StringToByteArray(PublicKey!);
					publicKeyToken = StringToByteArray(PublicKeyToken!);
				}
				catch
				{
					LogError("Invalid PublicKey.");
					return false;
				}
			}
			else
			{
				LogError("You must specify a PublicKey & PublicKeyToken.");
				return false;
			}

			var keyFileName = Key?.ItemSpec;
			if (string.IsNullOrEmpty(keyFileName) || !File.Exists(keyFileName))
			{
				LogError("Need to specify a valid *.snk key file in Key.");
				return false;
			}

			if (Assemblies == null)
			{
				LogError("Must specify Assemblies.");
				return false;
			}

			try
			{
				if (!IsCore)
				{
					LogMessage($"Starting dotnet...");
					var assemblies = string.Join(";", Assemblies
						.Select(a => a.ItemSpec)
						.ToArray());

					var dll = Assembly.GetExecutingAssembly().Location;

					var startInfo = new ProcessStartInfo("dotnet.exe", $"\"{dll}\" \"{assemblies}\" " +
						$"\"{PublicKey}\" \"{PublicKeyToken}\" \"{Key?.ItemSpec}\" " +
						$"\"{Source?.ItemSpec}\"");
					startInfo.CreateNoWindow = true;
					startInfo.UseShellExecute = false;
					startInfo.RedirectStandardError = true;
					startInfo.RedirectStandardOutput = true;
					var p = new Process();
					p.StartInfo = startInfo;
					//p.EnableRaisingEvents = true;
					p.OutputDataReceived += (sender, args) =>
					{
						if (args.Data != null) LogMessage(args.Data);
					};
					p.ErrorDataReceived += (sender, args) =>
					{
						if (args.Data != null) LogError(args.Data);
					};
					p.EnableRaisingEvents = true;
					p.Start();
					p.BeginOutputReadLine();
					p.BeginErrorReadLine();
					p.WaitForExit(30000);
					LogMessage($"Replaced public keys.");
				}
				else
				{
					using (var keyFile = new FileStream(keyFileName, FileMode.Open, FileAccess.Read))
					{
						StrongNameKeyPair? key = null;
						try
						{
							key = new StrongNameKeyPair(keyFile);
						}
						catch { }

						foreach (var assemblyItem in Assemblies)
						{
							var assemblyFileName = assemblyItem.ItemSpec;
							var assemblyCopyName = $"{assemblyFileName}.Copy.dll";

							using (var assembly = AssemblyDefinition.ReadAssembly(assemblyFileName))
							{
								assembly.Name.PublicKey = publicKey;
								assembly.Name.PublicKeyToken = publicKeyToken;
								assembly.Write(assemblyCopyName);
							}

							if (File.Exists(assemblyCopyName))
							{
								File.Copy(assemblyCopyName, assemblyFileName, true);
								File.Delete(assemblyCopyName);
								LogMessage($"Replaced public key in {assemblyFileName}.");
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogError($"Exception creating FakeStrongName: {ex}");
				return false;
			}
			return true;
		}
	}
}
