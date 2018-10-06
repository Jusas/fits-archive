using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Entities;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveLib.Services
{
    public class FitsDatabaseService : IFitsDatabaseService
    {
        public class FitsDatabaseServiceException : Exception
        {
            public FitsDatabaseServiceException()
            {
            }

            public FitsDatabaseServiceException(string message) : base(message)
            {
            }

            public FitsDatabaseServiceException(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected FitsDatabaseServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        private readonly IFitsFileInfoService _fileInfoService;
        private readonly ILogService _logService;

        private bool _logBesideDatabase = true;
        private string _commonLogName = null;

        public FitsDatabaseService(IFitsFileInfoService fitsFileInfoService,
            ILogService logService)
        {
            _fileInfoService = fitsFileInfoService;
            _logService = logService;
        }

        public IFitsDatabase GetFitsDatabase(string databaseFilename, bool createIfNotExist)
        {
            // Using file based log by default.
            // Maybe add log (file or stream) as parameter later on.

            ILog log = null;
            if (_logBesideDatabase)
                log = !_logService.HasLog(databaseFilename)
                    ? _logService.InitializeLog(databaseFilename, databaseFilename + ".log")
                    : _logService.GetLog(databaseFilename);
            else
                log = _logService.GetLog(_commonLogName);
            
            return new FitsDatabase(_fileInfoService, log, databaseFilename, createIfNotExist);
        }

        public void SetLoggingOptions(FitsDatabaseLogging opts, string commonLogName = null)
        {
            _logBesideDatabase = opts == FitsDatabaseLogging.LogBesideDatabase;
            if (!_logBesideDatabase)
            {
                if (commonLogName == null)
                    throw new FitsDatabaseServiceException("If LogToCommonLog is chosen, the common log name must be provided!");
                _commonLogName = commonLogName;
            }

        }
    }
}
