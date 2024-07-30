
namespace System.Resources.Tools;

//
// Summary:
//     Defines an interface that enables the strongly typed resource builder (System.Resources.Tools.StronglyTypedResourceBuilder
//     object) to determine which types and properties are available so it can emit
//     the correct Code Document Object Model (CodeDOM) tree.
public interface ITargetAwareCodeDomProvider
{
	//
	// Summary:
	//     Indicates whether the specified type on the project target framework has a specified
	//     named property.
	//
	// Parameters:
	//   type:
	//     The type whose properties are to be queried.
	//
	//   propertyName:
	//     The name of the property to find in type.
	//
	//   isWritable:
	//     A flag that indicates whether the property must include a get accessor.
	//
	// Returns:
	//     true if type on the project target framework has a property named propertyname;
	//     otherwise, false.
	bool SupportsProperty(Type type, string propertyName, bool isWritable);
}