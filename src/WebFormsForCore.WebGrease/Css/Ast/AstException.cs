// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AstException.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   Custom Ast Exception
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.Css.Ast
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>Custom Ast Exception</summary>
    [Serializable]
    public class AstException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="AstException"/> class.</summary>
        public AstException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AstException"/> class.</summary>
        /// <param name="message">The message.</param>
        public AstException(string message) : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AstException"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public AstException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AstException"/> class.</summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected AstException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}