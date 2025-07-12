using System.ComponentModel;

namespace System.Web.UI.MobileControls.Adapters;

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
