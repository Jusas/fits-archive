using System;
using System.IO;
using FitsArchiveLib.Entities;

namespace FitsArchiveLib.Interfaces
{
    public delegate void LogMessageHandler(LogMessage logMessage);

    public interface ILog : IDisposable
    {
        /// <summary>
        /// The log stream itself
        /// </summary>
        Stream LogStream { get; }

        /// <summary>
        /// Logs an event.
        /// Performs standard trace and additionally notifies all listeners 
        /// (handlers)
        /// </summary>
        void Write(LogEventCategory category, string message, Exception exception = null);

        /// <summary>
        /// Subscribes to listen to log messages. All logged messages will be relayed 
        /// to the listener.
        /// </summary>
        /// <param name="categories">The categories of messages the listener wants to receive</param>
        /// <param name="handler">The message handler</param>
        void Subscribe(LogMessageHandler handler, LogEventCategory categories);

        /// <summary>
        /// Unsubscribes a message handler from the given log source and from the given categories.
        /// It is possible to remove a single or multiple categories from the handler.
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="handler"></param>
        void UnSubscribe(LogMessageHandler handler, LogEventCategory categories);

    }
}
