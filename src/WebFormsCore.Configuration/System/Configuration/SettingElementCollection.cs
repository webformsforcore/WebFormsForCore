// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingElementCollection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Contains a collection of <see cref="T:System.Configuration.SettingElement" /> objects. This class cannot be inherited.</summary>
  public sealed class SettingElementCollection : ConfigurationElementCollection
  {
    /// <summary>Gets the type of the configuration collection.</summary>
    /// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> object of the collection.</returns>
    public override ConfigurationElementCollectionType CollectionType
    {
      get => ConfigurationElementCollectionType.BasicMap;
    }

    protected override string ElementName => "setting";

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new SettingElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) ((SettingElement) element).Key;
    }

    /// <summary>Gets a <see cref="T:System.Configuration.SettingElement" /> object from the collection.</summary>
    /// <param name="elementKey">A string value representing the <see cref="T:System.Configuration.SettingElement" /> object in the collection.</param>
    /// <returns>A <see cref="T:System.Configuration.SettingElement" /> object.</returns>
    public SettingElement Get(string elementKey)
    {
      return (SettingElement) this.BaseGet((object) elementKey);
    }

    /// <summary>Adds a <see cref="T:System.Configuration.SettingElement" /> object to the collection.</summary>
    /// <param name="element">The <see cref="T:System.Configuration.SettingElement" /> object to add to the collection.</param>
    public void Add(SettingElement element) => this.BaseAdd((ConfigurationElement) element);

    /// <summary>Removes a <see cref="T:System.Configuration.SettingElement" /> object from the collection.</summary>
    /// <param name="element">A <see cref="T:System.Configuration.SettingElement" /> object.</param>
    public void Remove(SettingElement element)
    {
      this.BaseRemove(this.GetElementKey((ConfigurationElement) element));
    }

    /// <summary>Removes all <see cref="T:System.Configuration.SettingElement" /> objects from the collection.</summary>
    public void Clear() => this.BaseClear();
  }
}
