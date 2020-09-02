using Fireasy.Common.ComponentModel;
using Fireasy.Common.Extensions;
using Fireasy.Web.EasyUI;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet.Areas.Admin.Controllers
{
    public class ModuleController : Controller
    {
        private readonly IAdminService _adminService;

        public ModuleController(IAdminService adminService)
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

        public ActionResult IconBrowser()
        {
            return View();
        }

        /// <summary>
        /// 根据ID获取模块。
        /// </summary>
        /// <param name="id">信息ID。</param>
        /// <returns></returns>
        public async Task<JsonResult> Get(int id)
        {
            var info = await _adminService.GetModuleAsync(id);
            return Json(info);
        }

        /// <summary>
        /// 获取下一个顺序号。
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetNextOrderNo(int? parentId)
        {
            return Json(await _adminService.GetModuleNextOrderNoAsync(parentId));
        }

        /// <summary>
        /// 保存模块。
        /// </summary>
        /// <param name="id">id。</param>
        /// <param name="info">要保存的数据。</param>
        /// <returns>id</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Save(int? id, SysModule info)
        {
            id = await _adminService.SaveModuleAsync(id, info);
            return Json(Result.Success("保存成功。", id));
        }

        /// <summary>
        /// 根据查询条件获取模块。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        /// <param name="currentId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task<JsonResult> Data(int? id, int? targetId, int? currentId, ItemFlag? flag = null)
        {
            var converter = new DynamicTreeNodeJsonConverter<SysModule>(s => s.Name, s => s.Url, s => s.State);
            var list = await _adminService.GetModulesAsync(id, targetId, currentId, null);

            if (id != null)
            {
                return this.Json(list, converter);
            }
            else
            {
                return this.Json(ItemFlagHelper.Insert(list, flag, s => new { id = 0, text = s.GetDescription() }), converter);
            }
        }

        /// <summary>
        /// 按关键字搜索。
        /// </summary>
        /// <param name="keyword">关键字。</param>
        /// <returns></returns>
        public async Task<JsonResult> Search(string keyword)
        {
            var list = await _adminService.SearchModulesAsync(keyword);
            return Json(list);
        }

        /// <summary>
        /// 删除模块。
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            await _adminService.DeleteModuleAsync(id);
            return Json(Result.Success("删除成功。"));
        }

        /// <summary>
        /// 启用模块。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Enable(int id)
        {
            await _adminService.SetModuleStateAsync(id, StateFlags.Enabled);
            return Json(Result.Success("启用成功。"));
        }

        /// <summary>
        /// 禁用模块。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Disable(int id)
        {
            await _adminService.SetModuleStateAsync(id, StateFlags.Disabled);
            return Json(Result.Success("禁用成功。"));
        }

        /// <summary>
        /// 模块上移。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MoveUp(int id)
        {
            var result = await _adminService.MoveModuleUpAsync(id) ?
                Result.Success("上移成功。") :
                Result.Fail("已经是第一个模块了。");

            return Json(result);
        }

        /// <summary>
        /// 模块下移。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MoveDown(int id)
        {
            var result = await _adminService.MoveModuleDownAsync(id) ?
                Result.Success("下移成功。") :
                Result.Fail("已经是最后一个模块了。");

            return Json(result);
        }
    }
}