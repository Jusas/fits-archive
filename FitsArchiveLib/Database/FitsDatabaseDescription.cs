using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Attributes;
using FitsArchiveLib.Interfaces;
using Ninject.Infrastructure.Language;

namespace FitsArchiveLib.Database
{
    public static class FitsDatabaseDescription
    {
        private static List<IndexedFitsKeyword> _indexedFitsKeywords;

        static FitsDatabaseDescription()
        {
            var headerRowType = typeof(FitsHeaderIndexedRow);
            var fitsFields = headerRowType.GetProperties().Where(x => x.HasAttribute<FitsFieldAttribute>())
                .Select(x => x.GetCustomAttribute<FitsFieldAttribute>());
            var keywords = fitsFields.Select(x => new IndexedFitsKeyword()
            {
                IsMultiValue = x.MultiValue,
                Keyword = x.Name,
                QueryHints = x.VarianceValue ? KeywordQueryHints.VarianceValue : KeywordQueryHints.None,
                ValueType = x.Numeric ? KeywordValueType.Number :
                    x.DateLike ? KeywordValueType.Date : KeywordValueType.String
            });
            _indexedFitsKeywords = new List<IndexedFitsKeyword>(keywords);
        }

        public static IReadOnlyList<IndexedFitsKeyword> IndexedFitsKeywords => _indexedFitsKeywords;
    }
}
