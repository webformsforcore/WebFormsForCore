namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

internal enum ValidationSourceFlag
{
	needToValidateQueryString = 1,
	needToValidateForm = 2,
	needToValidateCookies = 4,
	needToValidateHeaders = 8,
	needToValidateServerVariables = 16, // 0x00000010
	contentEncodingResolved = 32, // 0x00000020
	needToValidatePostedFiles = 64, // 0x00000040
	needToValidateRawUrl = 128, // 0x00000080
	needToValidatePath = 256, // 0x00000100
	needToValidatePathInfo = 512, // 0x00000200
	hasValidateInputBeenCalled = 32768, // 0x00008000
	needToValidateCookielessHeader = 65536, // 0x00010000
}
