
#nullable disable
namespace System.Configuration
{
  /// <summary>Specifies the special setting category of a application settings property.</summary>
  public enum SpecialSetting
  {
    /// <summary>The configuration property represents a connection string, typically for a data store or network resource.</summary>
    ConnectionString,
    /// <summary>The configuration property represents a Uniform Resource Locator (URL) to a Web service.</summary>
    WebServiceUrl,
  }
}
