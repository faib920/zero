using Fireasy.Common.ComponentModel;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Services;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet.Controllers
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

        public ActionResult Index()
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

        /// <summary>
        /// 获取验证码图片。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetValidateImage(string key)
        {
            var code = ValidateHelper.GenerateCode();
            ValidateHelper.Cache(key, code);

            var bytes = ValidateHelper.GenerateImage(code, 80, 28);
            return File(bytes, "image/png");
        }

        /// <summary>
        /// 验证验证码。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ValidateCode(string key, string code)
        {
            return Json(ValidateHelper.Validate(key, code));
        }
    }
}