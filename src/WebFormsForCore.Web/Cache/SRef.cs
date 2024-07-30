using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Web;

namespace System.Web.Caching {
    internal class SRef {
#if NETFRAMEWORK
        private static Type s_type = Type.GetType("System.SizedReference", true, false);
#else
		private static Type s_type = null;
#endif
        private Object _sizedRef;
        private long _lastReportedSize; // This helps tremendously when looking at large dumps
        
        internal SRef(Object target) {
#if NETFRAMEWORK
            _sizedRef = HttpRuntime.CreateNonPublicInstance(s_type, new object[] {target});
#endif
        }
        
        internal long ApproximateSize {
            [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
            get {
#if NETFRAMEWORK
                object o = s_type.InvokeMember("ApproximateSize",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, 
                                               null, // binder
                                               _sizedRef, // target
                                               null, // args
                                               CultureInfo.InvariantCulture);
                return _lastReportedSize = (long) o;
#else
                return 1024;
#endif
            }
        }
        
        [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        internal void Dispose() {
#if NETFRAMEWORK
            s_type.InvokeMember("Dispose",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, 
                                null, // binder
                                _sizedRef, // target
                                null, // args
                                CultureInfo.InvariantCulture);
#endif
        }
    }

    internal class SRefMultiple {
        private List<SRef> _srefs = new List<SRef>();

        internal void AddSRefTarget(Object o) {
            _srefs.Add(new SRef(o));
        }

        internal long ApproximateSize {
            [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
            get {
                return _srefs.Sum(s => s.ApproximateSize);
            }
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted = true)]
        internal void Dispose() {
            foreach (SRef s in _srefs) {
                s.Dispose();
            }
        }
    }
}
