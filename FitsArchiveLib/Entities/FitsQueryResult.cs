using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Database;

namespace FitsArchiveLib.Entities
{
    public class FitsQueryResult
    {
        public class ResultItem
        {
            //public FitsTableRow FileInfo { get; }
            //public FitsHeaderIndexedRow Headers { get; }
            //public PlateSolveRow PlateSolve { get; }

            //public ResultItem(FitsTableRow fileInfo, FitsHeaderIndexedRow headers, PlateSolveRow platesolve)
            //{
            //    FileInfo = fileInfo;
            //    Headers = headers;
            //    PlateSolve = platesolve;
            //}
        }

        private readonly List<ResultItem> _resultItems;
        public int NumResults => _resultItems.Count;
        public IReadOnlyList<ResultItem> Items => _resultItems;

        public FitsQueryResult(IEnumerable<ResultItem> resultItems)
        {
            _resultItems = new List<ResultItem>(resultItems);
        }

        
    }
}
