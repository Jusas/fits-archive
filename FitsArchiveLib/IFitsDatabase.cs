using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib
{
    public enum FileDbAddStatus
    {
        ReadingHeaders,
        InsertingAndUpdating,
        TransactionCompleted
    }

    public delegate void AddStatusHandler(
        FileDbAddStatus status, string latestHandledFile, int processedFiles, int totalFiles);

    public interface IFitsDatabase : IDisposable
    {
        event AddStatusHandler OnAddStatusUpdated;

        string DatabaseFile { get; }
        int FileCount { get; }

        Task AddFiles(IEnumerable<string> filePaths);
        Task AddDirectory(string directoryPath, bool recursive);

    }
}
