﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remote.Linq
{
    public static class IQueryableExtensions
    {
        private static readonly MethodInfo _lambdaExpressionMethodInfo = typeof(Expression)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == "Lambda" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[1].ParameterType == typeof(ParameterExpression[]));

        private static readonly MethodInfo _orderByMethodInfo = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == "OrderBy" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2);

        private static readonly MethodInfo _orderByDescendingMethodInfo = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == "OrderByDescending" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2);

        private static readonly MethodInfo _thenByMethodInfo = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == "ThenBy" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2);

        private static readonly MethodInfo _thenByDescendingMethodInfo = typeof(System.Linq.Queryable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(m =>
                m.Name == "ThenByDescending" &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 2);


        /// <summary>
        /// Creates an instance of <see cref="IQueryable{T}" /> that utilizes the data provider specified
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        public static IQueryable<T> AsQueryable<T>(this IQueryable<T> resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
        {
            return RemoteQueryable.Create<T>(dataProvider);
        }

        /// <summary>
        /// Creates an instance of <see cref="IQueryable" /> that utilizes the data provider specified
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resource"></param>
        /// <param name="dataProvider"></param>
        /// <returns></returns>
        public static IQueryable AsQueryable<T>(this IQueryable resource, Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
        {
            return RemoteQueryable.Create(resource.ElementType, dataProvider);
        }

        private static IOrderedQueryable<T> Sort<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression, MethodInfo methodInfo)
        {
            var exp = lambdaExpression.Body;
            var resultType = exp.Type;
            var funcType = typeof(Func<,>).MakeGenericType(typeof(T), resultType);
            var lambdaExpressionMethodInfo = _lambdaExpressionMethodInfo.MakeGenericMethod(funcType);

            var funcExpression = lambdaExpressionMethodInfo.Invoke(null, new object[] { exp, lambdaExpression.Parameters.ToArray() });

            var method = methodInfo.MakeGenericMethod(typeof(T), resultType);
            var result = method.Invoke(null, new object[] { queryable, funcExpression });

            return (IOrderedQueryable<T>)result;
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, _orderByMethodInfo);
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, _orderByDescendingMethodInfo);
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, _thenByMethodInfo);
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> queryable, LambdaExpression lambdaExpression)
        {
            return queryable.Sort<T>(lambdaExpression, _thenByDescendingMethodInfo);
        }

        /// <summary>
        /// Applies this query instance to a queryable
        /// </summary>
        /// <param name="queriable"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> ApplyQuery<TEntity>(this IQueryable<TEntity> queryable, Query<TEntity> query)
        {
            return queryable
                .ApplyFilters(query)
                .ApplySorting(query)
                .ApplyPaging(query);
        }

        /// <summary>
        /// Applies this query instance to a queryable
        /// </summary>
        /// <param name="queriable"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> ApplyQuery<TEntity>(this IQueryable<TEntity> queryable, Query query)
        {
            var q = Query<TEntity>.CreateFromNonGeneric(query);
            return queryable.ApplyQuery(q);
        }

        private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> queriable, Query<T> query)
        {
            foreach (var filter in query.FilterExpressions)
            {
                var predicate = filter.ToLinqExpression<T, bool>();
                queriable = queriable.Where(predicate);
            }
            return queriable;
        }

        private static IQueryable<T> ApplySorting<T>(this IQueryable<T> queriable, Query<T> query)
        {
            IOrderedQueryable<T> orderedQueriable = null;
            foreach (var sort in query.SortExpressions)
            {
                var exp = sort.Operand.ToLinqExpression();
                if (ReferenceEquals(orderedQueriable, null))
                {
                    switch (sort.SortDirection)
                    {
                        case Remote.Linq.Expressions.SortDirection.Ascending:
                            orderedQueriable = queriable.OrderBy(exp);
                            break;
                        case Remote.Linq.Expressions.SortDirection.Descending:
                            orderedQueriable = queriable.OrderByDescending(exp);
                            break;
                    }
                }
                else
                {
                    switch (sort.SortDirection)
                    {
                        case Remote.Linq.Expressions.SortDirection.Ascending:
                            orderedQueriable = orderedQueriable.ThenBy(exp);
                            break;
                        case Remote.Linq.Expressions.SortDirection.Descending:
                            orderedQueriable = orderedQueriable.ThenByDescending(exp);
                            break;
                    }
                }
            }
            return orderedQueriable ?? queriable;
        }

        private static IQueryable<T> ApplyPaging<T>(this IQueryable<T> queriable, Query<T> query)
        {
            if (query.SkipValue.HasValue)
            {
                queriable = queriable.Skip(query.SkipValue.Value);
            }
            if (query.TakeValue.HasValue)
            {
                queriable = queriable.Take(query.TakeValue.Value);
            }
            return queriable;
        }
    }
}