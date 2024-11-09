using System.ComponentModel;

#nullable disable
namespace System.Web.Mobile
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
