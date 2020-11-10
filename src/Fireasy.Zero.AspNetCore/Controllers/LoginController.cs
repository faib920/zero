﻿using Fireasy.Common.ComponentModel;
using Fireasy.Common.Logging;
using Fireasy.Data;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fireasy.Zero.AspNetCore.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private IAdminService _adminService;
        private IEncryptProvider _encryptProvider;

        public LoginController(IAdminService adminService, IEncryptProvider encryptProvider)
        {
            _adminService = adminService;
            _encryptProvider = encryptProvider;
        }

        public async Task<IActionResult> Test()
        {
            var rr = await _adminService.GetUsersAsync(2, "0002", null, null, new DataPager(2, 0), null);
            return Json(rr);
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
        public async Task<JsonResult> CheckLogin(string account, string password, [FromServices] ILogger logger)
        {
            var session = await _adminService.CheckLoginAsync(account, t => _encryptProvider.Validate(password, t), null);
            if (session != null)
            {
                HttpContext.SetSession(session);
                HttpContext.SignIn(session);

                logger.Info($"{account}登录到系统");

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