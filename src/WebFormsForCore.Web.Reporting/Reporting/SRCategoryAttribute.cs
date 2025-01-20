
using System;
using System.ComponentModel;

#nullable disable
namespace Microsoft.Reporting
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false)]
  internal sealed class SRCategoryAttribute : CategoryAttribute
  {
    private string m_value;
    private string m_key;

    public SRCategoryAttribute(string key) => this.m_key = key;

    protected override string GetLocalizedString(string value)
    {
      if (this.m_value == null)
      {
        // ISSUE: reference to a compiler-generated method
        this.m_value = CommonStrings.Keys.GetString(this.m_key);
      }
      return this.m_value;
    }
  }
}
