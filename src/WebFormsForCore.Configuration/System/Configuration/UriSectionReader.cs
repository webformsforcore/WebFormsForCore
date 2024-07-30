
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Xml;

#nullable disable
namespace System.Configuration
{
  internal class UriSectionReader
  {
    private const string rootElementName = "configuration";
    private string configFilePath;
    private XmlReader reader;
    private UriSectionData sectionData;

    private UriSectionReader(string configFilePath, UriSectionData parentData)
    {
      this.configFilePath = configFilePath;
      this.sectionData = new UriSectionData();
      if (parentData == null)
        return;
      this.sectionData.IriParsing = parentData.IriParsing;
      this.sectionData.IdnScope = parentData.IdnScope;
      foreach (KeyValuePair<string, SchemeSettingInternal> schemeSetting in parentData.SchemeSettings)
        this.sectionData.SchemeSettings.Add(schemeSetting.Key, schemeSetting.Value);
    }

    public static UriSectionData Read(string configFilePath)
    {
      return UriSectionReader.Read(configFilePath, (UriSectionData) null);
    }

    public static UriSectionData Read(string configFilePath, UriSectionData parentData)
    {
      return new UriSectionReader(configFilePath, parentData).GetSectionData();
    }

    private UriSectionData GetSectionData()
    {
      new FileIOPermission(FileIOPermissionAccess.Read, this.configFilePath).Assert();
      try
      {
        if (File.Exists(this.configFilePath))
        {
          using (FileStream input = new FileStream(this.configFilePath, FileMode.Open, FileAccess.Read))
          {
            using (this.reader = XmlReader.Create((Stream) input, new XmlReaderSettings()
            {
              IgnoreComments = true,
              IgnoreWhitespace = true,
              IgnoreProcessingInstructions = true
            }))
            {
              if (this.ReadConfiguration())
                return this.sectionData;
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
      }
      return (UriSectionData) null;
    }

    private bool ReadConfiguration()
    {
      if (!this.ReadToUriSection())
        return false;
      while (this.reader.Read())
      {
        if (this.IsEndElement("uri"))
          return true;
        if (this.reader.NodeType != XmlNodeType.Element)
          return false;
        string name = this.reader.Name;
        if (UriSectionReader.AreEqual(name, "iriParsing"))
        {
          if (this.ReadIriParsing())
            continue;
        }
        else if (UriSectionReader.AreEqual(name, "idn"))
        {
          if (this.ReadIdnScope())
            continue;
        }
        else if (UriSectionReader.AreEqual(name, "schemeSettings") && this.ReadSchemeSettings())
          continue;
        return false;
      }
      return false;
    }

    private bool ReadIriParsing()
    {
      bool result;
      if (!bool.TryParse(this.reader.GetAttribute("enabled"), out result))
        return false;
      this.sectionData.IriParsing = new bool?(result);
      return true;
    }

    private bool ReadIdnScope()
    {
      string attribute = this.reader.GetAttribute("enabled");
      try
      {
        this.sectionData.IdnScope = new UriIdnScope?((UriIdnScope) Enum.Parse(typeof (UriIdnScope), attribute, true));
        return true;
      }
      catch (ArgumentException ex)
      {
        return false;
      }
    }

    private bool ReadSchemeSettings()
    {
      while (this.reader.Read())
      {
        if (this.IsEndElement("schemeSettings"))
          return true;
        if (this.reader.NodeType != XmlNodeType.Element)
          return false;
        string name = this.reader.Name;
        if (UriSectionReader.AreEqual(name, "add"))
        {
          if (this.ReadAddSchemeSetting())
            continue;
        }
        else if (UriSectionReader.AreEqual(name, "remove"))
        {
          if (this.ReadRemoveSchemeSetting())
            continue;
        }
        else if (UriSectionReader.AreEqual(name, "clear"))
        {
          this.ClearSchemeSetting();
          continue;
        }
        return false;
      }
      return false;
    }

    private static bool AreEqual(string value1, string value2)
    {
      return string.Compare(value1, value2, StringComparison.OrdinalIgnoreCase) == 0;
    }

    private bool ReadAddSchemeSetting()
    {
      string attribute1 = this.reader.GetAttribute("name");
      string attribute2 = this.reader.GetAttribute("genericUriParserOptions");
      if (!string.IsNullOrEmpty(attribute1))
      {
        if (!string.IsNullOrEmpty(attribute2))
        {
          try
          {
            GenericUriParserOptions options = (GenericUriParserOptions) Enum.Parse(typeof (GenericUriParserOptions), attribute2);
            SchemeSettingInternal schemeSettingInternal = new SchemeSettingInternal(attribute1, options);
            this.sectionData.SchemeSettings[schemeSettingInternal.Name] = schemeSettingInternal;
            return true;
          }
          catch (ArgumentException ex)
          {
            return false;
          }
        }
      }
      return false;
    }

    private bool ReadRemoveSchemeSetting()
    {
      string attribute = this.reader.GetAttribute("name");
      if (string.IsNullOrEmpty(attribute))
        return false;
      this.sectionData.SchemeSettings.Remove(attribute);
      return true;
    }

    private void ClearSchemeSetting() => this.sectionData.SchemeSettings.Clear();

    private bool IsEndElement(string elementName)
    {
      return this.reader.NodeType == XmlNodeType.EndElement && string.Compare(this.reader.Name, elementName, StringComparison.OrdinalIgnoreCase) == 0;
    }

    private bool ReadToUriSection()
    {
      if (!this.reader.ReadToFollowing("configuration") || this.reader.Depth != 0)
        return false;
      while (this.reader.ReadToFollowing("uri"))
      {
        if (this.reader.Depth == 1)
          return true;
      }
      return false;
    }
  }
}
