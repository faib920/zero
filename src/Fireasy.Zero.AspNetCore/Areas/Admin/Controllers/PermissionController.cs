using Fireasy.Common.ComponentModel;
using Fireasy.Common.Extensions;
using Fireasy.Web.EasyUI;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fireasy.Zero.AspNetCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class PermissionController : Controller
    {
        private IAdminService adminService;

        public PermissionController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Function()
        {
            return View();
        }

        public ActionResult Data()
        {
            return View();
        }

        /// <summary>
        /// 根据角色获取相应的模块及操作。
        /// </summary>
        /// <param name="hosting"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetModulesByRole([FromServices]JsonSerializeOptionHosting hosting, int? roleId)
        {
            if (roleId == null)
            {
                return Json(new string[0]);
            }

            var converter = new DynamicTreeNodeJsonConverter<SysModule>(s => s.Name, s => s.Permissible, s => s.SysOperates);
            hosting.Option.Converters.Add(converter);

            var list = await adminService.GetModulesByRoleAsync((int)roleId);
            return Json(list);
        }

        /// <summary>
        /// 保存功能权限。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <param name="modules">勾选的模块ID列表。</param>
        /// <param name="opers">勾选的模块与操作的列表。</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SaveFuncPermissions(int roleId, List<int> modules, Dictionary<int, List<int>> opers)
        {
            await adminService.SaveFuncRolePermissions(roleId, modules, opers);

            return Json(Result.Success("保存成功。"));
        }

        /// <summary>
        /// 根据角色获取相应的用户。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <returns></returns>
        public async Task<JsonResult> GetUsersByRole(int? roleId)
        {
            if (roleId == null)
            {
                return Json(new string[0]);
            }

            var pager = EasyUIHelper.GetDataPager(HttpContext);
            var sorting = EasyUIHelper.GetSorting(HttpContext);

            var list = await adminService.GetUsersByRoleAsync((int)roleId, pager, sorting);

            return Json(list);
        }
    }
}
