// Decompiled with JetBrains decompiler
// Type: System.Configuration.SRDescriptionAttribute
// Assembly: System.Configuration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F80D3B8-83DB-4C4E-BE29-E92F4607776E
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Configuration\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Configuration.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Configuration.xml

using System.ComponentModel;

#nullable disable
namespace System.Configuration
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class SRDescriptionAttribute : DescriptionAttribute
  {
    private bool replaced;

    public SRDescriptionAttribute(string description)
      : base(description)
    {
    }

    public override string Description
    {
      get
      {
        if (!this.replaced)
        {
          this.replaced = true;
          this.DescriptionValue = SR.GetString(base.Description);
        }
        return base.Description;
      }
    }
  }
}
