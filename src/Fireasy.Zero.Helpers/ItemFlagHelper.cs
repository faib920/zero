// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fireasy.Zero.Helpers
{
    /// <summary>
    /// 下拉列表项辅助类。
    /// </summary>
    public class ItemFlagHelper
    {
        /// <summary>
        /// 往现有列表的索引 0 处插入一个默认项。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="flag"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IList Insert<T>(IEnumerable<T> list, ItemFlag? flag, Func<ItemFlag, object> func) where T : class
        {
            if (flag == null)
            {
                return list.ToList();
            }

            var objects = list.ToList<object>();
            objects.Insert(0, func(flag.Value));
            return objects;
        }
    }
}
