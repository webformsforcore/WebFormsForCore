// Decompiled with JetBrains decompiler
// Type: System.Web.Services.ResDescriptionAttribute
// Assembly: System.Web.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76230F59-2009-49E6-BE83-7D49154374AE
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\System.Web.Services\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.Services.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.Services.xml

using System.ComponentModel;

#nullable disable
namespace System.Web.Services
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class ResDescriptionAttribute : DescriptionAttribute
  {
    private bool replaced;

    public ResDescriptionAttribute(string description)
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
          this.DescriptionValue = Res.GetString(base.Description);
        }
        return base.Description;
      }
    }
  }
}
