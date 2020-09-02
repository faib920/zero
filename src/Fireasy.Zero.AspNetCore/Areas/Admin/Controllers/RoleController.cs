using Fireasy.Common.ComponentModel;
using Fireasy.Web.EasyUI;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fireasy.Zero.AspNetCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class RoleController : Controller
    {
        private IAdminService _adminService;

        public RoleController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        /// <summary>
        /// 根据ID获取角色。
        /// </summary>
        /// <param name="id">信息ID。</param>
        /// <returns></returns>
        public async Task<JsonResult> Get(int id)
        {
            var info = await _adminService.GetRoleAsync(id);
            return Json(info);
        }

        /// <summary>
        /// 根据查询条件获取角色。
        /// </summary>
        /// <param name="state">启用状态</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public async Task<JsonResult> Data(StateFlags? state, string keyword)
        {
            var pager = EasyUIHelper.GetDataPager(HttpContext);
            var sorting = EasyUIHelper.GetSorting(HttpContext);

            var list = await _adminService.GetRolesAsync(state, keyword, pager, sorting);
            return Json(list);
        }

        /// <summary>
        /// 保存角色。
        /// </summary>
        /// <param name="id">id。</param>
        /// <param name="info">要保存的数据。</param>
        /// <returns>id</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Save(int? id, SysRole info)
        {
            id = await _adminService.SaveRoleAsync(id, info);
            return Json(Result.Success("保存成功。", id));
        }

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            await _adminService.DeleteRoleAsync(id);
            return Json(Result.Success("删除成功。"));
        }

        /// <summary>
        /// 启用角色。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Enable(int id)
        {
            await _adminService.SetRoleStateAsync(id, StateFlags.Enabled);
            return Json(Result.Success("启用成功。"));
        }

        /// <summary>
        /// 禁用角色。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Disable(int id)
        {
            await _adminService.SetRoleStateAsync(id, StateFlags.Disabled);
            return Json(Result.Success("禁用成功。"));
        }
    }
}
