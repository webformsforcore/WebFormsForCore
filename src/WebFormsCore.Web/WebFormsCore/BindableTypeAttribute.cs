namespace System.ComponentModel.DataAnnotations;

//
// Summary:
//     Specifies whether a type is typically used for binding.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, AllowMultiple = false, Inherited = true)]
public sealed class BindableTypeAttribute : Attribute
{
	//
	// Summary:
	//     Gets a value indicating that a type is typically used for binding.
	//
	// Returns:
	//     true if the property is typically used for binding; otherwise, false.
	public bool IsBindable { get; set; }

	//
	// Summary:
	//     Initializes a new instance of the System.ComponentModel.DataAnnotations.BindableTypeAttribute
	//     class.
	public BindableTypeAttribute()
	{
		IsBindable = true;
	}
}
