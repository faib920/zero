using Fireasy.Common.Logging;
using Fireasy.Zero.AspNetCore.Models;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            var session = HttpContext.GetSession();

            if (session == null)
            {
                return Redirect("~/Login");
            }

            ViewBag.Modules = await adminService.GetPurviewModulesAsync(session.UserID);
            ViewBag.UserName = session.UserName;

            return View();
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
