
#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class DataSourceCredentials
  {
    private string m_name = "";
    private string m_userID = "";
    private string m_password = "";

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public string UserId
    {
      get => this.m_userID;
      set => this.m_userID = value;
    }

    public string Password
    {
      get => this.m_password;
      set => this.m_password = value;
    }

    internal Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials ToSoapCredentials()
    {
      return new Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution.DataSourceCredentials()
      {
        DataSourceName = this.Name,
        UserName = this.UserId,
        Password = this.Password
      };
    }
  }
}
