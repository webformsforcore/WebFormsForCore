using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Reporting.WebForms;

internal sealed class DataSourceWrapper : IDataSource
{
	private DataSourceViewSelectCallbackEvent m_selectCompletionEvent;

	private object m_cachedValue;

	private readonly ReportDataSource m_ds;

	string IDataSource.Name => m_ds.Name;

	object IDataSource.Value => GetIDataSource();

	internal DataSourceWrapper(ReportDataSource ds)
	{
		m_ds = ds;
		StartAsyncSelect();
	}

	private void StartAsyncSelect()
	{
		object value = m_ds.Value;
		if (value is ObjectDataSource || !(value is IDataSource))
		{
			return;
		}
		DataSourceView dataSourceView = null;
		if (!string.IsNullOrEmpty(m_ds.DataMember))
		{
			dataSourceView = ((IDataSource)value).GetView(m_ds.DataMember);
		}
		else
		{
			ICollection viewNames = ((IDataSource)value).GetViewNames();
			IEnumerator enumerator = viewNames.GetEnumerator();
			if (enumerator != null && enumerator.MoveNext())
			{
				dataSourceView = ((IDataSource)value).GetView(enumerator.Current as string);
			}
		}
		if (dataSourceView == null)
		{
			throw new InvalidOperationException(Errors.DataControl_ViewNotFound("", m_ds.Name));
		}
		m_selectCompletionEvent = new DataSourceViewSelectCallbackEvent(dataSourceView);
		m_selectCompletionEvent.InvokeSelect();
	}

	private object GetIDataSource()
	{
		if (m_cachedValue != null)
		{
			return m_cachedValue;
		}
		object obj = m_ds.Value;
		if (obj is ObjectDataSource)
		{
			obj = (m_cachedValue = ((ObjectDataSource)obj).Select());
		}
		else if (obj is IDataSource)
		{
			if (m_selectCompletionEvent != null)
			{
				m_selectCompletionEvent.WaitForSelectCompleted();
				obj = (m_cachedValue = m_selectCompletionEvent.Data);
			}
			else
			{
				obj = null;
			}
		}
		return obj;
	}
}
