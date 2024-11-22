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
using System.Xml.Linq;

namespace Redesigner.Library
{
	/// <summary>
	/// This class knows how to read and parse a "web.config" file to extract any user-control or
	/// namespace declarations contained within it.
	/// </summary>
	public class WebConfigReader
	{
		#region Properties

		/// <summary>
		/// The raw XDocument of the loaded web.config file.
		/// </summary>
		public XDocument WebConfig { get; private set; }

		/// <summary>
		/// The tag registrations extracted from the web.config file.
		/// </summary>
		public IEnumerable<TagRegistration> TagRegistrations { get; private set; }

		public bool Net45OrAbove { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Load the web.config file found at the given filename.
		/// </summary>
		public void LoadWebConfig(ICompileContext compileContext, string filename, string rootPath, out bool net45OrAbove)
		{
			if (!File.Exists(filename))
			{
				TagRegistrations = new List<TagRegistration>();
				WebConfig = new();
				net45OrAbove = true;
				return;
			}

			compileContext.Verbose("Loading and processing \"{0}\"...", filename);
			compileContext.VerboseNesting++;

			try
			{
				// Load and decode the XML itself.
				XDocument webConfig = XDocument.Load(filename);

				compileContext.Verbose("\"{0}\" loaded as an XDocument.", filename);

				var targetFrameworkString = (string)webConfig.Element("configuration")
					?.Element("system.web")
					?.Element("compilation")
					?.Attribute("targetFramework") ?? "8.0";
				Version targetFramework;
				if (!Version.TryParse(targetFrameworkString, out targetFramework)) targetFramework = new Version(8, 0);
				net45OrAbove = targetFramework >= new Version(4, 5);

				// Find the <add> declarations in the <configuration><system.web><controls> section.
				IEnumerable<XElement> adds = ExtractAddSectionFromWebConfig(webConfig, filename);

				compileContext.Verbose("Found {0} <add> declarations.", adds.Count());

				// Parse the <add> declarations into a TagRegistrations list.
				List<TagRegistration> tagRegistrations = new List<TagRegistration>();
				foreach (XElement add in adds)
				{
					// Extract all the (important) attributes for this <add> declaration.
					XAttribute tagPrefixAttribute = add.Attribute("tagPrefix");
					XAttribute tagNameAttribute = add.Attribute("tagName");
					XAttribute srcAttribute = add.Attribute("src");
					XAttribute namespaceAttribute = add.Attribute("namespace");
					XAttribute assemblyAttribute = add.Attribute("assembly");

					// Make sure this declaration makes sense; if not, just skip it and hope for the best.
					bool isValid = ValidateAddElement(compileContext, filename, tagPrefixAttribute, namespaceAttribute, tagNameAttribute, srcAttribute, assemblyAttribute);
					if (!isValid) continue;

					// It's valid, so decode it.
					TagRegistration tagRegistration;
					if (namespaceAttribute != null)
					{
						tagRegistration = new TagRegistration
						{
							Kind = TagRegistrationKind.Namespace,
							TagPrefix = (tagPrefixAttribute != null ? tagPrefixAttribute.Value : null),
							AssemblyFilename = (assemblyAttribute != null ? assemblyAttribute.Value : null),
							Namespace = namespaceAttribute.Value,
						};
						compileContext.Verbose("Registering namespace: <{0}:*> now includes \"{1}\"{2}",
							tagRegistration.TagPrefix, tagRegistration.Namespace,
							string.IsNullOrEmpty(tagRegistration.AssemblyFilename) ? string.Empty : " in assembly \"" + tagRegistration.AssemblyFilename + "\"");
					}
					else
					{
						tagRegistration = new TagRegistration
						{
							Kind = TagRegistrationKind.SingleUserControl,
							TagPrefix = (tagPrefixAttribute != null ? tagPrefixAttribute.Value : null),
							TagName = (tagNameAttribute != null ? tagNameAttribute.Value : null),
							SourceFilename = (srcAttribute != null ? srcAttribute.Value : null),
						};
						compileContext.Verbose("Registering user control: <{0}:{1}> is \"{2}\"",
							tagRegistration.TagPrefix, tagRegistration.TagName, tagRegistration.SourceFilename);
					}

					// And add it to the list of known tag registrations.
					tagRegistrations.Add(tagRegistration);
				}

				// If everything was successful, share the results with the world.
				WebConfig = webConfig;
				TagRegistrations = tagRegistrations;
			}
			finally
			{
				compileContext.VerboseNesting--;
				compileContext.Verbose("Finished processing \"{0}\".", filename);
				compileContext.Verbose("");
			}
		}

		/// <summary>
		/// Extract and validate the &lt;add&gt; section from the loaded XDocument.
		/// </summary>
		/// <returns>All of the &lt;add&gt; declarations found inside the configuration/system.web/pages/controls section of the given XDocument.</returns>
		private IEnumerable<XElement> ExtractAddSectionFromWebConfig(XDocument webConfig, string filename)
		{
			if (webConfig == null)
				throw new RedesignerException("XDocument.Load for \"{0}\" returned null.", filename);

			XElement configuration = webConfig.Element("configuration");
			if (configuration == null)
				return Enumerable.Empty<XElement>();
				// throw new RedesignerException("\"{0}\" is missing its required <configuration> section.", filename);

			XElement systemWeb = configuration.Element("system.web");
			if (systemWeb == null)
				return Enumerable.Empty<XElement>();
				// throw new RedesignerException("\"{0}\" is missing its required <configuration><system.web> section.", filename);

			XElement pages = systemWeb.Element("pages");
			if (pages == null)
				return Enumerable.Empty<XElement>();
				// throw new RedesignerException("\"{0}\" is missing its required <configuration><system.web><pages><controls> section.", filename);

			XElement controls = pages.Element("controls");
			if (controls == null)
				return Enumerable.Empty<XElement>();
				// throw new RedesignerException("\"{0}\" is missing its required <configuration><system.web><pages><controls> section.", filename);

			IEnumerable<XElement> adds = controls.Elements();
			if (adds == null)
				return Enumerable.Empty<XElement>();
				//throw new RedesignerException("\"{0}\" contains no <add> control declarations in its <configuration><system.web><pages><controls> section.", filename);

			return adds;
		}

		/// <summary>
		/// Ensure that the given &lt;add&gt; element is well-formed.
		/// </summary>
		/// <returns>True if it is well-formed, false if it is broken in some way.</returns>
		private bool ValidateAddElement(ICompileContext compileContext, string filename, XAttribute tagPrefixAttribute, XAttribute namespaceAttribute, XAttribute tagNameAttribute, XAttribute srcAttribute, XAttribute assemblyAttribute)
		{
			// Must have a tag prefix.
			if (tagPrefixAttribute == null || string.IsNullOrEmpty(tagPrefixAttribute.Value))
			{
				compileContext.Warning("Found a bad <add> declaration (without the required tagPrefix attribute) in \"{0}\".  Skipping it.", filename);
				return false;
			}

			// Must have either tagName+src or namespace, but you may not have both.
			if ((namespaceAttribute != null && (tagNameAttribute != null || srcAttribute != null)))
			{
				compileContext.Warning("Found a bad <add> declaration (with both a namespace and a src or tagName attribute) in \"{0}\".  Skipping it.", filename);
				return false;
			}
			if (namespaceAttribute == null && tagNameAttribute == null && srcAttribute == null)
			{
				compileContext.Warning("Found a bad <add> declaration (with neither a namespace nor a src nor a tagName attribute) in \"{0}\".  Skipping it.", filename);
				return false;
			}
			if (namespaceAttribute == null && (tagNameAttribute != null && srcAttribute == null))
			{
				compileContext.Warning("Found a bad <add> declaration (with a tagName attribute but no src attribute) in \"{0}\".  Skipping it.", filename);
				return false;
			}
			if (namespaceAttribute == null && tagNameAttribute == null)
			{
				compileContext.Warning("Found a bad <add> declaration (with a src attribute but no tagName attribute) in \"{0}\".  Skipping it.", filename);
				return false;
			}

			// The assembly attribute is only legit if you have a namespace.
			if (srcAttribute != null && assemblyAttribute != null)
			{
				compileContext.Warning("Found a bad <add> declaration (has tagName/src attributes with an assembly attribute) in \"{0}\".  Skipping it.", filename);
				return false;
			}

			// Make sure the provided attributes contain content.
			if (namespaceAttribute != null)
			{
				if (string.IsNullOrEmpty(namespaceAttribute.Value))
				{
					compileContext.Warning("Found a bad <add> declaration (has an empty namespace declaration) in \"{0}\".  Skipping it.", filename);
					return false;
				}
				if (assemblyAttribute != null && string.IsNullOrEmpty(assemblyAttribute.Value))
				{
					compileContext.Warning("Found a bad <add> declaration (has an empty attribute declaration) in \"{0}\".  Skipping it.", filename);
					return false;
				}
			}
			if (tagNameAttribute != null)
			{
				if (string.IsNullOrEmpty(tagNameAttribute.Value))
				{
					compileContext.Warning("Found a bad <add> declaration (has an empty tagName declaration) in \"{0}\".  Skipping it.", filename);
					return false;
				}
				if (string.IsNullOrEmpty(srcAttribute.Value))
				{
					compileContext.Warning("Found a bad <add> declaration (has an empty src declaration) in \"{0}\".  Skipping it.", filename);
					return false;
				}
			}

			return true;
		}

		#endregion
	}
}
