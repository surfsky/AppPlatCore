﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace App.Components
{
    /// <summary>
    /// Order by enum
    /// http://fineui.com/bbs/forum.php?mod=viewthread&tid=3844
    /// </summary>
    public enum OrderByType
    {
        OrderBy = 0,
        OrderByDescending = 1,
        ThenBy = 2,
        ThenByDescending = 3
    }

    /// <summary>EF Linq query extensions</summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/3945645/sorting-gridview-with-entity-framework#
    /// https://stackoverflow.com/questions/7265186/how-do-i-specify-the-linq-orderby-argument-dynamically
    /// </remarks>
    public static class QueryExtensions
    {
        /// <summary>Sort By</summary>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string sortExpression)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            string sortDirection = String.Empty;
            string propertyName = String.Empty;
            sortExpression = sortExpression.Trim();
            int spaceIndex = sortExpression.Trim().IndexOf(" ");
            if (spaceIndex < 0)
            {
                propertyName = sortExpression;
                sortDirection = "ASC";
            }
            else
            {
                propertyName = sortExpression.Substring(0, spaceIndex);
                sortDirection = sortExpression.Substring(spaceIndex + 1).Trim();
            }

            // 关联属性
            if (propertyName.IndexOf('.') > 0)
            {
                if (sortDirection == "ASC")
                    return source.OrderBy(propertyName);
                else
                    return source.OrderByDescending(propertyName);
            }
            if (String.IsNullOrEmpty(propertyName))
                return source;

            ParameterExpression parameter = Expression.Parameter(source.ElementType, String.Empty);
            MemberExpression property = Expression.Property(parameter, propertyName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);
            string methodName = (sortDirection == "ASC") ? "OrderBy" : "OrderByDescending";
            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new Type[] { source.ElementType, property.Type },
                                                source.Expression, Expression.Quote(lambda));
            return source.Provider.CreateQuery<T>(methodCallExpression);
        }


        /// <summary>升序排序</summary>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, OrderByType.OrderBy);
        }

        /// <summary>降序排序</summary>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, OrderByType.OrderByDescending);
        }

        /// <summary>应用排序</summary>
        public static IOrderedQueryable<T> ApplyOrder<T>(this IQueryable<T> source, string property, OrderByType orderType)
        {
            var methodName = orderType.ToString();

            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ 
                System.Reflection.PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            object result = typeof(Queryable).GetMethods().Single(method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                            .MakeGenericMethod(typeof(T), type)
                            .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        /// <summary>ThenBy</summary>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, OrderByType.ThenBy);
        }

        /// <summary>ThenByDescending</summary>
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, OrderByType.ThenByDescending);
        }

    }
}