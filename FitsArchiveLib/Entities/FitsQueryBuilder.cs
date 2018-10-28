using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FitsArchiveLib.Attributes;
using FitsArchiveLib.Database;
using FitsArchiveLib.Interfaces;
using LinqToDB.Extensions;
using Ninject.Infrastructure.Language;

namespace FitsArchiveLib.Entities
{
    public class FitsQueryBuilder : IFitsQueryBuilder
    {
        // Add methods to add query parameters, and return self, then validate(), then build().
        // FitsDatabase will then use this object to run a query.
        // FitsDatabase will have a method that returns a new IFitsQueryBuilder.

        public FitsQueryBuilder()
        {

        }

        /// <summary>
        /// Add a RA/DEC constraint to the query.
        /// RA and DEC can be in either decimal degrees or in HH MM SS.sss, +/-DD MM SS.sss format.
        /// Radius is expected as arc minutes.
        /// </summary>
        /// <param name="ra">Center RA, either in degrees or 'HH MM SS.sss' format</param>
        /// <param name="dec">Center DEC, either in degrees or '+/-DD MM SS.sss' format</param>
        /// <param name="radius">Search radius, in arc minutes</param>
        /// <returns>The same FitsQueryBuilder for chaining.</returns>
        public FitsQueryBuilder RaDecRadius(string ra, string dec, double radius)
        {
            return this;
        }

        /// <summary>
        /// Add a RA/DEC constraint to the query.
        /// RA and DEC must be decimal degrees.
        /// Radius is expected as arc minutes.
        /// </summary>
        /// <param name="ra">Center RA in degrees</param>
        /// <param name="dec">Center DEC in degrees</param>
        /// <param name="radius">Search radius, in arc minutes</param>
        /// <returns>The same FitsQueryBuilder for chaining.</returns>
        public IFitsQueryExpression RaDecRadius(double ra, double dec, double radius)
        {
            throw new Exception();
            //Expression<Func<FitsHeaderIndexedRow, bool>> lambda = (x) => (x.Ra > 0.0 && x.Dec > 0.0) || (x.Filter == "X");
            //return new FitsQueryExpression()
            //{
            //    Expression = lambda
            //};
        }

        /// <summary>
        /// Add keyword value matching to the query.
        /// </summary>
        /// <param name="fitsKeyword"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IFitsQueryExpression KeywordMatching<T>(string fitsKeyword, T value)
        {
            var valueType = typeof(T);
            var headerType = typeof(FitsHeaderIndexedRow);
            var headerKeywordProps = headerType.GetProperties().Where(p => p.HasAttribute<FitsFieldAttribute>());
            var matchingProp = headerKeywordProps.FirstOrDefault(p => 
                p.GetCustomAttribute<FitsFieldAttribute>().Name == fitsKeyword);
            if(matchingProp == null)
                throw new FitsDatabaseException($"Cannot add keyword matching for keyword '{fitsKeyword}' to the query, " +
                                                $"this keyword doesn't exist in the indexed keywords");
            
            if(matchingProp.PropertyType != valueType && matchingProp.PropertyType.ToNullableUnderlying() != valueType)
                throw new FitsDatabaseException($"Cannot add keyword matching for keyword '{fitsKeyword}' to the query, " +
                                                $"the given value is not of type '{matchingProp.PropertyType.Name}'");
            
            var exprParamType = typeof(FitsSearchResult);
            var targetValue = Expression.Constant(value);
            var parameter = Expression.Parameter(exprParamType, "x");
            Expression property = Expression.PropertyOrField(parameter, nameof(FitsSearchResult.HeaderData));
            property = Expression.PropertyOrField(property, matchingProp.Name);
            var nonNullableType = valueType.ToNullableUnderlying();
            property = Expression.Convert(property, nonNullableType);

            var body = Expression.Equal(property, targetValue);
            var lambda = Expression.Lambda<Func<FitsSearchResult, bool>>(body, parameter);

            var expr = new FitsQueryExpression()
            {
                Expression = lambda
            };
            return expr;
        }

        public FitsQueryBuilder KeywordSearch<T>(string fitsKeyword, string searchString)
        {
            return this;
        }

        
    }
}
