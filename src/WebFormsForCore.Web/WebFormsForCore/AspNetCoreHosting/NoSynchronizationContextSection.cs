using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Hosting
{
	internal readonly ref struct NoSynchronizationContextSection: IDisposable
	{
		private readonly SynchronizationContext oldSynchronizationContext;

		public NoSynchronizationContextSection()
		{
			oldSynchronizationContext = SynchronizationContext.Current;
			SynchronizationContext.SetSynchronizationContext((SynchronizationContext)null);
		}

		public void Dispose()
		{
			SynchronizationContext.SetSynchronizationContext(oldSynchronizationContext);
		}
	}
}
