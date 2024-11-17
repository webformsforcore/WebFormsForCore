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

namespace Redesigner.Library
{
	/// <summary>
	/// The current state of the parser.  This is used to form a stack of parser-state data
	/// when recursing into nested includes.
	/// </summary>
	public class MarkupReaderState
	{
		/// <summary>
		/// The filename of the current file being parsed.
		/// </summary>
		public readonly string Filename;

		/// <summary>
		/// The line number in the current file being parsed.
		/// </summary>
		public readonly int Line;

		/// <summary>
		/// The position in the file of the last greater-than character, used for optimizing the regex searches.
		/// </summary>
		public readonly int LastGreaterThanIndex;

		/// <summary>
		/// The raw text of the file itself.
		/// </summary>
		public readonly string Text;

		/// <summary>
		/// The current read-pointer into the text.
		/// </summary>
		public readonly int Src;

		/// <summary>
		/// Whether any controls that are found should be generated as public control declarations or whether they
		/// are only private/internal declarations.
		/// </summary>
		public readonly bool ShouldGenerateOutput;

		/// <summary>
		/// Construct a copy of the parser state.
		/// </summary>
		public MarkupReaderState(string filename, int line, string text, int src, int lastGreaterThanIndex, bool shouldGenerateOutput)
		{
			Filename = filename;
			Line = line;
			Text = text;
			Src = src;
			LastGreaterThanIndex = lastGreaterThanIndex;
			ShouldGenerateOutput = shouldGenerateOutput;
		}
	}
}
