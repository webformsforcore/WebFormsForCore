// Decompiled with JetBrains decompiler
// Type: System.Web.UI.MobileControls.Adapters.SRCategoryAttribute
// Assembly: System.Web.Mobile, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 96756CA9-E864-4FF7-8232-10244D5A6636
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Web.Mobile.dll
// XML documentation location: C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8.1\System.Web.Mobile.xml

using System.ComponentModel;

#nullable disable
namespace System.Web.UI.MobileControls.Adapters
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class SRCategoryAttribute : CategoryAttribute
  {
    public SRCategoryAttribute(string category)
      : base(category)
    {
    }

    protected override string GetLocalizedString(string value) => SR.GetString(value);
  }
}
