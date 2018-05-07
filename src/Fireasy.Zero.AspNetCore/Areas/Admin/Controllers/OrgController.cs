using Fireasy.Common.ComponentModel;
using Fireasy.Common.Extensions;
using Fireasy.Web.EasyUI;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Fireasy.Zero.AspNetCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OrgController : Controller
    {
        private IAdminService adminService;

        public OrgController(IAdminService adminService)
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
        /// 根据ID获取机构。
        /// </summary>
        /// <param name="id">信息ID。</param>
        /// <returns></returns>
        public JsonResult Get(int id)
        {
            var info = adminService.GetOrg(id);
            return Json(info);
        }

        /// <summary>
        /// 获取下一个顺序号。
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public JsonResult GetNextOrderNo(int? parentId)
        {
            return Json(adminService.GetOrgNextOrderNo(parentId));
        }

        /// <summary>
        /// 保存机构。
        /// </summary>
        /// <param name="id">id。</param>
        /// <param name="info">要保存的数据。</param>
        /// <returns>id</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Save(int? id, SysOrg info)
        {
            id = adminService.SaveOrg(id, info);
            return Json(Result.Success("保存成功。", id));
        }

        /// <summary>
        /// 保存多个机构。
        /// </summary>
        /// <param name="parentId">隶属机构ID。</param>
        /// <param name="rows">多个机构数据。</param>
        /// <returns></returns>
        public JsonResult SaveRows(int? parentId, List<SysOrg> rows)
        {
            adminService.SaveOrgs(parentId, rows);
            return Json(Result.Success("保存成功。"));
        }

        /// <summary>
        /// 根据查询条件获取机构。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetId"></param>
        /// <param name="currentId"></param>
        /// <param name="attribute"></param>
        /// <param name="state"></param>
        /// <param name="attType">附加的信息。</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public JsonResult DataDemand(int? id, int? targetId, int? currentId, OrgAttribute? attribute = null, StateFlags? state = null, ItemFlag? flag = null)
        {
            var converter = new DynamicTreeNodeJsonConverter<SysOrg>(s => s.Name, s => s.Code, s => s.AttributeName, s => s.State);

            var list = adminService.GetOrgs(id, targetId, currentId, state, null, attribute);

            return this.Json(id != null ? list : ItemFlagHelper.Insert(list, flag, s => new { id = 0, text = s.GetDescription() }), converter);
        }

        /// <summary>
        /// 根据查询条件获取机构。
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="attribute"></param>
        /// <param name="attType">附加的信息。</param>
        /// <param name="corpType">企业类别。</param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public JsonResult Data(int? targetId, OrgAttribute? attribute = null, ItemFlag? flag = null)
        {
            var converter = new DynamicTreeNodeJsonConverter<SysOrg>(s => s.Name, s => s.Code, s => s.AttributeName, s => s.State);

            var session = HttpContext.GetSession();

            var list = adminService.GetOrgs(session.UserID, null, attribute);
            if (targetId != null)
            {
                ExpandTarget(list, (int)targetId);
            }

            return this.Json(ItemFlagHelper.Insert(list, flag, s => new { id = 0, text = s.GetDescription() }), converter);
        }

        /// <summary>
        /// 按关键字搜索。
        /// </summary>
        /// <param name="appKey">应用key。</param>
        /// <param name="keyword">关键字。</param>
        /// <returns></returns>
        public JsonResult Search(string keyword)
        {
            var list = adminService.SearchOrgs(keyword);
            return Json(list);
        }

        /// <summary>
        /// 删除机构。
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            adminService.DeleteOrg(id);
            return Json(Result.Success("删除成功。"));
        }

        /// <summary>
        /// 启用机构。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Enable(int id)
        {
            adminService.SetOrgState(id, StateFlags.Enabled);
            return Json(Result.Success("启用成功。"));
        }

        /// <summary>
        /// 禁用机构。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Disable(int id)
        {
            adminService.SetOrgState(id, StateFlags.Disabled);
            return Json(Result.Success("禁用成功。"));
        }

        /// <summary>
        /// 展开到目标节点。
        /// </summary>
        /// <param name="list"></param>
        /// <param name="parentId"></param>
        private void ExpandTarget(List<SysOrg> list, int parentId)
        {
            var org = adminService.GetOrg(parentId);
            if (org == null)
            {
                return;
            }

            var code = org.Code;
            var length = code.Length;
            var children = list;

            for (var i = 0; i < length; i += 2)
            {
                org = children.Find(s => s.Code == code.Substring(0, i + 2));
                if (org != null)
                {
                    org.IsLoaded = true;
                    children = org.Children;
                }
            }
        }
    }
}