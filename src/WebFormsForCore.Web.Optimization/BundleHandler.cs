// Copyright (c) Microsoft Corporation, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Web.Hosting;

namespace System.Web.Optimization {
    internal sealed class BundleHandler : IHttpHandler {
        public BundleHandler(Bundle requestBundle, string bundleVirtualPath) {
            // NOTE: requestBundle is never null
            RequestBundle = requestBundle;
            BundleVirtualPath = bundleVirtualPath;
        }

        public Bundle RequestBundle { get; private set; }
        public string BundleVirtualPath { get; private set; }

        public bool IsReusable {
            get { return false; }
        }

        internal static string GetBundleUrlFromContext(HttpContextBase context) {
            return context.Request.AppRelativeCurrentExecutionFilePath + context.Request.PathInfo;
        }

        internal static bool RemapHandlerForBundleRequests(HttpApplication app) {
            HttpContextBase context = new HttpContextWrapper(app.Context);

            // Don't block requests to existing files or directories
            string requestPath = context.Request.AppRelativeCurrentExecutionFilePath;
            VirtualPathProvider vpp = HostingEnvironment.VirtualPathProvider;
            if (vpp.FileExists(requestPath) || vpp.DirectoryExists(requestPath)) {
                return false;
            }

            string bundleRequestPath = GetBundleUrlFromContext(context);

            // Check if this request matches a bundle in the app
            Bundle requestBundle = BundleTable.Bundles.GetBundleFor(bundleRequestPath);
            if (requestBundle != null) {
                context.RemapHandler(new BundleHandler(requestBundle, bundleRequestPath));
                return true;
            }

            return false;
        }

        public void ProcessRequest(HttpContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            // Make sure we don't get any extra content in this handler (like Application.BeginRequest stuff);
            context.Response.Clear();
            BundleContext bundleContext = new BundleContext(new HttpContextWrapper(context), BundleTable.Bundles, BundleVirtualPath);
            RequestBundle.ProcessRequest(bundleContext);
        }
    }
}
