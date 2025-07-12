using System;
using System.Security;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[SecurityCritical]
internal struct FieldAccessor<T>(Func<T> getter, Action<T> setter)
{
	private readonly Func<T> _getter = getter;
	private readonly Action<T> _setter = setter;

	public T Value
	{
		get => this._getter();
		set => this._setter(value);
	}
}
