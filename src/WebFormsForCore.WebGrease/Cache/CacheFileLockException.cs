// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheFileLockException.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Threading;

    using WebGrease.Extensions;

    /// <summary>The file lock exception.</summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Not needed for this exception")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "Not needed for this exception")]
    public sealed class CacheFileLockException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="CacheFileLockException"/> class.</summary>
        /// <param name="lockFile">The lock file.</param>
        /// <param name="innerException">The inner Exception.</param>
        public CacheFileLockException(string lockFile, Exception innerException = null)
            : base(
                "Could not create the cache lock file because it already exists: {0}\r\nThis usually indicates that you another process is already running using this lockfile.".InvariantFormat(lockFile), 
                innerException)
        {
        }
    }
}