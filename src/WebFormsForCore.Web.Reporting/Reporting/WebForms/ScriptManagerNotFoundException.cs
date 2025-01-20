
using System;
using System.Configuration;
using System.Runtime.Serialization;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  [Serializable]
  public sealed class ScriptManagerNotFoundException : ConfigurationErrorsException
  {
    internal ScriptManagerNotFoundException()
      : base(Microsoft.Reporting.WebForms.Errors.ScriptManagerNotFound)
    {
    }

    private ScriptManagerNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
