
using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Reporting
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = false)]
  internal sealed class SRDescriptionAttribute : DescriptionAttribute
  {
    private string m_key;
    private bool m_initialized;

    public SRDescriptionAttribute(string key)
    {
      this.m_key = key;
      this.m_initialized = false;
    }

    public override string Description
    {
      get
      {
        if (!this.m_initialized)
        {
          this.DescriptionValue = CommonStrings.Keys.GetString(this.m_key);
          this.m_initialized = true;
        }
        return base.Description;
      }
    }
  }
}
