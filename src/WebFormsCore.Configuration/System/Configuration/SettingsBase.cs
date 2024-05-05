// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsBase
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;
using System.Configuration.Provider;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides the base class used to support user property settings.</summary>
  public abstract class SettingsBase
  {
    private SettingsPropertyCollection _Properties;
    private SettingsProviderCollection _Providers;
    private SettingsPropertyValueCollection _PropertyValues;
    private SettingsContext _Context;
    private bool _IsSynchronized;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsBase" /> class.</summary>
    protected SettingsBase() => this._PropertyValues = new SettingsPropertyValueCollection();

    /// <summary>Gets or sets the value of the specified settings property.</summary>
    /// <param name="propertyName">A <see cref="T:System.String" /> containing the name of the property to access.</param>
    /// <returns>If found, the value of the named settings property.</returns>
    /// <exception cref="T:System.Configuration.SettingsPropertyNotFoundException">There are no properties associated with the current object, or the specified property could not be found.</exception>
    /// <exception cref="T:System.Configuration.SettingsPropertyIsReadOnlyException">An attempt was made to set a read-only property.</exception>
    /// <exception cref="T:System.Configuration.SettingsPropertyWrongTypeException">The value supplied is of a type incompatible with the settings property, during a set operation.</exception>
    public virtual object this[string propertyName]
    {
      get
      {
        if (!this.IsSynchronized)
          return this.GetPropertyValueByName(propertyName);
        lock (this)
          return this.GetPropertyValueByName(propertyName);
      }
      set
      {
        if (this.IsSynchronized)
        {
          lock (this)
            this.SetPropertyValueByName(propertyName, value);
        }
        else
          this.SetPropertyValueByName(propertyName, value);
      }
    }

    private object GetPropertyValueByName(string propertyName)
    {
      if (this.Properties == null || this._PropertyValues == null || this.Properties.Count == 0)
        throw new SettingsPropertyNotFoundException(SR.GetString("SettingsPropertyNotFound", (object) propertyName));
      SettingsProperty property = this.Properties[propertyName];
      if (property == null)
        throw new SettingsPropertyNotFoundException(SR.GetString("SettingsPropertyNotFound", (object) propertyName));
      SettingsPropertyValue propertyValue = this._PropertyValues[propertyName];
      if (propertyValue == null)
      {
        this.GetPropertiesFromProvider(property.Provider);
        propertyValue = this._PropertyValues[propertyName];
        if (propertyValue == null)
          throw new SettingsPropertyNotFoundException(SR.GetString("SettingsPropertyNotFound", (object) propertyName));
      }
      return propertyValue.PropertyValue;
    }

    private void SetPropertyValueByName(string propertyName, object propertyValue)
    {
      if (this.Properties == null || this._PropertyValues == null || this.Properties.Count == 0)
        throw new SettingsPropertyNotFoundException(SR.GetString("SettingsPropertyNotFound", (object) propertyName));
      SettingsProperty property = this.Properties[propertyName];
      if (property == null)
        throw new SettingsPropertyNotFoundException(SR.GetString("SettingsPropertyNotFound", (object) propertyName));
      if (property.IsReadOnly)
        throw new SettingsPropertyIsReadOnlyException(SR.GetString("SettingsPropertyReadOnly", (object) propertyName));
      if (propertyValue != null && !property.PropertyType.IsInstanceOfType(propertyValue))
        throw new SettingsPropertyWrongTypeException(SR.GetString("SettingsPropertyWrongType", (object) propertyName));
      SettingsPropertyValue propertyValue1 = this._PropertyValues[propertyName];
      if (propertyValue1 == null)
      {
        this.GetPropertiesFromProvider(property.Provider);
        propertyValue1 = this._PropertyValues[propertyName];
        if (propertyValue1 == null)
          throw new SettingsPropertyNotFoundException(SR.GetString("SettingsPropertyNotFound", (object) propertyName));
      }
      propertyValue1.PropertyValue = propertyValue;
    }

    /// <summary>Initializes internal properties used by <see cref="T:System.Configuration.SettingsBase" /> object.</summary>
    /// <param name="context">The settings context related to the settings properties.</param>
    /// <param name="properties">The settings properties that will be accessible from the <see cref="T:System.Configuration.SettingsBase" /> instance.</param>
    /// <param name="providers">The initialized providers that should be used when loading and saving property values.</param>
    public void Initialize(
      SettingsContext context,
      SettingsPropertyCollection properties,
      SettingsProviderCollection providers)
    {
      this._Context = context;
      this._Properties = properties;
      this._Providers = providers;
    }

    /// <summary>Stores the current values of the settings properties.</summary>
    public virtual void Save()
    {
      if (this.IsSynchronized)
      {
        lock (this)
          this.SaveCore();
      }
      else
        this.SaveCore();
    }

    private void SaveCore()
    {
      if (this.Properties == null || this._PropertyValues == null || this.Properties.Count == 0)
        return;
      foreach (SettingsProvider provider in (ProviderCollection) this.Providers)
      {
        SettingsPropertyValueCollection collection = new SettingsPropertyValueCollection();
        foreach (SettingsPropertyValue propertyValue in this.PropertyValues)
        {
          if (propertyValue.Property.Provider == provider)
            collection.Add(propertyValue);
        }
        if (collection.Count > 0)
          provider.SetPropertyValues(this.Context, collection);
      }
      foreach (SettingsPropertyValue propertyValue in this.PropertyValues)
        propertyValue.IsDirty = false;
    }

    /// <summary>Gets the collection of settings properties.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsPropertyCollection" /> collection containing all the <see cref="T:System.Configuration.SettingsProperty" /> objects.</returns>
    public virtual SettingsPropertyCollection Properties => this._Properties;

    /// <summary>Gets a collection of settings providers.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsProviderCollection" /> containing <see cref="T:System.Configuration.SettingsProvider" /> objects.</returns>
    public virtual SettingsProviderCollection Providers => this._Providers;

    /// <summary>Gets a collection of settings property values.</summary>
    /// <returns>A collection of <see cref="T:System.Configuration.SettingsPropertyValue" /> objects representing the actual data values for the properties managed by the <see cref="T:System.Configuration.SettingsBase" /> instance.</returns>
    public virtual SettingsPropertyValueCollection PropertyValues => this._PropertyValues;

    /// <summary>Gets the associated settings context.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsContext" /> associated with the settings instance.</returns>
    public virtual SettingsContext Context => this._Context;

    private void GetPropertiesFromProvider(SettingsProvider provider)
    {
      SettingsPropertyCollection collection = new SettingsPropertyCollection();
      foreach (SettingsProperty property in this.Properties)
      {
        if (property.Provider == provider)
          collection.Add(property);
      }
      if (collection.Count <= 0)
        return;
      foreach (SettingsPropertyValue propertyValue in provider.GetPropertyValues(this.Context, collection))
      {
        if (this._PropertyValues[propertyValue.Name] == null)
          this._PropertyValues.Add(propertyValue);
      }
    }

    /// <summary>Provides a <see cref="T:System.Configuration.SettingsBase" /> class that is synchronized (thread safe).</summary>
    /// <param name="settingsBase">The class used to support user property settings.</param>
    /// <returns>A <see cref="T:System.Configuration.SettingsBase" /> class that is synchronized.</returns>
    public static SettingsBase Synchronized(SettingsBase settingsBase)
    {
      settingsBase._IsSynchronized = true;
      return settingsBase;
    }

    /// <summary>Gets a value indicating whether access to the object is synchronized (thread safe).</summary>
    /// <returns>
    /// <see langword="true" /> if access to the <see cref="T:System.Configuration.SettingsBase" /> is synchronized; otherwise, <see langword="false" />.</returns>
    [Browsable(false)]
    public bool IsSynchronized => this._IsSynchronized;
  }
}
