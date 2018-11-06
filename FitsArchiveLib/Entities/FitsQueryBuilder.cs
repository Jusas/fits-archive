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
using FitsArchiveLib.Utils;
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
        public IFitsQueryExpression RaDecRadius(string ra, string dec, double radius)
        {

            var dRa = CoordinateTransform.HmsToDegrees(ra);
            var dDec = CoordinateTransform.DmsToDegrees(dec);
            var dRadius = CoordinateTransform.ArcminToDegrees((int)radius);

            return RaDecRadius(dRa, dDec, dRadius);

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
            Expression<Func<FitsSearchResult, bool>> expr =
                (x) => x.HeaderData.ParsedRa >= (ra - radius) && x.HeaderData.ParsedRa <= (ra + radius) &&
                       x.HeaderData.ParsedDec >= (dec - radius) && x.HeaderData.ParsedDec <= (dec + radius);

            return new FitsQueryExpression()
            {
                Expression = expr
            };
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
            
            // The expression works on FitsSearchResult, which combines all the related SQL tables.
            // Construct the expression dynamically from the property name linked to the FITS header name.

            var exprParamType = typeof(FitsSearchResult);
            
            // ... == targetValue
            var targetValue = Expression.Constant(value);
            // ((FitsSearchResult)x) => ...
            var parameter = Expression.Parameter(exprParamType, "x");
            // ((FitsSearchResult)x) => x.HeaderData.property
            Expression property = Expression.PropertyOrField(parameter, nameof(FitsSearchResult.HeaderData));
            property = Expression.PropertyOrField(property, matchingProp.Name);

            // Convert property from nullable to non-nullable type because expressions can't
            // really handle nullables (value gets always converted to non-nullable, and comparison
            // between nullables and non-nullables will throw).
            var nonNullableType = valueType.ToNullableUnderlying();
            property = Expression.Convert(property, nonNullableType);

            // ((FitsSearchResult)x) => x.HeaderData.property == targetValue
            var body = Expression.Equal(property, targetValue);
            var lambda = Expression.Lambda<Func<FitsSearchResult, bool>>(body, parameter);

            var expr = new FitsQueryExpression()
            {
                Expression = lambda
            };
            return expr;
        }

        /// <summary>
        /// Search for string inside a FITS keyword value.
        /// Both the fitsKeyword and the searchString must be strings.
        /// </summary>
        /// <param name="fitsKeyword"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public IFitsQueryExpression KeywordSearch(string fitsKeyword, string searchString)
        {
            var exprParamType = typeof(FitsSearchResult);
            var headerType = typeof(FitsHeaderIndexedRow);
            var headerKeywordProps = headerType.GetProperties().Where(p => p.HasAttribute<FitsFieldAttribute>());
            var matchingProp = headerKeywordProps.FirstOrDefault(p =>
                p.GetCustomAttribute<FitsFieldAttribute>().Name == fitsKeyword);
            if (matchingProp == null)
                throw new FitsDatabaseException($"Cannot add keyword matching for keyword '{fitsKeyword}' to the query, " +
                                                $"this keyword doesn't exist in the indexed keywords");

            if (matchingProp.PropertyType != typeof(string))
                throw new FitsDatabaseException($"Cannot add keyword matching for keyword '{fitsKeyword}' to the query, " +
                                                $"the given value is not of type '{matchingProp.PropertyType.Name}'");
            
            var targetValue = Expression.Constant(searchString);            
            var parameter = Expression.Parameter(exprParamType, "x");
            Expression property = Expression.PropertyOrField(parameter, nameof(FitsSearchResult.HeaderData));
            property = Expression.PropertyOrField(property, matchingProp.Name);

            MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var body = Expression.Call(property, method, targetValue);
            var lambda = Expression.Lambda<Func<FitsSearchResult, bool>>(body, parameter);

            var expr = new FitsQueryExpression()
            {
                Expression = lambda
            };
            return expr;
        }

        /// <summary>
        /// Does a comparison with the given operator to a numeric FITS keyword value.
        /// </summary>
        /// <param name="fitsKeyword"></param>
        /// <param name="value"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public IFitsQueryExpression NumericValueComparison(string fitsKeyword, double value,
            NumericComparison comparison)
        {
            var exprParamType = typeof(FitsSearchResult);
            var headerType = typeof(FitsHeaderIndexedRow);
            var headerKeywordProps = headerType.GetProperties().Where(p => p.HasAttribute<FitsFieldAttribute>());
            var matchingProp = headerKeywordProps.FirstOrDefault(p =>
                p.GetCustomAttribute<FitsFieldAttribute>().Name == fitsKeyword);
            if (matchingProp == null)
                throw new FitsDatabaseException($"Cannot add keyword matching for keyword '{fitsKeyword}' to the query, " +
                                                $"this keyword doesn't exist in the indexed keywords");

            if (matchingProp.PropertyType != typeof(double))
                throw new FitsDatabaseException($"Cannot add keyword matching for keyword '{fitsKeyword}' to the query, " +
                                                $"the given value is not of type '{matchingProp.PropertyType.Name}'");

            var targetValue = Expression.Constant(value);
            var parameter = Expression.Parameter(exprParamType, "x");
            Expression property = Expression.PropertyOrField(parameter, nameof(FitsSearchResult.HeaderData));
            property = Expression.PropertyOrField(property, matchingProp.Name);

            Expression body = null;
            switch (comparison)
            {
                case NumericComparison.Eq:
                    body = Expression.Equal(property, targetValue);
                    break;
                case NumericComparison.Gt:
                    body = Expression.GreaterThan(property, targetValue);
                    break;
                case NumericComparison.Gte:
                    body = Expression.GreaterThanOrEqual(property, targetValue);
                    break;
                case NumericComparison.Lt:
                    body = Expression.LessThan(property, targetValue);
                    break;
                case NumericComparison.Lte:
                    body = Expression.LessThanOrEqual(property, targetValue);
                    break;
            }

            var lambda = Expression.Lambda<Func<FitsSearchResult, bool>>(body, parameter);

            var expr = new FitsQueryExpression()
            {
                Expression = lambda
            };
            return expr;

        }

    }
}
