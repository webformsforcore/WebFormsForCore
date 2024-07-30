// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowException.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   WorkflowException class. Represents errors that occur in CSL Framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>WorkflowException class. Represents errors that occur in CSL Framework.</summary>
    [global::System.Serializable]
    public class WorkflowException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowException"/> class.
        /// </summary>
        public WorkflowException() 
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowException"/> class.</summary>
        /// <param name="message">The message.</param>
        public WorkflowException(string message) : base(message) 
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowException"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public WorkflowException(string message, Exception inner) : base(message, inner) 
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WorkflowException"/> class.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).</exception>
        protected WorkflowException(
          SerializationInfo info, 
          StreamingContext context)
            : base(info, context) 
        {
        }
    }
}
