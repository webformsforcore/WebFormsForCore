#if NETCOREAPP && false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web
{
	public class ToolboxBitmapAttribute: Attribute
	{
		public ToolboxBitmapAttribute(string imageFile) { }
		public ToolboxBitmapAttribute(Type t) { }
		public ToolboxBitmapAttribute(Type t, string name) { }
	}
}
#endif