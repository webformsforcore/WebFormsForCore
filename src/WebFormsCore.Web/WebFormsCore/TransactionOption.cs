#if !NETFRAMEWORK

namespace System.EnterpriseServices;

//
// Summary:
//     Specifies the automatic transaction type requested by the component.
[Serializable]
public enum TransactionOption
{
	//
	// Summary:
	//     Ignores any transaction in the current context.
	Disabled,
	//
	// Summary:
	//     Creates the component in a context with no governing transaction.
	NotSupported,
	//
	// Summary:
	//     Shares a transaction, if one exists.
	Supported,
	//
	// Summary:
	//     Shares a transaction, if one exists, and creates a new transaction if necessary.
	Required,
	//
	// Summary:
	//     Creates the component with a new transaction, regardless of the state of the
	//     current context.
	RequiresNew
}
#endif