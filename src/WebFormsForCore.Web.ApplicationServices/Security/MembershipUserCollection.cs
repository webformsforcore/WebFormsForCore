using System.Collections;
using System.Runtime.CompilerServices;

#nullable disable
namespace System.Web.Security
{
  /// <summary>A collection of <see cref="T:System.Web.Security.MembershipUser" /> objects.</summary>
  [TypeForwardedFrom("System.Web, Version=2.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class MembershipUserCollection : IEnumerable, ICollection
  {
    private Hashtable _Indices;
    private ArrayList _Values;
    private bool _ReadOnly;

    /// <summary>Creates a new, empty membership user collection.</summary>
    public MembershipUserCollection()
    {
      this._Indices = new Hashtable(10, (IEqualityComparer) StringComparer.CurrentCultureIgnoreCase);
      this._Values = new ArrayList();
    }

    /// <summary>Adds the specified membership user to the collection.</summary>
    /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser" /> object to add to the collection.</param>
    /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="T:System.ArgumentNullException">The <see cref="P:System.Web.Security.MembershipUser.UserName" /> of the <paramref name="user" /> is null.</exception>
    /// <exception cref="T:System.ArgumentException">A <see cref="T:System.Web.Security.MembershipUser" /> object with the same <see cref="P:System.Web.Security.MembershipUser.UserName" /> value as <paramref name="user" /> already exists in the collection.</exception>
    public void Add(MembershipUser user)
    {
      if (user == null)
        throw new ArgumentNullException(nameof (user));
      if (this._ReadOnly)
        throw new NotSupportedException();
      int index = this._Values.Add((object) user);
      try
      {
        this._Indices.Add((object) user.UserName, (object) index);
      }
      catch
      {
        this._Values.RemoveAt(index);
        throw;
      }
    }

    /// <summary>Removes the membership user object with the specified user name from the collection.</summary>
    /// <param name="name">The user name of the <see cref="T:System.Web.Security.MembershipUser" /> object to remove from the collection.</param>
    /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
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

    /// <summary>Gets the membership user in the collection referenced by the specified user name.</summary>
    /// <param name="name">The <see cref="P:System.Web.Security.MembershipUser.UserName" /> of the <see cref="T:System.Web.Security.MembershipUser" /> to retrieve from the collection.</param>
    /// <returns>A <see cref="T:System.Web.Security.MembershipUser" /> object representing the user specified by <paramref name="name" />.</returns>
    public MembershipUser this[string name]
    {
      get
      {
        object index1 = this._Indices[(object) name];
        if (index1 == null || !(index1 is int index2))
          return (MembershipUser) null;
        return index2 >= this._Values.Count ? (MembershipUser) null : (MembershipUser) this._Values[index2];
      }
    }

    /// <summary>Gets an enumerator that can iterate through the membership user collection.</summary>
    /// <returns>An <see cref="T:System.Collections.IEnumerator" /> for the entire <see cref="T:System.Web.Security.MembershipUserCollection" />.</returns>
    public IEnumerator GetEnumerator() => this._Values.GetEnumerator();

    /// <summary>Makes the contents of the membership user collection read-only.</summary>
    public void SetReadOnly()
    {
      if (this._ReadOnly)
        return;
      this._ReadOnly = true;
      this._Values = ArrayList.ReadOnly(this._Values);
    }

    /// <summary>Removes all membership user objects from the collection.</summary>
    public void Clear()
    {
      this._Values.Clear();
      this._Indices.Clear();
    }

    /// <summary>Gets the number of membership user objects in the collection.</summary>
    /// <returns>The number of <see cref="T:System.Web.Security.MembershipUser" /> objects in the collection.</returns>
    public int Count => this._Values.Count;

    /// <summary>Gets a value indicating whether the membership user collection is thread safe.</summary>
    /// <returns>Always <see langword="false" /> because thread-safe membership user collections are not supported.</returns>
    public bool IsSynchronized => false;

    /// <summary>Gets the synchronization root.</summary>
    /// <returns>Always <see langword="this" />, because synchronization of membership user collections is not supported.</returns>
    public object SyncRoot => (object) this;

    /// <summary>Copies the contents of the <see cref="T:System.Web.Security.MembershipUserCollection" /> object to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
    /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination for the objects copied from the <see cref="T:System.Web.Security.MembershipUserCollection" /> object. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="array" /> is <see langword="null" />.</exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">
    /// <paramref name="index" /> is less than 0.</exception>
    /// <exception cref="T:System.ArgumentException">
    ///         <paramref name="array" /> is multidimensional.
    /// -or-
    /// <paramref name="index" /> is greater than or equal to the length of <paramref name="array" />.
    /// -or-
    /// The number of elements in the source <see cref="T:System.Web.Security.MembershipUserCollection" /> is greater than the available space from <paramref name="index" /> to the end of the destination array.</exception>
    /// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Web.Security.MembershipUserCollection" /> cannot be cast automatically to the type of the destination array.</exception>
    void ICollection.CopyTo(Array array, int index) => this._Values.CopyTo(array, index);

    /// <summary>Copies the membership user collection to a one-dimensional array.</summary>
    /// <param name="array">A one-dimensional array of type <see cref="T:System.Web.Security.MembershipUser" /> that is the destination of the elements copied from the <see cref="T:System.Web.Security.MembershipUserCollection" />. The array must have zero-based indexing.</param>
    /// <param name="index">The zero-based index in the array at which copying begins.</param>
    public void CopyTo(MembershipUser[] array, int index)
    {
      this._Values.CopyTo((Array) array, index);
    }
  }
}
