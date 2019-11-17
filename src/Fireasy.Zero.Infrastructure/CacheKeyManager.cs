// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Caching;
using Fireasy.Common.Extensions;
using Fireasy.Common.Threading;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Properties;
using System;
using System.Collections.Generic;

namespace Fireasy.Zero.Infrastructure
{
    /// <summary>
    /// 缓存键管理。将实体类型与缓存键进行管理，以便在实体对象持久化时能够获取对应的缓存键。
    /// </summary>
    public class CacheKeyManager
    {
        private static ReadWriteLocker locker = new ReadWriteLocker();
        private const string ROOT_KEY = "_Zero_Keys_";

        /// <summary>
        /// 添加类型与缓存键的映射。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cacheKey"></param>
        public static void AddKeys(Type type, string cacheKey)
        {
            var types = AnalysisType(type);
            AddKeys(types, cacheKey);
        }

        /// <summary>
        /// 添加类型与缓存键的映射。
        /// </summary>
        /// <param name="types"></param>
        /// <param name="cacheKey"></param>
        public static void AddKeys(List<Type> types, string cacheKey)
        {
            var cacheMgr = CacheManagerFactory.CreateManager();
            foreach (var ctype in types)
            {
                locker.LockWrite(() =>
                    {
                        //每一个实体类与一个List相对应，List里存放与该实体相关的缓存键
                        var keyDict = cacheMgr.TryGet(ROOT_KEY, () => new Dictionary<string, List<string>>());

                        if (!keyDict.TryGetValue(ctype.Name, out List<string> keys))
                        {
                            keys = new List<string>();
                            keyDict.Add(ctype.Name, keys);
                        }

                        if (!keys.Contains(cacheKey))
                        {
                            keys.Add(cacheKey);
                        }

                        cacheMgr.Add(ROOT_KEY, keyDict, NeverExpired.Instance);
                    });
            }
        }

        /// <summary>
        /// 获取类型所关联的缓存键列表。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetCacheKeys(Type type)
        {
            var cacheMgr = CacheManagerFactory.CreateManager();
            if (cacheMgr.TryGet(ROOT_KEY, out Dictionary<string, List<string>> keyDict))
            {
                if (keyDict.TryGetValue(type.Name, out List<string> keys))
                {
                    return keys;
                }
            }

            return null;
        }

        private static List<Type> AnalysisType(Type type)
        {
            var types = new List<Type>();
            if (type.IsGenericType)
            {
                type.GetGenericArguments().ForEach(s => types.AddRange(AnalysisType(s)));
            }
            else if (type.IsArray)
            {
                types.AddRange(AnalysisType(type.GetElementType()));
            }
            else if (typeof(IEntity).IsAssignableFrom(type))
            {
                types.Add(type);

                foreach (var property in PropertyUnity.GetRelatedProperties(type))
                {
                    if (property is EntityProperty)
                    {
                        types.Add(property.Type);
                    }
                }
            }

            return types;
        }
    }
}
