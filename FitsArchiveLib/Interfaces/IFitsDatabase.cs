using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitsArchiveLib.Database;

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

        IQueryable<FitsTableRow> FileListAsQueryable();
        IQueryable<FitsHeaderIndexedRow> HeadersIndexedAsQueryable();
        IQueryable<PlateSolvesTable> PlateSolvesAsQueryable();
    }
}
