using System;
using System.Collections;
using System.Security;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[SecurityCritical]
internal sealed class LazilyValidatingHashtable : Hashtable
{
	public LazilyValidatingHashtable(Hashtable innerTable, SimpleValidateStringCallback validateCallback)
	  : base(innerTable.Count, (IEqualityComparer)StringComparer.OrdinalIgnoreCase)
	{
		foreach (DictionaryEntry dictionaryEntry in innerTable)
			base[dictionaryEntry.Key] = (object)new LazilyEvaluatedNameObjectEntry(dictionaryEntry.Value, validateCallback);
	}

	public override object this[object key]
	{
		[SecuritySafeCritical]
		get
		{
			object obj = base[key];
			return !(obj is LazilyEvaluatedNameObjectEntry evaluatedNameObjectEntry) ? obj : evaluatedNameObjectEntry.GetValidatedObject();
		}
		[SecuritySafeCritical]
		set => base[key] = value;
	}
}
