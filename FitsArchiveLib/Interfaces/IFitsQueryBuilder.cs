using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitsArchiveLib.Interfaces
{
    public enum NumericComparison
    {
        Lt,
        Lte,
        Eq,
        Gt,
        Gte
    }

    /// <summary>
    /// Empty interface, merely to act as an outside handle to the internal FitsQueryExpression.
    /// </summary>
    public interface IFitsQueryExpression
    {
    }

    public interface IFitsQueryBuilder
    {
        IFitsQueryExpression RaDecRadius(string ra, string dec, double radius);
        IFitsQueryExpression RaDecRadius(double ra, double dec, double radius);
        IFitsQueryExpression KeywordMatching<T>(string fitsKeyword, T value);
        IFitsQueryExpression KeywordSearch(string fitsKeyword, string searchString);
        IFitsQueryExpression NumericValueComparison(string fitsKeyword, double value,
            NumericComparison comparison);
    }

}
