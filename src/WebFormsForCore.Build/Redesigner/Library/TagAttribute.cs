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
	/// A single name=value attribute within a tag.  These are immutable once constructed.
	/// </summary>
	public class TagAttribute
	{
		#region Fields

		/// <summary>
		/// The name of this attribute.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// The value of this attribute for the given name.
		/// </summary>
		public readonly string Value;

		#endregion

		#region Methods

		/// <summary>
		/// Construct a new attribute with the given name/value pair.
		/// </summary>
		/// <param name="name">The name of this attribute.</param>
		/// <param name="value">The value of this attribute.</param>
		public TagAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}

		/// <summary>
		/// Convert this attribute to a string for easy debugging.
		/// </summary>
		/// <returns>This attribute, converted to an equivalent raw HTML form.</returns>
		public override string ToString()
		{
			return string.Format("{0}=\"{1}\"", Name, System.Web.HttpUtility.HtmlEncode(Value));
		}

		#endregion
	}
}
