// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Safe.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using WebGrease.Extensions;

    /// <summary>
    /// The safe class, adds a timeout on lock(). 
    /// The Resulting im code is the same when using lock() but with a exception when the timeout is reached.
    /// </summary>
    internal sealed class Safe : IDisposable
    {
        #region Fields

        /// <summary>
        /// The default lock timeout, set to 5 seconds for production, can be set to much lower or much higher when debugging.
        /// 5 seconds is neccesary for some image spriting scenario's that need to wait on each other on small virtual machines.
        /// </summary>
        internal const int DefaultLockTimeout = 5000;

        /// <summary>
        /// The maximum lock timeout value, used for scenario's that need to match the normal lock() in production.
        /// This can be set to a much lower value when debugging threading/dead-locking issues.
        /// </summary>
        internal const int MaxLockTimeout = int.MaxValue;

        /// <summary>The unique key locks.</summary>
        private static readonly IDictionary<string, object> UniqueKeyLocks = new Dictionary<string, object>();

        /// <summary>The secured flags.</summary>
        private readonly bool[] securedFlags;

        /// <summary>The padlocks.</summary>
        private readonly object[] padlocks;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Safe"/> class.</summary>
        /// <param name="padlockObjects">The padlock objects.</param>
        /// <param name="millisecondTimeout">The millisecond timeout.</param>
        public Safe(object[] padlockObjects, int millisecondTimeout)
        {
            this.padlocks = padlockObjects;
            this.securedFlags = new bool[this.padlocks.Length];
            for (int i = 0; i < this.padlocks.Length; i++)
            {
                this.securedFlags[i] = Monitor.TryEnter(this.padlocks[i], millisecondTimeout);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="Safe"/> class.</summary>
        /// <param name="padlockObject">The padlock.</param>
        /// <param name="milliSecondTimeout">The milli second timeout.</param>
        private Safe(object padlockObject, int milliSecondTimeout)
            : this(new[] { padlockObject }, milliSecondTimeout)
        {
        }

        #endregion

        /// <summary>Gets a value indicating whether secured.</summary>
        private bool Secured
        {
            get
            {
                return this.securedFlags.All(s => s);
            }
        }

        #region Public Methods and Operators

        /// <summary>The dispose.</summary>
        public void Dispose()
        {
            for (int i = 0; i < this.securedFlags.Length; i++)
            {
                if (this.securedFlags[i])
                {
                    Monitor.Exit(this.padlocks[i]);
                    this.securedFlags[i] = false;
                }
            }
        }

        /// <summary>
        /// Executes the action after acquiring a lock on the padlock object. 
        /// Will throw an exception timeout after DefaultTimeout is reached and no lock has been aquired.
        /// </summary>
        /// <param name="padlock">The padlock.</param>
        /// <param name="action">The code to run.</param>
        internal static void Lock(object padlock, Action action)
        {
            Lock(padlock, DefaultLockTimeout, action);
        }

        /// <summary>
        /// Executes the action after acquiring a lock on unique path of the file, this makes sure no file actions on the same file are run at the same time. 
        /// Will throw an exception timeout after DefaultTimeout is reached and no lock has been aquired.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <param name="fileAction">The file action.</param>
        internal static void FileLock(FileSystemInfo fileInfo, Action fileAction)
        {
            FileLock(fileInfo, DefaultLockTimeout, fileAction);
        }

        /// <summary>
        /// Executes the action after acquiring a lock on unique path of the file, this makes sure no file actions on the same file are run at the same time. 
        /// Will throw an exception timeout after DefaultTimeout is reached and no lock has been aquired.
        /// </summary>
        /// <param name="fileInfoItems">The file info items.</param>
        /// <param name="fileAction">The file action.</param>
        internal static void LockFiles(IEnumerable<FileInfo> fileInfoItems, Action fileAction)
        {
            var uniqueKeyLocks = new List<object>();
            Lock(
                UniqueKeyLocks,
                () =>
                {
                    foreach (var fileInfoItem in fileInfoItems)
                    {
                        var uniqueKey = fileInfoItem.FullName.ToUpperInvariant();
                        object lockObject;
                        if (!UniqueKeyLocks.TryGetValue(uniqueKey, out lockObject))
                        {
                            UniqueKeyLocks.Add(uniqueKey, lockObject = new object());
                        }

                        uniqueKeyLocks.Add(lockObject);
                    }
                });

            Lock(uniqueKeyLocks.ToArray(), MaxLockTimeout, fileAction);
        }

        /// <summary>
        /// Executes the action after acquiring a lock on unique path of the file, this makes sure no file actions on the same file are run at the same time. 
        /// Will throw an exception timeout after millisecondTimeout is reached and no lock has been aquired.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <param name="millisecondTimeout">The millisecond timeout.</param>
        /// <param name="fileAction">The file action.</param>
        internal static void FileLock(FileSystemInfo fileInfo, int millisecondTimeout, Action fileAction)
        {
            var file = fileInfo.FullName.ToUpperInvariant();
            UniqueKeyLock(file, millisecondTimeout, fileAction);
        }

        /// <summary>
        /// Executes the action after acquiring a lock using the unique value of the string.
        /// Will throw an exception timeout after millisecondTimeout is reached and no lock has been aquired.
        /// </summary>
        /// <param name="uniqueKey">The unique key.</param>
        /// <param name="millisecondTimeout">The millisecond timeout.</param>
        /// <param name="fileAction">The file action.</param>
        internal static void UniqueKeyLock(string uniqueKey, int millisecondTimeout, Action fileAction)
        {
            object uniqueKeyLock = null;
            Lock(
                UniqueKeyLocks,
                () =>
                {
                    if (!UniqueKeyLocks.TryGetValue(uniqueKey, out uniqueKeyLock))
                    {
                        UniqueKeyLocks.Add(uniqueKey, uniqueKeyLock = new object());
                    }
                });

            Lock(uniqueKeyLock, millisecondTimeout, fileAction);
        }

        /// <summary>
        /// Executes the action after acquiring a lock on the padlock object. 
        /// Will throw an exception timeout after DefaultTimeout is reached and no lock has been aquired.
        /// Returns the result of the action.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="padlock">The padlock.</param>
        /// <param name="action">The code to run.</param>
        /// <returns>The <see cref="TResult"/>.</returns>
        internal static TResult Lock<TResult>(object padlock, Func<TResult> action)
        {
            return Lock(padlock, DefaultLockTimeout, action);
        }

        /// <summary>
        /// Executes the action after acquiring a lock on the padlock object. 
        /// Will throw an exception timeout after millisecondTimeout is reached and no lock has been aquired.
        /// Returns the result of the action.
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="padlock">The padlock.</param>
        /// <param name="millisecondTimeout">The millisecond timeout.</param>
        /// <param name="action">The code to run.</param>
        /// <returns>The <see cref="TResult"/>.</returns>
        internal static TResult Lock<TResult>(object padlock, int millisecondTimeout, Func<TResult> action)
        {
            using (var bolt = new Safe(padlock, millisecondTimeout))
            {
                if (bolt.Secured)
                {
                    return action();
                }

                throw new TimeoutException(ResourceStrings.SafeLockFailedMessage.InvariantFormat(millisecondTimeout));
            }
        }

        /// <summary>
        /// Executes the action after acquiring a lock for each padlock object in the array. 
        /// Will throw an exception timeout after millisecondTimeout is reached and no lock has been aquired.
        /// </summary>
        /// <param name="padlocks">The padlock objects.</param>
        /// <param name="millisecondTimeout">The millisecond timeout.</param>
        /// <param name="action">The code to run.</param>
        internal static void Lock(object[] padlocks, int millisecondTimeout, Action action)
        {
            using (var bolt = new Safe(padlocks, millisecondTimeout))
            {
                if (bolt.Secured)
                {
                    action();
                }
                else
                {
                    throw new TimeoutException(ResourceStrings.SafeLockFailedMessage.InvariantFormat(millisecondTimeout));
                }
            }
        }

        /// <summary>
        /// Executes the action after acquiring a lock on the padlock object. 
        /// Will throw an exception timeout after millisecondTimeout is reached and no lock has been acquired.
        /// </summary>
        /// <param name="padlock">The padlock.</param>
        /// <param name="millisecondTimeout">The millisecond timeout.</param>
        /// <param name="action">The code to run.</param>
        internal static void Lock(object padlock, int millisecondTimeout, Action action)
        {
            using (var bolt = new Safe(padlock, millisecondTimeout))
            {
                if (bolt.Secured)
                {
                    action();
                }
                else
                {
                    throw new TimeoutException(ResourceStrings.SafeLockFailedMessage.InvariantFormat(millisecondTimeout));
                }
            }
        }

        /// <summary>The safe write to file stream.</summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        internal static bool WriteToFileStream(string filePath, Action<FileStream> action)
        {
            return WriteToFileStream(filePath, 10, 500, action);
        }

        /// <summary>The safe write to file stream.</summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="maxRetries">The max retries.</param>
        /// <param name="millisecondsTimeoutBetweenTries">The milliseconds timeout between tries.</param>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catch all on purpose for retry.")]
        private static bool WriteToFileStream(string fullPath, int maxRetries, int millisecondsTimeoutBetweenTries, Action<FileStream> action)
        {
            int numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    // Attempt to open the file exclusively.
                    using (var fs = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                    {
                        fs.ReadByte();
                        fs.Seek(0, SeekOrigin.Begin);

                        action(fs);

                        return true;
                    }
                }
                catch (Exception)
                {
                    if (numTries == maxRetries)
                    {
                        return false;
                    }

                    // Wait for the lock to be released
                    Thread.Sleep(millisecondsTimeoutBetweenTries);
                }
            }
        }

        #endregion
    }
}