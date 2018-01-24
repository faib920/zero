// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Logging;
using System;

namespace Fireasy.Zero.Helpers
{
    /// <summary>
    /// 日志辅助。
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 记录错误日志。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exp"></param>
        public static void Error(string message, Exception exp)
        {
            var logger = LoggerFactory.CreateLogger();
            if (logger != null)
            {
                logger.Error(message, exp);
            }
        }

        /// <summary>
        /// 记录信息。
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            var logger = LoggerFactory.CreateLogger();
            if (logger != null)
            {
                logger.Info(message);
            }
        }
    }
}
