using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Hosting
{
	public interface IAspNetCoreWorkerRequest
	{
		public AspNetCoreHost Host { get; }
		public Microsoft.AspNetCore.Http.HttpContext Context { get; }
	}
}
