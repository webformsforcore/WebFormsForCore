#if !NETFRAMEWORK

using System.Reflection;

#nullable disable
namespace System.Resources
{
  internal interface IAliasResolver
  {
    AssemblyName ResolveAlias(string alias);

    void PushAlias(string alias, AssemblyName name);
  }
}
#endif