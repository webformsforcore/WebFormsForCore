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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Redesigner.Library
{
	/// <summary>
	/// Metadata about a single assignable property in a control, constructed via reflection.
	/// </summary>
	public class ReflectedControlProperty
	{
		#region Fields and Properties

		/// <summary>
		/// The control this property belongs to.
		/// </summary>
		public readonly ReflectedControl ReflectedControl;

		/// <summary>
		/// The .NET reflected property info for the actual property on the control's class.
		/// </summary>
		public readonly PropertyInfo PropertyInfo;

		/// <summary>
		/// The [PersistenceMode] attribute on this property, if any.
		/// </summary>
		public readonly System.Web.UI.PersistenceModeAttribute PersistenceModeAttribute;

		/// <summary>
		/// The [TemplateInstance] attribute on this property, if any.
		/// </summary>
		public readonly System.Web.UI.TemplateInstanceAttribute TemplateInstanceAttribute;

		/// <summary>
		/// The [TemplateContainer] attribute on this property, if any.
		/// </summary>
		public readonly System.Web.UI.TemplateContainerAttribute TemplateContainerAttribute;

		/// <summary>
		/// Whether this property is an ITemplate type.
		/// </summary>
		public readonly bool IsTemplateProperty;

		/// <summary>
		/// Whether this property is an IEnumerable type.
		/// </summary>
		public readonly bool IsCollectionProperty;

		/// <summary>
		/// If this property is an IEnumerable type, these are the types of objects that can be added to it via its Add() methods.
		/// </summary>
		public readonly IEnumerable<Type> CollectionItemTypes;

		/// <summary>
		/// The name of this property.
		/// </summary>
		public string PropertyName
		{
			get
			{
				return PropertyInfo.Name;
			}
		}

		/// <summary>
		/// The PersistenceMode of this property, taken from its [PersistenceMode] attribute if it has one, or null if it doesn't.
		/// </summary>
		public System.Web.UI.PersistenceMode? PersistenceMode
		{
			get
			{
				return PersistenceModeAttribute != null ? PersistenceModeAttribute.Mode : (System.Web.UI.PersistenceMode?)null;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Construct an instance of a ReflectedControlProperty, given the control for this property and the
		/// pre-reflected PropertyInfo for the property in question.
		/// </summary>
		/// <param name="reflectedControl">The control that owns this property.</param>
		/// <param name="propertyInfo">The pre-reflected PropertyInfo for this property.</param>
		public ReflectedControlProperty(ReflectedControl reflectedControl, PropertyInfo propertyInfo)
		{
			ReflectedControl = reflectedControl;
			PropertyInfo = propertyInfo;

			System.Web.UI.PersistenceModeAttribute[] persistenceModeAttributes = (System.Web.UI.PersistenceModeAttribute[])propertyInfo.GetCustomAttributes(typeof(System.Web.UI.PersistenceModeAttribute), true);
			PersistenceModeAttribute = persistenceModeAttributes.Length == 0 ? null : persistenceModeAttributes[0];

			IsTemplateProperty = typeof(System.Web.UI.ITemplate).IsAssignableFrom(PropertyInfo.PropertyType);
			IsCollectionProperty = typeof(IEnumerable).IsAssignableFrom(PropertyInfo.PropertyType) && !IsTemplateProperty;

			if (IsTemplateProperty)
			{
				System.Web.UI.TemplateInstanceAttribute[] templateInstanceAttributes = (System.Web.UI.TemplateInstanceAttribute[])propertyInfo.GetCustomAttributes(typeof(System.Web.UI.TemplateInstanceAttribute), true);
				TemplateInstanceAttribute = templateInstanceAttributes.Length == 0 ? null : templateInstanceAttributes[0];

				System.Web.UI.TemplateContainerAttribute[] templateContainerAttributes = (System.Web.UI.TemplateContainerAttribute[])propertyInfo.GetCustomAttributes(typeof(System.Web.UI.TemplateContainerAttribute), true);
				TemplateContainerAttribute = templateContainerAttributes.Length == 0 ? null : templateContainerAttributes[0];
			}
			else if (IsCollectionProperty)
			{
				CollectionItemTypes = GetCollectionItemTypes(PropertyInfo.PropertyType);
			}
		}

		/// <summary>
		/// Determine which types of items may be stored inside the given collection by examining its Add() methods
		/// to see what they accept.
		/// </summary>
		/// <param name="collectionType">The collection type to examine.</param>
		/// <returns>An IEnumerable of all of the different kinds of items that may be added to this collection.</returns>
		private static IEnumerable<Type> GetCollectionItemTypes(Type collectionType)
		{
			MethodInfo[] collectionMethods = collectionType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

			List<Type> collectionItemTypes = new List<Type>();

			// Find all of the Add() methods for this collection that take exactly one parameter.  Those parameter
			// types represent the base classes of the allowed possible types that can be added to this collection.
			foreach (MethodInfo methodInfo in collectionMethods)
			{
				if (string.Compare(methodInfo.Name, "Add", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					ParameterInfo[] parameterInfos = methodInfo.GetParameters();
					if (parameterInfos.Length == 1)
					{
						collectionItemTypes.Add(parameterInfos[0].ParameterType);
					}
				}
			}

			return collectionItemTypes;
		}

		/// <summary>
		/// Convert this property to a string for easier debugging.
		/// </summary>
		/// <returns>A stringified version of this property's metadata (name, type, and persistence mode).</returns>
		public override string ToString()
		{
			return string.Format("[{0}] {1} {2}", PersistenceMode.HasValue ? PersistenceMode.Value.ToString() : "default", PropertyInfo.PropertyType.FullName, PropertyInfo.Name);
		}

		#endregion
	}
}
