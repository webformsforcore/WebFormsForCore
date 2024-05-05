// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.ConnectionManagementElementCollection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Represents a container for connection management configuration elements. This class cannot be inherited.</summary>
  [ConfigurationCollection(typeof (ConnectionManagementElement))]
  public sealed class ConnectionManagementElementCollection : ConfigurationElementCollection
  {
    /// <summary>Gets or sets the element at the specified position in the collection.</summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <returns>The <see cref="T:System.Net.Configuration.ConnectionManagementElement" /> at the specified location.</returns>
    public ConnectionManagementElement this[int index]
    {
      get => (ConnectionManagementElement) this.BaseGet(index);
      set
      {
        if (this.BaseGet(index) != null)
          this.BaseRemoveAt(index);
        this.BaseAdd(index, (ConfigurationElement) value);
      }
    }

    /// <summary>Gets or sets the element with the specified key.</summary>
    /// <param name="name">The key for an element in the collection.</param>
    /// <returns>The <see cref="T:System.Net.Configuration.ConnectionManagementElement" /> with the specified key or <see langword="null" /> if there is no element with the specified key.</returns>
    public ConnectionManagementElement this[string name]
    {
      get => (ConnectionManagementElement) this.BaseGet((object) name);
      set
      {
        if (this.BaseGet((object) name) != null)
          this.BaseRemove((object) name);
        this.BaseAdd((ConfigurationElement) value);
      }
    }

    /// <summary>Adds an element to the collection.</summary>
    /// <param name="element">The <see cref="T:System.Net.Configuration.ConnectionManagementElement" /> to add to the collection.</param>
    public void Add(ConnectionManagementElement element)
    {
      this.BaseAdd((ConfigurationElement) element);
    }

    /// <summary>Removes all elements from the collection.</summary>
    public void Clear() => this.BaseClear();

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new ConnectionManagementElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return element != null ? (object) ((ConnectionManagementElement) element).Key : throw new ArgumentNullException(nameof (element));
    }

    /// <summary>Returns the index of the specified configuration element.</summary>
    /// <param name="element">A <see cref="T:System.Net.Configuration.ConnectionManagementElement" />.</param>
    /// <returns>The zero-based index of <paramref name="element" />.</returns>
    public int IndexOf(ConnectionManagementElement element)
    {
      return this.BaseIndexOf((ConfigurationElement) element);
    }

    /// <summary>Removes the specified configuration element from the collection.</summary>
    /// <param name="element">The <see cref="T:System.Net.Configuration.ConnectionManagementElement" /> to remove.</param>
    public void Remove(ConnectionManagementElement element)
    {
      if (element == null)
        throw new ArgumentNullException(nameof (element));
      this.BaseRemove((object) element.Key);
    }

    /// <summary>Removes the element with the specified key.</summary>
    /// <param name="name">The key of the element to remove.</param>
    public void Remove(string name) => this.BaseRemove((object) name);

    /// <summary>Removes the element at the specified index.</summary>
    /// <param name="index">The zero-based index of the element to remove.</param>
    public void RemoveAt(int index) => this.BaseRemoveAt(index);
  }
}
