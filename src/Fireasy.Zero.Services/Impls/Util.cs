// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Zero.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Fireasy.Zero.Services.Impls
{
    public class Util
    {
        /// <summary>
        /// 递归构造树结构。
        /// </summary>
        /// <param name="children"></param>
        /// <param name="source"></param>
        /// <param name="parentCode">父节点编码。</param>
        /// <param name="length">每级编码的长度。</param>
        /// <param name="action">处理附加数据。</param>
        /// <param name="isLoaded">是否展开。</param>
        /// <param name="checkNull"></param>
        internal static void MakeChildren<T>(IList<T> children, IList<T> source, string parentCode, int length = 4, Action<T> action = null, bool isLoaded = true, Func<T, bool> checkNull = null) where T : ITreeNodeRecursion<T>
        {
            var list = source
                .Where(s => s.Code.StartsWith(parentCode) && s.Code.Length == parentCode.Length + length)
                .OrderBy(s => s.OrderNo);

            foreach (var child in list)
            {
                child.Children = new List<T>();
                MakeChildren(child.Children, source, child.Code, length, action, checkNull: checkNull);

                if (checkNull != null && child.Children.Count == 0)
                {
                    if (!checkNull(child))
                    {
                        children.Add(child);
                    }
                }
                else
                {
                    children.Add(child);
                }

                action?.Invoke(child);

                child.IsLoaded = parentCode.Length == 0 && isLoaded;
                child.HasChildren = child.Children.Count > 0;
            }

            //如果还有子节点
            if (children.Count == 0)
            {
                var nextChildren = source.Where(s => s.Code.StartsWith(parentCode) && s.Code.Length > parentCode.Length).ToList();
                if (nextChildren.Count > 0)
                {
                    foreach (var item in nextChildren.GroupBy(s => s.Code.Substring(0, parentCode.Length + length)))
                    {
                        MakeChildren(children, item.ToList(), item.Key, length, action, isLoaded);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="queryable">数据库查询。</param>
        /// <param name="target">目标数据，为新增的数据。</param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        internal static List<int> FindRepeatRows<T, V>(IQueryable<T> queryable, IEnumerable<V> target, Expression<Func<T, V, bool>> predicate)
        {
            var parExp = Expression.Parameter(typeof(T), "v");
            var lambda = predicate as LambdaExpression;
            var expression = ExpressionReplacer.Replace(lambda.Body, lambda.Parameters[0], parExp);
            lambda = Expression.Lambda(expression, lambda.Parameters[1]);
            expression = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(V) }, Expression.Constant(target, typeof(IEnumerable<V>)), lambda);
            lambda = Expression.Lambda(expression, parExp);
            expression = Expression.Call(typeof(Queryable), "Where", new[] { typeof(T) }, queryable.Expression, lambda);
            lambda = Expression.Lambda(expression, parExp);

            var list = queryable.Provider.Execute<IEnumerable<T>>(lambda).ToList();

            var func = predicate.Compile();
            return FindIndex(list, target, func);
        }

        private static List<int> FindIndex<T, V>(IEnumerable<T> list, IEnumerable<V> target, Func<T, V, bool> func)
        {
            var i = 1;
            var result = new List<int>();
            foreach (var item in target)
            {
                if (list.Any(s => func(s, item)))
                {
                    result.Add(i);
                }

                i++;
            }

            return result;
        }

        private class ExpressionReplacer : Fireasy.Common.Linq.Expressions.ExpressionVisitor
        {
            private Expression searchExp;
            private Expression replaceExp;

            public static Expression Replace(Expression expression, ParameterExpression searchExp, ParameterExpression replaceExp)
            {
                var replacer = new ExpressionReplacer { searchExp = searchExp, replaceExp = replaceExp };
                return replacer.Visit(expression);
            }

            protected override Expression VisitMember(MemberExpression memberExp)
            {
                var parExp = memberExp.Expression as ParameterExpression;
                if (parExp != null && parExp == searchExp)
                {
                    return Expression.MakeMemberAccess(replaceExp, memberExp.Member);
                }

                var exp = Visit(memberExp.Expression);
                return Expression.MakeMemberAccess(exp, memberExp.Member);
            }
        }
    }
}
