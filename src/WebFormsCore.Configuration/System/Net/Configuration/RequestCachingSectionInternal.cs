
using Microsoft.Win32;
using System.Configuration;
using System.Net.Cache;
using System.Threading;

#nullable disable
namespace System.Net.Configuration
{
	internal sealed class RequestCachingSectionInternal
	{
#if !WebFormsCore
		private RequestCache defaultCache;
		private HttpRequestCacheValidator httpRequestCacheValidator;
		private FtpRequestCacheValidator ftpRequestCacheValidator;
#endif
		private static object classSyncObject;
		private HttpRequestCachePolicy defaultHttpCachePolicy;
		private RequestCachePolicy defaultFtpCachePolicy;
		private RequestCachePolicy defaultCachePolicy;
		private bool disableAllCaching;
		private bool isPrivateCache;
		private TimeSpan unspecifiedMaximumAge;

		private RequestCachingSectionInternal()
		{
		}

		internal RequestCachingSectionInternal(RequestCachingSection section)
		{
			if (!section.DisableAllCaching)
			{
				this.defaultCachePolicy = new RequestCachePolicy(section.DefaultPolicyLevel);
				this.isPrivateCache = section.IsPrivateCache;
				this.unspecifiedMaximumAge = section.UnspecifiedMaximumAge;
			}
			else
				this.disableAllCaching = true;
#if !WebFormsCore
			this.httpRequestCacheValidator = new HttpRequestCacheValidator(false, this.UnspecifiedMaximumAge);
			this.ftpRequestCacheValidator = new FtpRequestCacheValidator(false, this.UnspecifiedMaximumAge);
			this.defaultCache = (RequestCache)new WinInetCache(this.IsPrivateCache, true, true);
#endif
			if (section.DisableAllCaching)
				return;
			HttpCachePolicyElement defaultHttpCachePolicy = section.DefaultHttpCachePolicy;
			if (defaultHttpCachePolicy.WasReadFromConfig)
				this.defaultHttpCachePolicy = defaultHttpCachePolicy.PolicyLevel != HttpRequestCacheLevel.Default ? new HttpRequestCachePolicy(defaultHttpCachePolicy.PolicyLevel) : new HttpRequestCachePolicy(defaultHttpCachePolicy.MinimumFresh != TimeSpan.MinValue ? HttpCacheAgeControl.MaxAgeAndMinFresh : HttpCacheAgeControl.MaxAgeAndMaxStale, defaultHttpCachePolicy.MaximumAge, defaultHttpCachePolicy.MinimumFresh != TimeSpan.MinValue ? defaultHttpCachePolicy.MinimumFresh : defaultHttpCachePolicy.MaximumStale);
			FtpCachePolicyElement defaultFtpCachePolicy = section.DefaultFtpCachePolicy;
			if (!defaultFtpCachePolicy.WasReadFromConfig)
				return;
			this.defaultFtpCachePolicy = new RequestCachePolicy(defaultFtpCachePolicy.PolicyLevel);
		}

		internal static object ClassSyncObject
		{
			get
			{
				if (RequestCachingSectionInternal.classSyncObject == null)
				{
					object obj = new object();
					Interlocked.CompareExchange(ref RequestCachingSectionInternal.classSyncObject, obj, (object)null);
				}
				return RequestCachingSectionInternal.classSyncObject;
			}
		}

		internal bool DisableAllCaching => this.disableAllCaching;

#if !WebFormsCore
		internal RequestCache DefaultCache => this.defaultCache;
#endif

		internal RequestCachePolicy DefaultCachePolicy => this.defaultCachePolicy;

		internal bool IsPrivateCache => this.isPrivateCache;

		internal TimeSpan UnspecifiedMaximumAge => this.unspecifiedMaximumAge;

		internal HttpRequestCachePolicy DefaultHttpCachePolicy => this.defaultHttpCachePolicy;

		internal RequestCachePolicy DefaultFtpCachePolicy => this.defaultFtpCachePolicy;

#if !WebFormsCore
		internal HttpRequestCacheValidator DefaultHttpValidator => this.httpRequestCacheValidator;

		internal FtpRequestCacheValidator DefaultFtpValidator => this.ftpRequestCacheValidator;
#endif

		internal static RequestCachingSectionInternal GetSection()
		{
			lock (RequestCachingSectionInternal.ClassSyncObject)
			{
				if (!(PrivilegedConfigurationManager.GetSection(ConfigurationStrings.RequestCachingSectionPath) is RequestCachingSection section))
					return (RequestCachingSectionInternal)null;
				try
				{
					return new RequestCachingSectionInternal(section);
				}
				catch (Exception ex)
				{
					if (!NclUtilities.IsFatal(ex))
						throw new ConfigurationErrorsException(SR.GetString("net_config_requestcaching"), ex);
					throw;
				}
			}
		}
	}
}
