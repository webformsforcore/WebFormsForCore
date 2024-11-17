//-------------------------------------------------------------------------------------------------
//
//  Redesigner
//
//  Copyright (c) 2012-3 by Sean Werkema
//  All rights reserved.
//
//  This software is released under the terms of the "New BSD License," as follows:
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted
//  provided that the following conditions are met:
//
//   * Redistributions of source code must retain the above copyright notice, this list of
//     conditions and the following disclaimer.
//
//   * Redistributions in binary form must reproduce the above copyright notice, this list of
//     conditions and the following disclaimer in the documentation and/or other materials
//     provided with the distribution.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
//  IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
//  AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
//  CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
//  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
//  OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace Redesigner.Library
{
	/// <summary>
	/// This class loads the given assemblies for reflection, caching them to avoid reloading them multiple times.
	/// </summary>
	public class AssemblyLoader
	{
		#region Properties and Fields

		/// <summary>
		/// The complete set of loaded assemblies, organized by assembly name.
		/// </summary>
		private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();

		/// <summary>
		/// The primary assembly (i.e., the website's DLL).
		/// </summary>
		public Assembly PrimaryAssembly { get; set; }

		/// <summary>
		/// Retrieve a loaded assembly, by name.  If it does not exist, this will throw an exception.
		/// </summary>
		/// <param name="name">The name of the assembly to get.</param>
		/// <returns>The loaded assembly.</returns>
		public Assembly this[string name]
		{
			get
			{
				if (!_loadedAssemblies.ContainsKey(name))
					throw new InvalidOperationException(string.Format("Assembly \"{0}\" is not loaded.", name));

				return _loadedAssemblies[name];
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Preload the given assemblies, looking in the given directory to match any assemblies
		/// that are not found in the Global Assembly Cache.
		/// </summary>
		/// <param name="compileContext">The context in which errors should be reported.</param>
		/// <param name="names">The names of the assemblies to preload.</param>
		/// <param name="directory">The directory to search in for assemblies not in the GAC.</param>
		public void PreloadAssemblies(ICompileContext compileContext, IEnumerable<string> names, string directory)
		{
			compileContext.Verbose("Loading assemblies for reflection.");
			compileContext.VerboseNesting++;

			string olddir = Directory.GetCurrentDirectory();
			Directory.SetCurrentDirectory(directory);

			try
			{
				foreach (string name in names)
				{
					if (_loadedAssemblies.ContainsKey(name)) continue;

					compileContext.Verbose("Loading assembly \"{0}\".", name);

					compileContext.VerboseNesting++;
					Assembly assembly = TryToLoadAssembly(compileContext, name);
					compileContext.VerboseNesting--;
					if (assembly == null)
						throw new RedesignerException("Could not find assembly \"{0}\".", name);

					_loadedAssemblies.Add(name, assembly);
				}
			}
			finally
			{
				Directory.SetCurrentDirectory(olddir);
				compileContext.VerboseNesting--;
			}

			compileContext.Verbose("Loaded all assemblies.");
			compileContext.Verbose("");
		}

		/// <summary>
		/// Attempt to load the given assembly, by name, either from the GAC, or from the current directory.
		/// </summary>
		/// <param name="compileContext">The context in which errors should be reported.</param>
		/// <param name="name">The name of the assembly to load.</param>
		/// <returns>The loaded assembly, or null if it could not be found.</returns>
		private static Assembly TryToLoadAssembly(ICompileContext compileContext, string name)
		{
			Assembly assembly;

			try
			{
				assembly = Assembly.Load(name);
				compileContext.Verbose("Found it in the GAC.");
			}
			catch (Exception)
			{
				try
				{
					compileContext.Verbose("Assembly is not in GAC, trying again as a file in directory of website DLL.");
					assembly = Assembly.LoadFrom(name);
					compileContext.Verbose("Found it in the website directory.");
				}
				catch (Exception)
				{
					try
					{
						compileContext.Verbose("Trying again with '.dll' added onto the end of the assembly name.");
						assembly = Assembly.LoadFrom(name + ".dll");
						compileContext.Verbose("Found it in the website directory.");
					}
					catch (Exception)
					{
						assembly = null;
					}
				}
			}

			return assembly;
		}

		#endregion
	}
}
