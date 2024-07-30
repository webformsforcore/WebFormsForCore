// ----------------------------------------------------------------------------------------------------
// <copyright file="SyntaxException.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------

namespace WebGrease.Preprocessing
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The syntax error exception
    /// </summary>
    [Serializable]
    public class PreprocessingException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PreprocessingException"/> class.
        /// </summary>
        public PreprocessingException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PreprocessingException"/> class.</summary>
        /// <param name="message">The message.</param>
        public PreprocessingException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PreprocessingException"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public PreprocessingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PreprocessingException"/> class.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).</exception>
        protected PreprocessingException(
          SerializationInfo info,
          StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}