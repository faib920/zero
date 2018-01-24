// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Aop;
using Fireasy.Zero.Infrastructure.Aop;

namespace Fireasy.Zero.Infrastructure
{
    [Intercept(typeof(CacheInterceptor))]
    [Intercept(typeof(TransactionInterceptor))]
    public abstract class BaseService : IAopSupport
    {
    }
}
