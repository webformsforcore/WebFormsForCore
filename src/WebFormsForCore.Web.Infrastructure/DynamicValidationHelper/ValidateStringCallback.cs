using System.Web.Util;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

internal delegate void ValidateStringCallback(string value, string collectionKey, RequestValidationSource requestCollection);
