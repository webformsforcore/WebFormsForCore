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

using System.Collections.Generic;

namespace Redesigner.Library
{
	/// <summary>
	/// The complete result of having parsed a .designer.cs file.
	/// </summary>
	public class DesignerInfo
	{
		/// <summary>
		/// The namespace in which the .designer class declaration was found.
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// The classname of the first (and only) class declared in the .designer file.
		/// </summary>
		public string ClassName { get; set; }

		/// <summary>
		/// The full typename of the first (and only) class declared in the .designer file.
		/// </summary>
		public string FullTypeName
		{
			get
			{
				return Namespace + "." + ClassName;
			}
		}

		/// <summary>
		/// The complete list of the properties that were declared in this .designer file.
		/// </summary>
		public List<DesignerPropertyDeclaration> PropertyDeclarations { get { return _propertyDeclarations; } set { _propertyDeclarations = value; } }
		private List<DesignerPropertyDeclaration> _propertyDeclarations = new List<DesignerPropertyDeclaration>();
	}
}
