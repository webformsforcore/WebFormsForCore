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
	/// The parsed equivalent of a &lt;add&gt; (web.config) or &lt;%Register%&gt; (markup) directive.
	/// This is a "dumb" class, containing only fields; all the intelligence of managing it is in
	/// other classes.
	/// </summary>
	public class TagRegistration
	{
		/// <summary>
		/// What kind of tag registration this is.
		/// </summary>
		public TagRegistrationKind Kind;

		/// <summary>
		/// The name of this tag in the markup (if appropriate).
		/// </summary>
		public string TagName;

		/// <summary>
		/// The prefix that will be used before instances of this tag(s) in the markup.
		/// </summary>
		public string TagPrefix;

		/// <summary>
		/// The assembly the code for this tag can be found in.
		/// </summary>
		public string AssemblyFilename;

		/// <summary>
		/// The namespace inside the given assembly where this tag prefix's code can be found.
		/// </summary>
		public string Namespace;

		/// <summary>
		/// The .ascx file containing the markup for this registered user control.
		/// </summary>
		public string SourceFilename;

		/// <summary>
		/// The C# full type name that this .ascx file says it inherits.
		/// </summary>
		public string Typename;
	}
}
