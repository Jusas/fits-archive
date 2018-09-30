using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveLib.Tests.Mocks
{
    // Minimal implementation of FITS file.
    class MockFitsFileInfo : IFitsFileInfo
    {
        public string FilePath { get; }
        public long FileSize { get; }
        public string FileHash { get; }
        public IReadOnlyList<string> HeaderKeys => _headers.Keys.ToList();

        private Dictionary<string, HeaderKeyValue> _headers = new Dictionary<string, HeaderKeyValue>();

        public MockFitsFileInfo(string filePath)
        {
            FilePath = filePath;
            _headers.Add("COMMENT", new HeaderKeyValue("COMMENT", Guid.NewGuid().ToString(), ""));
            _headers.Add("BITPIX", new HeaderKeyValue("BITPIX", 16, ""));
            _headers.Add("NAXIS1", new HeaderKeyValue("NAXIS1", 1014, ""));
            _headers.Add("NAXIS2", new HeaderKeyValue("NAXIS2", 1024, ""));
            FileSize = 1024 * 1024 * 2;
            FileHash =
                Convert.ToBase64String(
                    MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Join("", _headers.Values.Select(v => v.Value.ToString())))));
        }

        public HeaderKeyValue GetSingleHeaderValue(string key)
        {
            return _headers[key];
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderMultiValue(string key)
        {
            return new List<HeaderKeyValue>() {_headers[key]};
        }

        public bool HasHeaderSingleValue(string key)
        {
            return _headers.ContainsKey(key);
        }

        public bool HasHeaderMultiValue(string key)
        {
            return false;
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderNumericValues()
        {
            var ns = new[] {"BITPIX", "NAXIS1", "NAXIS2"};
            return _headers.Where(h => ns.Contains(h.Key)).Select(h => h.Value).ToList();
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderBooleanValues()
        {
            return new List<HeaderKeyValue>();
        }

        public IReadOnlyList<HeaderKeyValue> GetHeaderStringValues()
        {
            return new List<HeaderKeyValue>() {_headers["COMMENT"]};
        }
    }
}
