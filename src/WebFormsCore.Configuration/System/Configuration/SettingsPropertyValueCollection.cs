// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsPropertyValueCollection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections;

#nullable disable
namespace System.Configuration
{
  /// <summary>Contains a collection of settings property values that map <see cref="T:System.Configuration.SettingsProperty" /> objects to <see cref="T:System.Configuration.SettingsPropertyValue" /> objects.</summary>
  public class SettingsPropertyValueCollection : IEnumerable, ICloneable, ICollection
  {
    private Hashtable _Indices;
    private ArrayList _Values;
    private bool _ReadOnly;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> class.</summary>
    public SettingsPropertyValueCollection()
    {
      this._Indices = new Hashtable(10, (IEqualityComparer) StringComparer.CurrentCultureIgnoreCase);
      this._Values = new ArrayList();
    }

    /// <summary>Adds a <see cref="T:System.Configuration.SettingsPropertyValue" /> object to the collection.</summary>
    /// <param name="property">A <see cref="T:System.Configuration.SettingsPropertyValue" /> object.</param>
    /// <exception cref="T:System.NotSupportedException">An attempt was made to add an item to the collection, but the collection was marked as read-only.</exception>
    public void Add(SettingsPropertyValue property)
    {
      if (this._ReadOnly)
        throw new NotSupportedException();
      int index = this._Values.Add((object) property);
      try
      {
        this._Indices.Add((object) property.Name, (object) index);
      }
      catch (Exception ex)
      {
        this._Values.RemoveAt(index);
        throw;
      }
    }

    /// <summary>Removes a <see cref="T:System.Configuration.SettingsPropertyValue" /> object from the collection.</summary>
    /// <param name="name">The name of the <see cref="T:System.Configuration.SettingsPropertyValue" /> object.</param>
    /// <exception cref="T:System.NotSupportedException">An attempt was made to remove an item from the collection, but the collection was marked as read-only.</exception>
    public void Remove(string name)
    {
      if (this._ReadOnly)
        throw new NotSupportedException();
      object index1 = this._Indices[(object) name];
      if (index1 == null || !(index1 is int index2) || index2 >= this._Values.Count)
        return;
      this._Values.RemoveAt(index2);
      this._Indices.Remove((object) name);
      ArrayList arrayList = new ArrayList();
      foreach (DictionaryEntry index3 in this._Indices)
      {
        if ((int) index3.Value > index2)
          arrayList.Add(index3.Key);
      }
      foreach (string key in arrayList)
        this._Indices[(object) key] = (object) ((int) this._Indices[(object) key] - 1);
    }

    /// <summary>Gets an item from the collection.</summary>
    /// <param name="name">A <see cref="T:System.Configuration.SettingsPropertyValue" /> object.</param>
    /// <returns>The <see cref="T:System.Configuration.SettingsPropertyValue" /> object with the specified <paramref name="name" />.</returns>
    public SettingsPropertyValue this[string name]
    {
      get
      {
        object index1 = this._Indices[(object) name];
        if (index1 == null || !(index1 is int index2))
          return (SettingsPropertyValue) null;
        return index2 >= this._Values.Count ? (SettingsPropertyValue) null : (SettingsPropertyValue) this._Values[index2];
      }
    }

    /// <summary>Gets the <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection.</summary>
    /// <returns>The <see cref="T:System.Collections.IEnumerator" /> object as it applies to the collection.</returns>
    public IEnumerator GetEnumerator() => this._Values.GetEnumerator();

    /// <summary>Creates a copy of the existing collection.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> class.</returns>
    public object Clone()
    {
      return (object) new SettingsPropertyValueCollection(this._Indices, this._Values);
    }

    /// <summary>Sets the collection to be read-only.</summary>
    public void SetReadOnly()
    {
      if (this._ReadOnly)
        return;
      this._ReadOnly = true;
      this._Values = ArrayList.ReadOnly(this._Values);
    }

    /// <summary>Removes all <see cref="T:System.Configuration.SettingsPropertyValue" /> objects from the collection.</summary>
    public void Clear()
    {
      this._Values.Clear();
      this._Indices.Clear();
    }

    /// <summary>Gets a value that specifies the number of <see cref="T:System.Configuration.SettingsPropertyValue" /> objects in the collection.</summary>
    /// <returns>The number of <see cref="T:System.Configuration.SettingsPropertyValue" /> objects in the collection.</returns>
    public int Count => this._Values.Count;

    /// <summary>Gets a value that indicates whether access to the collection is synchronized (thread safe).</summary>
    /// <returns>
    /// <see langword="true" /> if access to the <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> collection is synchronized; otherwise, <see langword="false" />.</returns>
    public bool IsSynchronized => false;

    /// <summary>Gets the object to synchronize access to the collection.</summary>
    /// <returns>The object to synchronize access to the collection.</returns>
    public object SyncRoot => (object) this;

    /// <summary>Copies this <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> collection to an array.</summary>
    /// <param name="array">The array to copy the collection to.</param>
    /// <param name="index">The index at which to begin copying.</param>
    public void CopyTo(Array array, int index) => this._Values.CopyTo(array, index);

    private SettingsPropertyValueCollection(Hashtable indices, ArrayList values)
    {
      this._Indices = (Hashtable) indices.Clone();
      this._Values = (ArrayList) values.Clone();
    }
  }
}
