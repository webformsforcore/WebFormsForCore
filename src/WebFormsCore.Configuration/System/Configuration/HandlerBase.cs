
using System.Globalization;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal class HandlerBase
  {
    private HandlerBase()
    {
    }

    private static XmlNode GetAndRemoveAttribute(XmlNode node, string attrib, bool fRequired)
    {
      XmlNode xmlNode = node.Attributes.RemoveNamedItem(attrib);
      return !fRequired || xmlNode != null ? xmlNode : throw new ConfigurationErrorsException(SR.GetString("Config_missing_required_attribute", (object) attrib, (object) node.Name), node);
    }

    private static XmlNode GetAndRemoveStringAttributeInternal(
      XmlNode node,
      string attrib,
      bool fRequired,
      ref string val)
    {
      XmlNode andRemoveAttribute = HandlerBase.GetAndRemoveAttribute(node, attrib, fRequired);
      if (andRemoveAttribute != null)
        val = andRemoveAttribute.Value;
      return andRemoveAttribute;
    }

    internal static XmlNode GetAndRemoveStringAttribute(
      XmlNode node,
      string attrib,
      ref string val)
    {
      return HandlerBase.GetAndRemoveStringAttributeInternal(node, attrib, false, ref val);
    }

    private static XmlNode GetAndRemoveBooleanAttributeInternal(
      XmlNode node,
      string attrib,
      bool fRequired,
      ref bool val)
    {
      XmlNode andRemoveAttribute = HandlerBase.GetAndRemoveAttribute(node, attrib, fRequired);
      if (andRemoveAttribute != null)
      {
        try
        {
          val = bool.Parse(andRemoveAttribute.Value);
        }
        catch (Exception ex)
        {
          throw new ConfigurationErrorsException(SR.GetString(SR.GetString("Config_invalid_boolean_attribute", (object) andRemoveAttribute.Name)), ex, andRemoveAttribute);
        }
      }
      return andRemoveAttribute;
    }

    internal static XmlNode GetAndRemoveBooleanAttribute(XmlNode node, string attrib, ref bool val)
    {
      return HandlerBase.GetAndRemoveBooleanAttributeInternal(node, attrib, false, ref val);
    }

    private static XmlNode GetAndRemoveIntegerAttributeInternal(
      XmlNode node,
      string attrib,
      bool fRequired,
      ref int val)
    {
      XmlNode andRemoveAttribute = HandlerBase.GetAndRemoveAttribute(node, attrib, fRequired);
      if (andRemoveAttribute != null)
      {
        if (andRemoveAttribute.Value.Trim() != andRemoveAttribute.Value)
          throw new ConfigurationErrorsException(SR.GetString("Config_invalid_integer_attribute", (object) andRemoveAttribute.Name), andRemoveAttribute);
        try
        {
          val = int.Parse(andRemoveAttribute.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch (Exception ex)
        {
          throw new ConfigurationErrorsException(SR.GetString("Config_invalid_integer_attribute", (object) andRemoveAttribute.Name), ex, andRemoveAttribute);
        }
      }
      return andRemoveAttribute;
    }

    internal static XmlNode GetAndRemoveIntegerAttribute(XmlNode node, string attrib, ref int val)
    {
      return HandlerBase.GetAndRemoveIntegerAttributeInternal(node, attrib, false, ref val);
    }

    internal static void CheckForUnrecognizedAttributes(XmlNode node)
    {
      if (node.Attributes.Count != 0)
        throw new ConfigurationErrorsException(SR.GetString("Config_base_unrecognized_attribute", (object) node.Attributes[0].Name), node);
    }

    internal static string RemoveAttribute(XmlNode node, string name)
    {
      return node.Attributes.RemoveNamedItem(name)?.Value;
    }

    internal static string RemoveRequiredAttribute(XmlNode node, string name)
    {
      return HandlerBase.RemoveRequiredAttribute(node, name, false);
    }

    internal static string RemoveRequiredAttribute(XmlNode node, string name, bool allowEmpty)
    {
      XmlNode xmlNode = node.Attributes.RemoveNamedItem(name);
      if (xmlNode == null)
        throw new ConfigurationErrorsException(SR.GetString("Config_base_required_attribute_missing", (object) name), node);
      if (string.IsNullOrEmpty(xmlNode.Value) && !allowEmpty)
        throw new ConfigurationErrorsException(SR.GetString("Config_base_required_attribute_empty", (object) name), node);
      return xmlNode.Value;
    }

    internal static void CheckForNonElement(XmlNode node)
    {
      if (node.NodeType != XmlNodeType.Element)
        throw new ConfigurationErrorsException(SR.GetString("Config_base_elements_only"), node);
    }

    internal static bool IsIgnorableAlsoCheckForNonElement(XmlNode node)
    {
      if (node.NodeType == XmlNodeType.Comment || node.NodeType == XmlNodeType.Whitespace)
        return true;
      HandlerBase.CheckForNonElement(node);
      return false;
    }

    internal static void CheckForChildNodes(XmlNode node)
    {
      if (node.HasChildNodes)
        throw new ConfigurationErrorsException(SR.GetString("Config_base_no_child_nodes"), node.FirstChild);
    }

    internal static void ThrowUnrecognizedElement(XmlNode node)
    {
      throw new ConfigurationErrorsException(SR.GetString("Config_base_unrecognized_element"), node);
    }
  }
}
