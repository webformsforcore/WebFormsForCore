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

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Redesigner.Library
{
	/// <summary>
	/// An HTML tag parsed into its constituent name and attribute="value" pairs.  This contains
	/// only the opening/closing/empty tag itself, not any of its child elements.  This class is
	/// immutable once constructed.
	/// </summary>
	public class Tag : IEnumerable<TagAttribute>
	{
		#region Fields and properties

		/// <summary>
		/// The name of this tag, always lowercased to make comparison easier.
		/// </summary>
		public readonly string TagName;

		/// <summary>
		/// The original name of this tag, its original casing intact.
		/// </summary>
		public readonly string OriginalTagName;

		/// <summary>
		/// Whether this tag is empty (i.e., has a tailing self-closing / character).
		/// </summary>
		public readonly bool IsEmpty;

		/// <summary>
		/// The list of attribute pairs, in original markup order.
		/// </summary>
		private readonly List<TagAttribute> _attributes = new List<TagAttribute>();

		/// <summary>
		/// A lookup table for attributes by attribute name (lowercase).
		/// </summary>
		private readonly Dictionary<string, int> _attributeLookup = new Dictionary<string, int>();

		/// <summary>
		/// Whether this tag has duplicate attributes (like type="foo" followed by type="bar" on the same tag).
		/// </summary>
		public readonly bool HasDuplicates;

		#endregion

		#region Methods

		/// <summary>
		/// Given a matched regular expression and its embedded captures, decode those captures into a series
		/// of TagAttributes on a new Tag instance.
		/// </summary>
		/// <param name="match">The capture groups collected when this tag's regular-expression was matched on the input.</param>
		public Tag(Match match)
			: this(match, false)
		{
		}

		/// <summary>
		/// Given a matched regular expression and its embedded captures, decode those captures into a series
		/// of TagAttributes on a new Tag instance.
		/// </summary>
		/// <param name="match">The capture groups collected when this tag's regular-expression was matched on the input.</param>
		/// <param name="isDirective">If true, this tag should be processed like a &lt;% directive %gt;; if false, this tag
		/// should be processed like a normal tag.</param>
		public Tag(Match match, bool isDirective)
		{
			Group group;
			OriginalTagName = ((group = match.Groups["tagname"]) != null && group.Success ? group.Value : null);
			IsEmpty = ((group = match.Groups["empty"]) != null && group.Success);

			CaptureCollection attributeNames = match.Groups["attrname"].Captures;
			CaptureCollection attributeValues = match.Groups["attrval"].Captures;
			CaptureCollection equalSign = isDirective ? match.Groups["equal"].Captures : null;

			for (int i = 0; i < attributeNames.Count; i++)
			{
				string attributeName = attributeNames[i].ToString();
				string attributeValue = System.Web.HttpUtility.HtmlDecode(attributeValues[i].ToString());

				if (isDirective)
				{
					if (i == 0 && equalSign[i].ToString().Length <= 0)
					{
						OriginalTagName = attributeName;
						continue;
					}
				}

				HasDuplicates |= Add(attributeName, attributeValue);
			}

			TagName = (OriginalTagName != null ? OriginalTagName.ToLower() : null);
		}

		/// <summary>
		/// Get an enumerator that iterates over all of the attributes of this tag, in markup order, including duplicates.
		/// </summary>
		/// <returns>The attribute enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _attributes.GetEnumerator();
		}

		/// <summary>
		/// Get an enumerator that iterates over all of the attributes of this tag, in markup order, including duplicates.
		/// </summary>
		/// <returns>The attribute enumerator.</returns>
		public IEnumerator<TagAttribute> GetEnumerator()
		{
			return _attributes.GetEnumerator();
		}

		/// <summary>
		/// Add an attribute name=value pair to this tag.
		/// </summary>
		/// <param name="name">The name of the attribute to add.</param>
		/// <param name="value">The value to assign to this attribute.</param>
		/// <returns>True if this class has duplicates as a result of this add, false otherwise.</returns>
		private bool Add(string name, string value)
		{
			_attributes.Add(new TagAttribute(name, value));

			string nameLower = name.ToLower();
			if (_attributeLookup.ContainsKey(nameLower)) return true;

			_attributeLookup.Add(nameLower, _attributes.Count - 1);
			return false;
		}

		/// <summary>
		/// Retrieve an attribute's value, by name.
		/// </summary>
		/// <param name="attributeName">The name of the attribute to retrieve.</param>
		/// <returns>The value of that attribute.  If that attribute does not exist (if it was not added), this returns null.</returns>
		public string this[string attributeName]
		{
			get
			{
				if (attributeName == null) return TagName;

				string nameLower = attributeName.ToLower();
				if (_attributeLookup.ContainsKey(nameLower))
				{
					TagAttribute attribute = _attributes[_attributeLookup[nameLower]];
					return attribute.Value;
				}

				return null;
			}
		}

		/// <summary>
		/// Convert this tag to a string for easy debugging.
		/// </summary>
		/// <returns>A version of this tag converted back to HTML form, containing this tag's name, its attributes,
		/// and its optional trailing / if there is one.</returns>
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();

			if (!string.IsNullOrEmpty(OriginalTagName))
			{
				stringBuilder.Append(OriginalTagName);
			}

			foreach (TagAttribute tagAttribute in _attributes)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(tagAttribute.ToString());
			}

			if (IsEmpty)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append("/");
			}

			return stringBuilder.ToString();
		}

		#endregion
	}
}
