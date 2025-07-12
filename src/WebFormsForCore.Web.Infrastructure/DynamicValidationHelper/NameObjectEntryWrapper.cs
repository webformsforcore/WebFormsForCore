using System.Security;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[SecurityCritical]
internal sealed class NameObjectEntryWrapper
{
	private static readonly GranularValidationReflectionUtil _reflectionUtil = GranularValidationReflectionUtil.Instance;
	private readonly object _nameObjectEntry;

	private NameObjectEntryWrapper(object nameObjectEntry) => this._nameObjectEntry = nameObjectEntry;

	public string Key
	{
		get => NameObjectEntryWrapper._reflectionUtil.GetNameObjectEntryKey(this._nameObjectEntry);
	}

	public object Value
	{
		get => NameObjectEntryWrapper._reflectionUtil.GetNameObjectEntryValue(this._nameObjectEntry);
		set
		{
			NameObjectEntryWrapper._reflectionUtil.SetNameObjectEntryValue(this._nameObjectEntry, value);
		}
	}

	public static NameObjectEntryWrapper Wrap(object nameObjectEntry)
	{
		return nameObjectEntry == null ? (NameObjectEntryWrapper)null : new NameObjectEntryWrapper(nameObjectEntry);
	}
}
