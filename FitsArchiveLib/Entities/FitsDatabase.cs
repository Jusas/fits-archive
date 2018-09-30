using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;

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

        public event AddStatusHandler OnAddStatusUpdated;

        public string DatabaseFile { get; private set; }

        public int FileCount => QueryFileCount();
        private SQLiteConnection _connection;
        private IFitsFileInfoFactory _fitsReader;
        private ILog _log;

        private readonly object _readCounterMutex = new object();
        

        public FitsDatabase(IFitsFileInfoFactory fitsReader, 
            ILog log,
            string databaseFilename, bool createIfNotExist)
        {
            _fitsReader = fitsReader;
            _log = log;

            if (!File.Exists(databaseFilename) && createIfNotExist)
            {
                DatabaseFile = databaseFilename;
                CreateNewDatabase(databaseFilename);
                Open();
            }
            else if (File.Exists(databaseFilename))
            {
                DatabaseFile = databaseFilename;
                try
                {
                    Open();
                }
                catch (Exception e)
                {
                    var err = "Failed to open the database file";
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
            Open();
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
        
        private void Open()
        {
            string connString = $"Data Source={DatabaseFile};Version=3;";
            if(_connection == null)
                _connection = new SQLiteConnection(connString);
            if(_connection.State == ConnectionState.Closed || _connection.State == ConnectionState.Broken)
                _connection.Open();
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
            Open();
            var query = "SELECT COUNT(id) FROM Fits";
            using (var cmd = new SQLiteCommand(query, _connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return 0;
                    reader.Read();
                    return reader.GetInt32(0);
                }
            }
        }

        private async Task<IFitsFileInfo> ReadFitsFileInfo(string filePath, Action<string> readComplete)
        {
            try
            {
                return await Task.Run(() =>
                {
                    IFitsFileInfo ff = _fitsReader.CreateFitsFileInfo(filePath);
                    readComplete?.Invoke(filePath);
                    return ff;
                });
            }
            catch (Exception e)
            {
                _log?.Write(LogEventCategory.Error, "Reading FITS file info failed for file '{filePath}'", e);
                readComplete?.Invoke(filePath);
            }            
            return null;
        }

        private async Task InsertOrUpdateFitsFiles(IEnumerable<IFitsFileInfo> fitsFiles)
        {
            Open();

            int handled = 0;
            foreach (var ff in fitsFiles)
            {
                try
                {
                    var checkExist = "SELECT id, checksum FROM Fits WHERE filename = @fn";
                    long? fileId = null;
                    string checksum = null;
                    using (var cmd = new SQLiteCommand(checkExist, _connection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("fn", ff.FilePath));
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                fileId = reader.GetInt64(0);
                                checksum = reader.GetString(1);
                            }
                        }
                    }

                    // Exists in database, and file contents have changed.
                    // Requires an update.
                    if (fileId != null && checksum != ff.FileHash)
                    {
                        await PerformFitsTableTransaction(ff, fileId);
                        _log?.Write(LogEventCategory.Informational, 
                            $"Read and indexed {ff.HeaderKeys.Count} keywords from " +
                            $"a changed FITS file {Path.GetFileName(ff.FilePath)}");
                    }
                    // Does not exist in database; insert required.
                    else if (fileId == null)
                    {
                        await PerformFitsTableTransaction(ff, null);
                        _log?.Write(LogEventCategory.Informational,
                            $"Read and indexed {ff.HeaderKeys.Count} keywords from " +
                            $"a new FITS file {Path.GetFileName(ff.FilePath)}");
                    }
                    // If the file was present in the DB and checksum matches, nothing to do.
                    else
                    {
                        _log?.Write(LogEventCategory.Informational,
                            $"FITS file {Path.GetFileName(ff.FilePath)} is already indexed and has not changed, skipping.");
                    }
                }
                catch (Exception e)
                {
                    var err = "Extraction of FITS file header keywords failed";
                    _log?.Write(LogEventCategory.Error, err, e);
                }

                handled++;
                OnAddStatusUpdated?.Invoke(FileDbAddStatus.InsertingAndUpdating, ff.FilePath, handled, fitsFiles.Count());
            }

            OnAddStatusUpdated?.Invoke(FileDbAddStatus.TransactionCompleted, null, handled, fitsFiles.Count());
        }

        private async Task PerformFitsTableTransaction(IFitsFileInfo ff, long? fileId)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                using (var cmd = _connection.CreateCommand())
                {
                    cmd.Transaction = transaction;
                    if (fileId != null)
                    {
                        cmd.CommandText =
                            "UPDATE Fits SET filename = @fn, checksum = @cs, size = $sz, date_indexed = @dt " +
                            $"WHERE id = {fileId.Value}";
                    }
                    else
                    {
                        cmd.CommandText =
                            "INSERT INTO Fits (filename, checksum, size, date_indexed) VALUES (@fn, @cs, @sz, @dt)";
                    }
                    var parameters = new[]
                    {
                        new SQLiteParameter("fn", ff.FilePath),
                        new SQLiteParameter("cs", ff.FileHash),
                        new SQLiteParameter("sz", ff.FileSize),
                        new SQLiteParameter("dt", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
                    };
                    cmd.Parameters.AddRange(parameters);
                    await cmd.ExecuteNonQueryAsync();
                }

                using (var cmd = _connection.CreateCommand())
                {
                    var fitsId = fileId ?? _connection.LastInsertRowId;
                    cmd.Transaction = transaction;
                    var indexedKeywords = ConstructIndexedKeywordDictionary(ff);
                    if (fileId != null)
                    {
                        cmd.CommandText = $"UPDATE FitsHeaderIndexed SET " +
                                          string.Join(", ", indexedKeywords.Select(kw => $"'{kw.Key}'=@{SafeParamName(kw.Key)}")) +
                                          $" WHERE fits_id = {fileId.Value}";
                    }
                    else
                    {
                        cmd.CommandText = $"INSERT INTO FitsHeaderIndexed (fits_id, {string.Join(",", indexedKeywords.Keys.Select(k => $"'{k}'"))}) " +
                                          $"VALUES ({fitsId}, {string.Join(",", indexedKeywords.Keys.Select(k => "@" + SafeParamName(k)))})";
                    }
                    
                    var parameters = indexedKeywords.Select(kw => new SQLiteParameter(SafeParamName(kw.Key), kw.Value)).ToArray();
                    cmd.Parameters.AddRange(parameters);
                    await cmd.ExecuteNonQueryAsync();
                }

                transaction.Commit();                
            }
        }

        private string SafeParamName(string p)
        {
            return Regex.Replace(p, "[^a-zA-Z0-9_]+", "", RegexOptions.Compiled);
        }

        private Dictionary<string, object> ConstructIndexedKeywordDictionary(IFitsFileInfo ff)
        {
            var keys = ff.HeaderKeys;
            var keywords = new string[]
            {
                "AUTHOR",
                "BITPIX",
                "NAXIS1",
                "NAXIS2",
                "INSTRUME",
                "TELESCOP",
                "OBSERVER",
                "EXPTIME",
                "EXPOSURE",
                "GAIN",
                "CCD-TEMP",
                "XBINNING",
                "YBINNING",
                "FRAME",
                "FILTER",
                "FOCALLEN",
                "OBJECT",
                "OBJCTRA",
                "OBJCTDEC",
                "RA",
                "DEC",
                "RA_OBJ",
                "DEC_OBJ",
                "SITELAT",
                "SITELONG",
                "LATITUDE",
                "EQUINOX",
                "APERTURE",
                "TIME-OBS",
                "TIME-END",
                "DATE-OBS",
                "DATE-END",
                "AIRMASS",
                "PROGRAM",
                "SWCREATE",
                "OBJNAME"
            };
            var kvps = keywords.Select(
                kw => new KeyValuePair<string, object>(kw, keys.Contains(kw) ? ff.GetSingleHeaderValue(kw).Value : null));
            var indexedKeywords = new Dictionary<string, object>();
            foreach (var kvp in kvps)
            {
                indexedKeywords.Add(kvp.Key, kvp.Value);
            }

            if (keys.Contains("COMMENT"))
            {
                var comment = string.Join("\n", ff.GetHeaderMultiValue("COMMENT").Select(kv => kv.Comment));
                indexedKeywords.Add("COMMENT", comment);
            }
            return indexedKeywords;
            
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        

        //public static FitsDatabase FromFile(string filename)
        //{
        //    var connString = $"{filename};Version=3;";
        //    using (SQLiteConnection conn = new SQLiteConnection(connString))
        //    {
        //        conn.Open();
        //    }
        //}
    }
}
