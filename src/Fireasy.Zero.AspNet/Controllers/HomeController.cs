using Fireasy.Data;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdminService _adminService;

        public HomeController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Test()
        {
            var r = await _adminService.GetUsersAsync(2, "0003", Zero.Models.StateFlags.Enabled, null, new DataPager(20, 0), null);
            return Json(r);
        }

        public async Task<ActionResult> Index()
        {
            var session = HttpContext.GetSession();

            ViewBag.Modules = await _adminService.GetPurviewModulesAsync(session.UserID);
            ViewBag.UserName = session.UserName;

            return View();
        }

        public ActionResult Start()
        {
            return View();
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