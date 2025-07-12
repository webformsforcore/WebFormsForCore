using System.Collections;
using System.Security;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[SecurityCritical]
internal sealed class LazilyValidatingArrayList : ArrayList
{
	public LazilyValidatingArrayList(ArrayList innerList, SimpleValidateStringCallback validateCallback)
	  : base(innerList.Count)
	{
		for (int index = 0; index < innerList.Count; ++index)
			this.Add((object)new LazilyEvaluatedNameObjectEntry(innerList[index], validateCallback));
	}

	public override object this[int index]
	{
		[SecuritySafeCritical]
		get
		{
			object obj = base[index];
			return !(obj is LazilyEvaluatedNameObjectEntry evaluatedNameObjectEntry) ? obj : evaluatedNameObjectEntry.GetValidatedObject();
		}
		[SecuritySafeCritical]
		set => base[index] = value;
	}
}
