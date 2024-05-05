// Decompiled with JetBrains decompiler
// Type: System.Configuration.SchemeSettingInternal
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: 84F5A18A-F2B1-435C-B86E-09CE162E61E4
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.xml

#nullable disable
namespace System.Configuration
{
  internal sealed class SchemeSettingInternal
  {
    private string name;
    private GenericUriParserOptions options;

    public SchemeSettingInternal(string name, GenericUriParserOptions options)
    {
      this.name = name.ToLowerInvariant();
      this.options = options;
    }

    public string Name => this.name;

    public GenericUriParserOptions Options => this.options;
  }
}
