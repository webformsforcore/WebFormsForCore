using System;
using System.Reflection;
using System.Security;
using System.Web;

namespace Microsoft.Web.Infrastructure;

[SecurityCritical]
internal static class CommonAssemblies
{
	internal static readonly Assembly MicrosoftWebInfrastructure = typeof(CommonAssemblies).Assembly;
	internal static readonly Assembly mscorlib = typeof(object).Assembly;
	internal static readonly Assembly System = typeof(Uri).Assembly;
	internal static readonly Assembly SystemWeb = typeof(HttpContext).Assembly;
}
