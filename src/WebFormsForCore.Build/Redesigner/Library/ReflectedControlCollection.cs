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

namespace Redesigner.Library
{
	/// <summary>
	/// A collection of all known controls, determined by reflecting against the registered assemblies.
	/// </summary>
	public class ReflectedControlCollection
	{
		#region Fields

		/// <summary>
		/// The complete set of reflected controls, organized by full type name.
		/// </summary>
		private readonly Dictionary<string, ReflectedControl> _reflectedControls = new Dictionary<string, ReflectedControl>();

		/// <summary>
		/// The complete set of registered user controls and namespaces.
		/// </summary>
		private readonly IEnumerable<TagRegistration> _tagRegistrations;

		/// <summary>
		/// The complete set of known (and loaded) assemblies.
		/// </summary>
		private readonly AssemblyLoader _assemblies;

		#endregion

		#region Methods

		/// <summary>
		/// Construct a new control collection that will house all controls defined by the given set of
		/// registrations and assemblies.
		/// </summary>
		/// <param name="tagRegistrations">The tag registrations (from either the "web.config" file or
		/// from &lt;%@ Register %%gt; directives in the markup).</param>
		/// <param name="assemblies">The set of known assemblies, preloaded.</param>
		public ReflectedControlCollection(IEnumerable<TagRegistration> tagRegistrations, AssemblyLoader assemblies)
		{
			_tagRegistrations = tagRegistrations;
			_assemblies = assemblies;
		}

		/// <summary>
		/// Get a ReflectedControl for the given markup tag, if one exists, restricting the search
		/// to just class types that are included in the allowed-types list (or types that inherit from them).
		/// </summary>
		/// <param name="compileContext">The context in which errors should be reported.</param>
		/// <param name="tag">The complete markup tag to search for.</param>
		/// <param name="allowedTypes">The allowed types that can be returned.  If you want to accept
		/// all possible types, pass in a list containing just typeof(object).</param>
		/// <returns>The found control type.</returns>
		public ReflectedControl GetControl(ICompileContext compileContext, Tag tag, IEnumerable<Type> allowedTypes, bool net45OrAbove)
		{
			bool isNormalServerControl = tag.TagName.Contains(":");

			if (isNormalServerControl && _reflectedControls.ContainsKey(tag.TagName))
				return _reflectedControls[tag.TagName];

			ReflectedControl reflectedControl = new ReflectedControl(compileContext, tag, _tagRegistrations, _assemblies, allowedTypes, net45OrAbove);

			if (isNormalServerControl && tag.TagName.Contains(":"))
			{
				_reflectedControls[tag.TagName] = reflectedControl;
			}

			return reflectedControl;
		}

		#endregion
	}
}
