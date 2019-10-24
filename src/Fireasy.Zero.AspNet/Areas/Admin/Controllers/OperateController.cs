using Fireasy.Common.ComponentModel;
using Fireasy.Common.Serialization;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet.Areas.Admin.Controllers
{
    public class OperateController : Controller
    {
        private IAdminService adminService;

        public OperateController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        public ActionResult Index()
        {
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "content\\easyui-icon.css");
            var doc = new CssDocument();
            doc.Load(fileName);
            var names = doc.Elements.Select(s => new { Name = s.Name.Substring(1) }).ToArray();
            ViewBag.Icons = new JsonSerializer().Serialize(names);
            return View();
        }

        public async Task<JsonResult> Data(int moduleId)
        {
            var list = await adminService.GetOperatesAsync(moduleId);
            return Json(list);
        }

        public async Task<JsonResult> SaveRows(int moduleId, List<SysOperate> added, List<SysOperate> updated, List<SysOperate> deleted)
        {
            await adminService.SaveOperatesAsync(moduleId, added, updated, deleted);

            return Json(Result.Success("保存成功"));
        }
    }
}