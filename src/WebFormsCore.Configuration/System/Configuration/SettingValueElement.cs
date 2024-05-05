// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingValueElement
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Xml;

#nullable disable
namespace System.Configuration
{
  /// <summary>Contains the XML representing the serialized value of the setting. This class cannot be inherited.</summary>
  public sealed class SettingValueElement : ConfigurationElement
  {
    private static volatile ConfigurationPropertyCollection _properties;
    private static XmlDocument doc = new XmlDocument();
    private XmlNode _valueXml;
    private bool isModified;

    protected internal override ConfigurationPropertyCollection Properties
    {
      get
      {
        if (SettingValueElement._properties == null)
          SettingValueElement._properties = new ConfigurationPropertyCollection();
        return SettingValueElement._properties;
      }
    }

    /// <summary>Gets or sets the value of a <see cref="T:System.Configuration.SettingValueElement" /> object by using an <see cref="T:System.Xml.XmlNode" /> object.</summary>
    /// <returns>An <see cref="T:System.Xml.XmlNode" /> object containing the value of a <see cref="T:System.Configuration.SettingElement" />.</returns>
    public XmlNode ValueXml
    {
      get => this._valueXml;
      set
      {
        this._valueXml = value;
        this.isModified = true;
      }
    }

    protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
    {
      this.ValueXml = SettingValueElement.doc.ReadNode(reader);
    }

    /// <summary>Compares the current <see cref="T:System.Configuration.SettingValueElement" /> instance to the specified object.</summary>
    /// <param name="settingValue">The object to compare.</param>
    /// <returns>
    /// <see langword="true" /> if the <see cref="T:System.Configuration.SettingValueElement" /> instance is equal to the specified object; otherwise, <see langword="false" />.</returns>
    public override bool Equals(object settingValue)
    {
      return settingValue is SettingValueElement settingValueElement && object.Equals((object) settingValueElement.ValueXml, (object) this.ValueXml);
    }

    /// <summary>Gets a unique value representing the <see cref="T:System.Configuration.SettingValueElement" /> current instance.</summary>
    /// <returns>A unique value representing the <see cref="T:System.Configuration.SettingValueElement" /> current instance.</returns>
    public override int GetHashCode() => this.ValueXml.GetHashCode();

    protected internal override bool IsModified() => this.isModified;

    protected internal override void ResetModified() => this.isModified = false;

    protected internal override bool SerializeToXmlElement(XmlWriter writer, string elementName)
    {
      if (this.ValueXml == null)
        return false;
      if (writer != null)
        this.ValueXml.WriteTo(writer);
      return true;
    }

    protected internal override void Reset(ConfigurationElement parentElement)
    {
      base.Reset(parentElement);
      this.ValueXml = ((SettingValueElement) parentElement).ValueXml;
    }

    protected internal override void Unmerge(
      ConfigurationElement sourceElement,
      ConfigurationElement parentElement,
      ConfigurationSaveMode saveMode)
    {
      base.Unmerge(sourceElement, parentElement, saveMode);
      this.ValueXml = ((SettingValueElement) sourceElement).ValueXml;
    }
  }
}
