// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Caching;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Subscribes;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Fireasy.Zero.Infrastructure
{
    /// <summary>
    /// 实体的事件订阅器。用于在维护实体时，移除相关的缓存键。
    /// </summary>
    public class EntitySubscriber : EntityPersistentSubscriber
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

        protected override void OnAfterUpdate(IEntity entity)
        {
            RemoveCacheKeys(entity.EntityType);
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
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"清除应用缓存 {key}");
                Console.ResetColor();

                cacheMgr.Remove(key);
            }
        }
    }

    /// <summary>
    /// 演示异步事件订阅器。
    /// </summary>
    public class AsyncEntitySubscriber : AsyncEntityPersistentSubscriber
    {
        protected override Task<bool> OnBeforeCreateAsync(IEntity entity)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"创建 {entity.EntityType} 之前");
            Console.ResetColor();
            return base.OnBeforeCreateAsync(entity);
        }

        protected override Task OnAfterCreateAsync(IEntity entity)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"创建 {entity.EntityType} 之后");
            Console.ResetColor();
            return base.OnAfterCreateAsync(entity);
        }

        protected override Task<bool> OnBeforeUpdateAsync(IEntity entity)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"更新 {entity.EntityType} 之前");
            Console.ResetColor();
            return base.OnBeforeUpdateAsync(entity);
        }

        protected override Task OnAfterUpdateAsync(IEntity entity)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"更新 {entity.EntityType} 之后");
            Console.WriteLine(GetModifiedDetail(entity));
            Console.ResetColor();
            return base.OnAfterUpdateAsync(entity);
        }

        protected override Task<bool> OnBeforeRemoveAsync(IEntity entity)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"删除 {entity.EntityType} 之前");
            Console.ResetColor();
            return base.OnBeforeRemoveAsync(entity);
        }

        protected override Task OnAfterRemoveAsync(IEntity entity)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"删除 {entity.EntityType} 之后");
            Console.ResetColor();
            return base.OnAfterRemoveAsync(entity);
        }

        /// <summary>
        /// 获取修改的细节。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private string GetModifiedDetail(IEntity entity)
        {
            var sb = new StringBuilder();
            foreach (var p in entity.GetModifiedProperties())
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                var oldValue = entity.GetOldValue(p);
                var newValue = entity.GetValue(p);

                sb.Append($"{p} 由 {oldValue} 修改为 {newValue}");
            }

            return sb.ToString();
        }
    }
}
