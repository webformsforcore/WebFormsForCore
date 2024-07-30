// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceOverrideException.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Exception class for signaling errors if tokens are overwritten.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Activities
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.Serialization;

    /// <summary>Exception class for signaling errors if tokens are overwritten.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "Avoiding CAS problems in lower trust")]
    public class ResourceOverrideException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceOverrideException"/> class.
        /// </summary>
        public ResourceOverrideException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ResourceOverrideException"/> class.</summary>
        /// <param name="message">Error message.</param>
        public ResourceOverrideException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ResourceOverrideException"/> class.</summary>
        /// <param name="message">Error message.</param>
        /// <param name="inner">Inner exception.</param>
        public ResourceOverrideException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ResourceOverrideException"/> class.</summary>
        /// <param name="fileName">Full path to .token file where overriding occured.</param>
        /// <param name="tokenKey">Key name that got overwritten.</param>
        public ResourceOverrideException(string fileName, string tokenKey)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(tokenKey));
            Contract.Requires(!string.IsNullOrWhiteSpace(fileName));
            
            this.TokenKey = tokenKey;
            this.FileName = fileName;
        }

        /// <summary>Initializes a new instance of the <see cref="ResourceOverrideException"/> class.</summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Serialization context.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).</exception>
        protected ResourceOverrideException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }

        /// <summary>
        /// Gets a token file name where the override occured.
        /// </summary>
        /// <value>
        /// The name of the file where the override occured.
        /// </value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the duplicate key in the .token file.
        /// </summary>
        /// <value>
        /// Duplicate key in the .token file.
        /// </value>
        public string TokenKey { get; private set; }

#if !SILVERLIGHT
        /// <summary>
        /// Implements ISerializable.GetObjectData to persist LoaderException properties
        /// </summary>
        /// <param name="info">SerializationInfo instance</param>
        /// <param name="context">StreamingContext instance</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("FileName", this.FileName ?? string.Empty);
            info.AddValue("TokenKey", this.TokenKey ?? string.Empty);
            base.GetObjectData(info, context);
        }
#endif
    }
}
