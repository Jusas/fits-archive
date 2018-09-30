using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitsArchiveLib.Interfaces
{
    public enum FileDbAddStatus
    {
        ReadingHeaders,
        InsertingAndUpdating,
        TransactionCompleted
    }

    public delegate void AddStatusHandler(FileDbAddStatus status, 
        string latestHandledFile, int processedFiles, int totalFiles);

    public interface IFitsDatabase : IDisposable
    {
        event AddStatusHandler OnAddStatusUpdated;

        string DatabaseFile { get; }
        int FileCount { get; }

        Task AddFiles(IEnumerable<string> filePaths);

    }
}
