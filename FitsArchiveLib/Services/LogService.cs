using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Entities;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveLib.Services
{
    public class LogServiceException : Exception
    {
        public LogServiceException()
        {
        }

        public LogServiceException(string message) : base(message)
        {
        }

        public LogServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class LogService : ILogService
    {

        private Dictionary<string, ILog> _logs = new Dictionary<string, ILog>();

        public LogService()
        {
        }

        public ILog InitializeLog(string logId, string logfilename)
        {
            if (_logs.ContainsKey(logId))
            {
                throw new LogServiceException($"Log with id '{logId}' already exists");
            }
            if (!File.Exists(logfilename))
            {
                try
                {
                    var stream = File.Create(logfilename);
                    var log = new Log(stream);
                    _logs.Add(logId, log);
                    return log;
                }
                catch (Exception e)
                {
                    throw new LogServiceException($"Failed to create a new log with log id '{logId}'", e);
                }
            }
            else
            {
                try
                {
                    var stream = File.Open(logfilename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    var log = new Log(stream);
                    _logs.Add(logId, log);
                    return log;
                }
                catch (Exception e)
                {
                    throw new LogServiceException($"Failed to open existing log file for log id '{logId}' for appending", e);
                }
            }
        }

        public ILog InitializeLog(string logId, Stream stream)
        {
            if (_logs.ContainsKey(logId))
            {
                throw new LogServiceException($"Log with id '{logId}' already exists");
            }
            if (stream == null || !stream.CanWrite)
            {
                throw new LogServiceException($"Log stream is not writable for log id '{logId}'");
            }
            try
            {
                var log = new Log(stream);
                _logs.Add(logId, log);
                return log;
            }
            catch (Exception e)
            {
                throw new LogServiceException($"Failed to initialize log with id '{logId}'", e);
            }
        }

        public void Dispose()
        {
            foreach (var log in _logs.Values)
            {
                var stream = log.LogStream;
                log.Dispose();
                stream.Dispose();
            }
        }

        public bool HasLog(string logId)
        {
            return _logs.ContainsKey(logId);
        }

        public ILog GetLog(string logId)
        {
            if (_logs.ContainsKey(logId))
                return _logs[logId];
            throw new LogServiceException($"No log with id '{logId}' exists");
        }
    }
}
