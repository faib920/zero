// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Zero.Infrastructure;
#if NETSTANDARD2_0
using Fireasy.Common.Serialization;
using Microsoft.AspNetCore.Http;
using System.Text;
#else
using System.Web;
#endif

namespace Fireasy.Zero.Helpers
{
    /// <summary>
    /// Session会话辅助类。
    /// </summary>
    public static class SessionHelper
    {
        private const string SESSION_KEY = "zero_user";

        /// <summary>
        /// 获取用户会话 <see cref="SessionContext"/> 对象。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
#if NETSTANDARD2_0
        public static SessionContext GetSession(this HttpContext context)
        {
            byte[] bytes;
            if (context.Session.TryGetValue(SESSION_KEY, out bytes))
            {
                return new JsonSerializer().Deserialize<SessionContext>(Encoding.UTF8.GetString(bytes));
            }

            return null;
        }
#else
        public static SessionContext GetSession(this HttpContextBase context)
        {
            return context.Session[SESSION_KEY] as SessionContext;
        }

        /// <summary>
        /// 获取用户会话 <see cref="SessionContext"/> 对象。
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SessionContext GetSession(this HttpContext context)
        {
            return context.Session[SESSION_KEY] as SessionContext;
        }
#endif

        /// <summary>
        /// 设置当前的用户会话 <see cref="SessionContext"/> 对象。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="session"></param>
#if NETSTANDARD2_0
        public static void SetSession(this HttpContext context, SessionContext session)
        {
            var json = new JsonSerializer().Serialize(session);
            context.Session.Set(SESSION_KEY, Encoding.UTF8.GetBytes(json));
        }
#else
        public static void SetSession(this HttpContextBase context, SessionContext session)
        {
            context.Session[SESSION_KEY] = session;
        }

        /// <summary>
        /// 设置当前的用户会话 <see cref="SessionContext"/> 对象。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="session"></param>
        public static void SetSession(this HttpContext context, SessionContext session)
        {
            context.Session[SESSION_KEY] = session;
        }
#endif
    }
}
