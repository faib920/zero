using Fireasy.Common.ComponentModel;
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
        private IAdminService _adminService;

        public PermissionController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectUser()
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

        public ActionResult Auth()
        {
            return View();
        }

        /// <summary>
        /// 根据角色获取相应的模块及操作。
        /// </summary>
        /// <param name="hosting"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetModulesByRole([FromServices] JsonSerializeOptionHosting hosting, int? roleId)
        {
            if (roleId == null)
            {
                return Json(new string[0]);
            }

            var converter = new DynamicTreeNodeJsonConverter<SysModule>(s => s.Name, s => s.Permissible, s => s.SysOperates);
            hosting.Option.Converters.Add(converter);

            var list = await _adminService.GetModulesByRoleAsync((int)roleId);
            return Json(list);
        }

        /// <summary>
        /// 根据角色获取相应的机构。
        /// </summary>
        /// <param name="hosting"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetOrgsByRole([FromServices] JsonSerializeOptionHosting hosting, int? roleId)
        {
            if (roleId == null)
            {
                return Json(new string[0]);
            }

            var converter = new DynamicTreeNodeJsonConverter<SysOrg>(s => s.Name, s => s.Permissible, s => s.FullName);
            hosting.Option.Converters.Add(converter);

            var list = await _adminService.GetOrgsByRoleAsync((int)roleId);
            return Json(list);
        }

        /// <summary>
        /// 获取角色列表。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetRolesByUser(int userId)
        {
            var list = await _adminService.GetPurviewRolesAsync(userId);
            return Json(list);
        }

        /// <summary>
        /// 根据查询条件获取用户。
        /// </summary>
        /// <param name="orgCode">机构编码</param>
        /// <param name="keyword">关键字</param>
        /// <param name="roleId">角色ID。</param>
        /// <param name="state">启用状态</param>
        /// <returns></returns>
        public async Task<JsonResult> GetUsers(string orgCode, string keyword, int roleId)
        {
            var pager = EasyUIHelper.GetDataPager(HttpContext);
            var sorting = EasyUIHelper.GetSorting(HttpContext);

            var userId = HttpContext.GetSession().UserID;

            sorting = sorting.Replace("SexName", "Sex", "DegreeName", "DegreeNo", "TitleName", "TitleNo");

            var list = await _adminService.GetUsersByRoleExcludeAsync(userId, orgCode, roleId, keyword, pager, sorting);
            return Json(EasyUIHelper.Transfer(pager, list));
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
            await _adminService.SaveFuncRolePermissions(roleId, modules, opers);

            return Json(Result.Success("保存成功。"));
        }

        /// <summary>
        /// 保存数据权限。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <param name="org">勾选的机构ID列表。</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SaveDataPermissions(int roleId, List<int> orgs)
        {
            await _adminService.SaveOrgRolePermissionsAsync(roleId, orgs);

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

            var list = await _adminService.GetUsersByRoleAsync((int)roleId, pager, sorting);

            return Json(EasyUIHelper.Transfer(pager, list));
        }

        /// <summary>
        /// 为角色添加多个用户。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <param name="users">用户ID列表。</param>
        /// <returns></returns>
        public async Task<JsonResult> AddRoleUsers(int roleId, List<int> users)
        {
            await _adminService.AddRoleUsers(roleId, users);

            return Json(Result.Success("添加成功。"));
        }

        /// <summary>
        /// 移除角色中的指定的用户。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <param name="users">用户ID列表。</param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteRoleUsers(int roleId, List<int> users)
        {
            await _adminService.DeleteRoleUsers(roleId, users);

            return Json(Result.Success("删除成功。"));
        }

        /// <summary>
        /// 保存用户角色。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveUserRoles(int userId, List<int> roles)
        {
            await _adminService.SaveUserRoles(userId, roles);

            return Json(Result.Success("保存成功。"));
        }
    }
}
