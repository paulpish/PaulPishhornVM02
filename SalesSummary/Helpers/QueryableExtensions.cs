using System;
using System.Linq;
using System.Linq.Expressions;

namespace SalesSummary.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string orderByProperty, bool ascending)
        {
            var command = ascending ? "OrderBy" : "OrderByDescending";
            var type = typeof(T);
            var property = type.GetProperty(orderByProperty);
            if (property == null) throw new ArgumentException($"Property '{orderByProperty}' not found on type '{type.Name}'");

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<T>(resultExpression);
        }
    }
}
