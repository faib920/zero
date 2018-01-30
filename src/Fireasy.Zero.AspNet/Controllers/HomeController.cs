using Fireasy.Zero.Helpers;
using Fireasy.Zero.Services;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet.Controllers
{
    public class HomeController : Controller
    {
        private IAdminService adminService;

        public HomeController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        public ActionResult Index()
        {
            var session = HttpContext.GetSession();

            ViewBag.Modules = adminService.GetPurviewModules(session.UserID);
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