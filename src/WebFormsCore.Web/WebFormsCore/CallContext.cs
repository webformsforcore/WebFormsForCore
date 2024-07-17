#if !NETFRAMEWORK

using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Collections.Concurrent;
using System.Threading;

#nullable disable
namespace System.Runtime.Remoting.Messaging
{
	/// <summary>Provides a set of properties that are carried with the execution code path. This class cannot be inherited.</summary>
	[SecurityCritical]
	[ComVisible(true)]
	[Serializable]
	public sealed class CallContext
	{
		private CallContext()
		{
		}

		static AsyncLocal<ConcurrentDictionary<string, object?>> data = new();
		public static ConcurrentDictionary<string, object?> Data => data.Value ??= new ConcurrentDictionary<string, object?>();


#if !WebFormsCore
        internal static LogicalCallContext SetLogicalCallContext(LogicalCallContext callCtx)
    {
      ExecutionContext executionContext = Thread.CurrentThread.GetMutableExecutionContext();
      LogicalCallContext logicalCallContext = executionContext.LogicalCallContext;
      executionContext.LogicalCallContext = callCtx;
      return logicalCallContext;
    }
#endif
		/// <summary>Empties a data slot with the specified name.</summary>
		/// <param name="name">The name of the data slot to empty.</param>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		[SecurityCritical]
		public static void FreeNamedDataSlot(string name)
		{
#if !WebFormsCore
        ExecutionContext executionContext = Thread.CurrentThread.GetMutableExecutionContext();
        executionContext.LogicalCallContext.FreeNamedDataSlot(name);
        executionContext.IllogicalCallContext.FreeNamedDataSlot(name);
#else
			object? value;
			Data.TryRemove(name, out value);
#endif
		}

		/// <summary>Retrieves an object with the specified name from the logical call context.</summary>
		/// <param name="name">The name of the item in the logical call context.</param>
		/// <returns>The object in the logical call context associated with the specified name.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		[SecurityCritical]
		public static object LogicalGetData(string name)
		{
#if NETFRAMEWORK
			return Thread.CurrentThread.ExecutionContext.GetExecutionContextReader().LogicalCallContext.GetData(name);
#else
			object value;
			if (Data.TryGetValue(name, out value)) return value;
			return null;
#endif
		}

		private static object IllogicalGetData(string name)
		{
#if NETFRAMEWORK
			return Thread.CurrentThread.GetExecutionContextReader().IllogicalCallContext.GetData(name);
#else
			return LogicalGetData("_illogical:" + name);
#endif
		}

		internal static IPrincipal Principal
		{

#if NETFRAMEWORK
			[SecurityCritical]
			get
			{
				return Thread.CurrentThread.GetExecutionContextReader().LogicalCallContext.Principal;
			}
			[SecurityCritical]
			set
			{
				Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext.Principal = value;
			}
#else
			[SecurityCritical]
			get
			{
				IPrincipal principal = (IPrincipal)LogicalGetData("CallContext.Principal");
				return principal ?? Thread.CurrentPrincipal;
			}
			[SecurityCritical]
			set
			{
				Data.AddOrUpdate("CallContext.Principal", value, (key, obj) => value);
			}
#endif
		}

		/// <summary>Gets or sets the host context associated with the current thread.</summary>
		/// <returns>The host context associated with the current thread.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		public static object HostContext
		{
			[SecurityCritical]
			get
			{
#if NETFRAMEWORK
				ExecutionContext.Reader executionContextReader = Thread.CurrentThread.GetExecutionContextReader();
				return executionContextReader.IllogicalCallContext.HostContext ?? executionContextReader.LogicalCallContext.HostContext;
#else
				return LogicalGetData("CallContext.HostContext");
#endif
			}
			[SecurityCritical]
			set
			{
#if NETFRAMEWORK
				ExecutionContext executionContext = Thread.CurrentThread.GetMutableExecutionContext();
				if (value is ILogicalThreadAffinative)
				{
					executionContext.IllogicalCallContext.HostContext = (object)null;
					executionContext.LogicalCallContext.HostContext = value;
				}
				else
				{
					executionContext.IllogicalCallContext.HostContext = value;
					executionContext.LogicalCallContext.HostContext = (object)null;
				}
#else
				Data.AddOrUpdate("CallContext.HostContext", value, (key, obj) => value);
#endif
			}
		}

		/// <summary>Retrieves an object with the specified name from the <see cref="T:System.Runtime.Remoting.Messaging.CallContext" />.</summary>
		/// <param name="name">The name of the item in the call context.</param>
		/// <returns>The object in the call context associated with the specified name.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		[SecurityCritical]
		public static object GetData(string name)
		{
			return LogicalGetData(name);
		}

		/// <summary>Stores a given object and associates it with the specified name.</summary>
		/// <param name="name">The name with which to associate the new item in the call context.</param>
		/// <param name="data">The object to store in the call context.</param>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		[SecurityCritical]
		public static void SetData(string name, object data)
		{
#if NETFRAMEWORK
			if (data is ILogicalThreadAffinative)
			{
				CallContext.LogicalSetData(name, data);
			}
			else
			{
				ExecutionContext executionContext = Thread.CurrentThread.GetMutableExecutionContext();
				executionContext.LogicalCallContext.FreeNamedDataSlot(name);
				executionContext.IllogicalCallContext.SetData(name, data);
			}
#else
			Data.AddOrUpdate(name, data, (key, obj) => data);
#endif
		}

		/// <summary>Stores a given object in the logical call context and associates it with the specified name.</summary>
		/// <param name="name">The name with which to associate the new item in the logical call context.</param>
		/// <param name="data">The object to store in the logical call context, this object must be serializable.</param>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		[SecurityCritical]
		public static void LogicalSetData(string name, object data)
		{
#if NETFRAMEWORK
			ExecutionContext executionContext = Thread.CurrentThread.GetMutableExecutionContext();
			executionContext.IllogicalCallContext.FreeNamedDataSlot(name);
			executionContext.LogicalCallContext.SetData(name, data);
#else
			Data.AddOrUpdate(name, data, (key, obj) => data);
#endif
		}

#if NETFRAMEWORK
		/// <summary>Returns the headers that are sent along with the method call.</summary>
		/// <returns>The headers that are sent along with the method call.</returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		[SecurityCritical]
		public static Header[] GetHeaders()
		{
			return Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext.InternalGetHeaders();
		}

		/// <summary>Sets the headers that are sent along with the method call.</summary>
		/// <param name="headers">A <see cref="T:System.Runtime.Remoting.Messaging.Header" /> array of the headers that are to be sent along with the method call.</param>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission.</exception>
		[SecurityCritical]
		public static void SetHeaders(Header[] headers)
		{
			Thread.CurrentThread.GetMutableExecutionContext().LogicalCallContext.InternalSetHeaders(headers);
		}
#endif
	}
}

#endif