using System;

namespace FitsArchiveLib.Entities
{
    [Flags]
    public enum LogEventCategory
    {
        Verbose = 1 << 0,
        Informational = 1 << 1,
        Warning = 1 << 2,
        Error = 1 << 3
    }

    public struct LogMessage
    {

        /// <summary>
        /// The message
        /// </summary>
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public DateTime Date { get; private set; }

        public string DateString { get { return Date.ToString("HH:mm:ss"); } }

        /// <summary>
        /// The message category
        /// </summary>
        public LogEventCategory Category { get; set; }
        
        public LogMessage(LogEventCategory category, string message, Exception exception)
        {
            Category = category;
            Message = message;
            Date = DateTime.Now;
            Exception = exception;
        }
    }
}
