// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Aop;
using Fireasy.Common.Caching;
using Fireasy.Common.Extensions;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fireasy.Zero.Infrastructure.Aop
{
    /// <summary>
    /// 缓存管理的拦截器。
    /// </summary>
    public class CacheInterceptor : IInterceptor
    {
        private static MethodInfo MthCacheTryGet = typeof(ICacheManager).GetMethods().FirstOrDefault(s => s.Name == "TryGet" && s.GetParameters().Length == 3);
        private static MethodInfo MthCacheAdd = typeof(ICacheManager).GetMethods().FirstOrDefault(s => s.Name == "Add");
        private bool loadFromCache = false; //表示是否从缓存加载数据

        public void Initialize(InterceptContext context)
        {
        }

        public void Intercept(InterceptCallInfo info)
        {
            if (info.InterceptType == InterceptType.BeforeMethodCall)
            {
                //CheckDataCache(info);
            }
            else if (info.InterceptType == InterceptType.AfterMethodCall)
            {
                //UpdateDataCache(info);
            }
        }

        /// <summary>
        /// 检查数据缓存。
        /// </summary>
        /// <param name="info"></param>
        private void CheckDataCache(InterceptCallInfo info)
        {
            //判断是否加了 CacheSupportAttribute 特性
            if (info.Member.IsDefined<CacheSupportAttribute>())
            {
                var cacheMgr = CacheManagerFactory.CreateManager();
                var cacheKey = GetCacheKey(info);

                //检查缓存管理器里有没有对应的缓存项，如果有的话直接取出来赋给 ReturnValue，然后设置 Cancel 忽略方法调用
                if (cacheMgr.Contains(cacheKey))
                {
                    var method = MthCacheTryGet.MakeGenericMethod(info.ReturnType);
                    info.ReturnValue = method.FastInvoke(cacheMgr, new object[] { cacheKey, null, null });

                    if (info.ReturnValue != null)
                    {
                        loadFromCache = true;
                        info.Cancel = true;
                    }
                }
            }
        }

        /// <summary>
        /// 更新数据缓存。
        /// </summary>
        /// <param name="info"></param>
        private void UpdateDataCache(InterceptCallInfo info)
        {
            //判断是否加了 CacheSupportAttribute 特性
            if (info.Member.IsDefined<CacheSupportAttribute>())
            {
                var attr = info.Member.GetCustomAttribute<CacheSupportAttribute>();
                var relationTypes = info.Member.GetCustomAttributes<CacheRelationAttribute>().Select(s => s.RelationType).ToList();

                var returnType = (info.Member as MethodInfo).ReturnType;
                var cacheMgr = CacheManagerFactory.CreateManager();
                var cacheKey = GetCacheKey(info);

                if (loadFromCache)
                {
                    loadFromCache = false;
                    return;
                }

                var method = MthCacheAdd.MakeGenericMethod(info.ReturnType);
                method.FastInvoke(cacheMgr, cacheKey, info.ReturnValue, TimeSpan.FromSeconds(attr.Expired), null);

                CacheKeyManager.AddKeys(returnType, cacheKey);
                if (relationTypes.Count > 0)
                {
                    CacheKeyManager.AddKeys(relationTypes, cacheKey);
                }
            }
        }
        private static string GetCacheKey(InterceptCallInfo info)
        {
            var sb = new StringBuilder();
            sb.Append(info.Member.DeclaringType.Name);
            sb.Append("-");
            sb.Append(info.Member.Name);
            sb.Append(":");

            var parameters = (info.Member as MethodInfo).GetParameters();
            for (var i = 0; i < info.Arguments.Length; i++)
            {
                sb.AppendFormat("&{0}=", parameters[i].Name);
                if (info.Arguments[i] != null)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(parameters[i].ParameterType) && typeof(string) != parameters[i].ParameterType)
                    {
                        sb.Append("[");

                        foreach (var item in (IEnumerable)info.Arguments[i])
                        {
                            sb.AppendFormat("{0}-", item.ToString());
                        }

                        sb.Append("]");
                    }
                    else
                    {
                        sb.Append(info.Arguments[i]);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
