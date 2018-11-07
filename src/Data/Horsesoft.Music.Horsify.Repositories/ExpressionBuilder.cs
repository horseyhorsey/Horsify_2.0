using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Horsesoft.Music.Horsify.Repositories
{
    public class ExpressionBuilder
    {
        public static Expression<Func<TValue, bool>> BuildOrExpressionTree<TValue, TCompareAgainst>(
        IEnumerable<TCompareAgainst> wantedItems,
        Expression<Func<TValue, TCompareAgainst>> convertBetweenTypes)
        {
            ParameterExpression inputParam = convertBetweenTypes.Parameters[0];

            Expression binaryExpressionTree = BuildBinaryOrTree(wantedItems.GetEnumerator(), convertBetweenTypes.Body, null);

            return Expression.Lambda<Func<TValue, bool>>(binaryExpressionTree, new[] { inputParam });
        }

        public static Expression<Func<TValue, bool>> BuildAndNotExpressionTree<TValue, TCompareAgainst>(
        IEnumerable<TCompareAgainst> wantedItems,
        Expression<Func<TValue, TCompareAgainst>> convertBetweenTypes)
        {
            ParameterExpression inputParam = convertBetweenTypes.Parameters[0];

            Expression binaryExpressionTree = BuildBinaryAndNotTree(wantedItems.GetEnumerator(), convertBetweenTypes.Body, null);

            return Expression.Lambda<Func<TValue, bool>>(binaryExpressionTree, new[] { inputParam });
        }

        /// <summary>
        /// Builds the binary OR ELSE tree. Recursive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemEnumerator">The item enumerator.</param>
        /// <param name="expressionToCompareTo">The expression to compare to.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private static Expression BuildBinaryOrTree<T>(
                                  IEnumerator<T> itemEnumerator,
                                  Expression expressionToCompareTo,
                                  Expression expression)
        {
            if (itemEnumerator.MoveNext() == false)
                return expression;

            ConstantExpression constant = Expression.Constant(itemEnumerator.Current, typeof(T));
            BinaryExpression comparison = Expression.Equal(expressionToCompareTo, constant);

            BinaryExpression newExpression;

            if (expression == null)
                newExpression = comparison;
            else
                newExpression = Expression.OrElse(expression, comparison);

            return BuildBinaryOrTree(itemEnumerator, expressionToCompareTo, newExpression);
        }

        /// <summary>
        /// Builds the binary OR ELSE tree. Recursive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemEnumerator">The item enumerator.</param>
        /// <param name="expressionToCompareTo">The expression to compare to.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private static Expression BuildBinaryAndNotTree<T>(
                                  IEnumerator<T> itemEnumerator,
                                  Expression expressionToCompareTo,
                                  Expression expression)
        {
            if (itemEnumerator.MoveNext() == false)
                return expression;

            ConstantExpression constant = Expression.Constant(itemEnumerator.Current, typeof(T));
            BinaryExpression comparison = Expression.NotEqual(expressionToCompareTo, constant);

            BinaryExpression newExpression;

            if (expression == null)
                newExpression = comparison;
            else
                newExpression = Expression.NotEqual(comparison, constant);

            return BuildBinaryAndNotTree(itemEnumerator, expressionToCompareTo, newExpression);
        }
    }
}
