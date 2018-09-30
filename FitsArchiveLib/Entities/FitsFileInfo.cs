using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using FitsArchiveLib.Interfaces;
using nom.tam.fits;

namespace FitsArchiveLib.Entities
{
    public class FitsFileException : Exception
    {
        public FitsFileException()
        {
        }

        public FitsFileException(string message) : base(message)
        {
        }

        public FitsFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FitsFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public class FitsFileInfo : IFitsFileInfo
    {
        public string FilePath { get; }
        public long FileSize { get; }
        public string FileHash { get; }

        public IReadOnlyList<string> HeaderKeys
        {
            get { return _header.Keys.ToList(); }
        }

        private Dictionary<string, IList<HeaderKeyValue>> _header = 
            new Dictionary<string, IList<HeaderKeyValue>>();

        public FitsFileInfo(string filePath)
        {
            FilePath = filePath;
            Fits f = new Fits(filePath);
            var hdus = f.Read();
            
            var headers = hdus.Where(hdu => hdu is ImageHDU).Select(hdu => hdu.Header);
            headers.ToList().ForEach(header =>
            {
                var cur = header.GetCursor();
                object cardPtr = null;
                // return (HeaderCard) ((DictionaryEntry) this.cursor.Current).Value;
                while ((cardPtr = cur.Current) != null)
                {
                    var card = (HeaderCard) ((DictionaryEntry)cardPtr).Value;
                    cur.MoveNext();
                    IList<HeaderKeyValue> keyValues = null;
                    keyValues = _header.ContainsKey(card.Key) ? _header[card.Key] : new List<HeaderKeyValue>();
                    _header[card.Key] = keyValues;
                    var val = card.IsStringValue ? new HeaderKeyValue(card.Key, card.Value, card.Comment) : ConvertType(card);
                    keyValues.Add(val);
                }
            });
            f.Close();

            //FileStream s = null;
            //using (s = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    var md5 = MD5.Create();
            //    var hashBytes = md5.ComputeHash(s);
            //    FileHash = Convert.ToBase64String(hashBytes);
            //}
            // Instead of reading the whole file for hash (which will be slow, especially with large
            // files and there can be thousands to read per batch) we just read the headers,
            // and calculate a hash from them. We only care about the indexed headers anyway,
            // if they don't change, nothing in the index needs to change.

            var md5 = MD5.Create();
            var hashBytes = md5.ComputeHash(HeadersToBytes());
            FileHash = Convert.ToBase64String(hashBytes);
            FileInfo finfo = new FileInfo(filePath);
            FileSize = finfo.Length;
        }

        private byte[] HeadersToBytes()
        {
            List<string> all = new List<string>();
            foreach (var h in _header)
            {
                foreach (var headerKeyValue in h.Value)
                {
                    all.Add($"{h.Key}.{headerKeyValue.Value}.{headerKeyValue.Comment}");
                }
            }
            return Encoding.UTF8.GetBytes(string.Join("|", all));
        }

        private HeaderKeyValue ConvertType(HeaderCard card)
        {
            if (!card.KeyValuePair)
                return new HeaderKeyValue(card.Key, null, card.Comment);

            // We know it isn't a string, so it's either a boolean or a number.
            // Boolean is expected to be "T" or "F". In all other cases it's a number.
            if(card.Value == "T" || card.Value == "F")
                return new HeaderKeyValue(card.Key, card.Value == "T", card.Comment);

            double val = 0;
            var converted = double.TryParse(card.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out val);
            if(converted)
                return new HeaderKeyValue(card.Key, val, card.Comment);

            // Fallback is the header value as string.
            return new HeaderKeyValue(card.Key, card.Value, card.Comment);
        }

        public HeaderKeyValue GetSingleHeaderValue(string key)
        {
            if (_header.ContainsKey(key))
            {
                if (_header[key].Count > 0)
                    return _header[key].First();
            }

            throw new FitsFileException($"Header does not contain key '{key}'");
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderMultiValue(string key)
        {
            if (_header.ContainsKey(key))
            {
                return _header[key].ToList();
            }

            throw new FitsFileException($"Header does not contain key '{key}'");
        }

        public bool HasHeaderSingleValue(string key)
        {
            return _header.ContainsKey(key) && _header[key].Count == 1;
        }

        public bool HasHeaderMultiValue(string key)
        {
            return _header.ContainsKey(key) && _header[key].Count == 1;
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderNumericValues()
        {
            var keyValueListsFlattened = _header.Keys.ToList().SelectMany(key => _header[key]);
            return keyValueListsFlattened.Where(kv => kv.Type == typeof(double)).ToList();
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderBooleanValues()
        {
            var keyValueListsFlattened = _header.Keys.ToList().SelectMany(key => _header[key]);
            return keyValueListsFlattened.Where(kv => kv.Type == typeof(bool)).ToList();
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderStringValues()
        {
            var keyValueListsFlattened = _header.Keys.ToList().SelectMany(key => _header[key]);
            return keyValueListsFlattened.Where(kv => kv.Type == typeof(string)).ToList();
        }
    }
}
