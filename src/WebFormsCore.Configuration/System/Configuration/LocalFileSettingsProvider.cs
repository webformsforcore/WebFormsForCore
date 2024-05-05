// Decompiled with JetBrains decompiler
// Type: System.Configuration.LocalFileSettingsProvider
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Xml;

#nullable disable
namespace System.Configuration
{
	/// <summary>Provides persistence for application settings classes.</summary>
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class LocalFileSettingsProvider : SettingsProvider, IApplicationSettingsProvider
	{
		private string _appName = string.Empty;
		private ClientSettingsStore _store;
		private string _prevLocalConfigFileName;
		private string _prevRoamingConfigFileName;
		private LocalFileSettingsProvider.XmlEscaper _escaper;

		/// <summary>Gets or sets the name of the currently running application.</summary>
		/// <returns>A string that contains the application's display name.</returns>
		public override string ApplicationName
		{
			get => this._appName;
			set => this._appName = value;
		}

		private LocalFileSettingsProvider.XmlEscaper Escaper
		{
			get
			{
				if (this._escaper == null)
					this._escaper = new LocalFileSettingsProvider.XmlEscaper();
				return this._escaper;
			}
		}

		private ClientSettingsStore Store
		{
			get
			{
				if (this._store == null)
					this._store = new ClientSettingsStore();
				return this._store;
			}
		}

		/// <summary>Initializes the provider.</summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="values">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection values)
		{
			if (string.IsNullOrEmpty(name))
				name = nameof(LocalFileSettingsProvider);
			base.Initialize(name, values);
		}

		/// <summary>Returns the collection of setting property values for the specified application instance and settings property group.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <param name="properties">A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings property group whose values are to be retrieved.</param>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> containing the values for the specified settings property group.</returns>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.</exception>
		public override SettingsPropertyValueCollection GetPropertyValues(
		  SettingsContext context,
		  SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection propertyValues = new SettingsPropertyValueCollection();
			string sectionName = this.GetSectionName(context);
			IDictionary dictionary1 = this.Store.ReadSettings(sectionName, false);
			IDictionary dictionary2 = this.Store.ReadSettings(sectionName, true);
			ConnectionStringSettingsCollection settingsCollection = this.Store.ReadConnectionStrings();
			foreach (SettingsProperty property1 in properties)
			{
				string name1 = property1.Name;
				SettingsPropertyValue property2 = new SettingsPropertyValue(property1);
				if (property1.Attributes[(object)typeof(SpecialSettingAttribute)] is SpecialSettingAttribute attribute && attribute.SpecialSetting == SpecialSetting.ConnectionString)
				{
					string name2 = sectionName + "." + name1;
					property2.PropertyValue = settingsCollection == null || settingsCollection[name2] == null ? (property1.DefaultValue == null || !(property1.DefaultValue is string) ? (object)string.Empty : property1.DefaultValue) : (object)settingsCollection[name2].ConnectionString;
					property2.IsDirty = false;
					propertyValues.Add(property2);
				}
				else
				{
					bool flag = this.IsUserSetting(property1);
					if (flag && !ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
						throw new ConfigurationErrorsException(SR.GetString("UserSettingsNotSupported"));
					IDictionary dictionary3 = flag ? dictionary2 : dictionary1;
					if (dictionary3.Contains((object)name1))
					{
						StoredSetting storedSetting = (StoredSetting)dictionary3[(object)name1];
						string escapedString = storedSetting.Value.InnerXml;
						if (storedSetting.SerializeAs == SettingsSerializeAs.String)
							escapedString = this.Escaper.Unescape(escapedString);
						property2.SerializedValue = (object)escapedString;
					}
					else if (property1.DefaultValue != null)
						property2.SerializedValue = property1.DefaultValue;
					else
						property2.PropertyValue = (object)null;
					property2.IsDirty = false;
					propertyValues.Add(property2);
				}
			}
			return propertyValues;
		}

		/// <summary>Sets the values of the specified group of property settings.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <param name="values">A <see cref="T:System.Configuration.SettingsPropertyValueCollection" /> representing the group of property settings to set.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.
		/// -or-
		/// There was a general failure saving the settings to the configuration file.</exception>
		public override void SetPropertyValues(
		  SettingsContext context,
		  SettingsPropertyValueCollection values)
		{
			string sectionName = this.GetSectionName(context);
			IDictionary newSettings1 = (IDictionary)new Hashtable();
			IDictionary newSettings2 = (IDictionary)new Hashtable();
			foreach (SettingsPropertyValue settingsPropertyValue in values)
			{
				SettingsProperty property = settingsPropertyValue.Property;
				bool flag1 = this.IsUserSetting(property);
				if (settingsPropertyValue.IsDirty && flag1)
				{
					bool flag2 = LocalFileSettingsProvider.IsRoamingSetting(property);
					StoredSetting storedSetting = new StoredSetting(property.SerializeAs, this.SerializeToXmlElement(property, settingsPropertyValue));
					if (flag2)
						newSettings1[(object)property.Name] = (object)storedSetting;
					else
						newSettings2[(object)property.Name] = (object)storedSetting;
					settingsPropertyValue.IsDirty = false;
				}
			}
			if (newSettings1.Count > 0)
				this.Store.WriteSettings(sectionName, true, newSettings1);
			if (newSettings2.Count <= 0)
				return;
			this.Store.WriteSettings(sectionName, false, newSettings2);
		}

		/// <summary>Resets all application settings properties associated with the specified application to their default values.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.</exception>
		public void Reset(SettingsContext context)
		{
			string sectionName = this.GetSectionName(context);
			this.Store.RevertToParent(sectionName, true);
			this.Store.RevertToParent(sectionName, false);
		}

		/// <summary>Attempts to migrate previous user-scoped settings from a previous version of the same application.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> describing the current application usage.</param>
		/// <param name="properties">A <see cref="T:System.Configuration.SettingsPropertyCollection" /> containing the settings property group whose values are to be retrieved.</param>
		/// <exception cref="T:System.Configuration.ConfigurationErrorsException">A user-scoped setting was encountered but the current configuration only supports application-scoped settings.
		/// -or-
		/// The previous version of the configuration file could not be accessed.</exception>
		public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
		{
			SettingsPropertyCollection properties1 = new SettingsPropertyCollection();
			SettingsPropertyCollection properties2 = new SettingsPropertyCollection();
			foreach (SettingsProperty property in properties)
			{
				if (LocalFileSettingsProvider.IsRoamingSetting(property))
					properties2.Add(property);
				else
					properties1.Add(property);
			}
			if (properties2.Count > 0)
				this.Upgrade(context, properties2, true);
			if (properties1.Count <= 0)
				return;
			this.Upgrade(context, properties1, false);
		}

		private Version CreateVersion(string name)
		{
			try
			{
				return new Version(name);
			}
			catch (ArgumentException ex)
			{
				return (Version)null;
			}
			catch (OverflowException ex)
			{
				return (Version)null;
			}
			catch (FormatException ex)
			{
				return (Version)null;
			}
		}

		/// <summary>Returns the value of the named settings property for the previous version of the same application.</summary>
		/// <param name="context">A <see cref="T:System.Configuration.SettingsContext" /> that describes where the application settings property is used.</param>
		/// <param name="property">The <see cref="T:System.Configuration.SettingsProperty" /> whose value is to be returned.</param>
		/// <returns>A <see cref="T:System.Configuration.SettingsPropertyValue" /> representing the application setting if found; otherwise, <see langword="null" />.</returns>
		[FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery)]
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
		public SettingsPropertyValue GetPreviousVersion(
		  SettingsContext context,
		  SettingsProperty property)
		{
			string previousConfigFileName = this.GetPreviousConfigFileName(LocalFileSettingsProvider.IsRoamingSetting(property));
			if (!string.IsNullOrEmpty(previousConfigFileName))
				return this.GetSettingValuesFromFile(previousConfigFileName, this.GetSectionName(context), true, new SettingsPropertyCollection()
		{
		  property
		})[property.Name];
			return new SettingsPropertyValue(property)
			{
				PropertyValue = (object)null
			};
		}

		private string GetPreviousConfigFileName(bool isRoaming)
		{
			if (!ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
				throw new ConfigurationErrorsException(SR.GetString("UserSettingsNotSupported"));
			string previousConfigFileName = isRoaming ? this._prevRoamingConfigFileName : this._prevLocalConfigFileName;
			if (string.IsNullOrEmpty(previousConfigFileName))
			{
				string path1 = isRoaming ? ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigDirectory : ConfigurationManagerInternalFactory.Instance.ExeLocalConfigDirectory;
				Version version1 = this.CreateVersion(ConfigurationManagerInternalFactory.Instance.ExeProductVersion);
				Version version2 = (Version)null;
				DirectoryInfo directoryInfo = (DirectoryInfo)null;
				string path2 = (string)null;
				if (version1 == (Version)null)
					return (string)null;
				DirectoryInfo parent = Directory.GetParent(path1);
				if (parent.Exists)
				{
					foreach (DirectoryInfo directory in parent.GetDirectories())
					{
						Version version3 = this.CreateVersion(directory.Name);
						if (version3 != (Version)null && version3 < version1)
						{
							if (version2 == (Version)null)
							{
								version2 = version3;
								directoryInfo = directory;
							}
							else if (version3 > version2)
							{
								version2 = version3;
								directoryInfo = directory;
							}
						}
					}
					if (directoryInfo != null)
						path2 = Path.Combine(directoryInfo.FullName, ConfigurationManagerInternalFactory.Instance.UserConfigFilename);
					if (File.Exists(path2))
						previousConfigFileName = path2;
				}
				if (isRoaming)
					this._prevRoamingConfigFileName = previousConfigFileName;
				else
					this._prevLocalConfigFileName = previousConfigFileName;
			}
			return previousConfigFileName;
		}

		private string GetSectionName(SettingsContext context)
		{
			string str1 = (string)context[(object)"GroupName"];
			string str2 = (string)context[(object)"SettingsKey"];
			string name = str1;
			if (!string.IsNullOrEmpty(str2))
				name = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "{0}.{1}", new object[2]
				{
		  (object) name,
		  (object) str2
				});
			return XmlConvert.EncodeLocalName(name);
		}

		private SettingsPropertyValueCollection GetSettingValuesFromFile(
		  string configFileName,
		  string sectionName,
		  bool userScoped,
		  SettingsPropertyCollection properties)
		{
			SettingsPropertyValueCollection settingValuesFromFile = new SettingsPropertyValueCollection();
			IDictionary dictionary = ClientSettingsStore.ReadSettingsFromFile(configFileName, sectionName, userScoped);
			foreach (SettingsProperty property1 in properties)
			{
				string name = property1.Name;
				SettingsPropertyValue property2 = new SettingsPropertyValue(property1);
				if (dictionary.Contains((object)name))
				{
					StoredSetting storedSetting = (StoredSetting)dictionary[(object)name];
					string escapedString = storedSetting.Value.InnerXml;
					if (storedSetting.SerializeAs == SettingsSerializeAs.String)
						escapedString = this.Escaper.Unescape(escapedString);
					property2.SerializedValue = (object)escapedString;
					property2.IsDirty = true;
					settingValuesFromFile.Add(property2);
				}
			}
			return settingValuesFromFile;
		}

		private static bool IsRoamingSetting(SettingsProperty setting)
		{
			bool flag1 = true;// !ApplicationSettingsBase.IsClickOnceDeployed(AppDomain.CurrentDomain);
			bool flag2 = false;
			if (flag1)
				flag2 = setting.Attributes[(object)typeof(SettingsManageabilityAttribute)] is SettingsManageabilityAttribute attribute && (attribute.Manageability & SettingsManageability.Roaming) == SettingsManageability.Roaming;
			return flag2;
		}

		private bool IsUserSetting(SettingsProperty setting)
		{
			bool flag1 = setting.Attributes[(object)typeof(UserScopedSettingAttribute)] is UserScopedSettingAttribute;
			bool flag2 = setting.Attributes[(object)typeof(ApplicationScopedSettingAttribute)] is ApplicationScopedSettingAttribute;
			if (flag1 & flag2)
				throw new ConfigurationErrorsException(SR.GetString("BothScopeAttributes"));
			return flag1 | flag2 ? flag1 : throw new ConfigurationErrorsException(SR.GetString("NoScopeAttributes"));
		}

		private XmlNode SerializeToXmlElement(SettingsProperty setting, SettingsPropertyValue value)
		{
			string xmlString = null;
			XmlElement element = new XmlDocument().CreateElement(nameof(value));
			if (!(value.SerializedValue is string) && setting.SerializeAs == SettingsSerializeAs.Binary && value.SerializedValue is byte[] serializedValue)
				xmlString = Convert.ToBase64String(serializedValue);
			if (xmlString == null)
				xmlString = string.Empty;
			if (setting.SerializeAs == SettingsSerializeAs.String)
				xmlString = this.Escaper.Escape(xmlString);
			element.InnerXml = xmlString;
			XmlNode oldChild = (XmlNode)null;
			foreach (XmlNode childNode in element.ChildNodes)
			{
				if (childNode.NodeType == XmlNodeType.XmlDeclaration)
				{
					oldChild = childNode;
					break;
				}
			}
			if (oldChild != null)
				element.RemoveChild(oldChild);
			return (XmlNode)element;
		}

		[FileIOPermission(SecurityAction.Assert, AllFiles = FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery)]
		private void Upgrade(
		  SettingsContext context,
		  SettingsPropertyCollection properties,
		  bool isRoaming)
		{
			string previousConfigFileName = this.GetPreviousConfigFileName(isRoaming);
			if (string.IsNullOrEmpty(previousConfigFileName))
				return;
			SettingsPropertyCollection properties1 = new SettingsPropertyCollection();
			foreach (SettingsProperty property in properties)
			{
				if (!(property.Attributes[(object)typeof(NoSettingsVersionUpgradeAttribute)] is NoSettingsVersionUpgradeAttribute))
					properties1.Add(property);
			}
			SettingsPropertyValueCollection settingValuesFromFile = this.GetSettingValuesFromFile(previousConfigFileName, this.GetSectionName(context), true, properties1);
			this.SetPropertyValues(context, settingValuesFromFile);
		}

		private class XmlEscaper
		{
			private XmlDocument doc;
			private XmlElement temp;

			internal XmlEscaper()
			{
				this.doc = new XmlDocument();
				this.temp = this.doc.CreateElement(nameof(temp));
			}

			internal string Escape(string xmlString)
			{
				if (string.IsNullOrEmpty(xmlString))
					return xmlString;
				this.temp.InnerText = xmlString;
				return this.temp.InnerXml;
			}

			internal string Unescape(string escapedString)
			{
				if (string.IsNullOrEmpty(escapedString))
					return escapedString;
				this.temp.InnerXml = escapedString;
				return this.temp.InnerText;
			}
		}
	}
}
