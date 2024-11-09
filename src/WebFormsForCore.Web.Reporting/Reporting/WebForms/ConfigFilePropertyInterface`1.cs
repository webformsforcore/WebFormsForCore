// Decompiled with JetBrains decompiler
// Type: Microsoft.Reporting.WebForms.ConfigFilePropertyInterface`1
// Assembly: Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: F82ADCE6-59A1-4E44-AF2B-7E8AD7E2F93B
// Assembly location: C:\Users\simon\Downloads\Microsoft.ReportViewer.WebForms.dll

using System;
using System.Configuration;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  internal sealed class ConfigFilePropertyInterface<InterfaceType> where InterfaceType : class
  {
    private string m_propertyName;
    private string m_interfaceTypeName;
    private bool m_propertyLoaded;
    private Type m_propertyType;

    public ConfigFilePropertyInterface(string propertyName, string interfaceTypeName)
    {
      this.m_propertyName = propertyName;
      this.m_interfaceTypeName = interfaceTypeName;
    }

    public InterfaceType GetInstance()
    {
      this.EnsurePropertyLoaded();
      return this.m_propertyType == null ? default (InterfaceType) : (InterfaceType) Activator.CreateInstance(this.m_propertyType);
    }

    private void EnsurePropertyLoaded()
    {
      if (this.m_propertyLoaded || this.m_propertyName == null)
        return;
      string appSetting = ConfigurationManager.AppSettings[this.m_propertyName];
      if (!string.IsNullOrEmpty(appSetting))
      {
        Type type = Type.GetType(appSetting);
        if (type == null)
          throw new InvalidConfigFileTypeException(appSetting);
        this.m_propertyType = typeof (InterfaceType).IsAssignableFrom(type) ? type : throw new InvalidConfigFileTypeException(appSetting, this.m_interfaceTypeName);
      }
      this.m_propertyLoaded = true;
    }
  }
}
