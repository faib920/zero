// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Zero.Infrastructure;
using System;
using Fireasy.Common.Extensions;
using System.Linq;
#if NETSTANDARD2_0
using Fireasy.Common.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
#else
using System.Web;
using System.Web.Security;
using Fireasy.Common.Serialization;
#endif

namespace Fireasy.Zero.Helpers
{
    /// <summary>
    /// Forms验证的辅助类。
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        /// 存储当前登录用户的验证信息。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="session"></param>
#if NETSTANDARD2_0
        public static void SignIn(this HttpContext context, SessionContext session)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, session.UserName),
                    new Claim(ClaimTypes.Sid, session.UserID.ToString())
                };

            var userIdentity = new ClaimsIdentity(claims, "Passport");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties
                {
                    IsPersistent = false
                });
        }
#else
        public static void SignIn(this HttpContextBase context, SessionContext session)
        {
            var ticket = new FormsAuthenticationTicket(1, session.UserName, DateTime.Now, DateTime.Now.AddDays(1), false, session.UserID.ToString());
            var ticketData = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName) { Value = ticketData };

            context.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 存储当前登录用户的验证信息。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="session"></param>
        public static void SignIn(this HttpContext context, SessionContext session)
        {
            var ticket = new FormsAuthenticationTicket(1, session.UserName, DateTime.Now, DateTime.Now.AddDays(1), false, session.UserID.ToString());
            var ticketData = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName) { Value = ticketData };

            context.Response.AppendCookie(cookie);
        }

#endif

        /// <summary>
        /// 注销当前登录用户的验证信息。
        /// </summary>
        /// <param name="context"></param>
#if NETSTANDARD2_0
        public static void SignOut(this HttpContext context)
        {
            context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
#else
        public static void SignOut(this HttpContextBase context)
        {
            FormsAuthentication.SignOut();
        }
#endif

#if NETSTANDARD2_0
        public static int GetIdentity(this HttpContext context)
        {
            var claim = context.User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.Sid);
            return claim == null ? 0 : claim.Value.To<int>();
        }

#else
        public static int GetIdentity(this HttpContextBase context)
        {
            var cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return 0;
            }

            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            return ticket.UserData.To<int>();
        }

        public static int GetIdentity(this HttpContext context)
        {
            var cookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
            {
                return 0;
            }

            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            return ticket.UserData.To<int>();
        }

#endif
    }
}
