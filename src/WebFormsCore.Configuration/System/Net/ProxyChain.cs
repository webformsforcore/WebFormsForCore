
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

#nullable disable
namespace System.Net
{
	internal abstract class ProxyChain : IEnumerable<Uri>, IEnumerable, IDisposable
	{
		private List<Uri> m_Cache = new List<Uri>();
		private bool m_CacheComplete;
		private ProxyChain.ProxyEnumerator m_MainEnumerator;
		private Uri m_Destination;
		private HttpAbortDelegate m_HttpAbortDelegate;

		protected ProxyChain(Uri destination) => this.m_Destination = destination;

		public IEnumerator<Uri> GetEnumerator()
		{
			ProxyChain.ProxyEnumerator enumerator = new ProxyChain.ProxyEnumerator(this);
			if (this.m_MainEnumerator == null)
				this.m_MainEnumerator = enumerator;
			return (IEnumerator<Uri>)enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator() => (IEnumerator)this.GetEnumerator();

		public virtual void Dispose()
		{
		}

		internal IEnumerator<Uri> Enumerator
		{
			get
			{
				return this.m_MainEnumerator != null ? (IEnumerator<Uri>)this.m_MainEnumerator : this.GetEnumerator();
			}
		}

		internal Uri Destination => this.m_Destination;

		internal virtual void Abort()
		{
		}

		internal bool HttpAbort(HttpWebRequest request, WebException webException)
		{
			this.Abort();
			return true;
		}

		internal HttpAbortDelegate HttpAbortDelegate
		{
			get
			{
				if (this.m_HttpAbortDelegate == null)
					this.m_HttpAbortDelegate = new HttpAbortDelegate(this.HttpAbort);
				return this.m_HttpAbortDelegate;
			}
		}

		protected abstract bool GetNextProxy(out Uri proxy);

		private class ProxyEnumerator : IEnumerator<Uri>, IDisposable, IEnumerator
		{
			private ProxyChain m_Chain;
			private bool m_Finished;
			private int m_CurrentIndex = -1;
			private bool m_TriedDirect;

			internal ProxyEnumerator(ProxyChain chain) => this.m_Chain = chain;

			public Uri Current
			{
				get
				{
					if (this.m_Finished || this.m_CurrentIndex < 0)
						throw new InvalidOperationException(SR.GetString("InvalidOperation_EnumOpCantHappen"));
					return this.m_Chain.m_Cache[this.m_CurrentIndex];
				}
			}

			object IEnumerator.Current => (object)this.Current;

			public bool MoveNext()
			{
				if (this.m_Finished)
					return false;
				checked { ++this.m_CurrentIndex; }
				if (this.m_Chain.m_Cache.Count > this.m_CurrentIndex)
					return true;
				if (this.m_Chain.m_CacheComplete)
				{
					this.m_Finished = true;
					return false;
				}
				lock (this.m_Chain.m_Cache)
				{
					if (this.m_Chain.m_Cache.Count > this.m_CurrentIndex)
						return true;
					if (this.m_Chain.m_CacheComplete)
					{
						this.m_Finished = true;
						return false;
					}
					Uri proxy;
					while (this.m_Chain.GetNextProxy(out proxy))
					{
						if (proxy == (Uri)null)
						{
							if (!this.m_TriedDirect)
								this.m_TriedDirect = true;
							else
								continue;
						}
						this.m_Chain.m_Cache.Add(proxy);
						return true;
					}
					this.m_Finished = true;
					this.m_Chain.m_CacheComplete = true;
					return false;
				}
			}

			public void Reset()
			{
				this.m_Finished = false;
				this.m_CurrentIndex = -1;
			}

			public void Dispose()
			{
			}
		}
	}
}
