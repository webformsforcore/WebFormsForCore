using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

internal delegate void GetUnvalidatedCollectionsCallback(HttpContext context,
  out Func<NameValueCollection> formGetter,
  out Func<NameValueCollection> queryStringGetter);