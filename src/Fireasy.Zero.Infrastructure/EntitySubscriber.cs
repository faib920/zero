// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Caching;
using System;

namespace Fireasy.Zero.Infrastructure
{
    /// <summary>
    /// 实体的事件订阅器。用于在维护实体时，移除相关的缓存键。
    /// </summary>
    public class EntitySubscriber : Fireasy.Data.Entity.Subscribes.EntityPersistentSubscriber
    {
        protected override void OnCreate(Type entityType)
        {
            RemoveCacheKeys(entityType);
        }

        protected override void OnUpdate(Type entityType)
        {
            RemoveCacheKeys(entityType);
        }

        protected override void OnRemove(Type entityType)
        {
            RemoveCacheKeys(entityType);
        }

        /// <summary>
        /// 移除实体类型对应的缓存键。
        /// </summary>
        /// <param name="entityType"></param>
        private static void RemoveCacheKeys(Type entityType)
        {
            var keys = CacheKeyManager.GetCacheKeys(entityType);
            if (keys == null)
            {
                return;
            }

            var cacheMgr = CacheManagerFactory.CreateManager();
            foreach (var key in keys)
            {
                cacheMgr.Remove(key);
            }
        }
    }
}
