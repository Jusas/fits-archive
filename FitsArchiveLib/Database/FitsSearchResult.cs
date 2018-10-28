using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Database
{
    internal class FitsSearchResult
    {
        public FitsHeaderIndexedRow HeaderData;
        public FitsTableRow FileData;
        public PlateSolveRow PlateSolveData;
    }
}
