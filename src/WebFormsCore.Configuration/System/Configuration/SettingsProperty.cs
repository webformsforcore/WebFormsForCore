// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsProperty
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  /// <summary>Used internally as the class that represents metadata about an individual configuration property.</summary>
  public class SettingsProperty
  {
    private string _Name;
    private bool _IsReadOnly;
    private object _DefaultValue;
    private SettingsSerializeAs _SerializeAs;
    private SettingsProvider _Provider;
    private SettingsAttributeDictionary _Attributes;
    private Type _PropertyType;
    private bool _ThrowOnErrorDeserializing;
    private bool _ThrowOnErrorSerializing;

    /// <summary>Gets or sets the name of the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
    /// <returns>The name of the <see cref="T:System.Configuration.SettingsProperty" />.</returns>
    public virtual string Name
    {
      get => this._Name;
      set => this._Name = value;
    }

    /// <summary>Gets or sets a value specifying whether a <see cref="T:System.Configuration.SettingsProperty" /> object is read-only.</summary>
    /// <returns>
    /// <see langword="true" /> if the <see cref="T:System.Configuration.SettingsProperty" /> is read-only; otherwise, <see langword="false" />.</returns>
    public virtual bool IsReadOnly
    {
      get => this._IsReadOnly;
      set => this._IsReadOnly = value;
    }

    /// <summary>Gets or sets the default value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
    /// <returns>An object containing the default value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</returns>
    public virtual object DefaultValue
    {
      get => this._DefaultValue;
      set => this._DefaultValue = value;
    }

    /// <summary>Gets or sets the type for the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
    /// <returns>The type for the <see cref="T:System.Configuration.SettingsProperty" />.</returns>
    public virtual Type PropertyType
    {
      get => this._PropertyType;
      set => this._PropertyType = value;
    }

    /// <summary>Gets or sets a <see cref="T:System.Configuration.SettingsSerializeAs" /> object for the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsSerializeAs" /> object.</returns>
    public virtual SettingsSerializeAs SerializeAs
    {
      get => this._SerializeAs;
      set => this._SerializeAs = value;
    }

    /// <summary>Gets or sets the provider for the <see cref="T:System.Configuration.SettingsProperty" />.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsProvider" /> object.</returns>
    public virtual SettingsProvider Provider
    {
      get => this._Provider;
      set => this._Provider = value;
    }

    /// <summary>Gets a <see cref="T:System.Configuration.SettingsAttributeDictionary" /> object containing the attributes of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
    /// <returns>A <see cref="T:System.Configuration.SettingsAttributeDictionary" /> object.</returns>
    public virtual SettingsAttributeDictionary Attributes => this._Attributes;

    /// <summary>Gets or sets a value specifying whether an error will be thrown when the property is unsuccessfully deserialized.</summary>
    /// <returns>
    /// <see langword="true" /> if the error will be thrown when the property is unsuccessfully deserialized; otherwise, <see langword="false" />.</returns>
    public bool ThrowOnErrorDeserializing
    {
      get => this._ThrowOnErrorDeserializing;
      set => this._ThrowOnErrorDeserializing = value;
    }

    /// <summary>Gets or sets a value specifying whether an error will be thrown when the property is unsuccessfully serialized.</summary>
    /// <returns>
    /// <see langword="true" /> if the error will be thrown when the property is unsuccessfully serialized; otherwise, <see langword="false" />.</returns>
    public bool ThrowOnErrorSerializing
    {
      get => this._ThrowOnErrorSerializing;
      set => this._ThrowOnErrorSerializing = value;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsProperty" /> class. based on the supplied parameter.</summary>
    /// <param name="name">Specifies the name of an existing <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    public SettingsProperty(string name)
    {
      this._Name = name;
      this._Attributes = new SettingsAttributeDictionary();
    }

    /// <summary>Creates a new instance of the <see cref="T:System.Configuration.SettingsProperty" /> class based on the supplied parameters.</summary>
    /// <param name="name">The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    /// <param name="propertyType">The type of <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    /// <param name="provider">A <see cref="T:System.Configuration.SettingsProvider" /> object to use for persistence.</param>
    /// <param name="isReadOnly">A <see cref="T:System.Boolean" /> value specifying whether the <see cref="T:System.Configuration.SettingsProperty" /> object is read-only.</param>
    /// <param name="defaultValue">The default value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    /// <param name="serializeAs">A <see cref="T:System.Configuration.SettingsSerializeAs" /> object. This object is an enumeration used to set the serialization scheme for storing application settings.</param>
    /// <param name="attributes">A <see cref="T:System.Configuration.SettingsAttributeDictionary" /> object.</param>
    /// <param name="throwOnErrorDeserializing">A Boolean value specifying whether an error will be thrown when the property is unsuccessfully deserialized.</param>
    /// <param name="throwOnErrorSerializing">A Boolean value specifying whether an error will be thrown when the property is unsuccessfully serialized.</param>
    public SettingsProperty(
      string name,
      Type propertyType,
      SettingsProvider provider,
      bool isReadOnly,
      object defaultValue,
      SettingsSerializeAs serializeAs,
      SettingsAttributeDictionary attributes,
      bool throwOnErrorDeserializing,
      bool throwOnErrorSerializing)
    {
      this._Name = name;
      this._PropertyType = propertyType;
      this._Provider = provider;
      this._IsReadOnly = isReadOnly;
      this._DefaultValue = defaultValue;
      this._SerializeAs = serializeAs;
      this._Attributes = attributes;
      this._ThrowOnErrorDeserializing = throwOnErrorDeserializing;
      this._ThrowOnErrorSerializing = throwOnErrorSerializing;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsProperty" /> class, based on the supplied parameter.</summary>
    /// <param name="propertyToCopy">Specifies a copy of an existing <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    public SettingsProperty(SettingsProperty propertyToCopy)
    {
      this._Name = propertyToCopy.Name;
      this._IsReadOnly = propertyToCopy.IsReadOnly;
      this._DefaultValue = propertyToCopy.DefaultValue;
      this._SerializeAs = propertyToCopy.SerializeAs;
      this._Provider = propertyToCopy.Provider;
      this._PropertyType = propertyToCopy.PropertyType;
      this._ThrowOnErrorDeserializing = propertyToCopy.ThrowOnErrorDeserializing;
      this._ThrowOnErrorSerializing = propertyToCopy.ThrowOnErrorSerializing;
      this._Attributes = new SettingsAttributeDictionary(propertyToCopy.Attributes);
    }
  }
}
