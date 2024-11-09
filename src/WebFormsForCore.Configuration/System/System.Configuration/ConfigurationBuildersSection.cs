
using System.Collections.Specialized;
using System.Configuration.Internal;
using System.IO;
using System.Security.Permissions;

#nullable disable
namespace System.Configuration
{
  /// <summary>Provides programmatic access to the <see langword="&lt;configBuilders&gt;" /> section. This class can't be inherited.</summary>
  public sealed class ConfigurationBuildersSection : ConfigurationSection
  {
    private const string _ignoreLoadFailuresSwitch = "ConfigurationBuilders.IgnoreLoadFailure";
    private static ConfigurationPropertyCollection _properties;
    private static readonly ConfigurationProperty _propBuilders = new ConfigurationProperty("builders", typeof (ConfigurationBuilderSettings), (object) new ConfigurationBuilderSettings(), ConfigurationPropertyOptions.None);

    /// <summary>Returns a <see cref="T:System.Configuration.ConfigurationBuilder" /> object that has the provided configuration builder name.</summary>
    /// <param name="builderName">A configuration builder name or a comma-separated list of names. If <paramref name="builderName" /> is a comma-separated list of <see cref="T:System.Configuration.ConfigurationBuilder" /> names, a special aggregate <see cref="T:System.Configuration.ConfigurationBuilder" /> object that references and applies all named configuration builders is returned.</param>
    /// <returns>A <see cref="T:System.Configuration.ConfigurationBuilder" /> object that has the provided configuration <paramref name="builderName" />.</returns>
    /// <exception cref="T:System.Exception">A configuration provider type can't be instantiated under a partially trusted security policy (<see cref="T:System.Security.AllowPartiallyTrustedCallersAttribute" /> is not present on the target assembly).</exception>
    /// <exception cref="T:System.IO.FileNotFoundException">ConfigurationBuilders.IgnoreLoadFailure is disabled by default. If a bin-deployed configuration builder can't be found or instantiated for one of the sections read from the configuration file, a <see cref="T:System.IO.FileNotFoundException" /> is trapped and reported. If you wish to ignore load failures, enable ConfigurationBuilders.IgnoreLoadFailure.</exception>
    /// <exception cref="T:System.TypeLoadException">ConfigurationBuilders.IgnoreLoadFailure is disabled by default. While loading a configuration builder if a <see cref="T:System.TypeLoadException" /> occurs for one of the sections read from the configuration file, a <see cref="T:System.TypeLoadException" /> is trapped and reported. If you wish to ignore load failures, enable ConfigurationBuilders.IgnoreLoadFailure.</exception>
    public ConfigurationBuilder GetBuilderFromName(string builderName)
    {
      string[] strArray = builderName.Split(',');
      bool flag = AppDomain.CurrentDomain.GetData("ConfigurationBuilders.IgnoreLoadFailure") == null;
      if (strArray.Length == 1)
      {
        ProviderSettings builder = this.Builders[builderName];
        if (builder == null)
          throw new ConfigurationErrorsException(SR.GetString("Config_builder_not_found", (object) builderName));
        try
        {
          return this.InstantiateBuilder(builder);
        }
        catch (FileNotFoundException ex)
        {
          if (flag)
            throw;
        }
        catch (TypeLoadException ex)
        {
          if (flag)
            throw;
        }
        return (ConfigurationBuilder) null;
      }
      ConfigurationBuilderChain configurationBuilderChain = new ConfigurationBuilderChain();
      configurationBuilderChain.Initialize(builderName, (NameValueCollection) null);
      foreach (string str in strArray)
      {
        ProviderSettings builder = this.Builders[str.Trim()];
        if (builder == null)
          throw new ConfigurationErrorsException(SR.GetString("Config_builder_not_found", (object) str));
        try
        {
          configurationBuilderChain.Builders.Add(this.InstantiateBuilder(builder));
        }
        catch (FileNotFoundException ex)
        {
          if (flag)
            throw;
        }
        catch (TypeLoadException ex)
        {
          if (flag)
            throw;
        }
      }
      return configurationBuilderChain.Builders.Count == 0 ? (ConfigurationBuilder) null : (ConfigurationBuilder) configurationBuilderChain;
    }

    [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
    private ConfigurationBuilder CreateAndInitializeBuilderWithAssert(Type t, ProviderSettings ps)
    {
      ConfigurationBuilder reflectionPermission = (ConfigurationBuilder) TypeUtil.CreateInstanceWithReflectionPermission(t);
      NameValueCollection parameters = ps.Parameters;
      NameValueCollection config = new NameValueCollection(parameters.Count);
      foreach (string name in (NameObjectCollectionBase) parameters)
        config[name] = parameters[name];
      try
      {
        reflectionPermission.Initialize(ps.Name, config);
      }
      catch (Exception ex)
      {
        throw ExceptionUtil.WrapAsConfigException(SR.GetString("ConfigBuilder_init_error", (object) ps.Name), ex, (IConfigErrorInfo) null);
      }
      return reflectionPermission;
    }

    private ConfigurationBuilder InstantiateBuilder(ProviderSettings ps)
    {
      Type reflectionPermission = TypeUtil.GetTypeWithReflectionPermission(ps.Type, true);
      if (!typeof (ConfigurationBuilder).IsAssignableFrom(reflectionPermission))
        throw new ConfigurationErrorsException("[" + ps.Name + "] - " + SR.GetString("WrongType_of_config_builder"));
      return TypeUtil.IsTypeAllowedInConfig(reflectionPermission) ? this.CreateAndInitializeBuilderWithAssert(reflectionPermission, ps) : throw new ConfigurationErrorsException("[" + ps.Name + "] - " + SR.GetString("Type_from_untrusted_assembly", (object) reflectionPermission.FullName));
    }

    static ConfigurationBuildersSection()
    {
      ConfigurationBuildersSection._properties = new ConfigurationPropertyCollection();
      ConfigurationBuildersSection._properties.Add(ConfigurationBuildersSection._propBuilders);
    }

    protected internal override ConfigurationPropertyCollection Properties
    {
      get => ConfigurationBuildersSection._properties;
    }

    private ConfigurationBuilderSettings _Builders
    {
      get => (ConfigurationBuilderSettings) this[ConfigurationBuildersSection._propBuilders];
    }

    /// <summary>Gets a <see cref="T:System.Configuration.ConfigurationBuilderCollection" /> of all <see cref="T:System.Configuration.ConfigurationBuilder" /> objects in all participating configuration files.</summary>
    /// <returns>The <see cref="T:System.Configuration.ConfigurationBuilder" /> objects in all participating configuration files.</returns>
    [ConfigurationProperty("builders")]
    public ProviderSettingsCollection Builders => this._Builders.Builders;
  }
}
