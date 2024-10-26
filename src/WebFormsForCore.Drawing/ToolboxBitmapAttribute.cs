using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Drawing;

/// <summary>
/// ToolboxBitmapAttribute defines the images associated with a specified component.
/// The component can offer a small and large image (large is optional).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ToolboxBitmapAttribute : Attribute
{

	private ToolboxBitmapAttribute() { }
	public ToolboxBitmapAttribute(string imageFile) { }
	public ToolboxBitmapAttribute(Type t) { }

	public ToolboxBitmapAttribute(Type t, string name) { }

	public static readonly ToolboxBitmapAttribute Default = new();

}