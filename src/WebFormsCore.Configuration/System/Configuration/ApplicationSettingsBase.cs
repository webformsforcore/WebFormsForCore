using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Provider;
using System.Reflection;
using System.Security.Permissions;

#nullable disable
namespace System.Configuration
{
	/// <summary>Acts as a base class for deriving concrete wrapper classes to implement the application settings feature in Window Forms applications.</summary>
	public abstract class ApplicationSettingsBase : SettingsBase, INotifyPropertyChanged
	{
		private bool _explicitSerializeOnClass;
		private object[] _classAttributes;
		private IComponent _owner;
		private PropertyChangedEventHandler _onPropertyChanged;
		private SettingsContext _context;
		private SettingsProperty _init;
		private SettingsPropertyCollection _settings;
		private SettingsProviderCollection _providers;
		private SettingChangingEventHandler _onSettingChanging;
		private SettingsLoadedEventHandler _onSettingsLoaded;
		private SettingsSavingEventHandler _onSettingsSaving;
		private string _settingsKey = string.Empty;
		private bool _firstLoad = true;
		private bool _initialized;

		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class to its default state.</summary>
		protected ApplicationSettingsBase()
		{
		}

		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class using the supplied owner component.</summary>
		/// <param name="owner">The component that will act as the owner of the application settings object.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="owner" /> is <see langword="null" />.</exception>
		protected ApplicationSettingsBase(IComponent owner)
		  : this(owner, string.Empty)
		{
		}

		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class using the supplied settings key.</summary>
		/// <param name="settingsKey">A <see cref="T:System.String" /> that uniquely identifies separate instances of the wrapper class.</param>
		protected ApplicationSettingsBase(string settingsKey) => this._settingsKey = settingsKey;

		/// <summary>Initializes an instance of the <see cref="T:System.Configuration.ApplicationSettingsBase" /> class using the supplied owner component and settings key.</summary>
		/// <param name="owner">The component that will act as the owner of the application settings object.</param>
		/// <param name="settingsKey">A <see cref="T:System.String" /> that uniquely identifies separate instances of the wrapper class.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="owner" /> is <see langword="null" />.</exception>
		protected ApplicationSettingsBase(IComponent owner, string settingsKey)
		  : this(settingsKey)
		{
			this._owner = owner != null ? owner : throw new ArgumentNullException(nameof(owner));
			if (owner.Site == null || !(owner.Site.GetService(typeof(ISettingsProviderService)) is ISettingsProviderService service))
				return;
			foreach (SettingsProperty property in this.Properties)
			{
				SettingsProvider settingsProvider = service.GetSettingsProvider(property);
				if (settingsProvider != null)
					property.Provider = settingsProvider;
			}
			this.ResetProviders();
		}

		/// <summary>Gets the application settings context associated with the settings group.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsContext" /> associated with the settings group.</returns>
		[Browsable(false)]
		public override SettingsContext Context
		{
			get
			{
				if (this._context == null)
				{
					if (this.IsSynchronized)
					{
						lock (this)
						{
							if (this._context == null)
							{
								this._context = new SettingsContext();
								this.EnsureInitialized();
							}
						}
					}
					else
					{
						this._context = new SettingsContext();
						this.EnsureInitialized();
					}
				}
				return this._context;
			}
		}

		/// <summary>Gets the collection of settings properties in the wrapper.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing all the <see cref="T:System.Configuration.SettingsProperty" /> objects used in the current wrapper.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The associated settings provider could not be found or its instantiation failed.</exception>
		[Browsable(false)]
		public override SettingsPropertyCollection Properties
		{
			get
			{
				if (this._settings == null)
				{
					if (this.IsSynchronized)
					{
						lock (this)
						{
							if (this._settings == null)
							{
								this._settings = new SettingsPropertyCollection();
								this.EnsureInitialized();
							}
						}
					}
					else
					{
						this._settings = new SettingsPropertyCollection();
						this.EnsureInitialized();
					}
				}
				return this._settings;
			}
		}

		/// <summary>Gets a collection of property values.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> of property values.</returns>
		[Browsable(false)]
		public override SettingsPropertyValueCollection PropertyValues => base.PropertyValues;

		/// <summary>Gets the collection of application settings providers used by the wrapper.</summary>
		/// <returns>A <see cref="T:System.Configuration.SettingsProviderCollection" /> containing all the <see cref="T:System.Configuration.SettingsProvider" /> objects used by the settings properties of the current settings wrapper.</returns>
		[Browsable(false)]
		public override SettingsProviderCollection Providers
		{
			get
			{
				if (this._providers == null)
				{
					if (this.IsSynchronized)
					{
						lock (this)
						{
							if (this._providers == null)
							{
								this._providers = new SettingsProviderCollection();
								this.EnsureInitialized();
							}
						}
					}
					else
					{
						this._providers = new SettingsProviderCollection();
						this.EnsureInitialized();
					}
				}
				return this._providers;
			}
		}

		/// <summary>Gets or sets the settings key for the application settings group.</summary>
		/// <returns>A <see cref="T:System.String" /> containing the settings key for the current settings group.</returns>
		[Browsable(false)]
		public string SettingsKey
		{
			get => this._settingsKey;
			set
			{
				this._settingsKey = value;
				this.Context[(object)nameof(SettingsKey)] = (object)this._settingsKey;
			}
		}

		/// <summary>Occurs after the value of an application settings property is changed.</summary>
		public event PropertyChangedEventHandler PropertyChanged
		{
			add => this._onPropertyChanged += value;
			remove => this._onPropertyChanged -= value;
		}

		/// <summary>Occurs before the value of an application settings property is changed.</summary>
		public event SettingChangingEventHandler SettingChanging
		{
			add => this._onSettingChanging += value;
			remove => this._onSettingChanging -= value;
		}

		/// <summary>Occurs after the application settings are retrieved from storage.</summary>
		public event SettingsLoadedEventHandler SettingsLoaded
		{
			add => this._onSettingsLoaded += value;
			remove => this._onSettingsLoaded -= value;
		}

		/// <summary>Occurs before values are saved to the data store.</summary>
		public event SettingsSavingEventHandler SettingsSaving
		{
			add => this._onSettingsSaving += value;
			remove => this._onSettingsSaving -= value;
		}

		/// <summary>Returns the value of the named settings property for the previous version of the same application.</summary>
		/// <param name="propertyName">A <see cref="T:System.String" /> containing the name of the settings property whose value is to be returned.</param>
		/// <returns>An <see cref="T:System.Object" /> containing the value of the specified <see cref="T:System.Configuration.SettingsProperty" /> if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Configuration.SettingsPropertyNotFoundException">The property does not exist. The property count is zero or the property cannot be found in the data store.</exception>
		public object GetPreviousVersion(string propertyName)
		{
			SettingsProperty property = this.Properties.Count != 0 ? this.Properties[propertyName] : throw new SettingsPropertyNotFoundException();
			SettingsPropertyValue settingsPropertyValue = (SettingsPropertyValue)null;
			if (property == null)
				throw new SettingsPropertyNotFoundException();
			if (property.Provider is IApplicationSettingsProvider provider)
				settingsPropertyValue = provider.GetPreviousVersion(this.Context, property);
			return settingsPropertyValue?.PropertyValue;
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.PropertyChanged" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> that contains the event data.</param>
		protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (this._onPropertyChanged == null)
				return;
			this._onPropertyChanged((object)this, e);
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingChanging" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.Configuration.SettingChangingEventArgs" /> that contains the event data.</param>
		protected virtual void OnSettingChanging(object sender, SettingChangingEventArgs e)
		{
			if (this._onSettingChanging == null)
				return;
			this._onSettingChanging((object)this, e);
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsLoaded" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.Configuration.SettingsLoadedEventArgs" /> that contains the event data.</param>
		protected virtual void OnSettingsLoaded(object sender, SettingsLoadedEventArgs e)
		{
			if (this._onSettingsLoaded == null)
				return;
			this._onSettingsLoaded((object)this, e);
		}

		/// <summary>Raises the <see cref="E:System.Configuration.ApplicationSettingsBase.SettingsSaving" /> event.</summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
		protected virtual void OnSettingsSaving(object sender, CancelEventArgs e)
		{
			if (this._onSettingsSaving == null)
				return;
			this._onSettingsSaving((object)this, e);
		}

		/// <summary>Refreshes the application settings property values from persistent storage.</summary>
		public void Reload()
		{
			if (this.PropertyValues != null)
				this.PropertyValues.Clear();
			foreach (SettingsProperty property in this.Properties)
				this.OnPropertyChanged((object)this, new PropertyChangedEventArgs(property.Name));
		}

		/// <summary>Restores the persisted application settings values to their corresponding default properties.</summary>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be parsed.</exception>
		public void Reset()
		{
			if (this.Properties != null)
			{
				foreach (SettingsProvider provider in (ProviderCollection)this.Providers)
				{
					if (provider is IApplicationSettingsProvider settingsProvider)
						settingsProvider.Reset(this.Context);
				}
			}
			this.Reload();
		}

		/// <summary>Stores the current values of the application settings properties.</summary>
		public override void Save()
		{
			CancelEventArgs e = new CancelEventArgs(false);
			this.OnSettingsSaving((object)this, e);
			if (e.Cancel)
				return;
			base.Save();
		}

		/// <summary>Gets or sets the value of the specified application settings property.</summary>
		/// <param name="propertyName">A <see cref="T:System.String" /> containing the name of the property to access.</param>
		/// <returns>If found, the value of the named settings property; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Configuration.SettingsPropertyNotFoundException">There are no properties associated with the current wrapper or the specified property could not be found.</exception>
		/// <exception cref="T:System.Configuration.SettingsPropertyIsReadOnlyException">An attempt was made to set a read-only property.</exception>
		/// <exception cref="T:System.Configuration.SettingsPropertyWrongTypeException">The value supplied is of a type incompatible with the settings property, during a set operation.</exception>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be parsed.</exception>
		public override object this[string propertyName]
		{
			get
			{
				if (!this.IsSynchronized)
					return this.GetPropertyValue(propertyName);
				lock (this)
					return this.GetPropertyValue(propertyName);
			}
			set
			{
				SettingChangingEventArgs e = new SettingChangingEventArgs(propertyName, this.GetType().FullName, this.SettingsKey, value, false);
				this.OnSettingChanging((object)this, e);
				if (e.Cancel)
					return;
				base[propertyName] = value;
				this.OnPropertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>Updates application settings to reflect a more recent installation of the application.</summary>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">The configuration file could not be parsed.</exception>
		public virtual void Upgrade()
		{
			if (this.Properties != null)
			{
				foreach (SettingsProvider provider in (ProviderCollection)this.Providers)
				{
					if (provider is IApplicationSettingsProvider settingsProvider)
						settingsProvider.Upgrade(this.Context, this.GetPropertiesForProvider(provider));
				}
			}
			this.Reload();
		}

		private SettingsProperty CreateSetting(PropertyInfo propInfo)
		{
			object[] customAttributes = propInfo.GetCustomAttributes(false);
			SettingsProperty setting = new SettingsProperty(this.Initializer);
			bool flag = this._explicitSerializeOnClass;
			setting.Name = propInfo.Name;
			setting.PropertyType = propInfo.PropertyType;
			for (int index = 0; index < customAttributes.Length; ++index)
			{
				if (customAttributes[index] is Attribute attribute)
				{
					switch (attribute)
					{
						case DefaultSettingValueAttribute _:
							setting.DefaultValue = (object)((DefaultSettingValueAttribute)attribute).Value;
							continue;
						case ReadOnlyAttribute _:
							setting.IsReadOnly = true;
							continue;
						case SettingsProviderAttribute _:
							string providerTypeName = ((SettingsProviderAttribute)attribute).ProviderTypeName;
							Type type = Type.GetType(providerTypeName);
							if (type != (Type)null)
							{
								if (SecurityUtils.SecureCreateInstance(type) is SettingsProvider settingsProvider)
								{
									settingsProvider.Initialize((string)null, (NameValueCollection)null);
									settingsProvider.ApplicationName = ConfigurationManagerInternalFactory.Instance.ExeProductName;
									SettingsProvider provider = this._providers[settingsProvider.Name];
									if (provider != null)
										settingsProvider = provider;
									setting.Provider = settingsProvider;
									continue;
								}
								throw new ConfigurationErrorsException(SR.GetString("ProviderInstantiationFailed", (object)providerTypeName));
							}
							throw new ConfigurationErrorsException(SR.GetString("ProviderTypeLoadFailed", (object)providerTypeName));
						case SettingsSerializeAsAttribute _:
							setting.SerializeAs = ((SettingsSerializeAsAttribute)attribute).SerializeAs;
							flag = true;
							continue;
						default:
							setting.Attributes.Add((object)attribute.GetType(), (object)attribute);
							continue;
					}
				}
			}
			if (!flag)
				setting.SerializeAs = this.GetSerializeAs(propInfo.PropertyType);
			return setting;
		}

		private void EnsureInitialized()
		{
			if (this._initialized)
				return;
			this._initialized = true;
			Type type = this.GetType();
			if (this._context == null)
				this._context = new SettingsContext();
			this._context[(object)"GroupName"] = (object)type.FullName;
			this._context[(object)"SettingsKey"] = (object)this.SettingsKey;
			this._context[(object)"SettingsClassType"] = (object)type;
			PropertyInfo[] propertyInfoArray = this.SettingsFilter(type.GetProperties(BindingFlags.Instance | BindingFlags.Public));
			this._classAttributes = type.GetCustomAttributes(false);
			if (this._settings == null)
				this._settings = new SettingsPropertyCollection();
			if (this._providers == null)
				this._providers = new SettingsProviderCollection();
			for (int index = 0; index < propertyInfoArray.Length; ++index)
			{
				SettingsProperty setting = this.CreateSetting(propertyInfoArray[index]);
				if (setting != null)
				{
					this._settings.Add(setting);
					if (setting.Provider != null && this._providers[setting.Provider.Name] == null)
						this._providers.Add((ProviderBase)setting.Provider);
				}
			}
		}

		private SettingsProperty Initializer
		{
			get
			{
				if (this._init == null)
				{
					this._init = new SettingsProperty("");
					this._init.DefaultValue = (object)null;
					this._init.IsReadOnly = false;
					this._init.PropertyType = (Type)null;
					SettingsProvider settingsProvider = (SettingsProvider)new LocalFileSettingsProvider();
					if (this._classAttributes != null)
					{
						for (int index = 0; index < this._classAttributes.Length; ++index)
						{
							if (this._classAttributes[index] is Attribute classAttribute)
							{
								switch (classAttribute)
								{
									case ReadOnlyAttribute _:
										this._init.IsReadOnly = true;
										continue;
									case SettingsGroupNameAttribute _:
										if (this._context == null)
											this._context = new SettingsContext();
										this._context[(object)"GroupName"] = (object)((SettingsGroupNameAttribute)classAttribute).GroupName;
										continue;
									case SettingsProviderAttribute _:
										string providerTypeName = ((SettingsProviderAttribute)classAttribute).ProviderTypeName;
										Type type = Type.GetType(providerTypeName);
										if (type != (Type)null)
										{
											settingsProvider = SecurityUtils.SecureCreateInstance(type) is SettingsProvider instance ? instance : throw new ConfigurationErrorsException(SR.GetString("ProviderInstantiationFailed", (object)providerTypeName));
											continue;
										}
										throw new ConfigurationErrorsException(SR.GetString("ProviderTypeLoadFailed", (object)providerTypeName));
									case SettingsSerializeAsAttribute _:
										this._init.SerializeAs = ((SettingsSerializeAsAttribute)classAttribute).SerializeAs;
										this._explicitSerializeOnClass = true;
										continue;
									default:
										this._init.Attributes.Add((object)classAttribute.GetType(), (object)classAttribute);
										continue;
								}
							}
						}
					}
					settingsProvider.Initialize((string)null, (NameValueCollection)null);
					settingsProvider.ApplicationName = ConfigurationManagerInternalFactory.Instance.ExeProductName;
					this._init.Provider = settingsProvider;
				}
				return this._init;
			}
		}

		private SettingsPropertyCollection GetPropertiesForProvider(SettingsProvider provider)
		{
			SettingsPropertyCollection propertiesForProvider = new SettingsPropertyCollection();
			foreach (SettingsProperty property in this.Properties)
			{
				if (property.Provider == provider)
					propertiesForProvider.Add(property);
			}
			return propertiesForProvider;
		}

		private object GetPropertyValue(string propertyName)
		{
			if (this.PropertyValues[propertyName] != null)
				return base[propertyName];
			if (this._firstLoad)
			{
				this._firstLoad = false;
				if (this.IsFirstRunOfClickOnceApp())
					this.Upgrade();
			}
			object obj = base[propertyName];
			this.OnSettingsLoaded((object)this, new SettingsLoadedEventArgs(this.Properties[propertyName]?.Provider));
			return base[propertyName];
		}

		private SettingsSerializeAs GetSerializeAs(Type type)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			return converter.CanConvertTo(typeof(string)) & converter.CanConvertFrom(typeof(string)) ? SettingsSerializeAs.String : SettingsSerializeAs.Xml;
		}

		private bool IsFirstRunOfClickOnceApp()
		{
#if !WebFormsCore
            ActivationContext activationContext = AppDomain.CurrentDomain.ActivationContext;
            return ApplicationSettingsBase.IsClickOnceDeployed(AppDomain.CurrentDomain) && InternalActivationContextHelper.IsFirstRun(activationContext);
#else
			return false;
#endif
		}

		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
		internal static bool IsClickOnceDeployed(AppDomain appDomain)
		{
#if NETFRAMEWORK
            ActivationContext activationContext = appDomain.ActivationContext;
            return activationContext != null && activationContext.Form == ActivationContext.ContextForm.StoreBounded && !string.IsNullOrEmpty(activationContext.Identity.FullName);
#else
			return false;
#endif
		}

		private PropertyInfo[] SettingsFilter(PropertyInfo[] allProps)
		{
			ArrayList arrayList = new ArrayList();
			for (int index = 0; index < allProps.Length; ++index)
			{
				foreach (object customAttribute in allProps[index].GetCustomAttributes(false))
				{
					if (customAttribute as Attribute is SettingAttribute)
					{
						arrayList.Add((object)allProps[index]);
						break;
					}
				}
			}
			return (PropertyInfo[])arrayList.ToArray(typeof(PropertyInfo));
		}

		private void ResetProviders()
		{
			this.Providers.Clear();
			foreach (SettingsProperty property in this.Properties)
			{
				if (this.Providers[property.Provider.Name] == null)
					this.Providers.Add((ProviderBase)property.Provider);
			}
		}
	}
}
