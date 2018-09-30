using System;
using System.Collections.Generic;

namespace FitsArchiveLib.Interfaces
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

    public interface IFitsFileInfo
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
