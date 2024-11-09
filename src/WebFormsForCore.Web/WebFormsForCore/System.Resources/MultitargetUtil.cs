#if !NETFRAMEWORK

using System.Security;
using System.Threading;

#nullable disable
namespace System.Resources
{
  internal static class MultitargetUtil
  {
    public static string GetAssemblyQualifiedName(Type type, Func<Type, string> typeNameConverter)
    {
      string assemblyQualifiedName = (string) null;
      if (type != (Type) null)
      {
        if (typeNameConverter != null)
        {
          try
          {
            assemblyQualifiedName = typeNameConverter(type);
          }
          catch (Exception ex)
          {
            if (MultitargetUtil.IsSecurityOrCriticalException(ex))
              throw;
          }
        }
        if (string.IsNullOrEmpty(assemblyQualifiedName))
          assemblyQualifiedName = type.AssemblyQualifiedName;
      }
      return assemblyQualifiedName;
    }

    private static bool IsSecurityOrCriticalException(Exception ex)
    {
      switch (ex)
      {
        case NullReferenceException _:
        case StackOverflowException _:
        case OutOfMemoryException _:
        case ThreadAbortException _:
        case Web.ResponseEndException _:
        case ExecutionEngineException _:
        case IndexOutOfRangeException _:
        case AccessViolationException _:
          return true;
        default:
          return ex is SecurityException;
      }
    }
  }
}
#endif