// Decompiled with JetBrains decompiler
// Type: System.Configuration.ClientSettingsStore
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Collections;
using System.Configuration.Internal;
using System.IO;
using System.Security.Permissions;

#nullable disable
namespace System.Configuration
{
  internal sealed class ClientSettingsStore
  {
    private const string ApplicationSettingsGroupName = "applicationSettings";
    private const string UserSettingsGroupName = "userSettings";
    private const string ApplicationSettingsGroupPrefix = "applicationSettings/";
    private const string UserSettingsGroupPrefix = "userSettings/";

    private System.Configuration.Configuration GetUserConfig(bool isRoaming)
    {
      return ClientSettingsStore.ClientSettingsConfigurationHost.OpenExeConfiguration(isRoaming ? ConfigurationUserLevel.PerUserRoaming : ConfigurationUserLevel.PerUserRoamingAndLocal);
    }

    private ClientSettingsSection GetConfigSection(
      System.Configuration.Configuration config,
      string sectionName,
      bool declare)
    {
      string sectionName1 = "userSettings/" + sectionName;
      ClientSettingsSection configSection = (ClientSettingsSection) null;
      if (config != null)
      {
        configSection = config.GetSection(sectionName1) as ClientSettingsSection;
        if (configSection == null & declare)
        {
          this.DeclareSection(config, sectionName);
          configSection = config.GetSection(sectionName1) as ClientSettingsSection;
        }
      }
      return configSection;
    }

    private void DeclareSection(System.Configuration.Configuration config, string sectionName)
    {
      if (config.GetSectionGroup("userSettings") == null)
      {
        ConfigurationSectionGroup sectionGroup = (ConfigurationSectionGroup) new UserSettingsGroup();
        config.SectionGroups.Add("userSettings", sectionGroup);
      }
      ConfigurationSectionGroup sectionGroup1 = config.GetSectionGroup("userSettings");
      if (sectionGroup1 == null || sectionGroup1.Sections[sectionName] != null)
        return;
      ConfigurationSection section = (ConfigurationSection) new ClientSettingsSection();
      section.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
      section.SectionInformation.RequirePermission = false;
      sectionGroup1.Sections.Add(sectionName, section);
    }

    internal IDictionary ReadSettings(string sectionName, bool isUserScoped)
    {
      IDictionary dictionary = (IDictionary) new Hashtable();
      if (isUserScoped && !ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
        return dictionary;
      string str = isUserScoped ? "userSettings/" : "applicationSettings/";
      ConfigurationManager.RefreshSection(str + sectionName);
      if (ConfigurationManager.GetSection(str + sectionName) is ClientSettingsSection section)
      {
        foreach (SettingElement setting in (ConfigurationElementCollection) section.Settings)
          dictionary[(object) setting.Name] = (object) new StoredSetting(setting.SerializeAs, setting.Value.ValueXml);
      }
      return dictionary;
    }

    internal static IDictionary ReadSettingsFromFile(
      string configFileName,
      string sectionName,
      bool isUserScoped)
    {
      IDictionary dictionary = (IDictionary) new Hashtable();
      if (isUserScoped && !ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
        return dictionary;
      string str = isUserScoped ? "userSettings/" : "applicationSettings/";
      ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
      ConfigurationUserLevel userLevel = isUserScoped ? ConfigurationUserLevel.PerUserRoaming : ConfigurationUserLevel.None;
      if (isUserScoped)
      {
        fileMap.ExeConfigFilename = ConfigurationManagerInternalFactory.Instance.ApplicationConfigUri;
        fileMap.RoamingUserConfigFilename = configFileName;
      }
      else
        fileMap.ExeConfigFilename = configFileName;
      if (ConfigurationManager.OpenMappedExeConfiguration(fileMap, userLevel).GetSection(str + sectionName) is ClientSettingsSection section)
      {
        foreach (SettingElement setting in (ConfigurationElementCollection) section.Settings)
          dictionary[(object) setting.Name] = (object) new StoredSetting(setting.SerializeAs, setting.Value.ValueXml);
      }
      return dictionary;
    }

    internal ConnectionStringSettingsCollection ReadConnectionStrings()
    {
      return PrivilegedConfigurationManager.ConnectionStrings;
    }

    internal void RevertToParent(string sectionName, bool isRoaming)
    {
      if (!ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
        throw new ConfigurationErrorsException(SR.GetString("UserSettingsNotSupported"));
      System.Configuration.Configuration userConfig = this.GetUserConfig(isRoaming);
      ClientSettingsSection configSection = this.GetConfigSection(userConfig, sectionName, false);
      if (configSection == null)
        return;
      configSection.SectionInformation.RevertToParent();
      userConfig.Save();
    }

    internal void WriteSettings(string sectionName, bool isRoaming, IDictionary newSettings)
    {
      if (!ConfigurationManagerInternalFactory.Instance.SupportsUserConfig)
        throw new ConfigurationErrorsException(SR.GetString("UserSettingsNotSupported"));
      System.Configuration.Configuration userConfig = this.GetUserConfig(isRoaming);
      SettingElementCollection settings = (this.GetConfigSection(userConfig, sectionName, true) ?? throw new ConfigurationErrorsException(SR.GetString("SettingsSaveFailedNoSection"))).Settings;
      foreach (DictionaryEntry newSetting in newSettings)
      {
        SettingElement element = settings.Get((string) newSetting.Key);
        if (element == null)
        {
          element = new SettingElement();
          element.Name = (string) newSetting.Key;
          settings.Add(element);
        }
        StoredSetting storedSetting = (StoredSetting) newSetting.Value;
        element.SerializeAs = storedSetting.SerializeAs;
        element.Value.ValueXml = storedSetting.Value;
      }
      try
      {
        userConfig.Save();
      }
      catch (ConfigurationErrorsException ex)
      {
        throw new ConfigurationErrorsException(SR.GetString("SettingsSaveFailed", (object) ex.Message), (Exception) ex);
      }
    }

    private sealed class ClientSettingsConfigurationHost : DelegatingConfigHost
    {
      private const string ClientConfigurationHostTypeName = "System.Configuration.ClientConfigurationHost,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
      private const string InternalConfigConfigurationFactoryTypeName = "System.Configuration.Internal.InternalConfigConfigurationFactory,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
      private static volatile IInternalConfigConfigurationFactory s_configFactory;

      private IInternalConfigClientHost ClientHost => (IInternalConfigClientHost) this.Host;

      internal static IInternalConfigConfigurationFactory ConfigFactory
      {
        get
        {
          if (ClientSettingsStore.ClientSettingsConfigurationHost.s_configFactory == null)
            ClientSettingsStore.ClientSettingsConfigurationHost.s_configFactory = (IInternalConfigConfigurationFactory) TypeUtil.CreateInstanceWithReflectionPermission("System.Configuration.Internal.InternalConfigConfigurationFactory,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
          return ClientSettingsStore.ClientSettingsConfigurationHost.s_configFactory;
        }
      }

      private ClientSettingsConfigurationHost()
      {
      }

      public override void Init(IInternalConfigRoot configRoot, params object[] hostInitParams)
      {
      }

      public override void InitForConfiguration(
        ref string locationSubPath,
        out string configPath,
        out string locationConfigPath,
        IInternalConfigRoot configRoot,
        params object[] hostInitConfigurationParams)
      {
        ConfigurationUserLevel configurationParam = (ConfigurationUserLevel) hostInitConfigurationParams[0];
        this.Host = (IInternalConfigHost) TypeUtil.CreateInstanceWithReflectionPermission("System.Configuration.ClientConfigurationHost,System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        string str;
        switch (configurationParam)
        {
          case ConfigurationUserLevel.None:
            str = this.ClientHost.GetExeConfigPath();
            break;
          case ConfigurationUserLevel.PerUserRoaming:
            str = this.ClientHost.GetRoamingUserConfigPath();
            break;
          case ConfigurationUserLevel.PerUserRoamingAndLocal:
            str = this.ClientHost.GetLocalUserConfigPath();
            break;
          default:
            throw new ArgumentException(SR.GetString("UnknownUserLevel"));
        }
        this.Host.InitForConfiguration(ref locationSubPath, out configPath, out locationConfigPath, configRoot, null, null, (object) str);
      }

      private bool IsKnownConfigFile(string filename)
      {
        return string.Equals(filename, ConfigurationManagerInternalFactory.Instance.MachineConfigPath, StringComparison.OrdinalIgnoreCase) || string.Equals(filename, ConfigurationManagerInternalFactory.Instance.ApplicationConfigUri, StringComparison.OrdinalIgnoreCase) || string.Equals(filename, ConfigurationManagerInternalFactory.Instance.ExeLocalConfigPath, StringComparison.OrdinalIgnoreCase) || string.Equals(filename, ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigPath, StringComparison.OrdinalIgnoreCase);
      }

      internal static System.Configuration.Configuration OpenExeConfiguration(
        ConfigurationUserLevel userLevel)
      {
        return ClientSettingsStore.ClientSettingsConfigurationHost.ConfigFactory.Create(typeof (ClientSettingsStore.ClientSettingsConfigurationHost), (object) userLevel);
      }

      public override Stream OpenStreamForRead(string streamName)
      {
        return this.IsKnownConfigFile(streamName) ? this.Host.OpenStreamForRead(streamName, true) : this.Host.OpenStreamForRead(streamName);
      }

      public override Stream OpenStreamForWrite(
        string streamName,
        string templateStreamName,
        ref object writeContext)
      {
        return !string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeLocalConfigPath, StringComparison.OrdinalIgnoreCase) ? (!string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigPath, StringComparison.OrdinalIgnoreCase) ? this.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext) : (Stream) new ClientSettingsStore.QuotaEnforcedStream(this.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext, true), true)) : (Stream) new ClientSettingsStore.QuotaEnforcedStream(this.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext, true), false);
      }

      public override void WriteCompleted(string streamName, bool success, object writeContext)
      {
        if (string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeLocalConfigPath, StringComparison.OrdinalIgnoreCase) || string.Equals(streamName, ConfigurationManagerInternalFactory.Instance.ExeRoamingConfigPath, StringComparison.OrdinalIgnoreCase))
          this.Host.WriteCompleted(streamName, success, writeContext, true);
        else
          this.Host.WriteCompleted(streamName, success, writeContext);
      }
    }

    private sealed class QuotaEnforcedStream : Stream
    {
      private Stream _originalStream;
      private bool _isRoaming;

      internal QuotaEnforcedStream(Stream originalStream, bool isRoaming)
      {
        this._originalStream = originalStream;
        this._isRoaming = isRoaming;
      }

      public override bool CanRead => this._originalStream.CanRead;

      public override bool CanWrite => this._originalStream.CanWrite;

      public override bool CanSeek => this._originalStream.CanSeek;

      public override long Length => this._originalStream.Length;

      public override long Position
      {
        get => this._originalStream.Position;
        set
        {
          if (value < 0L)
            throw new ArgumentOutOfRangeException(nameof (value), SR.GetString("PositionOutOfRange"));
          this.Seek(value, SeekOrigin.Begin);
        }
      }

      public override void Close() => this._originalStream.Close();

      protected override void Dispose(bool disposing)
      {
        if (disposing && this._originalStream != null)
        {
          this._originalStream.Dispose();
          this._originalStream = (Stream) null;
        }
        base.Dispose(disposing);
      }

      public override void Flush() => this._originalStream.Flush();

      public override void SetLength(long value)
      {
        this.EnsureQuota(Math.Max(this._originalStream.Length, value));
        this._originalStream.SetLength(value);
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        return this._originalStream.Read(buffer, offset, count);
      }

      public override int ReadByte() => this._originalStream.ReadByte();

      public override long Seek(long offset, SeekOrigin origin)
      {
        if (!this.CanSeek)
          throw new NotSupportedException();
        long length = this._originalStream.Length;
        long val2;
        switch (origin)
        {
          case SeekOrigin.Begin:
            val2 = offset;
            break;
          case SeekOrigin.Current:
            val2 = this._originalStream.Position + offset;
            break;
          case SeekOrigin.End:
            val2 = length + offset;
            break;
          default:
            throw new ArgumentException(SR.GetString("UnknownSeekOrigin"), nameof (origin));
        }
        this.EnsureQuota(Math.Max(length, val2));
        return this._originalStream.Seek(offset, origin);
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        if (!this.CanWrite)
          throw new NotSupportedException();
        this.EnsureQuota(Math.Max(this._originalStream.Length, this._originalStream.CanSeek ? this._originalStream.Position + (long) count : this._originalStream.Length + (long) count));
        this._originalStream.Write(buffer, offset, count);
      }

      public override void WriteByte(byte value)
      {
        if (!this.CanWrite)
          throw new NotSupportedException();
        this.EnsureQuota(Math.Max(this._originalStream.Length, this._originalStream.CanSeek ? this._originalStream.Position + 1L : this._originalStream.Length + 1L));
        this._originalStream.WriteByte(value);
      }

      public override IAsyncResult BeginRead(
        byte[] buffer,
        int offset,
        int numBytes,
        AsyncCallback userCallback,
        object stateObject)
      {
        return this._originalStream.BeginRead(buffer, offset, numBytes, userCallback, stateObject);
      }

      public override int EndRead(IAsyncResult asyncResult)
      {
        return this._originalStream.EndRead(asyncResult);
      }

      public override IAsyncResult BeginWrite(
        byte[] buffer,
        int offset,
        int numBytes,
        AsyncCallback userCallback,
        object stateObject)
      {
        if (!this.CanWrite)
          throw new NotSupportedException();
        this.EnsureQuota(Math.Max(this._originalStream.Length, this._originalStream.CanSeek ? this._originalStream.Position + (long) numBytes : this._originalStream.Length + (long) numBytes));
        return this._originalStream.BeginWrite(buffer, offset, numBytes, userCallback, stateObject);
      }

      public override void EndWrite(IAsyncResult asyncResult)
      {
        this._originalStream.EndWrite(asyncResult);
      }

      private void EnsureQuota(long size)
      {
        IsolatedStoragePermission storagePermission = (IsolatedStoragePermission) new IsolatedStorageFilePermission(PermissionState.None);
        storagePermission.UserQuota = size;
        storagePermission.UsageAllowed = this._isRoaming ? IsolatedStorageContainment.DomainIsolationByRoamingUser : IsolatedStorageContainment.DomainIsolationByUser;
        storagePermission.Demand();
      }
    }
  }
}
