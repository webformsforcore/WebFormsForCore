// Decompiled with JetBrains decompiler
// Type: System.Web.Hosting.CustomLoaderAttribute
// Assembly: System.Web.ApplicationServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 49FC561C-A827-422E-A5C7-EDE4066C7817
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.ApplicationServices\v4.0_4.0.0.0__31bf3856ad364e35\System.Web.ApplicationServices.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.ApplicationServices.xml

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
