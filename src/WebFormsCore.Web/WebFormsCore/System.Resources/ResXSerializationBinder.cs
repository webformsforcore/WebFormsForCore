#if !NETFRAMEWORK

using System.ComponentModel.Design;
using System.Runtime.Serialization;

#nullable disable
namespace System.Resources
{
  internal class ResXSerializationBinder : SerializationBinder
  {
    private ITypeResolutionService typeResolver;
    private Func<Type, string> typeNameConverter;

    internal ResXSerializationBinder(ITypeResolutionService typeResolver)
    {
      this.typeResolver = typeResolver;
    }

    internal ResXSerializationBinder(Func<Type, string> typeNameConverter)
    {
      this.typeNameConverter = typeNameConverter;
    }

    public override Type BindToType(string assemblyName, string typeName)
    {
      if (this.typeResolver == null)
        return (Type) null;
      typeName = typeName + ", " + assemblyName;
      Type type = this.typeResolver.GetType(typeName);
      if (type == (Type) null)
      {
        string[] strArray = typeName.Split(',');
        if (strArray != null && strArray.Length > 2)
        {
          string name = strArray[0].Trim();
          for (int index = 1; index < strArray.Length; ++index)
          {
            string str = strArray[index].Trim();
            if (!str.StartsWith("Version=") && !str.StartsWith("version="))
              name = name + ", " + str;
          }
          type = this.typeResolver.GetType(name);
          if (type == (Type) null)
            type = this.typeResolver.GetType(strArray[0].Trim());
        }
      }
      return type;
    }

    public override void BindToName(
      Type serializedType,
      out string assemblyName,
      out string typeName)
    {
      typeName = (string) null;
      if (this.typeNameConverter != null)
      {
        string assemblyQualifiedName = MultitargetUtil.GetAssemblyQualifiedName(serializedType, this.typeNameConverter);
        if (!string.IsNullOrEmpty(assemblyQualifiedName))
        {
          int length = assemblyQualifiedName.IndexOf(',');
          if (length > 0 && length < assemblyQualifiedName.Length - 1)
          {
            assemblyName = assemblyQualifiedName.Substring(length + 1).TrimStart();
            string a = assemblyQualifiedName.Substring(0, length);
            if (string.Equals(a, serializedType.FullName, StringComparison.InvariantCulture))
              return;
            typeName = a;
            return;
          }
        }
      }
      base.BindToName(serializedType, out assemblyName, out typeName);
    }
  }
}
#endif