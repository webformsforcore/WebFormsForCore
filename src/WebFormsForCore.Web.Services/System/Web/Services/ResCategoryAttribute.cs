
using System.ComponentModel;

#nullable disable
namespace System.Web.Services
{
  [AttributeUsage(AttributeTargets.All)]
  internal sealed class ResCategoryAttribute : CategoryAttribute
  {
    public ResCategoryAttribute(string category)
      : base(category)
    {
    }

    protected override string GetLocalizedString(string value) => Res.GetString(value);
  }
}
