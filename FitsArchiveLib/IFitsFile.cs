using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib
{
    public struct HeaderKeyValue
    {
        public string Key;
        public object Value;
        public Type Type;
        public string Comment;

        public HeaderKeyValue(string key, object val, string comment)
        {
            Key = key;
            Value = val ?? "";
            Type = Value.GetType();
            Comment = comment;
        }
    }

    public interface IFitsFile
    {
        string FilePath { get; }
        long FileSize { get; }
        string FileHash { get; }
        IReadOnlyList<string> HeaderKeys { get; }
        HeaderKeyValue GetSingleHeaderValue(string key);
        IReadOnlyList<HeaderKeyValue> GetHeaderMultiValue(string key);
        bool HasHeaderSingleValue(string key);
        bool HasHeaderMultiValue(string key);
        IReadOnlyList<HeaderKeyValue> GetHeaderNumericValues();
        IReadOnlyList<HeaderKeyValue> GetHeaderBooleanValues();
        IReadOnlyList<HeaderKeyValue> GetHeaderStringValues();
    }
}
