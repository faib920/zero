using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fireasy.Zero.AspNetCore.Areas.Base.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DictItemController : Controller
    {
        private IAdminService _adminService;

        public DictItemController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetDictItems(string typeCode)
        {
            return Json(_adminService.GetDictItems(typeCode));
        }
    }
}