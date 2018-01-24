// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Aop;
using Fireasy.Common.Extensions;
using Fireasy.Data.Entity;

namespace Fireasy.Zero.Infrastructure.Aop
{
    /// <summary>
    /// 数据库事务的拦截器。
    /// </summary>
    public class TransactionInterceptor : IInterceptor
    {
        private EntityTransactionScope transaction;

        public void Initialize(InterceptContext context)
        {
        }

        public void Intercept(InterceptCallInfo info)
        {
            if (info.InterceptType == InterceptType.BeforeMethodCall)
            {
                CheckTransaction(info);
            }
            //执行方法后
            else if (info.InterceptType == InterceptType.AfterMethodCall)
            {
                CommitTransaction(info);
            }
            //最终完成后
            else if (info.InterceptType == InterceptType.Finally)
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                    transaction = null;
                }
            }
        }

        /// <summary>
        /// 检查是否需要开启事务。
        /// </summary>
        /// <param name="info"></param>
        private void CheckTransaction(InterceptCallInfo info)
        {
            //判断是否加了 TransactionSupportAttribute 特性
            if (info.Member.IsDefined<TransactionSupportAttribute>())
            {
                //判断是否已经开启事务
                if (EntityTransactionScope.Current == null)
                {
                    transaction = new EntityTransactionScope();
                }
            }
        }

        /// <summary>
        /// 提交事务。
        /// </summary>
        /// <param name="info"></param>
        private void CommitTransaction(InterceptCallInfo info)
        {
            //提交事务
            if (transaction != null)
            {
                transaction.Complete();
            }
        }
    }
}
