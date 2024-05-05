// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsPropertyCollection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections;

#nullable disable
namespace System.Configuration
{
  /// <summary>Contains a collection of <see cref="T:System.Configuration.SettingsProperty" /> objects.</summary>
  public class SettingsPropertyCollection : IEnumerable, ICloneable, ICollection
  {
    private Hashtable _Hashtable;
    private bool _ReadOnly;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> class.</summary>
    public SettingsPropertyCollection()
    {
      this._Hashtable = new Hashtable(10, (IEqualityComparer) StringComparer.CurrentCultureIgnoreCase);
    }

    /// <summary>Adds a <see cref="T:System.Configuration.SettingsProperty" /> object to the collection.</summary>
    /// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
    public void Add(SettingsProperty property)
    {
      if (this._ReadOnly)
        throw new NotSupportedException();
      this.OnAdd(property);
      this._Hashtable.Add((object) property.Name, (object) property);
      try
      {
        this.OnAddComplete(property);
      }
      catch
      {
        this._Hashtable.Remove((object) property.Name);
        throw;
      }
    }

    /// <summary>Removes a <see cref="T:System.Configuration.SettingsProperty" /> object from the collection.</summary>
    /// <param name="name">The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
    public void Remove(string name)
    {
      if (this._ReadOnly)
        throw new NotSupportedException();
      SettingsProperty property = (SettingsProperty) this._Hashtable[(object) name];
      if (property == null)
        return;
      this.OnRemove(property);
      this._Hashtable.Remove((object) name);
      try
      {
        this.OnRemoveComplete(property);
      }
      catch
      {
        this._Hashtable.Add((object) name, (object) property);
        throw;
      }
    }

    /// <summary>Gets the collection item with the specified name.</summary>
    /// <param name="name">The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    /// <returns>The <see cref="T:System.Configuration.SettingsProperty" /> object with the specified <paramref name="name" />.</returns>
    public SettingsProperty this[string name] => this._Hashtable[(object) name] as SettingsProperty;

    /// <summary>Gets the <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection.</summary>
    /// <returns>The <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection.</returns>
    public IEnumerator GetEnumerator() => this._Hashtable.Values.GetEnumerator();

    /// <summary>Creates a copy of the existing collection.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsPropertyCollection" /> class.</returns>
    public object Clone() => (object) new SettingsPropertyCollection(this._Hashtable);

    /// <summary>Sets the collection to be read-only.</summary>
    public void SetReadOnly()
    {
      if (this._ReadOnly)
        return;
      this._ReadOnly = true;
    }

    /// <summary>Removes all <see cref="T:System.Configuration.SettingsProperty" /> objects from the collection.</summary>
    /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
    public void Clear()
    {
      if (this._ReadOnly)
        throw new NotSupportedException();
      this.OnClear();
      this._Hashtable.Clear();
      this.OnClearComplete();
    }

    /// <summary>Performs additional, custom processing when adding to the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
    /// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    protected virtual void OnAdd(SettingsProperty property)
    {
    }

    /// <summary>Performs additional, custom processing after adding to the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
    /// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    protected virtual void OnAddComplete(SettingsProperty property)
    {
    }

    /// <summary>Performs additional, custom processing when clearing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
    protected virtual void OnClear()
    {
    }

    /// <summary>Performs additional, custom processing after clearing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
    protected virtual void OnClearComplete()
    {
    }

    /// <summary>Performs additional, custom processing when removing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
    /// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    protected virtual void OnRemove(SettingsProperty property)
    {
    }

    /// <summary>Performs additional, custom processing after removing the contents of the <see cref="T:System.Configuration.SettingsPropertyCollection" /> instance.</summary>
    /// <param name="property">A <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    protected virtual void OnRemoveComplete(SettingsProperty property)
    {
    }

    /// <summary>Gets a value that specifies the number of <see cref="T:System.Configuration.SettingsProperty" /> objects in the collection.</summary>
    /// <returns>The number of <see cref="T:System.Configuration.SettingsProperty" /> objects in the collection.</returns>
    public int Count => this._Hashtable.Count;

    /// <summary>Gets a value that indicates whether access to the collection is synchronized (thread safe).</summary>
    /// <returns>
    /// <see langword="true" /> if access to the <see cref="T:System.Configuration.SettingsPropertyCollection" /> is synchronized; otherwise, <see langword="false" />.</returns>
    public bool IsSynchronized => false;

    /// <summary>Gets the object to synchronize access to the collection.</summary>
    /// <returns>The object to synchronize access to the collection.</returns>
    public object SyncRoot => (object) this;

    /// <summary>Copies this <see cref="T:System.Configuration.SettingsPropertyCollection" /> object to an array.</summary>
    /// <param name="array">The array to copy the object to.</param>
    /// <param name="index">The index at which to begin copying.</param>
    public void CopyTo(Array array, int index) => this._Hashtable.Values.CopyTo(array, index);

    private SettingsPropertyCollection(Hashtable h) => this._Hashtable = (Hashtable) h.Clone();
  }
}
