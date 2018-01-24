using Fireasy.Common.ComponentModel;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fireasy.Zero.AspNetCore.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private IAdminService adminService;
        private IEncryptProvider encryptProvider;

        public LoginController(IAdminService adminService, IEncryptProvider encryptProvider)
        {
            this.adminService = adminService;
            this.encryptProvider = encryptProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 验证登录是否成功。
        /// </summary>
        /// <param name="account">帐号。</param>
        /// <param name="password">密码。</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CheckLogin(string account, string password)
        {
            var session = adminService.CheckLogin(account, t => encryptProvider.Validate(password, t), null);
            if (session != null)
            {
                HttpContext.SetSession(session);
                HttpContext.SignIn(session);

                return Json(Result.Success("登录成功"));
            }

            return Json(Result.Fail("登录失败，用户名或密码不匹配，或帐号被停用。"));
        }
    }
}