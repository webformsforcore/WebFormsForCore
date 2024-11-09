#nullable disable
namespace System.Web.Hosting
{
  /// <summary>Provides a custom loader to ASP.NET so that an application can provide its own implementation of the hosting environment.</summary>
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
  public sealed class CustomLoaderAttribute : Attribute
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Web.Hosting.CustomLoaderAttribute" /> class.</summary>
    /// <param name="customLoaderType">The type of the custom loader.</param>
    public CustomLoaderAttribute(Type customLoaderType)
    {
      this.CustomLoaderType = !(customLoaderType == (Type) null) ? customLoaderType : throw new ArgumentNullException(nameof (customLoaderType));
    }

    /// <summary>Gets the type of the custom loader.</summary>
    /// <returns>The type of the custom loader.</returns>
    public Type CustomLoaderType { get; private set; }
  }
}
