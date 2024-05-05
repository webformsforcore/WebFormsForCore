// Decompiled with JetBrains decompiler
// Type: System.Net.Configuration.RequestCachingSection
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

using System.Configuration;
using System.Net.Cache;
using System.Xml;

#nullable disable
namespace System.Net.Configuration
{
	/// <summary>Represents the configuration section for cache behavior. This class cannot be inherited.</summary>
	public sealed class RequestCachingSection : ConfigurationSection
	{
		private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
		private readonly ConfigurationProperty defaultHttpCachePolicy = new ConfigurationProperty(nameof(defaultHttpCachePolicy), typeof(HttpCachePolicyElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty defaultFtpCachePolicy = new ConfigurationProperty(nameof(defaultFtpCachePolicy), typeof(FtpCachePolicyElement), (object)null, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty defaultPolicyLevel = new ConfigurationProperty(nameof(defaultPolicyLevel), typeof(RequestCacheLevel), (object)RequestCacheLevel.BypassCache, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty disableAllCaching = new ConfigurationProperty(nameof(disableAllCaching), typeof(bool), (object)false, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty isPrivateCache = new ConfigurationProperty(nameof(isPrivateCache), typeof(bool), (object)true, ConfigurationPropertyOptions.None);
		private readonly ConfigurationProperty unspecifiedMaximumAge = new ConfigurationProperty(nameof(unspecifiedMaximumAge), typeof(TimeSpan), (object)TimeSpan.FromDays(1.0), ConfigurationPropertyOptions.None);

		/// <summary>Initializes a new instance of the <see cref="T:System.Net.Configuration.RequestCachingSection" /> class.</summary>
		public RequestCachingSection()
		{
			this.properties.Add(this.disableAllCaching);
			this.properties.Add(this.defaultPolicyLevel);
			this.properties.Add(this.isPrivateCache);
			this.properties.Add(this.defaultHttpCachePolicy);
			this.properties.Add(this.defaultFtpCachePolicy);
			this.properties.Add(this.unspecifiedMaximumAge);
		}

		/// <summary>Gets the default caching behavior for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.HttpCachePolicyElement" /> that defines the default cache policy.</returns>
		[ConfigurationProperty("defaultHttpCachePolicy")]
		public HttpCachePolicyElement DefaultHttpCachePolicy
		{
			get => (HttpCachePolicyElement)this[this.defaultHttpCachePolicy];
		}

		/// <summary>Gets the default FTP caching behavior for the local computer.</summary>
		/// <returns>A <see cref="T:System.Net.Configuration.FtpCachePolicyElement" /> that defines the default cache policy.</returns>
		[ConfigurationProperty("defaultFtpCachePolicy")]
		public FtpCachePolicyElement DefaultFtpCachePolicy
		{
			get => (FtpCachePolicyElement)this[this.defaultFtpCachePolicy];
		}

		/// <summary>Gets or sets the default cache policy level.</summary>
		/// <returns>A <see cref="T:System.Net.Cache.RequestCacheLevel" /> enumeration value.</returns>
		[ConfigurationProperty("defaultPolicyLevel", DefaultValue = RequestCacheLevel.BypassCache)]
		public RequestCacheLevel DefaultPolicyLevel
		{
			get => (RequestCacheLevel)this[this.defaultPolicyLevel];
			set => this[this.defaultPolicyLevel] = (object)value;
		}

		/// <summary>Gets or sets a Boolean value that enables caching on the local computer.</summary>
		/// <returns>
		/// <see langword="true" /> if caching is disabled on the local computer; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("disableAllCaching", DefaultValue = false)]
		public bool DisableAllCaching
		{
			get => (bool)this[this.disableAllCaching];
			set => this[this.disableAllCaching] = (object)value;
		}

		/// <summary>Gets or sets a Boolean value that indicates whether the local computer cache is private.</summary>
		/// <returns>
		/// <see langword="true" /> if the cache provides user isolation; otherwise, <see langword="false" />.</returns>
		[ConfigurationProperty("isPrivateCache", DefaultValue = true)]
		public bool IsPrivateCache
		{
			get => (bool)this[this.isPrivateCache];
			set => this[this.isPrivateCache] = (object)value;
		}

		/// <summary>Gets or sets a value used as the maximum age for cached resources that do not have expiration information.</summary>
		/// <returns>A <see cref="T:System.TimeSpan" /> that provides a default maximum age for cached resources.</returns>
		[ConfigurationProperty("unspecifiedMaximumAge", DefaultValue = "1.00:00:00")]
		public TimeSpan UnspecifiedMaximumAge
		{
			get => (TimeSpan)this[this.unspecifiedMaximumAge];
			set => this[this.unspecifiedMaximumAge] = (object)value;
		}

		protected internal override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			bool disableAllCaching = this.DisableAllCaching;
			base.DeserializeElement(reader, serializeCollectionKey);
			if (!disableAllCaching)
				return;
			this.DisableAllCaching = true;
		}

		protected override void PostDeserialize()
		{
			if (this.EvaluationContext.IsMachineLevel)
				return;
			try
			{
				ExceptionHelper.WebPermissionUnrestricted.Demand();
			}
			catch (Exception ex)
			{
				throw new ConfigurationErrorsException(SR.GetString("net_config_section_permission", (object)"requestCaching"), ex);
			}
		}

		protected internal override ConfigurationPropertyCollection Properties => this.properties;
	}
}
