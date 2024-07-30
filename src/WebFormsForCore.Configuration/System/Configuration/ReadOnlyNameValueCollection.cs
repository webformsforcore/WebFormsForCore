
using System.Collections;
using System.Collections.Specialized;

#nullable disable
namespace System.Configuration
{
  internal class ReadOnlyNameValueCollection : NameValueCollection
  {
    internal ReadOnlyNameValueCollection(IEqualityComparer equalityComparer)
      : base(equalityComparer)
    {
    }

    internal ReadOnlyNameValueCollection(ReadOnlyNameValueCollection value)
      : base((NameValueCollection) value)
    {
    }

    internal void SetReadOnly() => this.IsReadOnly = true;
  }
}
