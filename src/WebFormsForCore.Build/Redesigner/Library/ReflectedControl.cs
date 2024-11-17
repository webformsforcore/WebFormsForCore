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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.UI;

namespace Redesigner.Library
{
	/// <summary>
	/// Metadata about a single web control.
	/// </summary>
	public class ReflectedControl
	{
		#region Fields and Static Data

		/// <summary>
		/// The user-control or namespace registration that contained this control.
		/// </summary>
		public readonly TagRegistration TagRegistration;

		/// <summary>
		/// The name of this control (as it appears in markup, without its prefix).
		/// </summary>
		public readonly string TagName;

		/// <summary>
		/// The namespace prefix for this control.
		/// </summary>
		public readonly string TagPrefix;

		/// <summary>
		/// The actual .NET classname for the class that backs this control.
		/// </summary>
		public string ClassName { get { return ControlType.Name; } }

		/// <summary>
		/// The actual .NET type of the class that backs this control.
		/// </summary>
		public readonly Type ControlType;

		/// <summary>
		/// The [ParseChildren] attribute for this control, if one exists.
		/// </summary>
		public readonly System.Web.UI.ParseChildrenAttribute ParseChildrenAttribute;

		/// <summary>
		/// The [ControlBuilder] attribute for this control, if one exists.
		/// </summary>
		public readonly System.Web.UI.ControlBuilderAttribute ControlBuilderAttribute;

		/// <summary>
		/// All of the properties for this control that can be set within the markup.
		/// </summary>
		public readonly Dictionary<string, ReflectedControlProperty> ControlProperties;

		/// <summary>
		/// If the [ParseChildren] attribute says that this class has a default collection property
		/// into which child controls will be parsed, this is that default collection property.
		/// </summary>
		public ReflectedControlProperty DefaultCollectionProperty;

		/// <summary>
		/// A static "special" tag registration for the HTML controls --- any standard HTML elements
		/// that have had runat="server" applied to them.
		/// </summary>
		private static readonly TagRegistration _htmlTagRegistration = new TagRegistration
		{
			Kind = TagRegistrationKind.HtmlControl,
			AssemblyFilename = Common.SystemWebAssemblyName,
			Namespace = "System.Web.UI.HtmlControls",
			TagPrefix = string.Empty,
		};

		#endregion

		#region Methods

		/// <summary>
		/// Construct a new blob of metadata for a single control, performing any reflection needed to determine its
		/// structure and parsing behavior.
		/// </summary>
		/// <param name="compileContext">The context in which errors should be reported.</param>
		/// <param name="tag">The complete tag for this control, as found in the markup.</param>
		/// <param name="tagRegistrations">The known list of registrations, formed from the directives in the
		/// "web.config" and any &lt;%@ Register %&gt; directives in the markup.</param>
		/// <param name="assemblies">The complete list of known pre-loaded assemblies for reflection.</param>
		/// <param name="allowedTypes">The allowable types of control that may be returned.  If the matching
		/// .NET class type for this control does not match one of these types (or is not derivable from one
		/// of these types), this constructor will throw an exception.</param>
		public ReflectedControl(ICompileContext compileContext, Tag tag, IEnumerable<TagRegistration> tagRegistrations, AssemblyLoader assemblies, IEnumerable<Type> allowedTypes)
		{
			// Decode the tag declaration.
			DecodeFullTagNameWithPrefix(tag.TagName, out TagPrefix, out TagName);

			// Find the matching C# type for that tag declaration.
			if (string.IsNullOrEmpty(TagPrefix))
			{
				TagRegistration = _htmlTagRegistration;
				ControlType = FindMatchingHtmlControlType(tag);
			}
			else
			{
				ControlType = FindMatchingControlType(TagPrefix, TagName, tagRegistrations, assemblies, out TagRegistration);
			}
			if (ControlType == null)
				throw new InvalidOperationException(string.Format("No matching type for <{0}> was found in any of the loaded assemblies.", tag.TagName));

			// If we are restricted to only load certain types (such as the nested not-a-control instances inside a DataPager control),
			// check that the control we have found matches one of those types.
			if (!allowedTypes.Any(t => t.IsAssignableFrom(ControlType)))
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Found matching type for <{0}>, but it is a {1}, not one of the {2} allowed types:\r\n",
					tag.TagName, ControlType.FullName, allowedTypes.Count());
				foreach (Type allowedType in allowedTypes)
				{
					stringBuilder.AppendFormat("- {0}\r\n", allowedType.FullName);
				}
				throw new InvalidOperationException(stringBuilder.ToString());
			}

			// Extract the [ParseChildren] attribute, if it has one.
			System.Web.UI.ParseChildrenAttribute[] parseChildrenAttributes = (System.Web.UI.ParseChildrenAttribute[])ControlType.GetCustomAttributes(typeof(System.Web.UI.ParseChildrenAttribute), true);
			ParseChildrenAttribute = parseChildrenAttributes.Length == 0 ? null : parseChildrenAttributes[0];

			// Extract the [ControlBuilder] attribute, if it has one.
			System.Web.UI.ControlBuilderAttribute[] controlBuilderAttributes = (System.Web.UI.ControlBuilderAttribute[])ControlType.GetCustomAttributes(typeof(System.Web.UI.ControlBuilderAttribute), true);
			ControlBuilderAttribute = controlBuilderAttributes.Length == 0 ? null : controlBuilderAttributes[0];

			// Extract the type's properties, since their declarations control what's legal in the markup.
			ControlProperties = CollectControlProperties(compileContext, ControlType, this);

			// HtmlControls have to be handled specially, since they have [ParseChildren(true)] in many cases but
			// aren't really using it for anything.
			if (ControlType.Namespace == _htmlTagRegistration.Namespace)
			{
				ParseChildrenAttribute = new ParseChildrenAttribute(false);
			}

			// Validate the ParseChildrenAttribute, which may be broken or weird.
			if (ParseChildrenAttribute != null)
			{
				if (!string.IsNullOrEmpty(ParseChildrenAttribute.DefaultProperty))
				{
					string propertyName = ParseChildrenAttribute.DefaultProperty.ToLower();		// ASP.NET also ignores case on this; see internals of ControlBuilder.CreateChildBuilder() for details.
					if (!ControlProperties.ContainsKey(propertyName))
					{
						throw new InvalidOperationException(string.Format("The [ParseChildren] attribute on class \"{0}\" names a default property \"{1}\" that does not exist in that class.",
							ControlType.FullName, ParseChildrenAttribute.DefaultProperty));
					}

					DefaultCollectionProperty = ControlProperties[propertyName];
					if (!DefaultCollectionProperty.IsCollectionProperty)
					{
						throw new InvalidOperationException(string.Format("The [ParseChildren] attribute on class \"{0}\" names a default property \"{1}\" that is not a collection property.  The default property must always be a collection property.",
							ControlType.FullName, ParseChildrenAttribute.DefaultProperty));
					}
				}
			}
		}

		/// <summary>
		/// Given a tag name that may contain a colon, such as "asp:Panel", decompose it into its constituent pieces,
		/// a tag name ("Panel") and a tag prefix ("asp").
		/// </summary>
		/// <param name="fullTagNameWithPrefix">The full tag name, from markup.</param>
		/// <param name="tagPrefix">The tag prefix.  If there is no prefix, this will be string.Empty.</param>
		/// <param name="tagName">The tag name.</param>
		private static void DecodeFullTagNameWithPrefix(string fullTagNameWithPrefix, out string tagPrefix, out string tagName)
		{
			int colonIndex = fullTagNameWithPrefix.IndexOf(':');
			if (colonIndex < 0)
			{
				// Could be one of the HTML server tags, so we have to try to process it.
				tagPrefix = string.Empty;
				tagName = fullTagNameWithPrefix;
				return;
			}

			tagPrefix = fullTagNameWithPrefix.Substring(0, colonIndex);
			tagName = fullTagNameWithPrefix.Substring(colonIndex + 1);
			if (string.IsNullOrEmpty(tagPrefix) || string.IsNullOrEmpty(tagName))
				throw new InvalidOperationException(string.Format("The name <{0}> is not well-formed with both a tag prefix and a tag name.", fullTagNameWithPrefix));
		}

		/// <summary>
		/// Search through the given set of tag registrations and assemblies to find a suitable .NET class type that
		/// matches the given tag prefix and tag name.  (This method handles server controls and user controls, not HTML
		/// controls; the tag prefix must not be empty.)
		/// </summary>
		/// <param name="tagPrefix">The tag prefix of the tag we're searching for.</param>
		/// <param name="tagName">The tag name of the tag we're searching for.</param>
		/// <param name="tagRegistrations">The known list of registrations, formed from the directives in the
		/// "web.config" and any &lt;%@ Register %&gt; directives in the markup.</param>
		/// <param name="assemblies">The complete list of known pre-loaded assemblies for reflection.</param>
		/// <param name="matchingTagRegistration">The matching tag registration for this tag prefix/name, if there is one.</param>
		/// <returns>The matching class type, if one exists, or null if there is no matching class type.</returns>
		private static Type FindMatchingControlType(string tagPrefix, string tagName, IEnumerable<TagRegistration> tagRegistrations, AssemblyLoader assemblies, out TagRegistration matchingTagRegistration)
		{
			// Since multiple tag registrations could match this declaration, we have no choice but to walk through
			// all of the tag registrations and just try to find the first one that matches.
			foreach (TagRegistration tagRegistration in tagRegistrations)
			{
				// Try the prefix first.  If that doesn't match, it's definitely not what we're looking for.
				if (string.Compare(tagRegistration.TagPrefix, tagPrefix, StringComparison.InvariantCultureIgnoreCase) != 0) continue;

				switch (tagRegistration.Kind)
				{
					case TagRegistrationKind.SingleUserControl:
						if (string.Compare(tagRegistration.TagName, tagName, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							// We matched a registered user control, so we have to partially parse it to find out
							// what namespace in the website assembly is associated with it.
							Assembly assembly = assemblies.PrimaryAssembly;
							Type type = assembly.GetType(tagRegistration.Typename);

							// Make sure we found it, and that it's actually a UserControl of some kind.
							if (type == null || !typeof(System.Web.UI.UserControl).IsAssignableFrom(type)) break;

							// We found it, so return it.
							matchingTagRegistration = tagRegistration;
							return type;
						}
						break;

					case TagRegistrationKind.Namespace:
						{
							// This registration describes an entire namespace worth of controls in an assembly somewhere.
							// So check see if tagName matches a class inside the given namespace in that assembly.
							Assembly assembly = assemblies[tagRegistration.AssemblyFilename];
							Type type = assembly.GetType(tagRegistration.Namespace + "." + tagName, false, true);

							// Make sure we found it.
							if (type == null) break;

							// We found it, so return it.
							matchingTagRegistration = tagRegistration;
							return type;
						}

					case TagRegistrationKind.HtmlControl:
						// Shouldn't be able to get here.
						throw new InvalidOperationException("Internal error; got an HtmlControl when attempting to process a registered user control or namespace.");
				}
			}

			matchingTagRegistration = null;
			return null;
		}

		/// <summary>
		/// Given a tag with runat="server" on an HTML element, find a suitable HTML server control for it.
		/// </summary>
		/// <param name="tag">The tag to find an HTML server control for.</param>
		/// <returns>The matching HTML server control.</returns>
		private static Type FindMatchingHtmlControlType(Tag tag)
		{
			switch (tag.TagName)
			{
				case "a":			return typeof(System.Web.UI.HtmlControls.HtmlAnchor);
				case "button":		return typeof(System.Web.UI.HtmlControls.HtmlButton);
				case "form":		return typeof(System.Web.UI.HtmlControls.HtmlForm);
				case "head":		return typeof(System.Web.UI.HtmlControls.HtmlHead);
				case "img":			return typeof(System.Web.UI.HtmlControls.HtmlImage);
				case "link":		return typeof(System.Web.UI.HtmlControls.HtmlLink);
				case "meta":		return typeof(System.Web.UI.HtmlControls.HtmlMeta);
				case "select":		return typeof(System.Web.UI.HtmlControls.HtmlSelect);
				case "table":		return typeof(System.Web.UI.HtmlControls.HtmlTable);
				case "td":			return typeof(System.Web.UI.HtmlControls.HtmlTableCell);
				case "th":			return typeof(System.Web.UI.HtmlControls.HtmlTableCell);
				case "tr":			return typeof(System.Web.UI.HtmlControls.HtmlTableRow);
				case "textarea":	return typeof(System.Web.UI.HtmlControls.HtmlTextArea);
				case "title":		return typeof(System.Web.UI.HtmlControls.HtmlTitle);

				case "input":
					string type = tag["type"];
					if (!string.IsNullOrEmpty(type))
					{
						switch (type.ToLower())
						{
							case "button":		return typeof(System.Web.UI.HtmlControls.HtmlInputButton);
							case "checkbox":	return typeof(System.Web.UI.HtmlControls.HtmlInputCheckBox);
							case "file":		return typeof(System.Web.UI.HtmlControls.HtmlInputFile);
							case "hidden":		return typeof(System.Web.UI.HtmlControls.HtmlInputHidden);
							case "image":		return typeof(System.Web.UI.HtmlControls.HtmlInputImage);
							case "password":	return typeof(System.Web.UI.HtmlControls.HtmlInputPassword);
							case "radio":		return typeof(System.Web.UI.HtmlControls.HtmlInputRadioButton);
							case "reset":		return typeof(System.Web.UI.HtmlControls.HtmlInputReset);
							case "submit":		return typeof(System.Web.UI.HtmlControls.HtmlInputSubmit);
							case "text":		return typeof(System.Web.UI.HtmlControls.HtmlInputText);
						}
					}
					return typeof(System.Web.UI.HtmlControls.HtmlGenericControl);

				default:
					return typeof(System.Web.UI.HtmlControls.HtmlGenericControl);
			}
		}

		/// <summary>
		/// Find all of the properties for this control, via reflection.
		/// </summary>
		/// <returns>A dictionary of properties for the given control type.</returns>
		private static Dictionary<string, ReflectedControlProperty> CollectControlProperties(ICompileContext compileContext, Type controlType, ReflectedControl reflectedControl)
		{
			Dictionary<string, ReflectedControlProperty> controlProperties = new Dictionary<string, ReflectedControlProperty>();

			// We have to include NonPublic properties, since internal, public, or protected properties can all be
			// legally referenced from the markup.
			PropertyInfo[] propertyInfos = controlType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				// Find out what kind of getters and setter this property has.  If it has none, or both of them are private, we can't work with it.
				MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
				MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
				bool hasUsableSetMethod = (setMethodInfo != null && (setMethodInfo.IsPublic || setMethodInfo.IsFamilyOrAssembly));
				bool hasUsableGetMethod = (getMethodInfo != null && (getMethodInfo.IsPublic || getMethodInfo.IsFamilyOrAssembly));
				if (!hasUsableSetMethod && !hasUsableGetMethod) continue;

				// We have a public-ish setter.  So add a ReflectedControlProperty instance for this property,
				// since it could be accessible from markup.
				ReflectedControlProperty reflectedControlProperty = new ReflectedControlProperty(reflectedControl, propertyInfo);

				// Add it to the set of known properties for this control.  We don't have the ability to support
				// case differentiation on property names, and ASP.NET will bork if anybody tries.  So if you have
				// multiple properties with the same name that differ only by case, that's just bad mojo, and you
				// should change your controls so that isn't true anymore.
				string lowerName = propertyInfo.Name.ToLower();
				if (controlProperties.ContainsKey(lowerName))
				{
					ReflectedControlProperty previousProperty = controlProperties[lowerName];
					Type previousPropertyDeclaringType = previousProperty.PropertyInfo.DeclaringType;
					PropertyInfo baseProperty = previousPropertyDeclaringType.BaseType.GetProperty(lowerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
					if (baseProperty == null)
					{
						compileContext.Warning(string.Format("The server control \"{0}\" contains multiple properties named \"{1}\".  Keeping the {2} declaration and discarding the {3} declaration.",
							controlType.FullName, propertyInfo.Name, controlProperties[lowerName].PropertyInfo.PropertyType.FullName, propertyInfo.PropertyType.FullName));
					}
					else
					{
						// This appears to be a case of a child class's property shadowing a parent class's property.
						// That's a safe thing, or should be, so we don't throw out a warning when it happens.
						compileContext.Verbose(string.Format("The server control \"{0}\" contains multiple properties named \"{1}\" (but one comes from a parent class, so the 'new' keyword was probably involved, and this should be safe).  Keeping the {2} declaration and discarding the {3} declaration.",
							controlType.FullName, propertyInfo.Name, controlProperties[lowerName].PropertyInfo.PropertyType.FullName, propertyInfo.PropertyType.FullName));
					}
					continue;
				}
				controlProperties.Add(lowerName, reflectedControlProperty);
			}

			return controlProperties;
		}

		#endregion
	}
}
