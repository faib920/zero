using Fireasy.Common.ComponentModel;
using Fireasy.Web.EasyUI;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        private IAdminService adminService;

        public RoleController(IAdminService adminService)
        {
            this.adminService = adminService;
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
            var info = await adminService.GetRoleAsync(id);
            return Json(info);
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
            id = await adminService.SaveRoleAsync(id, info);
            return Json(Result.Success("保存成功。", id));
        }

        /// <summary>
        /// 根据查询条件获取角色。
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="type">角色类别。</param>
        /// <param name="state"></param>
        /// <returns></returns>
        [EmptyArrayResult(true)]
        public async Task<JsonResult> Data(string keyword, int? type, StateFlags? state)
        {
            var pager = EasyUIHelper.GetDataPager();
            var sorting = EasyUIHelper.GetSorting();
            var list = await adminService.GetRolesAsync(state, keyword, pager, sorting);
            return Json(EasyUIHelper.Transfer(pager, list));
        }

        /// 删除角色。
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            await adminService.DeleteRoleAsync(id);
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
            await adminService.SetRoleStateAsync(id, StateFlags.Enabled);
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
            await adminService.SetRoleStateAsync(id, StateFlags.Disabled);
            return Json(Result.Success("禁用成功。"));
        }
    }
}