﻿#if !NETFRAMEWORK
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Loader;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Hosting
{
	public class AssemblyLoaderNetCore
	{
		static int Initialized = 0;
		public static void Init()
		{
			var initialized = Interlocked.Exchange(ref Initialized, 1);
			if (initialized == 0) AssemblyLoadContext.Default.Resolving += Resolve;
		}

		static string probingPaths = "";
		public static string ProbingPaths {
			get => probingPaths;
			set { probingPaths = value; paths = null; }
		}
		
		static readonly string exepath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
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

		public static Assembly Resolve(AssemblyLoadContext context, AssemblyName name)
		{
			return Paths
				.Select(p =>
				{
					var relativename = Path.Combine(p, $"{name.Name}.dll");
					return new
					{
						FullName = new DirectoryInfo(Path.Combine(exepath, relativename)).FullName,
						Name = relativename
					};
				})
				.Where(p => File.Exists(p.FullName))
				.Select(p => context.LoadFromAssemblyPath(p.FullName))
				.Where(assembly => assembly != null &&
					(name.Version == null || assembly.GetName().Version >= name.Version))
				.FirstOrDefault();
		}
	}
}
#endif