
using System.Configuration;

#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Represents a container for Web request module configuration elements. This class cannot be inherited.</summary>
  [ConfigurationCollection(typeof (WebRequestModuleElement))]
  public sealed class WebRequestModuleElementCollection : ConfigurationElementCollection
  {
    /// <summary>Gets or sets the element at the specified position in the collection.</summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <returns>The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> at the specified location.</returns>
    public WebRequestModuleElement this[int index]
    {
      get => (WebRequestModuleElement) this.BaseGet(index);
      set
      {
        if (this.BaseGet(index) != null)
          this.BaseRemoveAt(index);
        this.BaseAdd(index, (ConfigurationElement) value);
      }
    }

    /// <summary>Gets or sets the element with the specified key.</summary>
    /// <param name="name">The key for an element in the collection.</param>
    /// <returns>The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> with the specified key or <see langword="null" /> if there is no element with the specified key.</returns>
    public WebRequestModuleElement this[string name]
    {
      get => (WebRequestModuleElement) this.BaseGet((object) name);
      set
      {
        if (this.BaseGet((object) name) != null)
          this.BaseRemove((object) name);
        this.BaseAdd((ConfigurationElement) value);
      }
    }

    /// <summary>Adds an element to the collection.</summary>
    /// <param name="element">The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> to add to the collection.</param>
    public void Add(WebRequestModuleElement element)
    {
      this.BaseAdd((ConfigurationElement) element);
    }

    /// <summary>Removes all elements from the collection.</summary>
    public void Clear() => this.BaseClear();

    protected override ConfigurationElement CreateNewElement()
    {
      return (ConfigurationElement) new WebRequestModuleElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return element != null ? (object) ((WebRequestModuleElement) element).Key : throw new ArgumentNullException(nameof (element));
    }

    /// <summary>Returns the index of the specified configuration element.</summary>
    /// <param name="element">A <see cref="T:System.Net.Configuration.WebRequestModuleElement" />.</param>
    /// <returns>The zero-based index of <paramref name="element" />.</returns>
    public int IndexOf(WebRequestModuleElement element)
    {
      return this.BaseIndexOf((ConfigurationElement) element);
    }

    /// <summary>Removes the specified configuration element from the collection.</summary>
    /// <param name="element">The <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> to remove.</param>
    public void Remove(WebRequestModuleElement element)
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
