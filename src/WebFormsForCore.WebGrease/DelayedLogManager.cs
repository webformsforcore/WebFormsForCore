// ----------------------------------------------------------------------------------------------------
// <copyright file="DelayedLogManager.cs" company="Microsoft Corporation">
//   Copyright Microsoft Corporation, all rights reserved.
// </copyright>
// ----------------------------------------------------------------------------------------------------
namespace WebGrease
{
    using System;
    using System.Collections.Generic;

    using WebGrease.Css.Extensions;
    using WebGrease.Extensions;

    /// <summary>The delayed log manager, will send out all it's message on dispose, except for errors they are also outputted immediately.</summary>
    internal class DelayedLogManager
    {
        /// <summary>The message format for the logger.</summary>
        private const string MessageFormat = "{0} {1:HH:mm:ss.ff} {2}";

        /// <summary>The log name.</summary>
        private readonly string messagePrefix;

        /// <summary>The actions.</summary>
        private readonly IList<Tuple<string, Action<string>>> actions = new List<Tuple<string, Action<string>>>();

        /// <summary>The flush lock object.</summary>
        private readonly object flushLock = new object();

        /// <summary>If the log is flushed.</summary>
        private bool isFlushed;

        /// <summary>Initializes a new instance of the <see cref="DelayedLogManager"/> class.</summary>
        /// <param name="syncLogManager">The sync log manager.</param>
        /// <param name="messagePrefix">The log name.</param>
        public DelayedLogManager(LogManager syncLogManager, string messagePrefix = null)
        {
            this.messagePrefix = messagePrefix;
            this.LogManager = new LogManager(
                (m, importance) => this.AddTimedAction(m, message => syncLogManager.Information(message, importance)),
                m => this.AddTimedAction(m, syncLogManager.Warning),
                (subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, m) => this.AddTimedAction(m, message => syncLogManager.Warning(subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, message)),
                m => this.AddTimedAction(m, syncLogManager.Error),
                (exception, m, name) => this.AddTimedAction(m, message => syncLogManager.Error(exception, message, name)),
                (subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, m) => this.AddTimedAction(m, message => syncLogManager.Error(subcategory, code, keyword, file, number, columnNumber, lineNumber, endColumnNumber, message)));
        }

        /// <summary>Gets the log manager.</summary>
        public LogManager LogManager { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Flush()
        {
            if (!this.isFlushed)
            {
                Safe.Lock(this.flushLock, () =>
                {
                    if (!this.isFlushed)
                    {
                        this.isFlushed = true;
                        this.actions.ForEach(a => a.Item2(a.Item1));
                        this.actions.Clear();
                    }
                });
            }
        }

        /// <summary>Add a timed action.</summary>
        /// <param name="message">The message.</param>
        /// <param name="action">The action.</param>
        private void AddTimedAction(string message, Action<string> action)
        {
            var formattedMessage = MessageFormat.InvariantFormat(this.messagePrefix, DateTime.Now, message);
            Safe.Lock(this.flushLock, () =>
            {
                if (this.isFlushed)
                {
                    action(formattedMessage);
                }
                else
                {
                    this.actions.Add(Tuple.Create(formattedMessage, action));
                }
            });
        }
    }
}