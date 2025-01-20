
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

#nullable disable
namespace Microsoft.Reporting.WebForms
{
  public sealed class ReportDataSource
  {
    private string m_dataSourceID = "";
    private string m_dataMember;
    private string m_name = "";
    private object m_value;

    public ReportDataSource()
    {
    }

    public ReportDataSource(string name) => this.Name = name;

    public ReportDataSource(string name, object dataSourceValue)
      : this(name)
    {
      this.Value = dataSourceValue;
    }

    public ReportDataSource(string name, DataTable dataSourceValue)
      : this(name)
    {
      this.Value = (object) dataSourceValue;
    }

    public ReportDataSource(string name, string dataSourceId)
      : this(name)
    {
      this.DataSourceId = dataSourceId;
    }

    public ReportDataSource(string name, IDataSource dataSourceValue)
      : this(name)
    {
      this.Value = (object) dataSourceValue;
    }

    public ReportDataSource(string name, IEnumerable dataSourceValue)
      : this(name)
    {
      this.Value = (object) dataSourceValue;
    }

    internal event EventHandler Changed;

    internal void OnChanged()
    {
      if (this.Changed == null)
        return;
      this.Changed((object) this, (EventArgs) null);
    }

    [NotifyParentProperty(true)]
    [DefaultValue("")]
    [WebBrowsable(true)]
    public string Name
    {
      get => this.m_name;
      set
      {
        this.m_name = value;
        this.OnChanged();
      }
    }

    internal void SetValueWithoutChange(object dsValue) => this.m_value = dsValue;

    [NotifyParentProperty(true)]
    [WebBrowsable(true)]
    public string DataSourceId
    {
      get => this.m_dataSourceID;
      set
      {
        this.m_dataSourceID = value;
        this.OnChanged();
      }
    }

    [NotifyParentProperty(true)]
    [WebBrowsable(true)]
    public string DataMember
    {
      get => this.m_dataMember;
      set => this.m_dataMember = value;
    }

    public object Value
    {
      get => this.m_value;
      set
      {
        int num;
        switch (value)
        {
          case null:
label_5:
            this.m_value = value;
            this.OnChanged();
            return;
          case DataTable _:
          case IDataSource _:
            num = 1;
            break;
          default:
            num = value is IEnumerable ? 1 : 0;
            break;
        }
        if (num == 0)
          throw new ArgumentException(Errors.BadReportDataSourceType);
        goto label_5;
      }
    }
  }
}
