// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildWorkflowException.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <summary>
//   BuildWorkflowException class. This class represents errors that occured during
//   the CSL build process.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>BuildWorkflowException class. This class represents errors that occured during 
    /// the CSL build process.</summary>
    [Serializable]
    internal class BuildWorkflowException : WorkflowException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildWorkflowException"/> class.
        /// </summary>
        public BuildWorkflowException()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BuildWorkflowException"/> class.</summary>
        /// <param name="message">The message.</param>
        public BuildWorkflowException(string message)
            : base(message)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BuildWorkflowException"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public BuildWorkflowException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="BuildWorkflowException"/> class.</summary>
        /// <param name="message">The message.</param>
        /// <param name="subcategory">The subcategory.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="helpKeyword">The help keyword.</param>
        /// <param name="file">The file path.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="columnNumber">The column number.</param>
        /// <param name="endLineNumber">The end line number.</param>
        /// <param name="endColumnNumber">The end column number.</param>
        /// <param name="inner">The inner.</param>
        public BuildWorkflowException(string message, string subcategory, string errorCode, string helpKeyword, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, Exception inner)
            : base(message, inner)
        {
            this.HasDetailedError = true;
            this.Subcategory = subcategory;
            this.ErrorCode = errorCode;
            this.HelpKeyword = helpKeyword;
            this.File = file;
            this.LineNumber = lineNumber;
            this.ColumnNumber = columnNumber;
            this.EndLineNumber = endLineNumber;
            this.EndColumnNumber = endColumnNumber;
        }

        /// <summary>Initializes a new instance of the <see cref="BuildWorkflowException"/> class.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).</exception>
        protected BuildWorkflowException(
            SerializationInfo info, 
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>Gets a value indicating whether HasDetailedError.</summary>
        public bool HasDetailedError { get; private set; }

        /// <summary>Gets or sets Subcategory.</summary>
        public string Subcategory { get; set; }

        /// <summary>Gets or sets ErrorCode.</summary>
        public string ErrorCode { get; set; }

        /// <summary>Gets or sets HelpKeyword.</summary>
        public string HelpKeyword { get; set; }

        /// <summary>Gets or sets File.</summary>
        public string File { get; set; }

        /// <summary>Gets or sets LineNumber.</summary>
        public int LineNumber { get; set; }

        /// <summary>Gets or sets ColumnNumber.</summary>
        public int ColumnNumber { get; set; }

        /// <summary>Gets or sets EndLineNumber.</summary>
        public int EndLineNumber { get; set; }

        /// <summary>Gets or sets EndColumnNumber.</summary>
        public int EndColumnNumber { get; set; }

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

            info.AddValue("HasDetailedError", this.HasDetailedError);
            info.AddValue("Subcategory", this.Subcategory);
            info.AddValue("ErrorCode", this.ErrorCode);
            info.AddValue("HelpKeyword", this.HelpKeyword);
            info.AddValue("File", this.File);
            info.AddValue("LineNumber", this.LineNumber);
            info.AddValue("ColumnNumber", this.ColumnNumber);
            info.AddValue("EndLineNumber", this.EndLineNumber);
            info.AddValue("EndColumnNumber", this.EndColumnNumber);
            base.GetObjectData(info, context);
        }
#endif
    }
}
