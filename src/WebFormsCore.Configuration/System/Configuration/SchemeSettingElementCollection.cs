// Decompiled with JetBrains decompiler
// Type: System.Configuration.SchemeSettingElementCollection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Represents a collection of <see cref="T:System.Configuration.SchemeSettingElement" /> objects.</summary>
  [ConfigurationCollection(typeof (SchemeSettingElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap, AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
  public sealed class SchemeSettingElementCollection : ConfigurationElementCollection
  {
    internal const string AddItemName = "add";
    internal const string ClearItemsName = "clear";
    internal const string RemoveItemName = "remove";

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SchemeSettingElementCollection" /> class.</summary>
    public SchemeSettingElementCollection()
    {
      this.AddElementName = "add";
      this.ClearElementName = "clear";
      this.RemoveElementName = "remove";
    }

    /// <summary>Gets the default collection type of <see cref="T:System.Configuration.SchemeSettingElementCollection" />.</summary>
    /// <returns>The default collection type of <see cref="T:System.Configuration.SchemeSettingElementCollection" />.</returns>
    public override ConfigurationElementCollectionType CollectionType
    {
      get => ConfigurationElementCollectionType.AddRemoveClearMap;
    }

    /// <summary>Gets an item at the specified index in the <see cref="T:System.Configuration.SchemeSettingElementCollection" /> collection.</summary>
    /// <param name="index">The index of the <see cref="T:System.Configuration.SchemeSettingElement" /> to return.</param>
    /// <returns>The specified <see cref="T:System.Configuration.SchemeSettingElement" />.</returns>
    /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The <paramref name="index" /> parameter is less than zero.
    /// -or-
    /// The item specified by the parameter is <see langword="null" /> or has been removed.</exception>
    public SchemeSettingElement this[int index] => (SchemeSettingElement) this.BaseGet(index);

    /// <summary>Gets an item from the <see cref="T:System.Configuration.SchemeSettingElementCollection" /> collection.</summary>
    /// <param name="name">A string reference to the <see cref="T:System.Configuration.SchemeSettingElement" /> object within the collection.</param>
    /// <returns>A <see cref="T:System.Configuration.SchemeSettingElement" /> object contained in the collection.</returns>
    public SchemeSettingElement this[string name]
    {
      get => (SchemeSettingElement) this.BaseGet((object) name);
    }

    /// <summary>The index of the specified <see cref="T:System.Configuration.SchemeSettingElement" />.</summary>
    /// <param name="element">The <see cref="T:System.Configuration.SchemeSettingElement" /> for the specified index location.</param>
    /// <returns>The index of the specified <see cref="T:System.Configuration.SchemeSettingElement" />; otherwise, -1.</returns>
    public int IndexOf(SchemeSettingElement element)
    {
      return this.BaseIndexOf((ConfigurationElement) element);
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new SchemeSettingElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return (object) ((SchemeSettingElement) element).Name;
    }
  }
}
