using System.Collections;
using System.Security;
using System.Text;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[SecurityCritical]
internal sealed class LazilyEvaluatedNameObjectEntry
{
	private bool _hasBeenValidated;
	private readonly object _nameObjectEntry;
	private readonly NameObjectEntryWrapper _nameObjectEntryWrapper;
	private readonly SimpleValidateStringCallback _validateCallback;

	public LazilyEvaluatedNameObjectEntry(object nameObjectEntry, SimpleValidateStringCallback validateCallback)
	{
		this._nameObjectEntry = nameObjectEntry;
		this._nameObjectEntryWrapper = NameObjectEntryWrapper.Wrap(nameObjectEntry);
		this._validateCallback = validateCallback;
	}

	public object GetValidatedObject()
	{
		if (!this._hasBeenValidated)
		{
			this.ValidateObject();
			this._hasBeenValidated = true;
		}
		return this._nameObjectEntry;
	}

	private void ValidateObject()
	{
		this._validateCallback(LazilyEvaluatedNameObjectEntry.GetAsOneString(this._nameObjectEntryWrapper.Value), this._nameObjectEntryWrapper.Key);
	}

	private static string GetAsOneString(object value)
	{
		return value is ArrayList list ? LazilyEvaluatedNameObjectEntry.GetAsOneString(list) : (string)null;
	}

	private static string GetAsOneString(ArrayList list)
	{
		int count = list != null ? list.Count : 0;
		if (count == 1)
			return (string)list[0];
		if (count <= 1)
			return (string)null;
		StringBuilder stringBuilder = new StringBuilder((string)list[0]);
		for (int index = 1; index < count; ++index)
		{
			stringBuilder.Append(',');
			stringBuilder.Append((string)list[index]);
		}
		return stringBuilder.ToString();
	}
}
