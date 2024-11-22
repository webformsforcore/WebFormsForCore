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
using System.Text.RegularExpressions;

namespace Redesigner.Library
{
	/// <summary>
	/// This class knows how to read the markup of .aspx and .ascx files, parse it, and extract
	/// control declarations from it.
	/// </summary>
	public class MarkupReader
	{
		#region Fields, Properties, and Static Data

		#region Current parsing state

		/// <summary>
		/// A stack of markup states, used for processing nested includes.
		/// </summary>
		private readonly Stack<MarkupReaderState> _stateStack = new Stack<MarkupReaderState>();

		/// <summary>
		/// The name of the .aspx or .ascx file currently being processed.
		/// </summary>
		private string _filename;

		/// <summary>
		/// The current line number within the current .aspx or .ascx file.
		/// </summary>
		private int _line;

		/// <summary>
		/// The raw text of the current .aspx or .ascx file.
		/// </summary>
		private string _text;

		/// <summary>
		/// The current read-pointer into the raw text of the .aspx or .ascx file.
		/// </summary>
		private int _src;

		/// <summary>
		/// The position in the file of the last greater-than character, used for optimizing the regex searches.
		/// </summary>
		private int _lastGreaterThanIndex;

		/// <summary>
		/// Whether we are currently parsing inside a &lt;script&gt; tag (and thus ignoring everything that's not
		/// a server-side declaration until the closing &lt;/script&gt; tag).
		/// </summary>
		private bool _isInScriptTag;

		/// <summary>
		/// What line this script tag started on, if we're inside a script tag (used for reporting runaway script
		/// tags on the proper line).
		/// </summary>
		private int _scriptStartLine;

		#endregion

		#region Assemblies and Reflected Control Data

		/// <summary>
		/// The complete set of currently-loaded assemblies.
		/// </summary>
		private AssemblyLoader _assemblies;

		/// <summary>
		/// The directory where additional assemblies can be found (such as the main website DLL).
		/// </summary>
		private string _assemblyDirectory;

		/// <summary>
		/// The root path to the website (where the "web.config" should be found and "~/" should evaluate to).
		/// </summary>
		private string _rootPath;

		/// <summary>
		/// The complete list of registered user controls and server-control namespaces.
		/// </summary>
		private List<TagRegistration> _tagRegistrations;

		/// <summary>
		/// A cache of all of the controls we have reflected against, along with tools to reflect and add
		/// additional controls to it.
		/// </summary>
		private ReflectedControlCollection _reflectedControlCollection;

		/// <summary>
		/// The stack of server-control tags that have been seen so far (represented as a List for easier management).
		/// </summary>
		private List<string> _controlStack;

		#endregion

		#region Output Data

		/// <summary>
		/// The context in which errors get reported.
		/// </summary>
		private ICompileContext _compileContext;

		/// <summary>
		/// Whether any server controls or user controls that we parse are public and should be added to
		/// the .designer file.
		/// </summary>
		private bool _shouldGenerateOutput;

		/// <summary>
		/// The list of server controls or user controls that should be generated in the designer file.
		/// </summary>
		private List<OutputControl> _outputControls;

		/// <summary>
		/// The main &lt;% Page %&gt; or &lt;% Control %&gt; directive from this .aspx or .ascx file.
		/// </summary>
		private Tag _mainDirective;

		/// <summary>
		/// The .NET class type associated with the code-behind of this .aspx or .ascx file.
		/// </summary>
		private Type _mainDirectiveType;

		#endregion

		#region Regular Expressions for Parsing Markup

		// These parsing regexes come directly from ASP.NET itself, obtained by inspecting its BaseParser
		// instance at runtime in a debugger and by inspecting it in Reflector.  They're cloned verbatim
		// so that we get the same parsing behavior ASP.NET uses, right or wrong.
		private static readonly Regex _tagRegex = new Regex(@"\G<(?<tagname>[\w:\.]+)(\s+(?<attrname>\w[-\w:]*)(\s*=\s*""(?<attrval>[^""]*)""|\s*=\s*'(?<attrval>[^']*)'|\s*=\s*(?<attrval><%#.*?%>)|\s*=\s*(?<attrval>[^\s=/>]*)|(?<attrval>\s*?)))*\s*(?<empty>/)?>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _directiveRegex = new Regex(@"\G<%\s*@(\s*(?<attrname>\w[\w:]*(?=\W))(\s*(?<equal>=)\s*""(?<attrval>[^""]*)""|\s*(?<equal>=)\s*'(?<attrval>[^']*)'|\s*(?<equal>=)\s*(?<attrval>[^\s%>]*)|(?<equal>)(?<attrval>\s*?)))*\s*?%>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _endTagRegex = new Regex(@"\G</(?<tagname>[\w:\.]+)\s*>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _codeRegex = new Regex(@"\G<%(?!@)(?<code>.*?)%>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _expressionRegex = new Regex(@"\G<%\s*?=(?<code>.*?)?%>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _databindingRegex = new Regex(@"\G<%#(?<code>.*?)?%>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _commentRegex = new Regex(@"\G<%--(([^-]*)-)*?-%>", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _includeRegex = new Regex(@"\G<!--\s*#(?i:include)\s*(?<pathtype>[\w]+)\s*=\s*[""']?(?<filename>[^\""']*?)[""']?\s*-->", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		private static readonly Regex _textRegex = new Regex(@"\G[^<]+", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		// This regex isn't used by ASP.NET, but is useful for our purposes when validating content.
		private static readonly Regex _whitespaceRegex = new Regex(@"^[\x00-\x20]+$", RegexOptions.Singleline | RegexOptions.Compiled);

		#endregion

		#region Static Data

		/// <summary>
		/// A list of types that includes all types.
		/// </summary>
		private static readonly List<Type> _allTypes = new List<Type> { typeof(object) };

		/// <summary>
		/// A list of types that only includes controls.
		/// </summary>
		private static readonly List<Type> _justControlTypes = new List<Type> { typeof(System.Web.UI.Control) };

		#endregion

		#endregion

		#region Public Methods

		/// <summary>
		/// Load the markup of the given .aspx or .ascx file, parse it, and return a collection of its public
		/// control declarations.
		/// </summary>
		/// <param name="compileContext">The context in which errors get reported.</param>
		/// <param name="filename">The name of the .aspx or .ascx file to load and parse.</param>
		/// <param name="tagRegistrations">The list of registered user controls or namespaces (from the web.config).</param>
		/// <param name="assemblies">The pre-loaded set of registered assemblies.</param>
		/// <param name="assemblyDirectory">The disk directory where additional assemblies (including the website's DLL) may be found.</param>
		/// <param name="rootPath">The disk directory of the root path of the website.</param>
		/// <returns>A collection of public control declarations and the main directive from the top of the file.</returns>
		public MarkupInfo LoadMarkup(ICompileContext compileContext, string filename, IEnumerable<TagRegistration> tagRegistrations, AssemblyLoader assemblies, string assemblyDirectory, string rootPath, bool net45OrAbove)
		{
			_compileContext = compileContext;

			_assemblies = assemblies;
			_assemblyDirectory = assemblyDirectory;
			_isInScriptTag = false;
			_scriptStartLine = 0;
			_rootPath = rootPath;

			_controlStack = new List<string>();
			_outputControls = new List<OutputControl>();
			_shouldGenerateOutput = true;
			_mainDirective = null;

			_tagRegistrations = tagRegistrations.ToList();
			_reflectedControlCollection = new ReflectedControlCollection(_tagRegistrations, assemblies);

			LoadMarkupInternal(filename, net45OrAbove);

			return new MarkupInfo
			{
				OutputControls = _outputControls,
				MainDirective = _mainDirective,
				ClassType = _mainDirectiveType,
				Assemblies = _assemblies,
			};
		}

		/// <summary>
		/// Read and parse the main directive from the top of the given .aspx or .ascx file and return it.
		/// </summary>
		/// <param name="compileContext">The context in which errors get reported.</param>
		/// <param name="filename">The name of the .aspx or .ascx file to load and parse.</param>
		/// <param name="assemblies">The pre-loaded set of registered assemblies.</param>
		/// <param name="assemblyDirectory">The disk directory where additional assemblies (including the website's DLL) may be found.</param>
		/// <param name="rootPath">The disk directory of the root path of the website.</param>
		/// <returns>The main directive from the top of the .aspx or .ascx file.</returns>
		public Tag ReadMainDirective(ICompileContext compileContext, string filename, AssemblyLoader assemblies, string assemblyDirectory, string rootPath)
		{
			_compileContext = compileContext;

			filename = Path.GetFullPath(filename);

			Verbose("Reading markup file into memory.");
			string markupText;
			try
			{
				markupText = File.ReadAllText(filename);
			}
			catch (Exception e)
			{
				Verbose("Unable to read markup file:\r\n{0}", e.Message);
				throw;
			}

			_assemblies = assemblies;
			_assemblyDirectory = assemblyDirectory;
			_isInScriptTag = false;
			_scriptStartLine = 0;
			_rootPath = rootPath;

			_controlStack = new List<string>();
			_outputControls = new List<OutputControl>();
			_shouldGenerateOutput = false;

			PushState(filename, markupText);

			Verbose("Parsing just enough of the file to extract its main <%@ ... %> directive.");

			return ParseMainDirective();
		}

		#endregion

		#region State Management and Helper Methods

		/// <summary>
		/// Load the given markup file and parse it, adding its declarations to the current global state.
		/// This preserves parse state before the parse and restores it afterward, making it suitable for
		/// recursive inclusions.
		/// </summary>
		/// <param name="filename">The disk path to the file to include.</param>
		private void LoadMarkupInternal(string filename, bool net45OrAbove)
		{
			string markupText;

			filename = Path.GetFullPath(filename);

			Verbose("Reading markup file into memory.");
			try
			{
				markupText = File.ReadAllText(filename);
			}
			catch (Exception e)
			{
				Verbose("Unable to read markup file:\r\n{0}", e.Message);
				throw;
			}

			PushState(filename, markupText);

			ParseText(null, null, net45OrAbove);

			PopState();
		}

		/// <summary>
		/// Preserve the current parse state critical to the current file and then switch to
		/// a new file (in preparation for a nested file inclusion).
		/// </summary>
		/// <param name="filename">The name of the new file.</param>
		/// <param name="markupText">The markup text for the new file that is being included.</param>
		private void PushState(string filename, string markupText)
		{
			if (_stateStack.Count > 50)
			{
				Error("Too many nested include directives.  Do you have recursive file inclusion?");
				return;
			}

			_stateStack.Push(new MarkupReaderState(_filename, _line, _text, _src, _lastGreaterThanIndex, _shouldGenerateOutput));

			_filename = filename;
			_line = 1;
			_text = markupText;
			_src = 0;
			_lastGreaterThanIndex = markupText.LastIndexOf('>');
		}

		/// <summary>
		/// Restore the parse state at the end of a file inclusion to its previous file/line/character.
		/// </summary>
		private void PopState()
		{
			MarkupReaderState oldState = _stateStack.Pop();

			_filename = oldState.Filename;
			_line = oldState.Line;
			_text = oldState.Text;
			_src = oldState.Src;
			_lastGreaterThanIndex = oldState.LastGreaterThanIndex;
			_shouldGenerateOutput = oldState.ShouldGenerateOutput;
		}

		/// <summary>
		/// Count newlines in the text from the given start to the given end.  This recognizes all four major forms of
		/// newlines ("\r", "\n", "\r\n", and "\n\r"), which means it does a better job of correctly reporting line numbers
		/// than ASP.NET itself does.
		/// </summary>
		private static int CountNewlines(string text, int start, int end)
		{
			int newlines = 0;

			for (int i = start; i < end; )
			{
				if (text[i] == '\r')
				{
					i++;
					if (i < end && text[i] == '\n')
					{
						i++;
					}
					newlines++;
					continue;
				}
				if (text[i] == '\n')
				{
					i++;
					if (i < end && text[i] == '\r')
					{
						i++;
					}
					newlines++;
					continue;
				}
				i++;
			}

			return newlines;
		}

		#endregion

		#region Core parsing loops

		/// <summary>
		/// The core parsing loop.  This reads and processes normal text, tags, and directives until the
		/// end of the current file or until the given end-tag is found.
		/// </summary>
		/// <param name="untilEndTag">If non-null, if this end-tag is seen, it will cause parsing to immediately
		/// stop.  (But the end-tag itself will be consumed from the input.)</param>
		/// <param name="requiredTagTypes">If non-null, this specifies two different things simultaneously:
		/// That any tags found are implicitly runat="server" tags, and that they must be instances of (or inherit
		/// from) the given type of tags.  This parameter is primarily for use during processing of nested
		/// IEnumerable properties.</param>
		private void ParseText(string untilEndTag, IEnumerable<Type> requiredTagTypes, bool net45OrAbove)
		{
			do
			{
				Match match;

				if ((match = _textRegex.Match(_text, _src)).Success)
				{
					// Found a literal; skip it, but count its newlines to make sure we report errors correctly.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _directiveRegex.Match(_text, _src)).Success)
				{
					// Found a <%@ ... %> directive.
					ProcessDirective(match);

					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if ((match = _includeRegex.Match(_text, _src)).Success)
				{
					// Found a <!-- #include --> server-side include.

					// We have to process server-side includes, since they may produce more <%@ Register %> directives
					// or more controls that we need a .designer entry for.
					ProcessInclude(match, net45OrAbove);

					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if ((match = _commentRegex.Match(_text, _src)).Success)
				{
					// Discard comments.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _expressionRegex.Match(_text, _src)).Success)
				{
					// Ignore ASP.NET <%= ... %> expressions.  We're not running the template for real, after all.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _databindingRegex.Match(_text, _src)).Success)
				{
					// Ignore ASP.NET <%# ... %> databinding expressions.  We're not running the template for real, after all.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _codeRegex.Match(_text, _src)).Success)
				{
					// We have to ignore <% ... %> code blocks, since we're not processing the template for real.  This means
					// that it's entirely possible that we may miss a declaration that was moved from the .designer into
					// a code block, and thus generate an extra .designer entry for it.  There's not really that much we can
					// do about that, other than to tell people not to do something that crazy.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && _lastGreaterThanIndex > _src && (match = _tagRegex.Match(_text, _src)).Success)
				{
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;

					ProcessTag(match, requiredTagTypes, net45OrAbove);
				}
				else if ((match = _endTagRegex.Match(_text, _src)).Success)
				{
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;

					if (!ProcessEndTag(match, untilEndTag)) return;
				}
				else
				{
					// If we got here and didn't match anything, there must be a lonely '<' that is just sitting in the markup.
					// This might be bad, but it might not, since it could be a mixed markup/code declaration like this:
					//
					//     <input type='hidden' name='foo' value='<%# Eval("Foo") %>' />
					//
					Verbose("Found a '<' by itself, which might be a bug:  Did you forget to use '&lt;' instead?");
					_src++;
				}
			} while (_src < _text.Length);

			if (_isInScriptTag)
			{
				Error("<script> tag on line {0} has no ending </script>.", _scriptStartLine);
			}
		}

		/// <summary>
		/// Consume text from the current file until we have read its main directive, and then return
		/// that main directive.  If any non-whitespace non-comment text is found before the main directive
		/// in the file, this method will throw an error.
		/// </summary>
		/// <returns>The main directive of the file, decoded.</returns>
		private Tag ParseMainDirective()
		{
			do
			{
				Match match;

				if ((match = _textRegex.Match(_text, _src)).Success)
				{
					// Found a literal; skip it, but count its newlines to make sure we report errors correctly.
					Error("Found plain text before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first text in the file.");
				}
				else if (!_isInScriptTag && (match = _directiveRegex.Match(_text, _src)).Success)
				{
					// Found a <%@ ... %> directive.
					Tag tag = new Tag(match, true);

					if (string.IsNullOrEmpty(tag.TagName) || tag.TagName == "page" || tag.TagName == "control" ||
						tag.TagName == "master")
						return tag;

					Error("Found a <%@ ... %> directive before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first directive in the file.");
				}
				else if (_includeRegex.Match(_text, _src).Success)
				{
					// Found a <!-- #include --> server-side include.
					Error("Found an <!-- #include --> before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first directive in the file.");
				}
				else if (_commentRegex.Match(_text, _src).Success)
				{
					// Discard comments.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && _expressionRegex.Match(_text, _src).Success)
				{
					Error("Found a <%= ... %> expression before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first directive in the file.");
				}
				else if (!_isInScriptTag && _databindingRegex.Match(_text, _src).Success)
				{
					Error("Found a <%# ... %> expression before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first directive in the file.");
				}
				else if (!_isInScriptTag && _codeRegex.Match(_text, _src).Success)
				{
					Error("Found a <% ... %> code block before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first directive in the file.");
				}
				else if (!_isInScriptTag && _lastGreaterThanIndex > _src && _tagRegex.Match(_text, _src).Success)
				{
					Error("Found a <...> tag before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first content in the file.");
				}
				else if (_endTagRegex.Match(_text, _src).Success)
				{
					Error("Found a </...> tag before the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first content in the file.");
				}
				else
				{
					// If we got here and didn't match anything, there must be a lonely '<' that is just sitting in the markup.
					// This might be bad, but it might not, since it could be a mixed markup/code declaration like this:
					//
					//     <input type='hidden' name='foo' value='<%# Eval("Foo") %>' />
					//
					Verbose("Found a '<' by itself, which might be a bug:  Did you forget to use '&lt;' instead?");
					_src++;
				}
			} while (_src < _text.Length);

			Error("Missing the main <%@ Page ... %>, <%@ Control ... %> or <%@ Master ... %> directive.  Please make sure the main directive is the first content in the file.");
			return null;
		}

		/// <summary>
		/// While inside a server control, parse tag declarations as properties on that server control until reaching
		/// that server control's end tag.
		/// </summary>
		/// <param name="reflectedControl">The type of server control whose properties we are parsing.</param>
		private void ParseChildrenAsProperties(ReflectedControl reflectedControl, bool net45OrAbove)
		{
			int startLine = _line;

			string fullTagName = string.Format("{0}:{1}", reflectedControl.TagPrefix, reflectedControl.TagName).ToLower();

			do
			{
				Match match;

				if ((match = _textRegex.Match(_text, _src)).Success)
				{
					// Found a literal, which must be entirely whitespace if we find it.
					if (!_whitespaceRegex.IsMatch(match.Value))
					{
						Error("The {0} server control may contain only property declarations, and plain text was found.", reflectedControl.ClassName);
					}
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _directiveRegex.Match(_text, _src)).Success)
				{
					// Found a <%@ ... %> directive.
					ProcessDirective(match);

					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if ((match = _includeRegex.Match(_text, _src)).Success)
				{
					// Found a <!-- #include --> server-side include.

					// We have to process server-side includes, since they may produce more <%@ Register %> directives
					// or more controls that we need a .designer entry for.
					ProcessInclude(match, net45OrAbove);

					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if ((match = _commentRegex.Match(_text, _src)).Success)
				{
					// Discard comments.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _expressionRegex.Match(_text, _src)).Success)
				{
					// Ignore ASP.NET <%= ... %> expressions.  We're not running the template for real, after all.
					Error("There should not be a <%= ... %> expression used as a property in a {0} server control.", reflectedControl.ClassName);
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _databindingRegex.Match(_text, _src)).Success)
				{
					// Ignore ASP.NET <%# ... %> databinding expressions.  We're not running the template for real, after all.
					Error("There should not be a <%# ... %> expression used as a property in a {0} server control.", reflectedControl.ClassName);
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && (match = _codeRegex.Match(_text, _src)).Success)
				{
					// We have to ignore <% ... %> code blocks, since we're not processing the template for real.  This means
					// that it's entirely possible that we may miss a declaration that was moved from the .designer into
					// a code block, and thus generate an extra .designer entry for it.  There's not really that much we can
					// do about that, other than to tell people not to do something that crazy.
					Error("There should not be a <% ... %> code block used as a property in a {0} server control.", reflectedControl.ClassName);
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
				}
				else if (!_isInScriptTag && _lastGreaterThanIndex > _src && (match = _tagRegex.Match(_text, _src)).Success)
				{
					// Found a tag, which *should* match a property name in this control.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;
					ParseTagAsProperty(match, reflectedControl, net45OrAbove);
				}
				else if ((match = _endTagRegex.Match(_text, _src)).Success)
				{
					// Found an end tag, which should be the closing tag for this server control.
					int end = match.Index + match.Length;
					_line += CountNewlines(_text, _src, end);
					_src = end;

					string tagName = match.Groups["tagname"].Value;
					if (string.Compare(tagName, fullTagName, StringComparison.InvariantCultureIgnoreCase) == 0) return;

					Error("There should not be a </{0}> tag used as a property in a {1} server control.", tagName, reflectedControl.ClassName);
				}
				else
				{
					// If we got here and didn't match anything, there must be a lonely '<' that is just sitting in the markup.
					// This might be bad, but it might not, since it could be a mixed markup/code declaration like this:
					//
					//     <input type='hidden' name='foo' value='<%# Eval("Foo") %>' />
					//
					Verbose("Found a '<' by itself, which might be a bug:  Did you forget to use '&lt;' instead?");
					_src++;
				}
			} while (_src < _text.Length);

			if (_isInScriptTag)
			{
				Error("<script> tag on line {0} has no ending </script>.", _scriptStartLine);
			}
			else
			{
				Error("Missing </{0}> for <{0}> starting on line {1}.", fullTagName, startLine);
			}
		}

		#endregion

		#region Processing of Specific Tokens (directives, includes, tags, etc.)

		/// <summary>
		/// Having seen a &lt;%@ ... %gt; directive, decode it and process it.
		/// </summary>
		/// <param name="match">The capture groups collected when this directive's regular-expression was matched on the input.</param>
		private void ProcessDirective(Match match)
		{
			Tag tag = new Tag(match, true);

			if (string.IsNullOrEmpty(tag.TagName) || tag.TagName == "page" || tag.TagName == "control" || tag.TagName == "master")
			{
				Verbose("Found main directive: <%@ {0} %>", tag);
				ProcessMainDirective(tag);
			}
			else if (tag.TagName == "register")
			{
				Verbose("Found register directive: <%@ {0} %>", tag);
				ProcessRegisterDirective(tag);
			}
			else if (tag.TagName == "import")
			{
				Verbose("Ignoring import directive: <%@ {0} %>", tag);
			}
			else if (tag.TagName == "assembly")
			{
				Verbose("Ignoring assembly directive: <%@ {0} %>", tag);
			}
			else if (tag.TagName == "implements")
			{
				Verbose("Ignoring implements directive: <%@ {0} %>", tag);
			}
			else
			{
				Warning("Found unknown directive: <%@ {0} %>", tag);
			}
		}

		/// <summary>
		/// Given a "main" directive captured on the input, which would be "Page" for .aspx and "Control" for .ascx,
		/// decode its attributes and validate it against the current input file.
		/// </summary>
		/// <param name="tag">The complete decoded "main" directive.</param>
		private void ProcessMainDirective(Tag tag)
		{
			// Check its type to make sure it matches the filename.
			if (string.IsNullOrEmpty(tag.TagName))
			{
				// The main directive's type has been unfortunately omitted.  This is legal, but really not a very good practice.
				Warning("Main <%@ ... %> directive is missing the keyword \"{1}\".",  _filename.ToLower().EndsWith(".ascx") ? "Control" : "Page");
			}
			else if (tag.TagName == "page")
			{
				if (!_filename.ToLower().EndsWith(".aspx"))
				{
					Error("Main <%@ ... %> directive has the wrong keyword \"Control\".");
				}
			}
			else if (tag.TagName == "control")
			{
				if (!_filename.ToLower().EndsWith(".ascx"))
				{
					Error("Main <%@ ... %> directive has the wrong keyword \"Page\".");
				}
			}

			// There can be only one main directive per file.
			if (_mainDirective != null)
			{
				Error("This file has more than one main <%@ Page ... %> or <%@ Control ... %> directive.");
			}

			// It should have an inherits="" attribute that tells us the classname of the code-behind.
			string inherits = tag["inherits"];
			Type mainDirectiveType;
			if (string.IsNullOrEmpty(inherits))
			{
				Warning("Main <%@ ... %> directive is missing an Inherits=\"...\" attribute.  No code-behind is presumed to exist for this markup.");
				mainDirectiveType = null;
			}
			else
			{
				// Find the .NET class type that matches the given classname of the code-behind.
				mainDirectiveType = _assemblies.PrimaryAssembly?.GetType(inherits);
				if (mainDirectiveType == null)
				{
					Warning("Main <%@ ... %> directive says this markup inherits \"{0}\", but that class does not exist in the compiled website DLL.", inherits);
				}

				// Make sure that the type of the code-behind is a Page for pages and a UserControl for user controls.
				if (tag.TagName == "page" && !typeof(System.Web.UI.Page).IsAssignableFrom(mainDirectiveType))
				{
					Warning("Main <%@ ... %> directive says this markup inherits \"{0}\", but the class in the compiled website DLL does not inherit from System.Web.UI.Page!", inherits);
				}
				else if (tag.TagName == "control" && !typeof(System.Web.UI.UserControl).IsAssignableFrom(mainDirectiveType))
				{
					Warning("Main <%@ ... %> directive says this markup inherits \"{0}\", but the class in the compiled website DLL does not inherit from System.Web.UI.UserControl!", inherits);
				}
			}

			_mainDirective = tag;
			_mainDirectiveType = mainDirectiveType;
		}

		/// <summary>
		/// Having seen a &lt;%@ Register ... %gt; directive, decode it and process it.
		/// </summary>
		/// <param name="tag">The decoded &lt;%@ Register ... %gt; directive.</param>
		private void ProcessRegisterDirective(Tag tag)
		{
			string tagPrefix = tag["tagprefix"];
			string tagName = tag["tagname"];
			string src = tag["src"];
			string namespaceAttribute = tag["namespace"];
			string assembly = tag["assembly"];

			if (!string.IsNullOrEmpty(src) || !string.IsNullOrEmpty(tagName))
			{
				// We have a src="" or tagname="" attribute, so this is a <%@ Register %> for a user control.

				if (string.IsNullOrEmpty(tagPrefix))
				{
					Warning("The <%@ Register %> directive is missing the required TagPrefix attribute.  Skipping directive.");
					return;
				}
				if (string.IsNullOrEmpty(tagName))
				{
					Warning("The <%@ Register %> directive is missing the required TagName attribute.  Skipping directive.");
					return;
				}
				if (string.IsNullOrEmpty(src))
				{
					Warning("The <%@ Register %> directive is missing the required Src attribute.  Skipping directive.");
					return;
				}
				if (!string.IsNullOrEmpty(namespaceAttribute))
				{
					Warning("The <%@ Register %> directive cannot have both a Src/TagName attribute and a Namespace attribute.  Skipping directive.");
					return;
				}

				// Add the new tag registration to the list of whatever's already been registered.
				TagRegistration userControlTagRegistration = new TagRegistration
				{
					Kind = TagRegistrationKind.SingleUserControl,
					SourceFilename = src,
					TagPrefix = tagPrefix,
					TagName = tagName,
				};

				_tagRegistrations.Add(userControlTagRegistration);

				// Go resolve the user control to a real .NET type, which will involve a partial parse of its markup.
				Common.ResolveUserControls(_compileContext, new List<TagRegistration> { userControlTagRegistration }, _assemblies, _assemblyDirectory, _rootPath, Path.GetDirectoryName(_filename));
			}
			else if (!string.IsNullOrEmpty(namespaceAttribute))
			{
				// We have a namespace="" attribute, so this is a <%@ Register %> for a namespace full of server controls.

				Verbose("Registering namespace \"{0}\" as <{1}:...>.", namespaceAttribute, tagPrefix);

				_tagRegistrations.Add(new TagRegistration
				{
					Kind = TagRegistrationKind.Namespace,
					TagPrefix = tagPrefix,
                    Namespace = namespaceAttribute,
					AssemblyFilename = assembly,
				});

				// If this namespace is in a new assembly we haven't loaded yet, go load it.
				if (!string.IsNullOrEmpty(assembly))
				{
					_assemblies.PreloadAssemblies(_compileContext, new List<string> { assembly }, _assemblyDirectory);
				}
			}
			else
			{
				Warning("The <%@ Register %> directive should have either a Src/TagName attribute or a Namespace attribute.  Skipping directive.");
				return;
			}
		}

		/// <summary>
		/// Process a server-side &lt;!-- include --%gt;.
		/// </summary>
		/// <param name="match">The capture groups collected when this include's regular-expression was matched on the input.</param>
		private void ProcessInclude(Match match, bool net45OrAbove)
		{
			string pathtype = match.Groups["pathtype"].Value;
			string filename = match.Groups["filename"].Value;

			if (!string.IsNullOrEmpty(pathtype))
			{
				// A path type was provided, so decode relative to that.  If no path type is provided, we just resolve
				// the path relative to the current directory, just like ASP.NET does.  Generally, you should prefer
				// to use virtual paths, since they're safer.
				switch (pathtype.ToLower())
				{
					case "virtual":
						filename = Common.ResolveWebsitePath(_compileContext, filename, _rootPath, Path.GetDirectoryName(_filename));
						break;

					default:
						Error("Unknown path type in <!-- include --> directive.  Did you mean to write \"virtual=\"?");
						break;
				}
			}

			// Recursively load the additional markup and keep going with the parse.
			LoadMarkupInternal(Path.GetFullPath(filename), net45OrAbove);
		}

		/// <summary>
		/// Process a &lt;tag&gt; in the markup, which may just be plain HTML or it may be a control declaration.
		/// </summary>
		/// <param name="match">The capture groups collected when this tag's regular-expression was matched on the input.</param>
		/// <param name="requiredTagTypes">The allowable types of controls that may be used in this position, if any.
		/// If non-null, this specifies two different things simultaneously:  That this tag is implicitly a runat="server"
		/// tag, and that it must be an instance of (or inherit from) one of the given types.  This parameter is primarily
		/// for use during processing of nested IEnumerable properties.</param>
		private void ProcessTag(Match match, IEnumerable<Type> requiredTagTypes, bool net45OrAbove)
		{
			// Decide if this is a server control or just a plain HTML tag.
			Tag tag = new Tag(match);
			string runat = tag["runat"];
			bool isServerTag = !string.IsNullOrEmpty(runat);
			if (isServerTag && runat != "server")
			{
				Warning("Found runat= that is not set to \"server\".");
			}
			isServerTag |= (requiredTagTypes != null);

			// If this is a <script> tag, start processing the input as JavaScript until we reach a </script> tag.
			if (tag.TagName == "script")
			{
				Verbose("Found opening <script> tag.  Skipping all directives and controls until closing </script> tag...");
				_isInScriptTag = true;
			}

			// If this is a server tag --- a server-control or user-control declaration --- then go process it as one.
			if (isServerTag)
			{
				ProcessServerTag(tag, requiredTagTypes, net45OrAbove);
			}
		}

		/// <summary>
		/// Given a &lt;tag&gt; in the markup that is runat="server", process it as a possible control declaration.
		/// </summary>
		/// <param name="tag">The fully-decoded form of the tag.</param>
		/// <param name="requiredTagTypes">The allowable types of controls that may be used in this position, if any.
		/// If non-null, this specifies two different things simultaneously:  That this tag is implicitly a runat="server"
		/// tag, and that it must be an instance of (or inherit from) one of the given types.  This parameter is primarily
		/// for use during processing of nested IEnumerable properties.</param>
		private void ProcessServerTag(Tag tag, IEnumerable<Type> requiredTagTypes, bool net45OrAbove)
		{
			// Get the control's ID, if one has been assigned.
			string id = tag["id"];
			Verbose(string.IsNullOrEmpty(id) ? "Found server tag <{0}>." : "Found server tag <{0} ID=\"{1}\">.", tag.OriginalTagName, id);

			// Go get the control's type.
			ReflectedControl reflectedControl;
			try
			{
				reflectedControl = _reflectedControlCollection.GetControl(_compileContext, tag, requiredTagTypes ?? _justControlTypes, net45OrAbove);
			}
			catch (Exception e)
			{
				Error("Class for control <{0}> could not be loaded:\r\n{1}", tag.OriginalTagName, e.Message);
				return;
			}

			// If we're in a place where this control will need a .designer declaration, add one of those to the output.
			if (_shouldGenerateOutput)
			{
				OutputControl outputControl = new OutputControl
				                              {
				                              	Name = id,
				                              	ReflectedControl = reflectedControl,
				                              };
				Verbose("Adding {0} to the list of declared controls.", outputControl);
				_outputControls.Add(outputControl);
			}

			// If this is a self-closing tag, we're done; otherwise, parse and process anything inside this.
			if (!tag.IsEmpty)
			{
				ProcessServerTagContents(tag, requiredTagTypes != null, reflectedControl, net45OrAbove);
			}
		}

		/// <summary>
		/// Given a &lt;tag&gt; in the markup that is runat="server", process its children.
		/// </summary>
		/// <param name="tag">The fully-decoded form of the tag.</param>
		/// <param name="mustParseChildrenAsProperties">Whether children will be parsed as properties even if
		/// this tag does not explicitly state that they should be (as is the case in a server control being
		/// stored in an IEnumerable property of a parent server control).</param>
		/// <param name="reflectedControl">The metadata about this control, needed for identifying and filling in its properties.</param>
		private void ProcessServerTagContents(Tag tag, bool mustParseChildrenAsProperties, ReflectedControl reflectedControl, bool net45OrAbove)
		{
			_compileContext.VerboseNesting++;
			_controlStack.Add(tag.TagName);

			if (!mustParseChildrenAsProperties && (reflectedControl.ParseChildrenAttribute == null || !reflectedControl.ParseChildrenAttribute.ChildrenAsProperties))
				return;

			Verbose("This server control has ParseChildren(true), either explicitly or implicitly (by being part of a collection).");
			Verbose("Beginning special parse of children as properties.");

			ParseChildrenAsProperties(reflectedControl, net45OrAbove);

			Verbose("End of special parse of children as properties.");

			_controlStack.RemoveAt(_controlStack.Count - 1);
			_compileContext.VerboseNesting--;
		}

		/// <summary>
		/// Given a &lt;/tag&gt; in the markup, conclude any matching server tags, and possibly conclude the
		/// output if this is an expected end-tag for this section.
		/// </summary>
		/// <param name="match">The capture groups collected when this end-tag's regular-expression was matched on the input.</param>
		/// <param name="untilEndTag">If we're inside a server control, this should be its end-tag.</param>
		/// <returns>True if the caller should continue parsing at the same level of nesting;
		/// false if this end-tag concludes the server control or the input and the caller itself should not continue.</returns>
		private bool ProcessEndTag(Match match, string untilEndTag)
		{
			// See if this is the expected end-tag; if it is, return false.
			string tagName = match.Groups["tagname"].Value;
			if (string.Compare(tagName, untilEndTag, StringComparison.InvariantCultureIgnoreCase) == 0)
				return false;

			// See if this is a closing </script> tag.
			if (string.Compare(tagName, "script", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				Verbose("Found closing </script> tag.");
				_isInScriptTag = false;
			}
			else
			{
				// See if this concludes the server control on the top of the stack of controls.
				if (_controlStack.Count > 0
					&& string.Compare(_controlStack[_controlStack.Count - 1], tagName, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					_controlStack.RemoveAt(_controlStack.Count - 1);
					_compileContext.VerboseNesting--;
				}
			}

			// It's okay to keep parsing at this level.
			return true;
		}

		/// <summary>
		/// Inside a server control whose children are properties, &lt;tags&gt; must match up to properties
		/// on the control class.  This method attempts to associate a single parsed &lt;tag&gt; with its
		/// matching property on the class, and then parse this tag's children according to whatever kind of
		/// property it matches up to.
		/// </summary>
		/// <param name="match">The capture groups collected when this tag's regular-expression was matched on the input.</param>
		/// <param name="reflectedControl">The metadata about the parent control class that this tag is supposed to be a property on.</param>
		private void ParseTagAsProperty(Match match, ReflectedControl reflectedControl, bool net45OrAbove)
		{
			//
			// There are four scenarios here we have to consider:
			//
			//   1.  If the property that's matched is an ITemplate, and it's declared with [TemplateInstance.Single],
			//        then the markup contained inside it must be processed normally until a closing </tagname>
			//        and also needs to be declared in the designer file.
			//
			//   2.  If the property that's matched is a Control type, we need to process that control and its children
			//        normally until the closing </prefix:tagname>, but *not* declare them in the designer file.
			//
			//   3.  If the property that's matched is an IEnumerable type, we need to process its children until a closing
			//        </tagname>, and declare those children in the designer file.
			//
			//   4.  If there is no property that's matched, but we have a DefaultCollectionProperty for this control
			//        assigned by its [ParseChildren] attribute, then we have an implicit property match, and include
			//        the control in that collection (and generate an output declaration for it).
			//
			// Scenarios 2 and 3 can conveniently be handled by ParseText() using just different ending tags.

			Tag tag = new Tag(match);
			string tagName = tag.TagName;

			// Find the property that matches this tag.
			if (!reflectedControl.ControlProperties.ContainsKey(tagName))
			{
				if (reflectedControl.DefaultCollectionProperty == null)
				{
					Error("No property named \"{0}\" exists inside class {1}.", tagName, reflectedControl.ControlType.FullName);
					return;
				}

				// There's no direct property, but this control has a default collection property, so this child
				// control will be stuffed into that collection by ASP.NET.  
				ParseTagIntoDefaultCollection(tag, reflectedControl, net45OrAbove);
				return;
			}

			Verbose("Found <{0}> tag that matches a property in class {1}.", tagName, reflectedControl.ControlType.FullName);

			// Early-out:  If this is a self-closing tag, we don't need to process its children.
			if (tag.IsEmpty) return;

			// Based on the property that matches this tag, recurse into the proper kind of parsing state.
			ReflectedControlProperty reflectedControlProperty = reflectedControl.ControlProperties[tagName];
			if (reflectedControlProperty.IsTemplateProperty)
			{
				if (reflectedControlProperty.TemplateInstanceAttribute != null && reflectedControlProperty.TemplateInstanceAttribute.Instances == System.Web.UI.TemplateInstance.Single)
				{
					Verbose("This control property uses [TemplateInstance(TemplateInstance.Single)]; recursing.");
					ParseText(tagName, null, net45OrAbove);
				}
				else
				{
					Verbose("This control property is an ITemplate whose child controls do not belong in the designer file.");
					bool wasGeneratingOutput = _shouldGenerateOutput;
					_shouldGenerateOutput = false;
					ParseText(tagName, null, net45OrAbove);
					_shouldGenerateOutput = wasGeneratingOutput;
				}
			}
			else if (reflectedControlProperty.IsCollectionProperty)
			{
				Verbose("This control property is a list whose immediate children belong in the designer file.");
				ParseText(tagName, reflectedControlProperty.CollectionItemTypes, net45OrAbove);
			}
			else
			{
				Verbose("This control property has children, but neither it nor its children belong in the designer file.");
				bool wasGeneratingOutput = _shouldGenerateOutput;
				_shouldGenerateOutput = false;
				ParseText(tagName, null, net45OrAbove);
				_shouldGenerateOutput = wasGeneratingOutput;
			}
		}

		/// <summary>
		/// Found a tag that belongs in the default collection of the current server control.
		/// </summary>
		/// <param name="tag">The tag that we are attempting to parse.</param>
		/// <param name="reflectedControl">The control with the collection into which this tag will be placed.</param>
		private void ParseTagIntoDefaultCollection(Tag tag, ReflectedControl reflectedControl, bool net45OrAbove)
		{
			string tagName = tag.TagName;

			ReflectedControlProperty reflectedControlProperty = reflectedControl.DefaultCollectionProperty;
			Verbose("This <{0}> tag describes a control that belongs in the default control collection, \"{1}\".",
				tagName, reflectedControlProperty.PropertyName);

			ReflectedControl childControl = _reflectedControlCollection.GetControl(_compileContext, tag, _allTypes, net45OrAbove);

			if (childControl == null)
			{
				Error("No control type exists that matches tag <{0}>.", tagName);
				return;
			}

			// Visual Studio will also generate a declaration for this control if it has an ID, so we have to follow the same behavior.
			if (_shouldGenerateOutput)
			{
				_outputControls.Add(new OutputControl
                {
                	Name = tag["id"],
                	ReflectedControl = childControl,
                });
			}

			if (tag.IsEmpty) return;

			ParseText(tagName, reflectedControlProperty.CollectionItemTypes, net45OrAbove);
		}

		#endregion

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
