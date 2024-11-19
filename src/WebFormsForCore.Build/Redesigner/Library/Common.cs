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
using System.IO;
using System.Linq;
using System.Text;

namespace Redesigner.Library
{
	public class Common
	{
		/// <summary>
		/// The canonical name of the "web.config" file.
		/// </summary>
		private const string WebConfigFilename = "web.config";

		/// <summary>
		/// The canonical name of the "System.Web" assembly.
		/// </summary>
		public const string SystemWebAssemblyName = "System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
		public const string SystemWebExtensionsAssemblyName = "System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

		/// <summary>
		/// The standard tag registrations that are always included, regardless of whether they
		/// are mentioned in the "web.config" or markup.
		/// 
		/// Note that HTML server controls are handled specially and are not included here.
		/// </summary>
		private static readonly IEnumerable<TagRegistration> _standardTagRegistrations = new List<TagRegistration>
		{
			new TagRegistration
			{
				Kind = TagRegistrationKind.Namespace,
				AssemblyFilename = SystemWebAssemblyName,
				Namespace = "System.Web.UI.WebControls",
				TagPrefix = "asp",
			},
			new TagRegistration
			{
				Kind = TagRegistrationKind.Namespace,
				AssemblyFilename = SystemWebAssemblyName,
				Namespace = "System.Web.UI.WebControls.WebParts",
				TagPrefix = "asp",
			}
		};
		private static readonly IEnumerable<TagRegistration> _extensionTagRegistrations = new List<TagRegistration>
		{
			new TagRegistration
			{
				Kind = TagRegistrationKind.Namespace,
				AssemblyFilename = SystemWebExtensionsAssemblyName,
				Namespace = "System.Web.UI",
				TagPrefix = "asp",
			},
			new TagRegistration
			{
				Kind = TagRegistrationKind.Namespace,
				AssemblyFilename = SystemWebExtensionsAssemblyName,
				Namespace = "System.Web.UI.WebControls",
				TagPrefix = "asp",
			},
			new TagRegistration
			{
				Kind = TagRegistrationKind.Namespace,
				AssemblyFilename = SystemWebExtensionsAssemblyName,
				Namespace = "System.Web.UI.WebControls.Expressions",
				TagPrefix = "asp",
			}
		};

		/// <summary>
		/// For the given set of .aspx or .ascx files, generate all of their designer files.
		/// </summary>
		/// <param name="compileContext">The context in which errors are to be reported.</param>
		/// <param name="filenames">The filenames to generate.</param>
		/// <param name="rootPath">The root disk path of the website (usually the same as the path to "web.config").</param>
		/// <param name="websiteDllFileName">The disk path to the website's DLL.</param>
		public static bool GenerateDesignerFiles(ICompileContext compileContext, IEnumerable<string> filenames, string rootPath, string websiteDllFileName)
		{
			int filenameCount = filenames.Count();
			compileContext.BeginTask(filenameCount);

			// Load and parse the "web.config".
			WebConfigReader webConfigReader = new WebConfigReader();
			try
			{
				webConfigReader.LoadWebConfig(compileContext, Path.Combine(rootPath, WebConfigFilename), rootPath);
			}
			catch (Exception e)
			{
				compileContext.Error("Cannot load {0}:\r\n{1}", Path.Combine(rootPath, WebConfigFilename), e.Message);
				return false;
			}

			// Load any assemblies we know we'll need.  This includes the default assemblies, any declared
			// in the web.config, and, of course, the website's DLL itself.
			AssemblyLoader assemblyLoader = new AssemblyLoader();
			List<string> assemblyNames = new List<string>();
			assemblyNames.AddRange(_standardTagRegistrations.Where(r => !string.IsNullOrEmpty(r.AssemblyFilename)).Select(r => r.AssemblyFilename).Distinct());
			assemblyNames.AddRange(webConfigReader.TagRegistrations.Where(r => !string.IsNullOrEmpty(r.AssemblyFilename)).Select(r => r.AssemblyFilename).Distinct());
			string dllFullPath = Path.GetFullPath(websiteDllFileName);
			var dllsPath = Path.GetDirectoryName(dllFullPath);
			if (File.Exists(Path.Combine(dllsPath, "System.Web.Extensions.dll")))
				assemblyNames.Add(SystemWebExtensionsAssemblyName);
			assemblyNames.Add(dllFullPath);
			string assemblyDirectory = Path.GetDirectoryName(dllFullPath);
			assemblyLoader.PreloadAssemblies(compileContext, assemblyNames, assemblyDirectory);
			assemblyLoader.PrimaryAssembly = assemblyLoader[dllFullPath];

			// Add the default tag registrations, including those from System.Web and any declared in the "web.config".
			List<TagRegistration> tagRegistrations = new List<TagRegistration>();
			tagRegistrations.AddRange(_standardTagRegistrations);
			if (File.Exists(Path.Combine(dllsPath, "System.Web.Extensions.dll")))
				tagRegistrations.AddRange(_extensionTagRegistrations);
			tagRegistrations.AddRange(webConfigReader.TagRegistrations);

			// Spin through any user controls that were declared in the web.config and connect them to their actual
			// .NET class types via reflection.
			compileContext.Verbose("Resolving user controls declared in the web.config.");
			compileContext.VerboseNesting++;
			ResolveUserControls(compileContext, tagRegistrations, assemblyLoader, assemblyDirectory, rootPath, rootPath);
			compileContext.VerboseNesting--;
			compileContext.Verbose(string.Empty);

			// Now that all the setup is done, load and parse each individual markup file into its own .designer.cs output file.
			bool result = true;
			foreach (string filename in filenames)
			{
				compileContext.Verbose("Begin processing \"{0}\"...", filename);
				compileContext.Verbose("");
				compileContext.VerboseNesting++;

				compileContext.BeginFile(filename);
				bool succeeded = GenerateDesignerForFilename(compileContext, filename, tagRegistrations, assemblyLoader, assemblyDirectory, rootPath);
				result &= succeeded;
				compileContext.EndFile(filename, succeeded);

				compileContext.VerboseNesting--;
				compileContext.Verbose("");
				compileContext.Verbose("End processing \"{0}\".", filename);
			}
			return result;
		}

		/// <summary>
		/// Generate a replacement .designer.cs file for the given markup file, overwriting the existing
		/// .designer.cs file if there is one.
		/// </summary>
		public static bool GenerateDesignerForFilename(ICompileContext compileContext, string filename, IEnumerable<TagRegistration> tagRegistrations, AssemblyLoader assemblies, string assemblyDirectory, string rootPath)
		{
			string designer;
			string designerFilename = filename + ".designer.cs";

			// Load the markup from the .aspx, .ascx or .master file.
			MarkupReader markup = new MarkupReader();
			MarkupInfo markupInfo;
			try
			{
				markupInfo = markup.LoadMarkup(compileContext, filename, tagRegistrations, assemblies, assemblyDirectory, rootPath);
			}
			catch (Exception e)
			{
				compileContext.Error("{0}: Failed to load markup file:\r\n{1}", filename, e.Message);
				compileContext.Verbose("Stopping file processing due to exception.  Stack trace:\r\n{0}", e.StackTrace);
				return false;
			}

			// If we're not inheriting a real class, there's no reason for a designer file to exist.
			if (markupInfo.ClassType == null)
			{
				compileContext.Verbose("Skipping generating designer file because markup does not have an Inherits=\"...\" attribute.", filename);
				return true;
			}

			// Generate the output text for the new .designer.cs file.
			try
			{
				DesignerWriter designerWriter = new DesignerWriter();
				designer = designerWriter.CreateDesigner(compileContext, markupInfo);
			}
			catch (Exception e)
			{
				compileContext.Error("{0}: Cannot regenerate designer file:\r\n{1}", filename, e.Message);
				compileContext.Verbose("Stopping file processing due to exception.  Stack trace:\r\n{0}", e.StackTrace);
				return false;
			}

			// Save the output .designer.cs file to disk.
			try
			{
				File.WriteAllText(designerFilename, designer, Encoding.UTF8);
			}
			catch (Exception e)
			{
				compileContext.Error("{0}: Cannot open designer file for writing:\r\n{1}", designerFilename, e.Message);
				compileContext.Verbose("Stopping file processing due to exception.  Stack trace:\r\n{0}", e.StackTrace);
				return false;
			}

			return true;
		}

		/// <summary>
		/// For the given set of .aspx or .ascx files, analyze all of their designer files to determine whether
		/// they are valid.  (Valid means that they have all of the required property declarations, in the right
		/// order, with the whitespace and surrounding declarations such that Visual Studio would be able to read them.)
		/// </summary>
		/// <param name="compileContext">The context in which errors are to be reported.</param>
		/// <param name="filenames">The filenames to generate.</param>
		/// <param name="rootPath">The root disk path of the website (usually the same as the path to "web.config").</param>
		/// <param name="websiteDllFileName">The disk path to the website's DLL.</param>
		/// <returns>True if all designer files pass inspection; false if any of them fail.</returns>
		public static bool VerifyDesignerFiles(ICompileContext compileContext, IEnumerable<string> filenames, string rootPath, string websiteDllFileName)
		{
			int filenameCount = filenames.Count();
			compileContext.BeginTask(filenameCount);

			// Load and parse the "web.config".
			WebConfigReader webConfigReader = new WebConfigReader();
			try
			{
				webConfigReader.LoadWebConfig(compileContext, Path.Combine(rootPath, WebConfigFilename), rootPath);
			}
			catch (Exception e)
			{
				compileContext.Error("Cannot load {0}:\r\n{1}", Path.Combine(rootPath, WebConfigFilename), e.Message);
				return false;
			}

			// Load any assemblies we know we'll need.  This includes the default assemblies, any declared
			// in the web.config, and, of course, the website's DLL itself.
			AssemblyLoader assemblyLoader = new AssemblyLoader();
			List<string> assemblyNames = new List<string>();
			assemblyNames.AddRange(_standardTagRegistrations.Where(r => !string.IsNullOrEmpty(r.AssemblyFilename)).Select(r => r.AssemblyFilename).Distinct());
			assemblyNames.AddRange(webConfigReader.TagRegistrations.Where(r => !string.IsNullOrEmpty(r.AssemblyFilename)).Select(r => r.AssemblyFilename).Distinct());
			string dllFullPath = Path.GetFullPath(websiteDllFileName);
			string dllsPath = Path.GetDirectoryName(dllFullPath);
			assemblyNames.Add(dllFullPath);
			string assemblyDirectory = Path.GetDirectoryName(dllFullPath);
			assemblyLoader.PreloadAssemblies(compileContext, assemblyNames, assemblyDirectory);
			assemblyLoader.PrimaryAssembly = assemblyLoader[dllFullPath];

			// Add the default tag registrations, including those from System.Web and any declared in the "web.config".
			List<TagRegistration> tagRegistrations = new List<TagRegistration>();
			tagRegistrations.AddRange(_standardTagRegistrations);
			if (File.Exists(Path.Combine(dllsPath, "System.Web.Extensions.dll")))
				tagRegistrations.AddRange(_extensionTagRegistrations);
			tagRegistrations.AddRange(webConfigReader.TagRegistrations);

			// Spin through any user controls that were declared in the web.config and connect them to their actual
			// .NET class types via reflection.
			compileContext.Verbose("Resolving user controls declared in the web.config.");
			compileContext.VerboseNesting++;
			ResolveUserControls(compileContext, tagRegistrations, assemblyLoader, assemblyDirectory, rootPath, rootPath);
			compileContext.VerboseNesting--;
			compileContext.Verbose(string.Empty);

			bool result = true;

			// Now that all the setup is done, load and parse each individual markup file into its own .designer.cs output file.
			foreach (string filename in filenames)
			{
				compileContext.Verbose("Begin processing \"{0}\"...", filename);
				compileContext.Verbose("");
				compileContext.VerboseNesting++;

				compileContext.BeginFile(filename);
				bool succeeded = VerifyDesignerForFilename(compileContext, filename, tagRegistrations, assemblyLoader, assemblyDirectory, rootPath);
				result &= succeeded;
				compileContext.EndFile(filename, succeeded);

				compileContext.VerboseNesting--;
				compileContext.Verbose("");
				compileContext.Verbose("End processing \"{0}\".", filename);
			}

			return result;
		}

		/// <summary>
		/// Verify the current .designer.cs file for the given markup file.
		/// </summary>
		/// <returns>True if the file passes inspection, false if it fails.</returns>
		public static bool VerifyDesignerForFilename(ICompileContext compileContext, string filename, IEnumerable<TagRegistration> tagRegistrations, AssemblyLoader assemblies, string assemblyDirectory, string rootPath)
		{
			DesignerInfo designerInfo;
			string designerFilename = filename + ".designer.cs";

			// Load the markup from the .aspx or .ascx file.
			MarkupReader markup = new MarkupReader();
			MarkupInfo markupInfo;
			try
			{
				markupInfo = markup.LoadMarkup(compileContext, filename, tagRegistrations, assemblies, assemblyDirectory, rootPath);
			}
			catch (Exception e)
			{
				compileContext.Error("{0}: Failed to load markup file:\r\n{1}", filename, e.Message);
				compileContext.Verbose("Stopping file processing due to exception.  Stack trace:\r\n{0}", e.StackTrace);
				return false;
			}

			if (markupInfo.ClassType == null)
			{
				compileContext.Verbose("Skipping verification of .designer file, because markup has no Inherits=\"...\" attribute and therefore has no .designer file.", filename);
				return true;
			}

			compileContext.Verbose(string.Empty);

			// Read and parse the current .designer.cs file.
			try
			{
				DesignerReader designerReader = new DesignerReader();
				designerInfo = designerReader.LoadDesignerFile(compileContext, designerFilename);
			}
			catch (Exception e)
			{
				compileContext.Error("{0}: Cannot load designer file:\r\n{1}", filename, e.Message);
				compileContext.Verbose("Stopping file processing due to exception.  Stack trace:\r\n{0}", e.StackTrace);
				return false;
			}

			compileContext.Verbose(string.Empty);

			// And finally compare the expectations of the markup against the reality of the .designer.cs file.
			return CompareMarkupInfoToDesignerInfo(compileContext, filename, markupInfo, designerInfo);
		}

		/// <summary>
		/// Compare a parsed markup file against a parsed designer file to determine if they match each other.
		/// </summary>
		/// <param name="compileContext">The context in which errors should be reported.</param>
		/// <param name="filename">The filename to use for reporting errors.</param>
		/// <param name="markupInfo">The markup file to compare.</param>
		/// <param name="designerInfo">The designer file to compare.</param>
		/// <returns>True if they match, false if they do not.</returns>
		private static bool CompareMarkupInfoToDesignerInfo(ICompileContext compileContext, string filename, MarkupInfo markupInfo, DesignerInfo designerInfo)
		{
			compileContext.Verbose("Comparing markup controls to .designer file properties...");

			compileContext.Verbose("Comparing classnames.");

			// First, make sure the type names match; we *should* be talking about the same classes here.
			if (markupInfo.ClassType == null)
			{
				compileContext.Error("{0}: Designer file exists, but markup file has no Inherits=\"...\" attribute.", filename);
				return false;
			}
			if (markupInfo.ClassType.FullName != designerInfo.FullTypeName)
			{
				compileContext.Error("{0}: Designer file and markup file specify different type names (\"{1}\" in the markup, and \"{2}\" in the designer file.",
					filename, markupInfo.ClassType.FullName, designerInfo.FullTypeName);
				return false;
			}

			// Build lookup tables for the property declarations in the designer file and in the markup file.
			// We'll use these to make searching for property matches that much faster, and to detect duplicates,
			// and to ensure that we're talking about the same set of properties in both files.

			compileContext.Verbose("Checking for duplicate control declarations.");

			Dictionary<string, OutputControl> markupPropertiesByName = new Dictionary<string, OutputControl>();
			Dictionary<string, DesignerPropertyDeclaration> designerProperiesByName = new Dictionary<string, DesignerPropertyDeclaration>();

			List<string> duplicateMarkupProperties = new List<string>();
			foreach (OutputControl outputControl in markupInfo.OutputControls)
			{
				if (string.IsNullOrEmpty(outputControl.Name)) continue;

				if (markupPropertiesByName.ContainsKey(outputControl.Name))
				{
					duplicateMarkupProperties.Add(outputControl.Name);
				}
				else
				{
					markupPropertiesByName.Add(outputControl.Name, outputControl);
				}
			}

			List<string> duplicateDesignerProperties = new List<string>();
			foreach (DesignerPropertyDeclaration propertyDeclaration in designerInfo.PropertyDeclarations)
			{
				if (designerProperiesByName.ContainsKey(propertyDeclaration.Name))
				{
					duplicateDesignerProperties.Add(propertyDeclaration.Name);
				}
				else
				{
					designerProperiesByName.Add(propertyDeclaration.Name, propertyDeclaration);
				}
			}

			// Check the lookup tables for duplicates.  There shouldn't be any.

			if (duplicateMarkupProperties.Count > 0)
			{
				compileContext.Error("{0}: Malformed markup error: Found multiple controls in the markup that have the same ID.  Stopping verification now due to invalid markup file.  Duplicate IDs: {1}",
					filename, Join(duplicateMarkupProperties, ", "));
			}
			if (duplicateDesignerProperties.Count > 0)
			{
				compileContext.Error("{0}: Malformed designer error: Found multiple property declarations in the .designer file that have the same name.  Stopping verification now due to invalid designer file.  Duplicate names: {1}",
					filename, Join(duplicateDesignerProperties, ", "));
			}
			if (duplicateMarkupProperties.Count > 0 || duplicateDesignerProperties.Count > 0)
				return false;

			// Okay, now check to see if the markup or designer declare property names that the other doesn't have.

			compileContext.Verbose("Checking for missing control declarations.");

			Type contentControl = typeof(System.Web.UI.WebControls.Content);
			List<string> missingDesignerProperties = markupInfo.OutputControls
				.Where(p => !string.IsNullOrEmpty(p.Name) && p.ReflectedControl.ControlType != contentControl && !designerProperiesByName.ContainsKey(p.Name))
				.Select(p => p.Name)
				.ToList();
			List<string> missingMarkupProperties = designerInfo.PropertyDeclarations
				.Where(p => !string.IsNullOrEmpty(p.Name) && !markupPropertiesByName.ContainsKey(p.Name))
				.Select(p => p.Name)
				.ToList();

			if (missingDesignerProperties.Count > 0)
			{
				compileContext.Error("{0}: Missing property error: Found controls declared in the markup that do not exist in the .designer file.  Missing IDs: {1}",
					filename, Join(missingDesignerProperties, ", "));
			}
			if (missingMarkupProperties.Count > 0)
			{
				compileContext.Error("{0}: Missing control error: Found property declarations in the .designer file that have no control declaration in the markup.  Missing controls: {1}",
					filename, Join(missingMarkupProperties, ", "));
			}

			// We've now established that both files refer to the same set of names.  We now need to check
			// to make sure they all refer to the same control types.

			int numTypeMismatches = 0;

			compileContext.Verbose("Checking for type mismatches.");

			foreach (OutputControl outputControl in markupInfo.OutputControls)
			{
				if (string.IsNullOrEmpty(outputControl.Name)
					|| outputControl.ReflectedControl.ControlType == contentControl
					|| !designerProperiesByName.ContainsKey(outputControl.Name)) continue;

				DesignerPropertyDeclaration designerPropertyDeclaration = designerProperiesByName[outputControl.Name];
				if (designerPropertyDeclaration.PropertyTypeName != outputControl.ReflectedControl.ControlType.FullName)
				{
					compileContext.Error("{0}: Type mismatch: Control \"{1}\" has type {2} in the markup but type {3} in the .designer file.",
						filename, outputControl.Name, outputControl.ReflectedControl.ControlType.FullName, designerPropertyDeclaration.PropertyTypeName);
					numTypeMismatches++;
				}
			}

			if (missingDesignerProperties.Count > 0 || missingMarkupProperties.Count > 0 || numTypeMismatches > 0)
				return false;

			// One last very touchy check:  All the properties exist in both files, and they have the same names and
			// same types --- but are they in the right order?  Visual Studio is very picky about the order, and if
			// they don't match, the Visual Studio designer will break.

			compileContext.Verbose("Checking for mis-ordered declarations.");

			for (int m = 0, d = 0; m < markupInfo.OutputControls.Count; )
			{
				OutputControl outputControl = markupInfo.OutputControls[m++];
				if (string.IsNullOrEmpty(outputControl.Name)
					|| outputControl.ReflectedControl.ControlType == contentControl) continue;

				DesignerPropertyDeclaration designerPropertyDeclaration = designerInfo.PropertyDeclarations[d++];

				if (designerPropertyDeclaration.Name != outputControl.Name
					|| designerPropertyDeclaration.PropertyTypeName != outputControl.ReflectedControl.ControlType.FullName)
				{
					compileContext.Error("{0}: Ordering error: All of the same controls exist in both the markup and the .designer file, but they do not appear in the same order.", filename);
					return false;
				}
			}

			compileContext.Verbose("{0}: Success!", filename);
			return true;
		}

		/// <summary>
		/// Given a set of tag registrations for user controls, attempt to connect those tag registrations to actual
		/// .NET class types in the main website assembly.  This will update the Typename field for each TagRegistration
		/// where a matching class type is found; or if no matching class type is found, this will throw an exception.
		/// </summary>
		/// <param name="compileContext">The context in which errors should be reported.</param>
		/// <param name="tagRegistrations">The set of user-control registrations to resolve to real class types.</param>
		/// <param name="assemblies">The full set of preloaded assemblies.</param>
		/// <param name="assemblyDirectory">The directory where the main website DLL can be found.</param>
		/// <param name="rootPath">The real disk path to the root of the website's virtual directory.</param>
		/// <param name="currentDirectory">The current directory (for resolving relative paths).</param>
		public static void ResolveUserControls(ICompileContext compileContext, IEnumerable<TagRegistration> tagRegistrations, AssemblyLoader assemblies, string assemblyDirectory, string rootPath, string currentDirectory)
		{
			foreach (TagRegistration tagRegistration in tagRegistrations.Where(t => t.Kind == TagRegistrationKind.SingleUserControl))
			{
				compileContext.Verbose("Registering user control <{0}:{1}> as \"{2}\".", tagRegistration.TagPrefix, tagRegistration.TagName, tagRegistration.SourceFilename);

				compileContext.VerboseNesting++;

				string filename = ResolveWebsitePath(compileContext, tagRegistration.SourceFilename, rootPath, currentDirectory);

				MarkupReader userControlReader = new MarkupReader();
				Tag userControlMainDirective = userControlReader.ReadMainDirective(compileContext, filename, assemblies, assemblyDirectory, rootPath);

				if (string.IsNullOrEmpty(userControlMainDirective.TagName)
					&& string.Compare(userControlMainDirective.TagName, "control", StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					throw new RedesignerException("Cannot register user control \"{0}\":  Its main <% ... %> directive does not start with the \"Control\" keyword.  Is this actually a user control?", tagRegistration.SourceFilename);
				}

				string inheritsAttribute = userControlMainDirective["inherits"];
				if (string.IsNullOrEmpty(inheritsAttribute))
				{
					throw new RedesignerException("Cannot register user control \"{0}\":  Its main <% Control ... %> directive is missing the required Inherits=\"...\" attribute.", tagRegistration.SourceFilename);
				}

				tagRegistration.Typename = inheritsAttribute;

				compileContext.Verbose("User control registered as type \"{0}\".", inheritsAttribute);
				compileContext.VerboseNesting--;
			}
		}

		/// <summary>
		/// Given a set of filenames describing either .aspx files, .ascx files, or directories containing
		/// .aspx files and .ascx files, resolve those into a complete list of just .aspx and .ascx files,
		/// recursing as necessary.
		/// </summary>
		/// <param name="filenames">The original filename list.</param>
		/// <returns>The complete resolved list of filenames.</returns>
		public static IEnumerable<string> ResolveFilenames(IEnumerable<string> filenames)
		{
			List<string> result = new List<string>();

			foreach (string filename in filenames)
			{
				if (Directory.Exists(filename))
				{
					IEnumerable<string> files = ResolveDirectoryReference(filename);
					result.AddRange(files);
				}
				else
				{
					result.Add(filename);
				}
			}

			return result;
		}

		/// <summary>
		/// Given a filename known to be a directory, search through it to collect all of its constituent
		/// .aspx and .ascx files.
		/// </summary>
		/// <param name="directoryName">The directory name to search through.</param>
		/// <returns>The list of matching .aspx and .ascx files, resolved relative to the directory path itself.</returns>
		private static IEnumerable<string> ResolveDirectoryReference(string directoryName)
		{
			string[] foundFiles = Directory.GetFiles(directoryName, "*.*", SearchOption.AllDirectories);

			List<string> matchingFiles = foundFiles
				.Where(name => name.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase)
				               || name.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase))
				.ToList();

			return matchingFiles;
		}

		/// <summary>
		/// Given a virtual filename (possibly either rooted or relative), and a root path to the website,
		/// and the current disk directory, return a complete disk path for that virtual filename.
		/// </summary>
		/// <param name="compileContext">The context in which this is being resolved (for error-reporting).</param>
		/// <param name="virtualFilename">The virtual filename to resolve to a disk path to a real file.</param>
		/// <param name="rootPath">The root disk path to the website.</param>
		/// <param name="currentDirectory">The current directory (for evaluating relative paths).</param>
		/// <returns>The fully-resolved disk path to the given file.</returns>
		public static string ResolveWebsitePath(ICompileContext compileContext, string virtualFilename, string rootPath, string currentDirectory)
		{
			string filename;

			if (virtualFilename.StartsWith("~/") || virtualFilename.StartsWith(@"~\"))
			{
				// Rooted virtual path.
				filename = virtualFilename.Substring(2).Replace('/', '\\');
				return Path.Combine(rootPath, filename);
			}

			if (virtualFilename.StartsWith("/") || virtualFilename.StartsWith(@"\"))
				throw new RedesignerException("Illegal virtual path \"{0}\".  Virtual paths should be relative to the current path, or should start with \"~/\".", virtualFilename);

			// Relative virtual path.
			filename = virtualFilename.Replace('/', '\\');
			return Path.Combine(currentDirectory, filename);
		}

		/// <summary>
		/// Given a string, return another string where the original is repeated 'count' times.
		/// </summary>
		public static string RepeatString(string str, int count)
		{
			switch (count)
			{
				case 0: return string.Empty;
				case 1: return str;
				case 2: return str + str;
				case 3: return str + str + str;

				default:
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < count; i++)
					{
						stringBuilder.Append(str);
					}
					return stringBuilder.ToString();
			}
		}

		/// <summary>
		/// Given a series of strings, combine them into a single string, placing the given separator string between each.
		/// </summary>
		/// <param name="items">The items to join.</param>
		/// <param name="separator">The separator "glue" string to place between them.</param>
		/// <returns>The single joined string.</returns>
		public static string Join(IEnumerable<string> items, string separator)
		{
			switch (items.Count())
			{
				case 0:
					return string.Empty;

				case 1:
					return items.First();

				case 2:
					return items.First() + separator + items.Skip(1).Take(1);

				default:
					StringBuilder stringBuilder = new StringBuilder();
					bool isFirst = true;
					foreach (string item in items)
					{
						if (!isFirst)
						{
							stringBuilder.Append(separator);
						}
						stringBuilder.Append(item);
						isFirst = false;
					}
					return stringBuilder.ToString();
			}
		}
	}
}
