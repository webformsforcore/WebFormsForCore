
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class ValidValue
  {
    private string m_label;
    private string m_value;

    internal ValidValue(string label, string value)
    {
      this.m_label = label;
      this.m_value = value;
    }

    public string Label => this.m_label;

    public string Value => this.m_value;
  }
}
