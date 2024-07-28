
using System.Configuration;
using System.Net.Cache;
using System.Xml;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the default FTP cache policy for network resources. This class cannot be inherited.</summary>
	public sealed class FtpCachePolicyElement : ConfigurationElement
	{
		private bool wasReadFromConfig;
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty policyLevel = new ConfigurationProperty(nameof(policyLevel), typeof(RequestCacheLevel), (object)RequestCacheLevel.Default, ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.FtpCachePolicyElement" /> class.</summary>
		public FtpCachePolicyElement() => this.properties.Add(this.policyLevel);
		protected internal override ConfigurationPropertyCollection Properties => this.properties;

		/// <summary>Gets or sets FTP caching behavior for the local machine.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.RequestCacheLevel" /> value that specifies the cache behavior.</returns>
		[ConfigurationProperty("policyLevel", DefaultValue = RequestCacheLevel.Default)]
		public RequestCacheLevel PolicyLevel
		{
			get => (RequestCacheLevel)this[this.policyLevel];
			set => this[this.policyLevel] = (object)value;
		}

		protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			this.wasReadFromConfig = true;
			base.DeserializeElement(reader, serializeCollectionKey);
		}

		protected internal override void Reset(ConfigurationElement parentElement)
		{
			if (parentElement != null)
				this.wasReadFromConfig = ((FtpCachePolicyElement)parentElement).wasReadFromConfig;
			base.Reset(parentElement);
		}

		internal bool WasReadFromConfig => this.wasReadFromConfig;
	}
}
