using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
//using Microsoft.AspNet.FriendlyUrls;

namespace WebFormsCore.TestApp
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
#if false
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);
#endif
        }
    }
}
