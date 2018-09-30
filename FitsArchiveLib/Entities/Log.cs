using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveLib.Entities
{
    public class Log : ILog
    {

        //-------------------------------------------------------------------------------------------------------
        #region FIELDS AND PROPERTIES
        //-------------------------------------------------------------------------------------------------------

        private StreamWriter _streamWriter;

        private readonly object _mutex = new object();

        private readonly Dictionary<LogMessageHandler, LogEventCategory> _handlers = 
            new Dictionary<LogMessageHandler, LogEventCategory>();

        public Stream LogStream { get; private set; }

        #endregion


        //-------------------------------------------------------------------------------------------------------
        #region METHODS
        //-------------------------------------------------------------------------------------------------------

        public Log(Stream logOutputStream)
        {
            LogStream = logOutputStream;
            InitializeLogStream();
        }

        private void Trace(LogMessage msg)
        {
            lock (_mutex)
            {
                var formatted = string.Format("[{0:D2}:{1:D2}:{2:D2}.{3:D3}] [{4}] {5}",
                    msg.Date.Hour, msg.Date.Minute, msg.Date.Second, msg.Date.Millisecond,
                    msg.Category, msg.Message);                
                Console.WriteLine(formatted);
                _streamWriter.WriteLine(formatted);

                if (msg.Exception != null)
                {
                    var ex = msg.Exception.ToString();
                    Console.WriteLine(ex);
                    _streamWriter.WriteLine(ex);
                }

                _streamWriter.Flush();
            }
        }

        private void InitializeLogStream()
        {
            var now = DateTime.Now;

            _streamWriter = new StreamWriter(LogStream);
            _streamWriter.WriteLine("====================================================================");
            _streamWriter.WriteLine("LOGGING SESSION STARTED");
            _streamWriter.WriteLine("DATE: " + now.ToString("s"));
            _streamWriter.WriteLine("====================================================================");
            _streamWriter.Flush();

        }

        // make sure this gets called
        public void Dispose()
        {
            if (_streamWriter != null)
            {
                _streamWriter.Flush();
                _streamWriter.Dispose();
            }
        }

        public void Write(LogEventCategory category, string message, Exception exception = null)
        {
            lock (_mutex)
            {
                var msg = new LogMessage(category, message, exception);
                Trace(msg);
                foreach (var h in _handlers)
                {
                    if ((h.Value & category) > 0)
                    {
                        h.Key(msg);
                    }
                }
            }
        }

        public void Subscribe(LogMessageHandler handler, LogEventCategory categories)
        {
            lock (_mutex)
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                if (_handlers.ContainsKey(handler))
                    _handlers[handler] |= categories;
                else
                    _handlers.Add(handler, categories);                
            }
        }

        public void UnSubscribe(LogMessageHandler handler, LogEventCategory categories)
        {
            lock (_mutex)
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                if (!_handlers.ContainsKey(handler))
                    return;

                _handlers[handler] ^= categories;
                if (_handlers[handler] == 0)
                    _handlers.Remove(handler);
            }
        }

        #endregion
    }
}
