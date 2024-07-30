
using System.Collections;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides key/value pair configuration information from a configuration section.</summary>
  public class DictionarySectionHandler : IConfigurationSectionHandler
  {
    /// <summary>Creates a new configuration handler and adds it to the section-handler collection based on the specified parameters.</summary>
    /// <param name="parent">Parent object.</param>
    /// <param name="context">Configuration context object.</param>
    /// <param name="section">Section XML node.</param>
    /// <returns>A configuration object.</returns>
    public virtual object Create(object parent, object context, XmlNode section)
    {
      Hashtable hashtable = parent != null ? (Hashtable) ((Hashtable) parent).Clone() : new Hashtable((IEqualityComparer) StringComparer.OrdinalIgnoreCase);
      HandlerBase.CheckForUnrecognizedAttributes(section);
      foreach (XmlNode childNode in section.ChildNodes)
      {
        if (!HandlerBase.IsIgnorableAlsoCheckForNonElement(childNode))
        {
          if (childNode.Name == "add")
          {
            HandlerBase.CheckForChildNodes(childNode);
            string key = HandlerBase.RemoveRequiredAttribute(childNode, this.KeyAttributeName);
            string str = !this.ValueRequired ? HandlerBase.RemoveAttribute(childNode, this.ValueAttributeName) : HandlerBase.RemoveRequiredAttribute(childNode, this.ValueAttributeName);
            HandlerBase.CheckForUnrecognizedAttributes(childNode);
            if (str == null)
              str = "";
            hashtable[(object) key] = (object) str;
          }
          else if (childNode.Name == "remove")
          {
            HandlerBase.CheckForChildNodes(childNode);
            string key = HandlerBase.RemoveRequiredAttribute(childNode, this.KeyAttributeName);
            HandlerBase.CheckForUnrecognizedAttributes(childNode);
            hashtable.Remove((object) key);
          }
          else if (childNode.Name.Equals("clear"))
          {
            HandlerBase.CheckForChildNodes(childNode);
            HandlerBase.CheckForUnrecognizedAttributes(childNode);
            hashtable.Clear();
          }
          else
            HandlerBase.ThrowUnrecognizedElement(childNode);
        }
      }
      return (object) hashtable;
    }

    /// <summary>Gets the XML attribute name to use as the key in a key/value pair.</summary>
    /// <returns>A string value containing the name of the key attribute.</returns>
    protected virtual string KeyAttributeName => "key";

    /// <summary>Gets the XML attribute name to use as the value in a key/value pair.</summary>
    /// <returns>A string value containing the name of the value attribute.</returns>
    protected virtual string ValueAttributeName => "value";

    internal virtual bool ValueRequired => false;
  }
}
