
using System.ComponentModel;
using System.Configuration;
using System.Globalization;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents a URI prefix and the associated class that handles creating Web requests for the prefix. This class cannot be inherited.</summary>
	public sealed class WebRequestModuleElement : ConfigurationElement
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty prefix = new ConfigurationProperty(nameof(prefix), typeof(string), (object)null, ConfigurationPropertyOptions.IsKey);
		private readonly ConfigurationProperty type = new ConfigurationProperty(nameof(type), typeof(WebRequestModuleElement.TypeAndName), (object)null, (TypeConverter)new WebRequestModuleElement.TypeTypeConverter(), (ConfigurationValidatorBase)null, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> class.</summary>
		public WebRequestModuleElement()
		{
			this.properties.Add(this.prefix);
			this.properties.Add(this.type);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> class using the specified URI prefix and type information.</summary>
		/// <param name="prefix">A string containing a URI prefix.</param>
		/// <param name="type">A string containing the type and assembly information for the class that handles creating requests for resources that use the <paramref name="prefix" /> URI prefix.</param>
		public WebRequestModuleElement(string prefix, string type)
		  : this()
		{
			this.Prefix = prefix;
			this[this.type] = (object)new WebRequestModuleElement.TypeAndName(type);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.WebRequestModuleElement" /> class using the specified URI prefix and type identifier.</summary>
		/// <param name="prefix">A string containing a URI prefix.</param>
		/// <param name="type">A <see cref="T:System.Type" /> that identifies the class that handles creating requests for resources that use the <paramref name="prefix" /> URI prefix.</param>
		public WebRequestModuleElement(string prefix, Type type)
		  : this()
		{
			this.Prefix = prefix;
			this.Type = type;
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the URI prefix for the current Web request module.</summary>
		/// <returns>A string that contains a URI prefix.</returns>
		[ConfigurationProperty("prefix", IsRequired = true, IsKey = true)]
		public string Prefix
		{
			get => (string)this[this.prefix];
			set => this[this.prefix] = (object)value;
		}

		/// <summary>Gets or sets a class that creates Web requests.</summary>
		/// <returns>A <see cref="T:System.Type" /> instance that identifies a Web request module.</returns>
		[ConfigurationProperty("type")]
		[TypeConverter(typeof(WebRequestModuleElement.TypeTypeConverter))]
		public Type Type
		{
			get => ((WebRequestModuleElement.TypeAndName)this[this.type])?.type;
			set => this[this.type] = (object)new WebRequestModuleElement.TypeAndName(value);
		}

		internal string Key => this.Prefix;

		private class TypeAndName
		{
			public readonly Type type;
			public readonly string name;

			public TypeAndName(string name)
			{
				this.type = Type.GetType(name, true, true);
				this.name = name;
			}

			public TypeAndName(Type type) => this.type = type;

			public override int GetHashCode() => this.type.GetHashCode();

			public override bool Equals(object comparand)
			{
				return this.type.Equals(((WebRequestModuleElement.TypeAndName)comparand).type);
			}
		}

		private class TypeTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(
			  ITypeDescriptorContext context,
			  CultureInfo culture,
			  object value)
			{
				return value is string ? (object)new WebRequestModuleElement.TypeAndName((string)value) : base.ConvertFrom(context, culture, value);
			}

			public override object ConvertTo(
			  ITypeDescriptorContext context,
			  CultureInfo culture,
			  object value,
			  Type destinationType)
			{
				if (!(destinationType == typeof(string)))
					return base.ConvertTo(context, culture, value, destinationType);
				WebRequestModuleElement.TypeAndName typeAndName = (WebRequestModuleElement.TypeAndName)value;
				return typeAndName.name != null ? (object)typeAndName.name : (object)typeAndName.type.AssemblyQualifiedName;
			}
		}
	}
}
