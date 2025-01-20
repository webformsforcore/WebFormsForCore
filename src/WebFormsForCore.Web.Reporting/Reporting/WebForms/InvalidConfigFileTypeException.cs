
using System;
using System.Configuration;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class InvalidConfigFileTypeException : ConfigurationErrorsException
  {
    internal InvalidConfigFileTypeException(string typeName)
      : base(Microsoft.Reporting.WebForms.Errors.CantFindConfigFileType(typeName))
    {
      // ISSUE: reference to a compiler-generated method (out of statement scope)
    }

    internal InvalidConfigFileTypeException(string typeName, string expectedTypeName)
      : base(Microsoft.Reporting.WebForms.Errors.MissingInterfaceOnConfigFileType(typeName, expectedTypeName))
    {
      // ISSUE: reference to a compiler-generated method (out of statement scope)
    }

    private InvalidConfigFileTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
