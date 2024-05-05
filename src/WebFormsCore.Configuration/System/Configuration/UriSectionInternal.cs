// Decompiled with JetBrains decompiler
// Type: System.Configuration.UriSectionInternal
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

#nullable disable
namespace System.Configuration
{
	internal sealed class UriSectionInternal
	{
		private static readonly object classSyncObject = new object();
		private UriIdnScope idnScope;
		private bool iriParsing;
		private Dictionary<string, SchemeSettingInternal> schemeSettings;

		private UriSectionInternal()
		{
			this.schemeSettings = new Dictionary<string, SchemeSettingInternal>();
		}

		private UriSectionInternal(UriSection section)
		  : this()
		{
			this.idnScope = section.Idn.Enabled;
			this.iriParsing = section.IriParsing.Enabled;
			if (section.SchemeSettings == null)
				return;
			foreach (SchemeSettingElement schemeSetting in (ConfigurationElementCollection)section.SchemeSettings)
			{
				SchemeSettingInternal schemeSettingInternal = new SchemeSettingInternal(schemeSetting.Name, schemeSetting.GenericUriParserOptions);
				this.schemeSettings.Add(schemeSettingInternal.Name, schemeSettingInternal);
			}
		}

		private UriSectionInternal(
		  UriIdnScope idnScope,
		  bool iriParsing,
		  IEnumerable<SchemeSettingInternal> schemeSettings)
		  : this()
		{
			this.idnScope = idnScope;
			this.iriParsing = iriParsing;
			if (schemeSettings == null)
				return;
			foreach (SchemeSettingInternal schemeSetting in schemeSettings)
				this.schemeSettings.Add(schemeSetting.Name, schemeSetting);
		}

		internal UriIdnScope IdnScope => this.idnScope;

		internal bool IriParsing => this.iriParsing;

		internal SchemeSettingInternal GetSchemeSetting(string scheme)
		{
			SchemeSettingInternal schemeSettingInternal;
			return this.schemeSettings.TryGetValue(scheme.ToLowerInvariant(), out schemeSettingInternal) ? schemeSettingInternal : (SchemeSettingInternal)null;
		}

		internal static UriSectionInternal GetSection()
		{
			lock (UriSectionInternal.classSyncObject)
			{
				string str = (string)null;
				new FileIOPermission(PermissionState.Unrestricted).Assert();
				try
				{
					//str = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
				}
				finally
				{
					CodeAccessPermission.RevertAssert();
				}
				return UriSectionInternal.LoadUsingSystemConfiguration();
				// UriSectionInternal.IsWebConfig(str) ? UriSectionInternal.LoadUsingSystemConfiguration() : UriSectionInternal.LoadUsingCustomParser(str);
			}
		}

		private static UriSectionInternal LoadUsingSystemConfiguration()
		{
			try
			{
				return !(PrivilegedConfigurationManager.GetSection("uri") is UriSection section) ? (UriSectionInternal)null : new UriSectionInternal(section);
			}
			catch (ConfigurationException ex)
			{
				return (UriSectionInternal)null;
			}
		}

		private static UriSectionInternal LoadUsingCustomParser(string appConfigFilePath)
		{
			string path1 = (string)null;
			new FileIOPermission(PermissionState.Unrestricted).Assert();
			try
			{
				path1 = RuntimeEnvironment.GetRuntimeDirectory();
			}
			finally
			{
				CodeAccessPermission.RevertAssert();
			}
			UriSectionData parentData = UriSectionReader.Read(Path.Combine(Path.Combine(path1, "Config"), "machine.config"));
			UriSectionData uriSectionData1 = UriSectionReader.Read(appConfigFilePath, parentData);
			UriSectionData uriSectionData2 = (UriSectionData)null;
			if (uriSectionData1 != null)
				uriSectionData2 = uriSectionData1;
			else if (parentData != null)
				uriSectionData2 = parentData;
			return uriSectionData2 != null ? new UriSectionInternal(uriSectionData2.IdnScope.GetValueOrDefault(), uriSectionData2.IriParsing.GetValueOrDefault(), (IEnumerable<SchemeSettingInternal>)uriSectionData2.SchemeSettings.Values) : (UriSectionInternal)null;
		}

		private static bool IsWebConfig(string appConfigFile)
		{
			return AppDomain.CurrentDomain.GetData(".appVPath") is string || appConfigFile != null && (appConfigFile.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || appConfigFile.StartsWith("https://", StringComparison.OrdinalIgnoreCase));
		}
	}
}
