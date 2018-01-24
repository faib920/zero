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
    }
}
