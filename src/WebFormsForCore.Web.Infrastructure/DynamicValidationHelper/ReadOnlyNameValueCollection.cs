using System.Collections.Specialized;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

internal sealed class ReadOnlyNameValueCollection : NameValueCollection
{
	public ReadOnlyNameValueCollection(NameValueCollection col): base(col)
	{
		this.IsReadOnly = true;
	}
}
