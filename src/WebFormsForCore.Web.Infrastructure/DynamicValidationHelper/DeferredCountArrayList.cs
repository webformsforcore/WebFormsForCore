using System;
using System.Collections;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

internal sealed class DeferredCountArrayList : ArrayList
{
	private readonly Func<bool> _hasValidationFired;
	private readonly Action _disableValidation;
	private readonly Func<int> _fillInActualFormContents;

	public DeferredCountArrayList(Func<bool> hasValidationFired, Action disableValidation, Func<int> fillInActualFormContents)
	{
		this._hasValidationFired = hasValidationFired;
		this._disableValidation = disableValidation;
		this._fillInActualFormContents = fillInActualFormContents;
	}

	public override int Count
	{
		get
		{
			int count = this._fillInActualFormContents();
			if (this._hasValidationFired()) return 0;
			this._disableValidation();
			return count;
		}
	}
}
