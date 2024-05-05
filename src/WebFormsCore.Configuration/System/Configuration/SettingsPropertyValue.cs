// Decompiled with JetBrains decompiler
// Type: System.Configuration.SettingsPropertyValue
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
#if NETFRAMEWORK
using System.Runtime.Serialization.Formatters.Binary;
#else
using WebFormsCore.Serialization.Formatters.Binary;
#endif
using System.Security.Permissions;
using System.Xml.Serialization;

#nullable disable
namespace System.Configuration
{
  /// <summary>Contains the value of a settings property that can be loaded and stored by an instance of <see cref="T:System.Configuration.SettingsBase" />.</summary>
  public class SettingsPropertyValue
  {
    private object _Value;
    private object _SerializedValue;
    private bool _Deserialized;
    private bool _IsDirty;
    private SettingsProperty _Property;
    private bool _ChangedSinceLastSerialized;
    private bool _UsingDefaultValue = true;

    /// <summary>Gets the name of the property from the associated <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
    /// <returns>The name of the <see cref="T:System.Configuration.SettingsProperty" /> object.</returns>
    public string Name => this._Property.Name;

    /// <summary>Gets or sets whether the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has changed.</summary>
    /// <returns>
    /// <see langword="true" /> if the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has changed; otherwise, <see langword="false" />.</returns>
    public bool IsDirty
    {
      get => this._IsDirty;
      set => this._IsDirty = value;
    }

    /// <summary>Gets the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
    /// <returns>The <see cref="T:System.Configuration.SettingsProperty" /> object that describes the <see cref="T:System.Configuration.SettingsPropertyValue" /> object.</returns>
    public SettingsProperty Property => this._Property;

    /// <summary>Gets a Boolean value specifying whether the value of the <see cref="T:System.Configuration.SettingsPropertyValue" /> object is the default value as defined by the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property value on the associated <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
    /// <returns>
    /// <see langword="true" /> if the value of the <see cref="T:System.Configuration.SettingsProperty" /> object is the default value; otherwise, <see langword="false" />.</returns>
    public bool UsingDefaultValue => this._UsingDefaultValue;

    /// <summary>Initializes a new instance of the <see cref="T:System.Configuration.SettingsPropertyValue" /> class, based on supplied parameters.</summary>
    /// <param name="property">Specifies a <see cref="T:System.Configuration.SettingsProperty" /> object.</param>
    public SettingsPropertyValue(SettingsProperty property) => this._Property = property;

    /// <summary>Gets or sets the value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
    /// <returns>The value of the <see cref="T:System.Configuration.SettingsProperty" /> object. When this value is set, the <see cref="P:System.Configuration.SettingsPropertyValue.IsDirty" /> property is set to <see langword="true" /> and <see cref="P:System.Configuration.SettingsPropertyValue.UsingDefaultValue" /> is set to <see langword="false" />.
    /// When a value is first accessed from the <see cref="P:System.Configuration.SettingsPropertyValue.PropertyValue" /> property, and if the value was initially stored into the <see cref="T:System.Configuration.SettingsPropertyValue" /> object as a serialized representation using the <see cref="P:System.Configuration.SettingsPropertyValue.SerializedValue" /> property, the <see cref="P:System.Configuration.SettingsPropertyValue.PropertyValue" /> property will trigger deserialization of the underlying value.  As a side effect, the <see cref="P:System.Configuration.SettingsPropertyValue.Deserialized" /> property will be set to <see langword="true" />.
    /// If this chain of events occurs in ASP.NET, and if an error occurs during the deserialization process, the error is logged using the health-monitoring feature of ASP.NET. By default, this means that deserialization errors will show up in the Application Event Log when running under ASP.NET. If this process occurs outside of ASP.NET, and if an error occurs during deserialization, the error is suppressed, and the remainder of the logic during deserialization occurs. If there is no serialized value to deserialize when the deserialization is attempted, then <see cref="T:System.Configuration.SettingsPropertyValue" /> object will instead attempt to return a default value if one was configured as defined on the associated <see cref="T:System.Configuration.SettingsProperty" /> instance. In this case, if the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property was set to either <see langword="null" />, or to the string "[null]", then the <see cref="T:System.Configuration.SettingsPropertyValue" /> object will initialize the <see cref="P:System.Configuration.SettingsPropertyValue.PropertyValue" /> property to either <see langword="null" /> for reference types, or to the default value for the associated value type.  On the other hand, if <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property holds a valid object reference or string value (other than "[null]"), then the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property is returned instead.
    /// If there is no serialized value to deserialize when the deserialization is attempted, and no default value was specified, then an empty string will be returned for string types. For all other types, a default instance will be returned by calling <see cref="M:System.Activator.CreateInstance(System.Type)" /> - for reference types this means an attempt will be made to create an object instance using the default constructor.  If this attempt fails, then <see langword="null" /> is returned.</returns>
    /// <exception cref="T:System.ArgumentException">While attempting to use the default value from the <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property, an error occurred.  Either the attempt to convert <see cref="P:System.Configuration.SettingsProperty.DefaultValue" /> property to a valid type failed, or the resulting value was not compatible with the type defined by <see cref="P:System.Configuration.SettingsProperty.PropertyType" />.</exception>
    public object PropertyValue
    {
      get
      {
        if (!this._Deserialized)
        {
          this._Value = this.Deserialize();
          this._Deserialized = true;
        }
        if (this._Value != null && !this.Property.PropertyType.IsPrimitive && !(this._Value is string) && !(this._Value is DateTime))
        {
          this._UsingDefaultValue = false;
          this._ChangedSinceLastSerialized = true;
          this._IsDirty = true;
        }
        return this._Value;
      }
      set
      {
        this._Value = value;
        this._IsDirty = true;
        this._ChangedSinceLastSerialized = true;
        this._Deserialized = true;
        this._UsingDefaultValue = false;
      }
    }

    /// <summary>Gets or sets the serialized value of the <see cref="T:System.Configuration.SettingsProperty" /> object.</summary>
    /// <returns>The serialized value of a <see cref="T:System.Configuration.SettingsProperty" /> object.</returns>
    /// <exception cref="T:System.ArgumentException">The serialization options for the property indicated the use of a string type converter, but a type converter was not available.</exception>
    public object SerializedValue
    {
      [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)] get
      {
        if (this._ChangedSinceLastSerialized)
        {
          this._ChangedSinceLastSerialized = false;
          this._SerializedValue = this.SerializePropertyValue();
        }
        return this._SerializedValue;
      }
      [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)] set
      {
        this._UsingDefaultValue = false;
        this._SerializedValue = value;
      }
    }

    /// <summary>Gets or sets whether the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has been deserialized.</summary>
    /// <returns>
    /// <see langword="true" /> if the value of a <see cref="T:System.Configuration.SettingsProperty" /> object has been deserialized; otherwise, <see langword="false" />.</returns>
    public bool Deserialized
    {
      get => this._Deserialized;
      set => this._Deserialized = value;
    }

    private bool IsHostedInAspnet() => AppDomain.CurrentDomain.GetData(".appDomain") != null;

    private object Deserialize()
    {
      object obj = (object) null;
      if (this.SerializedValue != null)
      {
        try
        {
          if (this.SerializedValue is string)
          {
            obj = SettingsPropertyValue.GetObjectFromString(this.Property.PropertyType, this.Property.SerializeAs, (string) this.SerializedValue);
          }
          else
          {
            MemoryStream serializationStream = new MemoryStream((byte[]) this.SerializedValue);
            try
            {
              obj = new BinaryFormatter().Deserialize((Stream) serializationStream);
            }
            finally
            {
              serializationStream.Close();
            }
          }
        }
        catch (Exception ex)
        {
          try
          {
            if (this.IsHostedInAspnet())
            {
              object[] args = new object[3]
              {
                (object) this.Property,
                (object) this,
                (object) ex
              };
              Type.GetType("System.Web.Management.WebBaseEvent, System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", true).InvokeMember("RaisePropertyDeserializationWebErrorEvent", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, (Binder) null, (object) null, args, CultureInfo.InvariantCulture);
            }
          }
          catch
          {
          }
        }
        if (obj != null && !this.Property.PropertyType.IsAssignableFrom(obj.GetType()))
          obj = (object) null;
      }
      if (obj == null)
      {
        this._UsingDefaultValue = true;
        if (this.Property.DefaultValue == null || this.Property.DefaultValue.ToString() == "[null]")
          return this.Property.PropertyType.IsValueType ? SecurityUtils.SecureCreateInstance(this.Property.PropertyType) : (object) null;
        if (!(this.Property.DefaultValue is string))
        {
          obj = this.Property.DefaultValue;
        }
        else
        {
          try
          {
            obj = SettingsPropertyValue.GetObjectFromString(this.Property.PropertyType, this.Property.SerializeAs, (string) this.Property.DefaultValue);
          }
          catch (Exception ex)
          {
            throw new ArgumentException(SR.GetString("Could_not_create_from_default_value", (object) this.Property.Name, (object) ex.Message));
          }
        }
        if (obj != null && !this.Property.PropertyType.IsAssignableFrom(obj.GetType()))
          throw new ArgumentException(SR.GetString("Could_not_create_from_default_value_2", (object) this.Property.Name));
      }
      if (obj == null)
      {
        if (this.Property.PropertyType == typeof (string))
        {
          obj = (object) "";
        }
        else
        {
          try
          {
            obj = SecurityUtils.SecureCreateInstance(this.Property.PropertyType);
          }
          catch
          {
          }
        }
      }
      return obj;
    }

    private static object GetObjectFromString(
      Type type,
      SettingsSerializeAs serializeAs,
      string attValue)
    {
      if (type == typeof (string) && (attValue == null || attValue.Length < 1 || serializeAs == SettingsSerializeAs.String))
        return (object) attValue;
      if (attValue == null || attValue.Length < 1)
        return (object) null;
      switch (serializeAs)
      {
        case SettingsSerializeAs.String:
          TypeConverter converter = TypeDescriptor.GetConverter(type);
          return converter != null && converter.CanConvertTo(typeof (string)) && converter.CanConvertFrom(typeof (string)) ? converter.ConvertFromInvariantString(attValue) : throw new ArgumentException(SR.GetString("Unable_to_convert_type_from_string", (object) type.ToString()), nameof (type));
        case SettingsSerializeAs.Xml:
          StringReader stringReader = new StringReader(attValue);
          return new XmlSerializer(type).Deserialize((TextReader) stringReader);
        case SettingsSerializeAs.Binary:
          byte[] buffer = Convert.FromBase64String(attValue);
          MemoryStream serializationStream = (MemoryStream) null;
          try
          {
            serializationStream = new MemoryStream(buffer);
            return new BinaryFormatter().Deserialize((Stream) serializationStream);
          }
          finally
          {
            serializationStream?.Close();
          }
        default:
          return (object) null;
      }
    }

    private object SerializePropertyValue()
    {
      if (this._Value == null)
        return (object) null;
      if (this.Property.SerializeAs != SettingsSerializeAs.Binary)
        return (object) SettingsPropertyValue.ConvertObjectToString(this._Value, this.Property.PropertyType, this.Property.SerializeAs, this.Property.ThrowOnErrorSerializing);
      MemoryStream serializationStream = new MemoryStream();
      try
      {
        new BinaryFormatter().Serialize((Stream) serializationStream, this._Value);
        return (object) serializationStream.ToArray();
      }
      finally
      {
        serializationStream.Close();
      }
    }

    private static string ConvertObjectToString(
      object propValue,
      Type type,
      SettingsSerializeAs serializeAs,
      bool throwOnError)
    {
      if (serializeAs == SettingsSerializeAs.ProviderSpecific)
        serializeAs = type == typeof (string) || type.IsPrimitive ? SettingsSerializeAs.String : SettingsSerializeAs.Xml;
      try
      {
        switch (serializeAs)
        {
          case SettingsSerializeAs.String:
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter != null && converter.CanConvertTo(typeof (string)) && converter.CanConvertFrom(typeof (string)) ? converter.ConvertToInvariantString(propValue) : throw new ArgumentException(SR.GetString("Unable_to_convert_type_to_string", (object) type.ToString()), nameof (type));
          case SettingsSerializeAs.Xml:
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
            xmlSerializer.Serialize((TextWriter) stringWriter, propValue);
            return stringWriter.ToString();
          case SettingsSerializeAs.Binary:
            MemoryStream serializationStream = new MemoryStream();
            try
            {
              new BinaryFormatter().Serialize((Stream) serializationStream, propValue);
              return Convert.ToBase64String(serializationStream.ToArray());
            }
            finally
            {
              serializationStream.Close();
            }
        }
      }
      catch (Exception ex)
      {
        if (throwOnError)
          throw;
      }
      return (string) null;
    }
  }
}
