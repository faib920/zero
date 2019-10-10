using Fireasy.Common.ComponentModel;
using Fireasy.Web.EasyUI;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Models;
using Fireasy.Zero.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private IAdminService adminService;
        private IEncryptProvider encryptProvider;

        public UserController(IAdminService adminService, IEncryptProvider encryptProvider)
        {
            this.adminService = adminService;
            this.encryptProvider = encryptProvider;
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

        /// <summary>
        /// 根据ID获取用户信息。
        /// </summary>
        /// <param name="id">信息ID。</param>
        /// <returns></returns>
        public JsonResult Get(int id)
        {
            var info = adminService.GetUser(id);
            return Json(info);
        }

        public JsonResult GetMyInfo()
        {
            var session = HttpContext.GetSession();
            var info = adminService.GetUser(session.UserID);
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
        public async Task<JsonResult> Save(int? id, SysUser info, string password)
        {
            Func<string> creator = null;

            //新增时使用默认密码
            if (id == null && string.IsNullOrEmpty(password))
            {
                creator = () => encryptProvider.Create("123456");
            }
            //修改时如果填写了新密码
            else if (!string.IsNullOrEmpty(password))
            {
                creator = () => encryptProvider.Create(password);
            }

            id = await adminService.SaveUserAsync(id, info, creator);

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
        public async Task<JsonResult> SaveMyInfo(SysUser info, string oldPwd, string newPwd)
        {
            var session = HttpContext.GetSession();

            await adminService.SaveUserAsync(session.UserID, info, null);

            if (!string.IsNullOrEmpty(oldPwd))
            {
                adminService.ModifyUserPassword(session.UserID, t => encryptProvider.Validate(oldPwd, t), () => encryptProvider.Create(newPwd));
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
        public JsonResult SaveRows(int orgId, List<SysUser> rows)
        {
            adminService.SaveUsers(orgId, rows, () => encryptProvider.Create("123456"));
            return Json(Result.Success("保存成功。"));
        }

        /// <summary>
        /// 根据查询条件获取用户。
        /// </summary>
        /// <param name="orgCode">机构编码</param>
        /// <param name="keyword">关键字</param>
        /// <param name="state">启用状态</param>
        /// <returns></returns>
        public JsonResult Data(string orgCode, string keyword, StateFlags? state)
        {
            var pager = EasyUIHelper.GetDataPager();
            var sorting = EasyUIHelper.GetSorting();

            var userId = HttpContext.GetSession().UserID;

            sorting = sorting.Replace("SexName", "Sex", "DegreeName", "DegreeNo", "TitleName", "TitleNo");

            var list = adminService.GetUsers(userId, orgCode, state, keyword, pager, sorting);
            return Json(EasyUIHelper.Transfer(pager, list));
        }

        /// <summary>
        /// 导出数据。
        /// </summary>
        /// <param name="orgCode">机构编码</param>
        /// <param name="keyword">关键字</param>
        /// <param name="state">启用状态</param>
        /// <returns></returns>
        public FileResult Export(string orgCode, string keyword, StateFlags? state)
        {
            var userId = HttpContext.GetSession().UserID;

            var list = adminService.GetUsers(userId, orgCode, state, keyword, null, null);

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
        public JsonResult Delete(int id)
        {
            adminService.DeleteUser(id);
            return Json(Result.Success("删除成功。"));
        }

        /// <summary>
        /// 启用用户。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Enable(int id)
        {
            adminService.SetUserState(id, StateFlags.Enabled);
            return Json(Result.Success("启用成功。"));
        }

        /// <summary>
        /// 禁用用户。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Disable(int id)
        {
            adminService.SetUserState(id, StateFlags.Disabled);
            return Json(Result.Success("禁用成功。"));
        }

        /// <summary>
        /// 重设密码。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPwd(int id)
        {
            adminService.ResetUserPassword(id, "123456", () => encryptProvider.Create("123456"));
            return Json(Result.Success("成功重设了用户的密码。"));
        }


        /// <summary>
        /// 上传照片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public Result<string> UploadPhoto(int? userId)
        {
            if (Request.Files.Count == 0)
            {
                Result.Fail<object>("请选择文件");
            }

            var file = Request.Files[0];

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "photo");
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
            file.SaveAs(filePath);

            //存放到库里的路径
            var virPath = Path.Combine("photo", fileName).Replace("\\", "/");

            if (userId != null)
            {
                adminService.UpdateUserPhoto((int)userId, virPath);
            }

            return virPath;
        }
    }
}