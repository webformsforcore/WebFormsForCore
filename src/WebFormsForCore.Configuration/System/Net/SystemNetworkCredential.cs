using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace System.Net;

internal class SystemNetworkCredential : NetworkCredential
{
	internal static readonly SystemNetworkCredential defaultCredential = new SystemNetworkCredential();

	private SystemNetworkCredential()
		: base(string.Empty, string.Empty, string.Empty)
	{
	}
}
