// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;

namespace Fireasy.Zero.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheSupportAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expired"></param>
        public CacheSupportAttribute(int expired = 3600)
        {
            Expired = expired;
        }

        /// <summary>
        /// 获取或设置过期时间（秒）。
        /// </summary>
        public int Expired { get; set; }
    }
}
