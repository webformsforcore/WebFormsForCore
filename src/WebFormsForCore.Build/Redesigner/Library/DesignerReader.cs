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

using System.IO;
using System.Text.RegularExpressions;

namespace Redesigner.Library
{
	/// <summary>
	/// This class reads .designer files into memory and parses their declarations into
	/// a DesignerInfo object that can be readily compared against.
	/// </summary>
	public class DesignerReader
	{
		/// <summary>
		/// How to split lines.  Visual Studio isn't quite as smart as this, but it should be suitable anyway.
		/// </summary>
		private static readonly Regex _lineSplitter = new Regex(@"\r\n|\n\r|\r|\n", RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// If a given line is blank or contains only a comment, it will match this regex.
		/// </summary>
		private static readonly Regex _isBlankOrComment = new Regex(@"^[\x00-\x20]*(?:\/\/|$)", RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// This line begins a namespace (whose name is matched as the capture "namespace").
		/// </summary>
		private static readonly Regex _isNamespaceDeclaration = new Regex(@"^[\x00-\x20]*namespace[\x00-\x20]+(?<namespace>[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)*)[\x00-\x20]*(?<curlybrace>\{?)[\x00-\x20]*$", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		/// <summary>
		/// This line begins a public partial class (whose name is matched as the capture "classname").
		/// </summary>
		private static readonly Regex _isClassDeclaration = new Regex(@"^[\x00-\x20]*public[\x00-\x20]+partial[\x00-\x20]+class[\x00-\x20]+(?<classname>[A-Za-z_][A-Za-z0-9_]*)[\x00-\x20]*(?<curlybrace>\{?)[\x00-\x20]*$", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		/// <summary>
		/// This line contains a protected property declaration (whose type is matched as the capture "typename"
		/// and whose property name is matched as the capture "propertyname").
		/// </summary>
		private static readonly Regex _isPropertyDeclaration = new Regex(@"^[\x00-\x20]*protected[\x00-\x20]+global::(?<typename>[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)*)[\x00-\x20]+(?<propertyname>[A-Za-z_][A-Za-z0-9_]*)[\x00-\x20]*;[\x00-\x20]*$", RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// This line contains only a single closed curly brace.
		/// </summary>
		private static readonly Regex _isClosedCurlyBrace = new Regex(@"^[\x00-\x20]*\}[\x00-\x20]*$", RegexOptions.Singleline | RegexOptions.Compiled);

		/// <summary>
		/// The context in which to report errors during the parse.
		/// </summary>
		private ICompileContext _compileContext;

		/// <summary>
		/// The current line number being parsed.
		/// </summary>
		private int _line;

		private enum ParsingState
		{
			BeforeNamespace,
			BeforeClass,
			InsideClass,
			AfterClass,
			AfterNamespace,
		}

		/// <summary>
		/// Load and parse the given .designer file.
		/// </summary>
		/// <param name="compileContext">The context in which to report errors during the parse.</param>
		/// <param name="filename">The name of the .designer file to load and parse.</param>
		/// <returns>The parsed version of the .designer file.</returns>
		public DesignerInfo LoadDesignerFile(ICompileContext compileContext, string filename)
		{
			_compileContext = compileContext;
			_line = 0;

			Verbose("Loading .designer file from disk.");

			string designerText = File.ReadAllText(filename);

			return ParseDesignerText(compileContext, designerText);
		}

		/// <summary>
		/// Parse the text of the given designer file into a series of property declarations and return them.
		/// This uses simple line-by-line parsing, not a proper code parse, because Visual Studio does the same
		/// thing, and a .designer file is only considered valid if Visual Studio can read it.
		/// </summary>
		/// <param name="compileContext">The context in which to report errors during the parse.</param>
		/// <param name="designerText">The text of the designer file to parse.</param>
		/// <returns>The results of reading and parsing the designer file.</returns>
		public DesignerInfo ParseDesignerText(ICompileContext compileContext, string designerText)
		{
			_compileContext = compileContext;
			_line = 0;

			Verbose("Beginning parse of .designer file.");

			string[] lines = _lineSplitter.Split(designerText);

			Verbose("{0} lines of text found in .designer file.", lines.Length);

			DesignerInfo designerInfo = new DesignerInfo();

			ParsingState state = ParsingState.BeforeNamespace;

			for (_line = 1; _line <= lines.Length; _line++)
			{
				string currentLine = lines[_line - 1];

				// Skip blank lines and comment lines.
				if (_isBlankOrComment.IsMatch(currentLine)) continue;

				// We use a simple finite-state machine to perform the parsing.  It transitions on declarations and curly braces.
				switch (state)
				{
					case ParsingState.BeforeNamespace:
						{
							Match match;
							if ((match = _isNamespaceDeclaration.Match(currentLine)).Success)
							{
								if (string.IsNullOrEmpty(match.Groups["curlybrace"].Value))
								{
									Error("There is a valid namespace declaration here, but the opening curly brace is not on the same line.  Visual Studio is strict about curly-brace placement, and probably cannot read this .designer file.");
								}
								else
								{
									designerInfo.Namespace = match.Groups["namespace"].Value;
									Verbose("Parsed a valid namespace declaration for namespace \"{0}\".", designerInfo.Namespace);
									state = ParsingState.BeforeClass;
								}
							}
							else
							{
								Error("There should be a valid namespace declaration here.  Visual Studio probably cannot read this .designer file.");
							}
						}
						break;

					case ParsingState.BeforeClass:
						{
							Match match;
							if ((match = _isClassDeclaration.Match(currentLine)).Success)
							{
								if (string.IsNullOrEmpty(match.Groups["curlybrace"].Value))
								{
									Error("There is a valid class declaration here, but the opening curly brace is not on the same line.  Visual Studio is strict about curly-brace placement, and probably cannot read this .designer file.");
								}
								else
								{
									designerInfo.ClassName = match.Groups["classname"].Value;
									Verbose("Parsed a valid class declaration for class \"{0}\".", designerInfo.ClassName);
									state = ParsingState.InsideClass;
								}
							}
							else
							{
								Error("There should be a valid partial class declaration here.  Visual Studio probably cannot read this .designer file.");
							}
						}
						break;

					case ParsingState.InsideClass:
						{
							Match match;
							if (_isClosedCurlyBrace.IsMatch(currentLine))
							{
								state = ParsingState.AfterClass;
								Verbose("End of class.");
							}
							else if ((match = _isPropertyDeclaration.Match(currentLine)).Success)
							{
								DesignerPropertyDeclaration propertyDeclaration = new DesignerPropertyDeclaration
								{
									PropertyTypeName = match.Groups["typename"].Value,
									Name = match.Groups["propertyname"].Value,
								};
								designerInfo.PropertyDeclarations.Add(propertyDeclaration);

								Verbose("Found property declaration: {0} {1}", propertyDeclaration.PropertyTypeName, propertyDeclaration.Name);
							}
							else
							{
								Warning("There should be a protected property declaration here.  Visual Studio may not be able to read this .designer file.");
							}
						}
						break;

					case ParsingState.AfterClass:
						if (_isClosedCurlyBrace.IsMatch(currentLine))
						{
							state = ParsingState.AfterNamespace;
							Verbose("End of namespace.");
						}
						else
						{
							Error("There should be a closing curly brace here.  Visual Studio probably cannot read this .designer file.");
						}
						break;

					case ParsingState.AfterNamespace:
						Error("There should be no more content after the end of the namespace.  Visual Studio probably cannot read this .designer file.");
						break;
				}
			}

			Verbose("Ended parse of .designer file.");

			return designerInfo;
		}

		#region Error-handling

		/// <summary>
		/// Write a verbose informational message.
		/// </summary>
		private void Verbose(string format, params object[] args)
		{
			if (_line > 0)
			{
				_compileContext.Verbose("Line {0}: {1}", _line, string.Format(format, args));
			}
			else
			{
				_compileContext.Verbose(format, args);
			}
		}

		/// <summary>
		/// Warn the user something is wrong.
		/// </summary>
		private void Warning(string format, params object[] args)
		{
			if (_line > 0)
			{
				_compileContext.Warning("Line {0}: {1}", _line, string.Format(format, args));
			}
			else
			{
				_compileContext.Warning(format, args);
			}
		}

		/// <summary>
		/// Abort because something failed hard.
		/// </summary>
		private void Error(string format, params object[] args)
		{
			throw new RedesignerException(
				_line > 0
				? string.Format("Line {0}: {1}", _line, string.Format(format, args))
				: string.Format(format, args)
			);
		}

		#endregion
	}
}
