#if !NETFRAMEWORK
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace System.Web.Hosting
{
	public class AssemblyLoaderNetCore
	{
		static int Initialized = 0;
		public static void Init(string binpath = null)
		{
			var initialized = Interlocked.Exchange(ref Initialized, 1);
			if (initialized == 0)
			{
				exepath = binpath ?? exepath;
				AssemblyLoadContext.Default.Resolving += Resolve;
			}
		}

		static string probingPaths = "";
		public static string ProbingPaths {
			get => probingPaths;
			set { probingPaths = value; paths = null; }
		}
		
		static string exepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		static string[] extensions = new string[] { exepath };
		static string tempPath = null;
		public static string TempPath {
			get => tempPath;
			set
			{
				tempPath = value;
				extensions = new string[] { exepath, tempPath };
				paths = null;
			}
		}

		static string[] paths = null;
		static string[] Paths => paths != null ? paths : paths =
			ProbingPaths
				.Replace('\\', Path.DirectorySeparatorChar)
				.Split(';')
				.Concat(extensions)
				.ToArray();
		public static bool UseNetFXGAC = false;

		public static Assembly Resolve(AssemblyLoadContext context, AssemblyName name)
		{
			//Debugger.Break();
			Debug.WriteLine($"Resolving {name.Name}");
			var paths = Paths;
			if (UseNetFXGAC)
			{
				var winAssemblyDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
					"Microsoft.NET", "assembly");
                byte[] tokenBytes = name.GetPublicKeyToken();
                string token = BitConverter
                    .ToString(tokenBytes)
                    .Replace("-", "")
                    .ToLowerInvariant();

				var subdir = Path.Combine(name.Name, $"v4.0_{name.Version}__{token}");
				var gacNative = OSInfo.Is64 ? "GAC_64" : "GAC_32";
				paths = paths
					.Concat(new[] { Path.Combine(winAssemblyDir, "GAC_MSIL", subdir),
						Path.Combine(winAssemblyDir, gacNative, subdir) })
					.ToArray();
			}
			var assembly = paths
				.Select(p =>
				{
					var relativename = Path.Combine(p, $"{name.Name}.dll");
					return new
					{
						FullName = Path.IsPathRooted(relativename) ? relativename :
							new DirectoryInfo(Path.Combine(exepath, relativename)).FullName,
						Name = relativename
					};
				})
				.Where(p => File.Exists(p.FullName))
				.Select(p => context.LoadFromAssemblyPath(p.FullName))
				.Where(assembly => assembly != null &&
					(name.Version == null || assembly.GetName().Version >= name.Version))
				.FirstOrDefault();
			if (assembly != null) Debug.WriteLine($"Loaded assembly {name.Name}");
			else Debug.WriteLine($"Failed to resolve assembly {name.Name}");
			return assembly;
		}
	}
}
#endif