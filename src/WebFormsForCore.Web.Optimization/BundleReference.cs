// Copyright (c) Microsoft Corporation, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Optimization;
using System.Web.UI;
using MapPathDelegate = System.Func<System.IServiceProvider, string, string>;

namespace Microsoft.AspNet.Web.Optimization.WebForms {
    [DefaultProperty("Path")]
    [NonVisualControl]
    public class BundleReference : Control {
        private const string PathKey = "Path";

        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue("")]
        public string Path {
            get {
                return ViewState[PathKey] as string ?? String.Empty;
            }
            set {
                ViewState[PathKey] = value;
            }
        }

        public override void RenderControl(HtmlTextWriter writer) {
            if (DesignMode) {
                writer.Write(GetDesignTimeHtml());
            }
            else {
                writer.Write(Styles.Render(Path));
            }
        }

        // Note: this will blow up in medium trust, so lazily get the delegate as it will only be invoked at design time (full trust)
        private static MapPathDelegate _mapPath;
        private static MapPathDelegate MapPath {
            get {
                if (_mapPath == null) {
                    // UrlPath.MapPath knows how to convert a virtual path or an app relative path to a physical path. 
                    var type = Type.GetType("System.Web.UI.Design.Util.UrlPath, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                    var method = type.GetMethod("MapPath", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { typeof(IServiceProvider), typeof(string) }, null);
                    _mapPath = (MapPathDelegate)Delegate.CreateDelegate(typeof(MapPathDelegate), method);
                }
                return _mapPath;
            }
        }

        private string GetDesignTimeHtml() {
            string bundlePath = MapPath(Site, BundleManifest.BundleManifestPath);
            if (String.IsNullOrEmpty(bundlePath)) {
                return null;
            }

            BundleManifest bundleManfiest;
            using (var stream = File.OpenRead(bundlePath)) {
                bundleManfiest = BundleManifest.ReadBundleManifest(stream);
            }

            var bundle = bundleManfiest.StyleBundles.FirstOrDefault(b => b.Path.Equals(Path, StringComparison.OrdinalIgnoreCase));
            if (bundle != null) {
                var builder = new StringBuilder();
                foreach (var item in bundle.Includes.Select(ResolveClientUrl)) {
                    builder.AppendFormat(@"<link href=""{0}"" rel=""stylesheet""/>", item);
                }
                return builder.ToString();
            }
            return null;
        }

    }
}
