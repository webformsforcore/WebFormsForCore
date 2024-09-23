using System;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using Mono.Cecil.Pdb;
using System.Runtime.InteropServices;

#nullable enable

namespace EstrellasDeEsperanza.WebFormsForCore.Build
{
	public class FakeStrongNameTask : Task
	{
		public const bool NeedsKey = false;

		public const bool CreateFakeStrongName = true;
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

			if (!CreateFakeStrongName) return true;

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
			if (NeedsKey && (string.IsNullOrEmpty(keyFileName) || !File.Exists(keyFileName)))
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
					string[] a = new string[Assemblies.Length];
					for (int i = 0; i < Assemblies.Length; i++)
					{
						a[i] = Assemblies[i].ItemSpec;
					}
					var assemblies = string.Join(";", a);

					var dll = Assembly.GetExecutingAssembly().Location;

					var startInfo = new ProcessStartInfo("dotnet.exe", $"\"{dll}\" \"{assemblies}\" " +
						$"\"{PublicKey ?? ""}\" \"{PublicKeyToken ?? ""}\" \"{Key?.ItemSpec ?? ""}\" " +
						$"\"{Source?.ItemSpec ?? ""}\"");
					startInfo.CreateNoWindow = true;
					startInfo.UseShellExecute = false;
					startInfo.RedirectStandardError = true;
					startInfo.RedirectStandardOutput = true;
					var p = new Process();
					p.StartInfo = startInfo;
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
				}
				else
				{
					StrongNameKeyPair? key = null;
					try
					{
						using (var keyFile = new FileStream(keyFileName, FileMode.Open, FileAccess.Read))
						{
							key = new StrongNameKeyPair(keyFile);
						}
					}
					catch { }

					foreach (var assemblyItem in Assemblies)
					{
						var assemblyFileName = assemblyItem.ItemSpec;
						//var assemblyCopyName = Path.ChangeExtension(assemblyFileName, ".Fake.dll");

						if (File.Exists(assemblyFileName))
						{

							bool success = false;

							var pdbFile = Path.ChangeExtension(assemblyFileName, ".pdb");
							bool withSymbols = File.Exists(pdbFile);
							var symReader = new PdbReaderProvider();
							var symWriter = new PdbWriterProvider();
							var mem = new MemoryStream();
							var readerParameters = new ReaderParameters() { ReadWrite = true, InMemory = true, ReadSymbols = withSymbols };

							if (withSymbols)
							{
								using (var pdbStream = new FileStream(pdbFile, FileMode.Open, FileAccess.Read))
									pdbStream.CopyTo(mem);

								mem.Seek(0, SeekOrigin.Begin);
								readerParameters.SymbolReaderProvider = symReader;
								readerParameters.SymbolStream = mem;
							}

							using (var assembly = AssemblyDefinition.ReadAssembly(assemblyFileName, readerParameters))
							{
								assembly.Name.PublicKey = publicKey;
								assembly.Name.PublicKeyToken = publicKeyToken;

								//File.Delete(assemblyFileName);
								//File.Delete(pdbFile);
								var writerParameters = new WriterParameters() { DeterministicMvid = true, WriteSymbols = withSymbols };
								Stream pdbWriteStream = null;
								
								if (withSymbols)
								{
									pdbWriteStream = new FileStream(pdbFile, FileMode.Create, FileAccess.ReadWrite);
									writerParameters.SymbolWriterProvider = symWriter;
									writerParameters.SymbolStream = pdbWriteStream;
								}
								
								assembly.Write(assemblyFileName, writerParameters);
								
								pdbWriteStream?.Close();

								success = true;
							}

							if (success)
							{
								//File.Copy(assemblyCopyName, assemblyFileName, true);
								//File.Copy(Path.ChangeExtension(assemblyCopyName, ".pdb"), Path.ChangeExtension(assemblyFileName, ".pdb"), true);
								LogMessage($"Replaced public key in {assemblyFileName} with {PublicKeyToken}.");
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
