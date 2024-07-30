
using System.Globalization;
using System.Reflection;
using System.Configuration;

#nullable disable
namespace System.Net
{
  internal class WebRequestPrefixElement
  {
    public string Prefix;
    internal IWebRequestCreate creator;
    internal Type creatorType;

    public IWebRequestCreate Creator
    {
      get
      {
        if (this.creator == null && this.creatorType != (Type) null)
        {
          lock (this)
          {
            if (this.creator == null)
              this.creator = (IWebRequestCreate) Activator.CreateInstance(this.creatorType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, (Binder) null, new object[0], CultureInfo.InvariantCulture);
          }
        }
        return this.creator;
      }
      set => this.creator = value;
    }

    public WebRequestPrefixElement(string P, Type creatorType)
    {
      if (!typeof (IWebRequestCreate).IsAssignableFrom(creatorType))
        throw new InvalidCastException(SR.GetString("net_invalid_cast", (object) creatorType.AssemblyQualifiedName, (object) "IWebRequestCreate"));
      this.Prefix = P;
      this.creatorType = creatorType;
    }

    public WebRequestPrefixElement(string P, IWebRequestCreate C)
    {
      this.Prefix = P;
      this.Creator = C;
    }
  }
}
