using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FitsArchiveLib.Attributes;
using FitsArchiveLib.Database;
using FitsArchiveLib.Interfaces;
using FitsArchiveLib.Utils;
using LinqToDB;
using LinqToDB.Mapping;
using Ninject.Infrastructure.Language;

namespace FitsArchiveLib.Entities
{
    public class FitsDatabaseException : Exception
    {
        public FitsDatabaseException()
        {
        }

        public FitsDatabaseException(string message) : base(message)
        {
        }

        public FitsDatabaseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FitsDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class FitsDatabase : IFitsDatabase
    {
        private class CountHolder
        {
            public int Count;
        }

        private enum DataChange
        {
            Created,
            Updated,
            NotChanged
        }

        public event AddStatusHandler OnAddStatusUpdated;

        public string DatabaseFile { get; private set; }

        public int FileCount => QueryFileCount();
        // private SQLiteConnection _connection;
        // private FitsDbContext _context;
        private List<FitsDbContext> _dbContexts = new List<FitsDbContext>();
        private IFitsFileInfoService _fitsReader;
        private ILog _log;

        private readonly object _readCounterMutex = new object();
        

        public FitsDatabase(IFitsFileInfoService fitsReader, 
            ILog log,
            string databaseFilename, bool createIfNotExist)
        {
            _fitsReader = fitsReader;
            _log = log;

            if (!File.Exists(databaseFilename) && createIfNotExist)
            {
                DatabaseFile = databaseFilename;
                CreateNewDatabase(databaseFilename);
            }
            else if (File.Exists(databaseFilename))
            {
                DatabaseFile = databaseFilename;
                try
                {
                    using (Connection())
                    {
                    }
                }
                catch (Exception e)
                {
                    var err = "Failed to read the database file";
                    _log?.Write(LogEventCategory.Error, err, e);
                    throw new FitsDatabaseException(err, e);
                }                
            }
            else
            {
                var err = "Database file does not exist, cannot open it";
                _log?.Write(LogEventCategory.Error, err);
                throw new FitsDatabaseException(err);
            }
        }


        public async Task AddFiles(IEnumerable<string> filePaths)
        {
            _log?.Write(LogEventCategory.Informational, $"Adding {filePaths.Count()} files to FITS database...");
            OnAddStatusUpdated?.Invoke(FileDbAddStatus.ReadingHeaders, null, 0, filePaths.Count());
            var fitsFiles = await ReadFitsFiles(filePaths);
            await InsertOrUpdateFitsFiles(fitsFiles.Where(ff => ff != null));
        }

        private async Task<IEnumerable<IFitsFileInfo>> ReadFitsFiles(IEnumerable<string> filePaths)
        {
            var numFiles = filePaths.Count();
            _log?.Write(LogEventCategory.Informational, $"Reading headers of {numFiles} files...");
            
            int readCount = 0;
            var awaitables = filePaths.Select(fp => ReadFitsFileInfo(fp, (file) =>
            {
                lock (_readCounterMutex)
                {
                    readCount++;
                }
                _log?.Write(LogEventCategory.Informational, $"Read {readCount}/{numFiles} headers");
            }));
            return await Task.WhenAll(awaitables);
        }

        private SQLiteConnection Connection() => new SQLiteConnection($"Data Source={DatabaseFile};");
        private FitsDbContext Context()
        {
            // Keep record of contexts, they'll need to be disposed in order to relinquish
            // the database for renaming, deletion, etc. when the database gets closed/disposed.
            var c = new FitsDbContext($"Data Source={DatabaseFile};");
            _dbContexts.Add(c);
            return c;
        }

        private void CreateNewDatabase(string databaseFilename)
        {
            var asm = Assembly.GetAssembly(this.GetType());
            var dbTemplateFile = asm.GetManifestResourceNames()
                .FirstOrDefault(fn => fn.Contains("template.db"));
            using (var stream = asm.GetManifestResourceStream(dbTemplateFile))
            {
                byte[] ba = new byte[stream.Length];
                stream.Read(ba, 0, ba.Length);
                var newFile = File.Create(databaseFilename, ba.Length);
                newFile.Write(ba, 0, ba.Length);
                newFile.Close();
            }
        }

        private int QueryFileCount()
        {
            using (var context = Context())
            {
                return context.Files.Count();
            }
        }

        private async Task<IFitsFileInfo> ReadFitsFileInfo(string filePath, Action<string> readComplete)
        {
            try
            {
                return await Task.Run(() =>
                {
                    IFitsFileInfo ff = _fitsReader.GetFitsFileInfo(filePath);
                    EventUtils.IgnoreExceptions(() => readComplete?.Invoke(filePath));
                    return ff;
                });
            }
            catch (Exception e)
            {
                _log?.Write(LogEventCategory.Error, "Reading FITS file info failed for file '{filePath}'", e);
                EventUtils.IgnoreExceptions(() => readComplete?.Invoke(filePath));
            }            
            return null;
        }

        private async Task InsertOrUpdateFitsFiles(IEnumerable<IFitsFileInfo> fitsFiles)
        {
            int handled = 0;
            foreach (var ff in fitsFiles)
            {
                try
                {
                    var dataChange = await PerformFitsTableChangesTransaction(ff);
                    if(dataChange == DataChange.Created)
                        _log?.Write(LogEventCategory.Informational,
                            $"Read and indexed {ff.HeaderKeys.Count} keywords from " +
                            $"a new FITS file {Path.GetFileName(ff.FilePath)}");
                    else if (dataChange == DataChange.Updated)
                        _log?.Write(LogEventCategory.Informational,
                            $"Read and indexed {ff.HeaderKeys.Count} keywords from " +
                            $"a changed FITS file {Path.GetFileName(ff.FilePath)}");
                    else
                        _log?.Write(LogEventCategory.Informational,
                            $"Read {Path.GetFileName(ff.FilePath)}, file was already indexed and unchanged");
                    
                }
                catch (Exception e)
                {
                    var err = "Reading of file / extraction of FITS header keywords failed";
                    _log?.Write(LogEventCategory.Error, err, e);
                }

                handled++;
                EventUtils.IgnoreExceptions(() => OnAddStatusUpdated?.Invoke(FileDbAddStatus.InsertingAndUpdating, ff.FilePath, handled, fitsFiles.Count()));
            }

            EventUtils.IgnoreExceptions(() => OnAddStatusUpdated?.Invoke(FileDbAddStatus.TransactionCompleted, null, handled, fitsFiles.Count()));

        }

        private async Task<DataChange> PerformFitsTableChangesTransaction(IFitsFileInfo ff)
        {
            return await Task.Run(() =>
            {
                var opResult = DataChange.NotChanged;
                using (var context = Context())
                {
                    context.Connection.Open();
                    using (var transaction = context.BeginTransaction())
                    {
                        var fileTableChanges = false;
                        var entry = context.Files.Where(x => x.Filename == ff.FilePath).ToList().FirstOrDefault();
                        if (entry != null && entry.Checksum != ff.FileHash)
                        {
                            fileTableChanges = true;
                            opResult = DataChange.Updated;
                            entry.Checksum = ff.FileHash;
                            entry.Size = ff.FileSize;
                            entry.DateIndexed = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        }
                        else if(entry == null)
                        {
                            fileTableChanges = true;
                            opResult = DataChange.Created;
                            entry = new FitsTableRow()
                            {
                                Checksum = ff.FileHash,
                                DateIndexed = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                                Filename = ff.FilePath,
                                Size = ff.FileSize
                            };

                            entry.Id = context.InsertWithInt64Identity(entry);

                        }
                        
                        if (fileTableChanges)
                        {
                            var createHeaderRow = false;
                            var headers = context.HeadersIndexedTable.Where(x => x.FitsId == entry.Id).ToList().FirstOrDefault();
                            if (headers == null)
                            {
                                createHeaderRow = true;
                                headers = new FitsHeaderIndexedRow()
                                {
                                    FitsId = entry.Id.Value
                                };
                            }
                            var headerProps = typeof(FitsHeaderIndexedRow).GetProperties()
                                .Where(x => x.HasAttribute<FitsFieldAttribute>());

                            foreach (var prop in headerProps)
                            {
                                var fitsFieldAttr = prop.GetCustomAttribute<FitsFieldAttribute>();
                                var dbColAndHeaderName = fitsFieldAttr.Name;
                                if (!ff.HeaderKeys.Contains(dbColAndHeaderName))
                                    continue;
                                var updatedVal = fitsFieldAttr.MultiValue
                                    ? string.Join("\n",
                                        ff.GetHeaderMultiValue(dbColAndHeaderName).Select(kv => kv.Value ?? kv.Comment))
                                    : ff.GetSingleHeaderValue(dbColAndHeaderName).Value;
                                prop.SetValue(headers, CoerceValue(prop, updatedVal));
                            }

                            if (createHeaderRow)
                                context.Insert(headers);
                        }

                        transaction.Commit();
                    }

                }
                return opResult;

            });

        }

        /// <summary>
        /// This is a bit stupid, but found no way to assign nullables values
        /// using reflection.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private object CoerceValue(PropertyInfo prop, object value)
        {
            var nullableInnerType = Nullable.GetUnderlyingType(prop.PropertyType);
            if (nullableInnerType != null)
            {
                if (typeof(int) == nullableInnerType)
                {
                    int? x = (int)(value is int ? (int)value : value is double ? (double)value : (long)value);
                    return x;
                }
                if (typeof(double) == nullableInnerType)
                {
                    double? x = (double)(value is int ? (int)value : value is double ? (double)value : (long)value);
                    return x;
                }
                if (typeof(long) == nullableInnerType)
                {
                    long? x = (long)(value is int ? (int)value : value is double ? (double)value : (long)value);
                    return x;
                }
                if (typeof(bool) == nullableInnerType)
                {
                    bool? x = (bool)value;
                    return x;
                }
            }
            return value;
        }
        
        
        public void Dispose()
        {
            _dbContexts.ForEach(c => c.Dispose());
        }

        public IQueryable<FitsTableRow> FileListAsQueryable()
        {
            // Note: "leaks" contexts. This instance will keep record or produced
            // contexts in order to dispose of them when the database gets disposed.
            return Context().Files.AsQueryable();
        }

        public IQueryable<FitsHeaderIndexedRow> HeadersIndexedAsQueryable()
        {
            // Note: "leaks" contexts. This instance will keep record or produced
            // contexts in order to dispose of them when the database gets disposed.
            return Context().HeadersIndexedTable.AsQueryable();
        }

        public IQueryable<PlateSolvesTable> PlateSolvesAsQueryable()
        {
            // Note: "leaks" contexts. This instance will keep record or produced
            // contexts in order to dispose of them when the database gets disposed.
            return Context().PlateSolvesTable.AsQueryable();
        }

    }
}
