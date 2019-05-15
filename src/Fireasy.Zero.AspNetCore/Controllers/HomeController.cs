using Fireasy.Zero.AspNetCore.Models;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Fireasy.Zero.AspNetCore.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IAdminService adminService;

        public HomeController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        public IActionResult Index()
        {
            var session = HttpContext.GetSession();

            ViewBag.Modules = adminService.GetPurviewModules(session.UserID);
            ViewBag.UserName = session.UserName;

            return View();
        }

        [HttpGet]
        public JsonResult TT()
        {
            throw new System.Exception("dfas");
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 退出系统。
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            HttpContext.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}
