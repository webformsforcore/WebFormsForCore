using System.ComponentModel;
using System.ComponentModel.Design;
using System.Security.Permissions;

#nullable disable
namespace System.Web.UI.Design
{

	public enum UrlBuilderOptions { None }
	/// <summary>Starts a URL editor that allows a user to select or create a URL. This class cannot be inherited.</summary>
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
	public sealed class UrlBuilder
	{
		private UrlBuilder()
		{
		}

		/// <summary>Creates a UI to create or pick a URL.</summary>
		/// <param name="component">The <see cref="T:System.ComponentModel.IComponent" /> whose site is to be used to access design-time services.</param>
		/// <param name="initialUrl">The initial URL to be shown in the picker window.</param>
		/// <param name="caption">The caption of the picker window.</param>
		/// <param name="filter">The filter string to use to optionally filter the files displayed in the picker window.</param>
		/// <returns>The URL returned from the UI.</returns>
		public static string BuildUrl(
		  IComponent component,
		  string initialUrl,
		  string caption,
		  string filter)
		{
			return UrlBuilder.BuildUrl(component, initialUrl, caption, filter, UrlBuilderOptions.None);
		}

		/// <summary>Creates a UI to create or pick a URL, using the specified <see cref="T:System.Web.UI.Design.UrlBuilderOptions" /> object.</summary>
		/// <param name="component">The <see cref="T:System.ComponentModel.IComponent" /> whose site is to be used to access design-time services.</param>
		/// <param name="owner">The <see cref="T:System.Windows.Forms.Control" /> used as the parent for the picker window.</param>
		/// <param name="initialUrl">The initial URL to be shown in the picker window.</param>
		/// <param name="caption">The caption of the picker window.</param>
		/// <param name="filter">The filter string to use to optionally filter the files displayed in the picker window.</param>
		/// <param name="options">A <see cref="T:System.Web.UI.Design.UrlBuilderOptions" /> indicating the options for URL selection.</param>
		/// <returns>The URL returned from the UI.</returns>
		public static string BuildUrl(
		  IComponent component,
		  string initialUrl,
		  string caption,
		  string filter,
		  UrlBuilderOptions options)
		{
			ISite site = component.Site;
			return site == null ? (string)null : UrlBuilder.BuildUrl((IServiceProvider)site, initialUrl, caption, filter, options);
		}

		/// <summary>Creates a UI to create or pick a URL, using the specified <see cref="T:System.Web.UI.Design.UrlBuilderOptions" /> object.</summary>
		/// <param name="serviceProvider">The <see cref="T:System.IServiceProvider" /> to be used to access design-time services.</param>
		/// <param name="owner">The <see cref="T:System.Windows.Forms.Control" /> used as the parent for the picker window.</param>
		/// <param name="initialUrl">The initial URL to be shown in the picker window.</param>
		/// <param name="caption">The caption of the picker window.</param>
		/// <param name="filter">The filter string to use to optionally filter the files displayed in the picker window.</param>
		/// <param name="options">A <see cref="T:System.Web.UI.Design.UrlBuilderOptions" /> indicating the options for URL selection.</param>
		/// <returns>The URL returned from the UI.</returns>
		public static string BuildUrl(
		  IServiceProvider serviceProvider,
		  string initialUrl,
		  string caption,
		  string filter,
		  UrlBuilderOptions options)
		{
			string baseUrl = string.Empty;
			string str = (string)null;
#if NETFRAMEWORK
			IDesignerHost service1 = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			if (service1 != null && service1.GetDesigner(service1.RootComponent) is WebFormsRootDesigner designer)
				baseUrl = designer.DocumentUrl;
			if (baseUrl.Length == 0)
			{
				IWebFormsDocumentService service2 = (IWebFormsDocumentService)serviceProvider.GetService(typeof(IWebFormsDocumentService));
				if (service2 != null)
					baseUrl = service2.DocumentUrl;
			}
			IWebFormsBuilderUIService service3 = (IWebFormsBuilderUIService)serviceProvider.GetService(typeof(IWebFormsBuilderUIService));
			if (service3 != null)
				str = service3.BuildUrl(null, initialUrl, baseUrl, caption, filter, options);
#endif
			return str;
		}
	}
}
