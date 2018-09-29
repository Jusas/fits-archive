using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace FitsArchiveLib
{
    public class FitsDatabase : IFitsDatabase
    {
        public event AddStatusHandler OnAddStatusUpdated;

        public string DatabaseFile { get; private set; }

        public int FileCount => QueryFileCount();
        private SQLiteConnection _connection;
        private IFitsFileFactory _fitsCreator;

        public FitsDatabase(IFitsFileFactory fitsCreator, 
            string databaseFilename, bool createIfNotExist)
        {
            _fitsCreator = fitsCreator;
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
                    throw new FitsDatabaseException("Failed to open the database file", e);
                }
                
            }
            else
            {
                throw new FitsDatabaseException("Database file does not exist, cannot open it");
            }
        }


        public async Task AddFiles(IEnumerable<string> filePaths)
        {
            Open();
            OnAddStatusUpdated?.Invoke(FileDbAddStatus.ReadingHeaders, null, 0, filePaths.Count());
            var awaitables = filePaths.Select(ReadFitsFile);
            var fitsFiles = await Task.WhenAll(awaitables);
            await InsertOrUpdateFitsFiles(fitsFiles.Where(ff => ff != null));
        }

        public async Task AddDirectory(string directoryPath, bool recursive)
        {
            throw new NotImplementedException();
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

        private async Task<IFitsFile> ReadFitsFile(string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    IFitsFile ff = _fitsCreator.CreateFitsFile(filePath);
                    return ff;
                });
            }
            catch (Exception e)
            {
                // TODO error reporting
            }
            return null;
        }

        private async Task InsertOrUpdateFitsFiles(IEnumerable<IFitsFile> fitsFiles)
        {
            Open();

            int handled = 0;
            foreach (var ff in fitsFiles)
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
                }
                // Does not exist in database; insert required.
                else if (fileId == null)
                {
                    await PerformFitsTableTransaction(ff, null);
                }
                // If the file was present in the DB and checksum matches, nothing to do.
                else
                {
                }

                handled++;
                OnAddStatusUpdated?.Invoke(FileDbAddStatus.InsertingAndUpdating, ff.FilePath, handled, fitsFiles.Count());
            }

            OnAddStatusUpdated?.Invoke(FileDbAddStatus.TransactionCompleted, null, handled, fitsFiles.Count());
        }

        private async Task PerformFitsTableTransaction(IFitsFile ff, long? fileId)
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

        private Dictionary<string, object> ConstructIndexedKeywordDictionary(IFitsFile ff)
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
