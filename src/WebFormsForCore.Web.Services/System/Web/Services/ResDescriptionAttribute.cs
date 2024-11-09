
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
