using Fireasy.Common.ComponentModel;
using Fireasy.Web.EasyUI;
using Fireasy.Zero.Dtos;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Fireasy.Zero.AspNetCore.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class UserController : Controller
    {
        private IAdminService _adminService;
        private IEncryptProvider _encryptProvider;

        public UserController(IAdminService adminService, IEncryptProvider encryptProvider)
        {
            _adminService = adminService;
            _encryptProvider = encryptProvider;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult EditMyInfo()
        {
            return View();
        }

        public ActionResult Select()
        {
            return View();
        }

        /// <summary>
        /// 根据ID获取用户信息。
        /// </summary>
        /// <param name="id">信息ID。</param>
        /// <returns></returns>
        public async Task<JsonResult> Get(int id)
        {
            var info = await _adminService.GetUserAsync(id);

            return Json(info);
        }

        /// <summary>
        /// 获取当前登录者的信息。
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetMyInfo()
        {
            var session = HttpContext.GetSession();
            var info = await _adminService.GetUserAsync(session.UserID);

            return Json(info);
        }

        /// <summary>
        /// 保存用户。
        /// </summary>
        /// <param name="id">id。</param>
        /// <param name="info">要保存的数据。</param>
        /// <param name="password">密码。</param>
        /// <returns>id</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Save(int? id, UserDto info, string password)
        {
            Func<string> creator = null;

            //新增时使用默认密码
            if (id == null && string.IsNullOrEmpty(password))
            {
                creator = () => _encryptProvider.Create("123456");
            }
            //修改时如果填写了新密码
            else if (!string.IsNullOrEmpty(password))
            {
                creator = () => _encryptProvider.Create(password);
            }

            id = await _adminService.SaveUserAsync(id, info, creator);

            var session = HttpContext.GetSession();
            if (session.UserID == (int)id)
            {
                session.UserName = info.Name;
            }

            return Json(Result.Success("保存成功。", id));
        }

        /// <summary>
        /// 保存用户。
        /// </summary>
        /// <param name="id">id。</param>
        /// <param name="info">要保存的数据。</param>
        /// <param name="oldPwd">密码。</param>
        /// <param name="newPwd"></param>
        /// <returns>id</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveMyInfo(UserDto info, string oldPwd, string newPwd)
        {
            var session = HttpContext.GetSession();

            await _adminService.SaveUserAsync(session.UserID, info, null);

            if (!string.IsNullOrEmpty(oldPwd))
            {
                await _adminService.ModifyUserPasswordAsync(session.UserID, t => _encryptProvider.Validate(oldPwd, t), () => _encryptProvider.Create(newPwd));
            }

            session.UserName = info.Name;

            HttpContext.SetSession(session);

            return Json(Result.Success("保存成功。", session.UserID));
        }

        /// <summary>
        /// 保存多个用户。
        /// </summary>
        /// <param name="orgId">隶属机构ID。</param>
        /// <param name="rows">多个用户数据。</param>
        /// <returns></returns>
        public async Task<JsonResult> SaveRows(int orgId, List<UserDto> rows)
        {
            await _adminService.SaveUsersAsync(orgId, rows, () => _encryptProvider.Create("123456"));
            return Json(Result.Success("保存成功。"));
        }

        /// <summary>
        /// 根据查询条件获取用户。
        /// </summary>
        /// <param name="orgCode">机构编码</param>
        /// <param name="keyword">关键字</param>
        /// <param name="state">启用状态</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> Data(string orgCode, string keyword, StateFlags? state)
        {
            var pager = EasyUIHelper.GetDataPager(HttpContext);
            var sorting = EasyUIHelper.GetSorting(HttpContext);

            var userId = HttpContext.GetSession().UserID;

            sorting = sorting.Replace("SexName", "Sex", "DegreeName", "DegreeNo", "TitleName", "TitleNo");

            var list = await _adminService.GetUsersAsync(userId, orgCode, state, keyword, pager, null);
            return Json(EasyUIHelper.Transfer(pager, list));
        }

        /// <summary>
        /// 导出数据。
        /// </summary>
        /// <param name="orgCode">机构编码</param>
        /// <param name="keyword">关键字</param>
        /// <param name="state">启用状态</param>
        /// <returns></returns>
        public async Task<FileResult> Export(string orgCode, string keyword, StateFlags? state)
        {
            var userId = HttpContext.GetSession().UserID;

            var list = await _adminService.GetUsersAsync(userId, orgCode, state, keyword, null, null);

            var bytes = ExcelHelper.Export("\\templates\\user.xlsx", list, null);

            return File(bytes, "application/vnd.ms-excel", "企业用户.xlsx");
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            await _adminService.DeleteUserAsync(id);
            return Json(Result.Success("删除成功。"));
        }

        /// <summary>
        /// 启用用户。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Enable(int id)
        {
            await _adminService.SetUserStateAsync(id, StateFlags.Enabled);
            return Json(Result.Success("启用成功。"));
        }

        /// <summary>
        /// 禁用用户。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Disable(int id)
        {
            await _adminService.SetUserStateAsync(id, StateFlags.Disabled);
            return Json(Result.Success("禁用成功。"));
        }

        /// <summary>
        /// 重设密码。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPwd(int id)
        {
            await _adminService.ResetUserPasswordAsync(id, "123456", () => _encryptProvider.Create("123456"));
            return Json(Result.Success("成功重设了用户的密码。"));
        }

        /// <summary>
        /// 上传照片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<Result<string>> UploadPhoto([FromServices] IHostingEnvironment env, int? userId)
        {
            if (Request.Form.Files.Count == 0)
            {
                Result.Fail<object>("请选择文件");
            }

            var file = Request.Form.Files[0];

            //存放到 wwwroot 目录下
            var path = Path.Combine(env.WebRootPath, "photo");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //取扩展名
            var ext = file.FileName.Substring(file.FileName.LastIndexOf("."));

            //新文件以guid+扩展名
            var fileName = Guid.NewGuid().ToString() + ext;

            //完整的路径
            var filePath = Path.Combine(path, fileName);

            //保存文件
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                file.CopyTo(fileStream);
            }

            //存放到库里的路径
            var virPath = Path.Combine("photo", fileName).Replace("\\", "/");

            if (userId != null)
            {
                await _adminService.UpdateUserPhotoAsync((int)userId, virPath);
            }

            return virPath;
        }
    }
}