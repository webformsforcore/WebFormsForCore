// Decompiled with JetBrains decompiler
// Type: System.Configuration.IConfigurationSystem
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Runtime.InteropServices;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides standard configuration methods.</summary>
  [ComVisible(false)]
  public interface IConfigurationSystem
  {
    /// <summary>Gets the specified configuration.</summary>
    /// <param name="configKey">The configuration key.</param>
    /// <returns>The object representing the configuration.</returns>
    object GetConfig(string configKey);

    /// <summary>Used for initialization.</summary>
    void Init();
  }
}
