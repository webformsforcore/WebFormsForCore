
using System.Configuration;
using System.Net.Cache;
using System.Xml;

#nullable disable
namespace System.Net.Configuration
{
  /// <summary>Represents the default HTTP cache policy for network resources. This class cannot be inherited.</summary>
  public sealed class HttpCachePolicyElement : ConfigurationElement
  {
    private bool wasReadFromConfig;
    private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty maximumAge = new ConfigurationProperty(nameof (maximumAge), typeof (TimeSpan), (object) TimeSpan.MaxValue, ConfigurationPropertyOptions.None);
    private readonly ConfigurationProperty maximumStale = new ConfigurationProperty(nameof (maximumStale), typeof (TimeSpan), (object) TimeSpan.MinValue, ConfigurationPropertyOptions.None);
    private readonly ConfigurationProperty minimumFresh = new ConfigurationProperty(nameof (minimumFresh), typeof (TimeSpan), (object) TimeSpan.MinValue, ConfigurationPropertyOptions.None);
    private readonly ConfigurationProperty policyLevel = new ConfigurationProperty(nameof (policyLevel), typeof (HttpRequestCacheLevel), (object) HttpRequestCacheLevel.Default, ConfigurationPropertyOptions.None);

    /// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.HttpCachePolicyElement" /> class.</summary>
    public HttpCachePolicyElement()
    {
      this.properties.Add(this.maximumAge);
      this.properties.Add(this.maximumStale);
      this.properties.Add(this.minimumFresh);
      this.properties.Add(this.policyLevel);
    }

		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets the maximum age permitted for a resource returned from the cache.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> value that specifies the maximum age for cached resources specified in the configuration file.</returns>
		[ConfigurationProperty("maximumAge", DefaultValue = "10675199.02:48:05.4775807")]
    public TimeSpan MaximumAge
    {
      get => (TimeSpan) this[this.maximumAge];
      set => this[this.maximumAge] = (object) value;
    }

    /// <summary>Gets or sets the maximum staleness value permitted for a resource returned from the cache.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> value that is set to the maximum staleness value specified in the configuration file.</returns>
    [ConfigurationProperty("maximumStale", DefaultValue = "-10675199.02:48:05.4775808")]
    public TimeSpan MaximumStale
    {
      get => (TimeSpan) this[this.maximumStale];
      set => this[this.maximumStale] = (object) value;
    }

    /// <summary>Gets or sets the minimum freshness permitted for a resource returned from the cache.</summary>
    /// <returns>A <see cref="T:System.TimeSpan" /> value that specifies the minimum freshness specified in the configuration file.</returns>
    [ConfigurationProperty("minimumFresh", DefaultValue = "-10675199.02:48:05.4775808")]
    public TimeSpan MinimumFresh
    {
      get => (TimeSpan) this[this.minimumFresh];
      set => this[this.minimumFresh] = (object) value;
    }

    /// <summary>Gets or sets HTTP caching behavior for the local machine.</summary>
    /// <returns>A <see cref="T:System.Net.Cache.HttpRequestCacheLevel" /> value that specifies the cache behavior.</returns>
    [ConfigurationProperty("policyLevel", IsRequired = true, DefaultValue = HttpRequestCacheLevel.Default)]
    public HttpRequestCacheLevel PolicyLevel
    {
      get => (HttpRequestCacheLevel) this[this.policyLevel];
      set => this[this.policyLevel] = (object) value;
    }

    protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
    {
      this.wasReadFromConfig = true;
      base.DeserializeElement(reader, serializeCollectionKey);
    }

    protected internal override void Reset(ConfigurationElement parentElement)
    {
      if (parentElement != null)
        this.wasReadFromConfig = ((HttpCachePolicyElement) parentElement).wasReadFromConfig;
      base.Reset(parentElement);
    }

    internal bool WasReadFromConfig => this.wasReadFromConfig;
  }
}
