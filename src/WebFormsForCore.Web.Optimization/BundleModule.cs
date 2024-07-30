// Copyright (c) Microsoft Corporation, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace System.Web.Optimization {
    /// <summary>
    /// <see cref="IHttpModule"/> that enables bundling to intercept and serve requests to bundle urls.
    /// </summary>
    public class BundleModule : IHttpModule {
        /// <summary>
        /// Dipose of any resources
        /// </summary>
        protected virtual void Dispose() {
        }

        /// <summary>
        /// Hooks the OnApplicationPostResolveRequestCachce event to remap to the bundle handler if the Http request is for a bundle.
        /// </summary>
        /// <param name="application"></param>
        protected virtual void Init(HttpApplication application) {
            if (application == null) {
                throw new ArgumentNullException("application");
            }

            application.PostResolveRequestCache += OnApplicationPostResolveRequestCache;
        }

        private void OnApplicationPostResolveRequestCache(object sender, EventArgs e) {
            HttpApplication app = (HttpApplication)sender;

            // If there are any bundles, see if its a bundle request first and don't do routing if so
            if (BundleTable.Bundles.Count > 0) {
                BundleHandler.RemapHandlerForBundleRequests(app);
            }
        }

        #region IHttpModule Members
        void IHttpModule.Dispose() {
            Dispose();
        }

        void IHttpModule.Init(HttpApplication application) {
            Init(application);
        }
        #endregion
    }
}
