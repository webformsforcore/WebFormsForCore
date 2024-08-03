using System;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Web.Hosting
{
	public class WebFormsTaskScheduler : TaskScheduler
	{
		const string ThreadPoolTaskSchedulerTypeName = "System.Threading.Tasks.ThreadPoolTaskScheduler, System.Private.CoreLib";
		static TaskScheduler threadPoolTaskScheduler = null;
		static Type threadPoolTaskSchedulerType = null;
		static Type ThreadPoolTaskSchedulerType => threadPoolTaskSchedulerType ??= Type.GetType(ThreadPoolTaskSchedulerTypeName);
		static TaskScheduler ThreadPoolTaskScheduler => threadPoolTaskScheduler ??= (TaskScheduler)Activator.CreateInstance(ThreadPoolTaskSchedulerType);
		/// <summary>
		/// Constructs a new ThreadPool task scheduler object or runs in a separate thread for
		/// long running task
		/// </summary>
		public WebFormsTaskScheduler()
		{
			int id = base.Id; // force ID creation of the default scheduler
		}


		// static delegate for threads allocated to handle LongRunning tasks.
		private static readonly ParameterizedThreadStart s_longRunningThreadWork = static s =>
		{
			//((Task)s).ExecuteEntryUnsafe(threadPoolThread: null);
			var executeEntryUnsafe = s.GetType().GetMethod("ExecuteEntryUnsafe", Reflection.BindingFlags.NonPublic);
			executeEntryUnsafe.Invoke(s, new object[] { null });
		};

		/// <summary>
		/// Schedules a task to the ThreadPool.
		/// </summary>
		/// <param name="task">The task to schedule.</param>
		protected internal override void QueueTask(Task task)
		{
			TaskCreationOptions options = task.CreationOptions;
			if ((options & TaskCreationOptions.LongRunning) != 0)
			{
				// Run LongRunning tasks on their own dedicated thread.
				new Thread(s_longRunningThreadWork)
				{
					IsBackground = false,
					Name = ".NET Long Running Task"
				}.UnsafeStart(task);
			}
			else
			{
				// Normal handling for non-LongRunning tasks.
				//ThreadPool.UnsafeQueueUserWorkItemInternal(task, (options & TaskCreationOptions.PreferFairness) == 0);
				var queueTask = ThreadPoolTaskSchedulerType.GetMethod("QueueTask", Reflection.BindingFlags.NonPublic);
				queueTask.Invoke(ThreadPoolTaskScheduler, new object[] { task });
			}
		}

		/// <summary>
		/// This internal function will do this:
		///   (1) If the task had previously been queued, attempt to pop it and return false if that fails.
		///   (2) Return whether the task is executed
		///
		/// IMPORTANT NOTE: TryExecuteTaskInline will NOT throw task exceptions itself. Any wait code path using this function needs
		/// to account for exceptions that need to be propagated, and throw themselves accordingly.
		/// </summary>
		protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
		{
			var tryExecuteTaskInline = ThreadPoolTaskSchedulerType.GetMethod("TryExecuteTaskInline", Reflection.BindingFlags.NonPublic);
			return (bool)tryExecuteTaskInline.Invoke(ThreadPoolTaskScheduler, new object[] { task, taskWasPreviouslyQueued });
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			var getScheduledTasks = ThreadPoolTaskSchedulerType.GetMethod("GetScheduledTasks", Reflection.BindingFlags.NonPublic);
			return (IEnumerable<Task>)getScheduledTasks.Invoke(ThreadPoolTaskScheduler, new object[0]);
		}
	}
}
