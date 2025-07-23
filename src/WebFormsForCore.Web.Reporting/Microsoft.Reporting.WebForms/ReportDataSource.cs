using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Microsoft.Reporting.WebForms;

public sealed class ReportDataSource
{
	private string m_dataSourceID = "";

	private string m_dataMember;

	private string m_name = "";

	private object m_value;

	[NotifyParentProperty(true)]
	[DefaultValue("")]
	[WebBrowsable(true)]
	public string Name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
			OnChanged();
		}
	}

	[NotifyParentProperty(true)]
	[WebBrowsable(true)]
	public string DataSourceId
	{
		get
		{
			return m_dataSourceID;
		}
		set
		{
			m_dataSourceID = value;
			OnChanged();
		}
	}

	[NotifyParentProperty(true)]
	[WebBrowsable(true)]
	public string DataMember
	{
		get
		{
			return m_dataMember;
		}
		set
		{
			m_dataMember = value;
		}
	}

	public object Value
	{
		get
		{
			return m_value;
		}
		set
		{
			if (value != null && !(value is DataTable) && !(value is IDataSource) && !(value is IEnumerable))
			{
				throw new ArgumentException(Errors.BadReportDataSourceType);
			}
			m_value = value;
			OnChanged();
		}
	}

	internal event EventHandler Changed;

	public ReportDataSource()
	{
	}

	public ReportDataSource(string name)
	{
		Name = name;
	}

	public ReportDataSource(string name, object dataSourceValue)
		: this(name)
	{
		Value = dataSourceValue;
	}

	public ReportDataSource(string name, DataTable dataSourceValue)
		: this(name)
	{
		Value = dataSourceValue;
	}

	public ReportDataSource(string name, string dataSourceId)
		: this(name)
	{
		DataSourceId = dataSourceId;
	}

	public ReportDataSource(string name, IDataSource dataSourceValue)
		: this(name)
	{
		Value = dataSourceValue;
	}

	public ReportDataSource(string name, IEnumerable dataSourceValue)
		: this(name)
	{
		Value = dataSourceValue;
	}

	internal void OnChanged()
	{
		if (this.Changed != null)
		{
			this.Changed(this, null);
		}
	}

	internal void SetValueWithoutChange(object dsValue)
	{
		m_value = dsValue;
	}
}
