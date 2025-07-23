using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Microsoft.Reporting.WebForms;

[ComVisible(false)]
public sealed class ReportParameterInfoCollection : ReadOnlyCollection<ReportParameterInfo>
{
	public ReportParameterInfo this[string name]
	{
		get
		{
			using (IEnumerator<ReportParameterInfo> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ReportParameterInfo current = enumerator.Current;
					if (string.Compare(current.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return current;
					}
				}
			}
			return null;
		}
	}

	internal ReportParameterInfoCollection(IList<ReportParameterInfo> parameterInfos)
		: base(parameterInfos)
	{
		using IEnumerator<ReportParameterInfo> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			ReportParameterInfo current = enumerator.Current;
			current.SetDependencies(this);
		}
	}

	internal ReportParameterInfoCollection()
		: base((IList<ReportParameterInfo>)new ReportParameterInfo[0])
	{
	}
}
