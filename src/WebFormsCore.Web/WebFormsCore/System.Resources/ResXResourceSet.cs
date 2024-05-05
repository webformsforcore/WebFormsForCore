#if !NETFRAMEWORK

using System.Collections;
using System.IO;
using System.Security.Permissions;

#nullable disable
namespace System.Resources
{
	/// <summary>Represents all resources in an XML resource (.resx) file.</summary>
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class ResXResourceSet : ResourceSet
	{
		/// <summary>Initializes a new instance of a <see cref="T:System.Resources.ResXResourceSet" /> class using the system default <see cref="T:System.Resources.ResXResourceReader" /> that opens and reads resources from the specified file.</summary>
		/// <param name="fileName">The name of the file to read resources from.</param>
		public ResXResourceSet(string fileName) : base(new ResXResourceReader(fileName)) { }

		/// <summary>Initializes a new instance of the <see cref="T:System.Resources.ResXResourceSet" /> class using the system default <see cref="T:System.Resources.ResXResourceReader" /> to read resources from the specified stream.</summary>
		/// <param name="stream">The <see cref="T:System.IO.Stream" /> of resources to be read. The stream should refer to an existing resource file.</param>
		public ResXResourceSet(Stream stream) : base(new ResXResourceReader(stream)) { }

		/// <summary>Returns the preferred resource reader class for this kind of <see cref="T:System.Resources.ResXResourceSet" />.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the preferred resource reader for this kind of <see cref="T:System.Resources.ResXResourceSet" />.</returns>
		public override Type GetDefaultReader() => typeof(ResXResourceReader);

		/// <summary>Returns the preferred resource writer class for this kind of <see cref="T:System.Resources.ResXResourceSet" />.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the preferred resource writer for this kind of <see cref="T:System.Resources.ResXResourceSet" />.</returns>
		public override Type GetDefaultWriter() => typeof(ResXResourceWriter);
	}
}
#endif