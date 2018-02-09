using Fireasy.Common.ComponentModel;
using Fireasy.Web.EasyUI;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
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
        public JsonResult Get(int id)
        {
            var info = adminService.GetRole(id);
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
        public JsonResult Save(int? id, SysRole info)
        {
            id = adminService.SaveRole(id, info);
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
        public JsonResult Data(string keyword, int? type, StateFlags? state)
        {
            var pager = EasyUIHelper.GetDataPager();
            var sorting = EasyUIHelper.GetSorting();
            var list = adminService.GetRoles(state, keyword, pager, sorting);
            return Json(EasyUIHelper.Transfer(pager, list));
        }

        /// 删除角色。
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            adminService.DeleteRole(id);
            return Json(Result.Success("删除成功。"));
        }

        /// <summary>
        /// 启用角色。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Enable(int id)
        {
            adminService.SetRoleState(id, StateFlags.Enabled);
            return Json(Result.Success("启用成功。"));
        }

        /// <summary>
        /// 禁用角色。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Disable(int id)
        {
            adminService.SetRoleState(id, StateFlags.Disabled);
            return Json(Result.Success("禁用成功。"));
        }
    }
}