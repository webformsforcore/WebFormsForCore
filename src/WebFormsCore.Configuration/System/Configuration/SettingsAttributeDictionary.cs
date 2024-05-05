// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsAttributeDictionary
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections;

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents a collection of key/value pairs used to describe a configuration object as well as a <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
  [Serializable]
  public class SettingsAttributeDictionary : Hashtable
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsAttributeDictionary" /> class.</summary>
    public SettingsAttributeDictionary()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsAttributeDictionary" /> class.</summary>
    /// <param name="attributes">A collection of key/value pairs that are related to configuration settings.</param>
    public SettingsAttributeDictionary(SettingsAttributeDictionary attributes)
      : base((IDictionary) attributes)
    {
    }
  }
}
