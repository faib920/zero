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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CacheRelationAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationType"></param>
        public CacheRelationAttribute(Type relationType)
        {
            RelationType = relationType;
        }

        /// <summary>
        /// 获取或设置关联的类型。
        /// </summary>
        public Type RelationType { get; set; }
    }
}
