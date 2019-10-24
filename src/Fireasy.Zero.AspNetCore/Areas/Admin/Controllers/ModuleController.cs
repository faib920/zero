using Fireasy.Common.ComponentModel;
using Fireasy.Common.Extensions;
using Fireasy.Web.EasyUI;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fireasy.Zero.AspNetCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class ModuleController : Controller
    {
        private IAdminService adminService;

        public ModuleController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        public ActionResult Index()
        {
            var a = Url.ActionContext;
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
            var info = await adminService.GetModuleAsync(id);
            return Json(info);
        }

        /// <summary>
        /// 获取下一个顺序号。
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetNextOrderNo(int? parentId)
        {
            return Json(await adminService.GetModuleNextOrderNoAsync(parentId));
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
            id = await adminService.SaveModuleAsync(id, info);
            return Json(Result.Success("保存成功。", id));
        }

        /// <summary>
        /// 根据查询条件获取模块。
        /// </summary>
        /// <param name="hosting">用来往 <see cref="JsonSerializeOption"/> 里加自定义的转换器。</param>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        /// <param name="currentId"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task<JsonResult> Data([FromServices]JsonSerializeOptionHosting hosting, int? id, int? targetId, int? currentId, ItemFlag? flag = null)
        {
            var converter = new DynamicTreeNodeJsonConverter<SysModule>(s => s.Name, s => s.Url, s => s.State);
            hosting.Option.Converters.Add(converter);

            var list = await adminService.GetModulesAsync(id, targetId, currentId, null);

            if (id != null)
            {
                return Json(list);
            }
            else
            {
                return Json(ItemFlagHelper.Insert(list, flag, s => new { id = 0, text = s.GetDescription() }));
            }
        }

        /// <summary>
        /// 按关键字搜索。
        /// </summary>
        /// <param name="keyword">关键字。</param>
        /// <returns></returns>
        public async Task<JsonResult> Search(string keyword)
        {
            var list = await adminService.SearchModulesAsync(keyword);
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
            await adminService.DeleteModuleAsync(id);
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
            await adminService.SetModuleStateAsync(id, StateFlags.Enabled);
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
            await adminService.SetModuleStateAsync(id, StateFlags.Disabled);
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
            var result = await adminService.MoveModuleUpAsync(id) ?
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
            var result = await adminService.MoveModuleDownAsync(id) ?
                Result.Success("下移成功。") :
                Result.Fail("已经是最后一个模块了。");

            return Json(result);
        }
    }
}