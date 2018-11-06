using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitsArchiveLib.Database;
using FitsArchiveLib.Entities;

namespace FitsArchiveLib.Interfaces
{
    public enum FileDbAddStatus
    {
        ReadingHeaders,
        InsertingAndUpdating,
        TransactionCompleted
    }

    public enum KeywordValueType
    {
        String,
        Date,
        Number
    }

    [Flags]
    public enum KeywordQueryHints
    {
        None,
        VarianceValue
    }

    public struct IndexedFitsKeyword
    {
        public string Keyword;
        public KeywordValueType ValueType;
        public bool IsMultiValue;
        public KeywordQueryHints QueryHints;
    }

    public delegate void AddStatusHandler(FileDbAddStatus status, 
        string latestHandledFile, int processedFiles, int totalFiles);

    public interface IFitsDatabase : IDisposable
    {
        event AddStatusHandler OnAddStatusUpdated;

        string DatabaseFile { get; }
        int FileCount { get; }
        IReadOnlyList<IndexedFitsKeyword> IndexedFitsKeyWords { get; }

        Task AddFiles(IEnumerable<string> filePaths);

        IFitsQueryBuilder GetQueryBuilder(); // this should probably be on its own, not a member
        Task<FitsQueryResult> RunQuery(IEnumerable<IFitsQueryExpression> queryExpressions);
    }
}
