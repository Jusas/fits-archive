using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FitsArchiveLib.Interfaces;

namespace FitsArchiveLib.Database
{
    internal class FitsQueryExpression : IFitsQueryExpression
    {
        // public Expression<Func<FitsHeaderIndexedRow, bool>> Expression;
        public Expression<Func<FitsSearchResult, bool>> Expression;
        
        // public Expression Expression;
    }
}
