// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Common.Caching;
using Fireasy.Common.Extensions;
using Fireasy.Common.Localization;
using Fireasy.Common.Logging;
using Fireasy.Common.Serialization;
using Fireasy.Common.Tasks;
using Fireasy.Data;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Linq;
using Fireasy.Data.Entity.Metadata;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fireasy.Zero.Services.Impls
{
    public class AdminService : BaseService, IAdminService
    {
        private DbContext context;

        public AdminService(DbContext context)
        {
            this.context = context;
        }

        #region 用户
        /// <summary>
        /// 验证用户帐号和密码。
        /// </summary>
        /// <param name="account">用户帐号。</param>
        /// <param name="validator">密码验证器。</param>
        /// <param name="tokenCreator">token生成器。</param>
        /// <returns></returns>
        public virtual async Task<SessionContext> CheckLoginAsync(string account, Func<string, bool> validator, Func<SysUser, string> tokenCreator)
        {
            var user = await context.SysUsers
                .Include(s => s.SysOrg)
                .FirstOrDefaultAsync(s => s.Account == account || s.Mobile == account, CancellationToken.None);

            if (user == null || !validator(user.Password))
            {
                throw new ClientNotificationException("你的帐号不存在或密码有误。");
            }

            if (user.State == 0)
            {
                throw new ClientNotificationException("你的帐号已被停用。");
            }

            if (tokenCreator != null)
            {
                user.Token = tokenCreator(user);
            }

            if (user.SysOrg == null)
            {
                throw new ClientNotificationException("你所属的机构失效，请联系管理员。");
            }

            user.LastLoginTime = DateTime.Now;
            await context.SysUsers.UpdateAsync(user);

            return new SessionContext { UserID = user.UserID, UserName = user.Name, OrgID = user.OrgID };
        }

        /// <summary>
        /// 更新用户设备号。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateDeviceNo(int userId, string deviceNo)
        {
            return await context.SysUsers.UpdateAsync(() => new SysUser { DeviceNo = deviceNo }, s => s.UserID == userId) > 0;
        }

        /// <summary>
        /// 获取指定的用户。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<SysUser> GetUserAsync(int userId)
        {
            var user = await context.SysUsers.GetAsync(userId);
            if (user != null)
            {
                user.Role = string.Join(",", context.SysUserRoles.Where(s => s.UserID == userId).Select(s => s.RoleID));
            }

            return user;
        }

        /// <summary>
        /// 获取指定的用户名称。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysUser))]
        public virtual async Task<string> GetUserNameAsync(int userId)
        {
            var user = await context.SysUsers.GetAsync(userId);
            return user == null ? string.Empty : user.Name;
        }

        /// <summary>
        /// 根据帐号获取一个用户。
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public virtual async Task<SysUser> GetUserByAccountAsync(string account)
        {
            return await context.SysUsers.FirstOrDefaultAsync(s => s.Account == account, CancellationToken.None);
        }

        /// <summary>
        /// 保存用户信息。
        /// </summary>
        /// <param name="userId">主键值。</param>
        /// <param name="info"></param>
        /// <param name="pwdCreator"></param>
        /// <returns></returns>
        [TransactionSupport]
        public virtual async Task<int> SaveUserAsync(int? userId, SysUser info, Func<string> pwdCreator)
        {
            if (await context.SysUsers.AnyAsync(s => s.Account == info.Account && userId != s.UserID))
            {
                throw new ClientNotificationException("帐号重复，不能重复添加。");
            }

            if (await context.SysUsers.AnyAsync(s => s.Mobile == info.Mobile && s.UserID != userId))
            {
                throw new ClientNotificationException(string.Format("手机号为{0}的用户已经存在。", info.Mobile));
            }

            var userRoles = new List<SysUserRole>();
            IEnumerable<int> roleIds = null;
            var roleNames = string.Empty;

            if (!string.IsNullOrEmpty(info.Role))
            {
                var posts = await context.SysRoles.ToListAsync();
                var array = new JsonSerializer().Deserialize<string[]>(info.Role);
                roleIds = array.Select(s => Convert.ToInt32(s));
                roleNames = string.Join("、", posts.Where(s => roleIds.Contains(s.RoleID)).Select(s => s.Name));
            }

            info.RoleNames = roleNames;
            info.PyCode = info.Name.ToPinyin();

            if (pwdCreator != null)
            {
                info.Password = pwdCreator();
            }

            if (userId == null)
            {
                await context.SysUsers.InsertAsync(info);
                userId = info.UserID;
            }
            else
            {
                await context.SysUsers.UpdateAsync(info, s => s.UserID == userId);
                await context.SysUserRoles.DeleteAsync(s => s.UserID == userId);
            }

            if (roleIds != null)
            {
                userRoles.AddRange(roleIds.Select(s => new SysUserRole { RoleID = s, UserID = (int)userId }));
                await context.SysUserRoles.BatchAsync(userRoles, (u, s) => u.Insert(s));
            }

            return (int)userId;
        }

        /// <summary>
        /// 保存用户信息。
        /// </summary>
        /// <param name="userId">主键值。</param>
        /// <param name="info"></param>
        public virtual async Task<bool> SaveUserAsync(int userId, SysUser info)
        {
            await context.SysUsers.UpdateAsync(info, s => s.UserID == userId);
            return true;
        }

        /// <summary>
        /// 保存多个用户。
        /// </summary>
        /// <param name="orgId">机构ID。</param>
        /// <param name="users"></param>
        /// <param name="pwdCreator"></param>
        /// <returns></returns>
        [TransactionSupport]
        public virtual async Task<bool> SaveUsersAsync(int orgId, List<SysUser> users, Func<string> pwdCreator)
        {
            var repeats = Util.FindRepeatRows(context.SysUsers, users, (s, t) => s.Mobile == t.Mobile);

            if (repeats == null || repeats.Count > 0)
            {
                throw new DataRepeatException("手机号", repeats);
            }

            users.ForEach(s =>
                {
                    s.PyCode = s.Name.ToPinyin();
                    s.OrgID = orgId;
                    s.Password = pwdCreator();
                    s.State = StateFlags.Enabled;
                });

            return await context.SysUsers.BatchAsync(users, (u, s) => u.Insert(s)) > 0;
        }

        /// <summary>
        /// 设置用户状态。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        public virtual async Task SetUserStateAsync(int userId, StateFlags state)
        {
            await context.SysUsers.UpdateAsync(() => new SysUser { State = state }, s => s.UserID == userId);
        }

        /// <summary>
        /// 重设用户的密码。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="pwdCreator">密码生成器。</param>
        public virtual async Task ResetUserPasswordAsync(int userId, string password, Func<string> pwdCreator)
        {
            await context.SysUsers.UpdateAsync(() => new SysUser { Password = pwdCreator() }, s => s.UserID == userId);
        }

        /// <summary>
        /// 修改用户的密码。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="validator">密码比较器。</param>
        /// <param name="pwdCreator">密码生成器。</param>
        public virtual async Task<bool> ModifyUserPasswordAsync(int userId, Func<string, bool> validator, Func<string> pwdCreator)
        {
            var user = await context.SysUsers.GetAsync(userId);
            if (user == null || !validator(user.Password))
            {
                throw new ClientNotificationException("输入的密码有误。");
            }

            user.Password = pwdCreator();
            await context.SysUsers.UpdateAsync(user);
            return true;
        }

        /// <summary>
        /// 获取用户列表。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="orgCode">机构编码。</param>
        /// <param name="state">状态。</param>
        /// <param name="keyword">关键字。</param>
        /// <param name="pager"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public virtual async Task<List<SysUser>> GetUsersAsync(int userId, string orgCode, StateFlags? state, string keyword, DataPager pager, SortDefinition sorting)
        {
            var orgs = GetPurviewOrgs(userId);

            return await context.SysUsers
                .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                .AssertWhere(state != null, s => s.State == state)
                //如果指定了orgCode则按orgCode左匹配，否则根据用户的数据权限进行逐一的左匹配
                .AssertWhere(!string.IsNullOrEmpty(orgCode) || orgs == null, s => s.SysOrg.Code.StartsWith(orgCode), s => s.BatchOr(orgs, (t, v) => t.SysOrg.Code.StartsWith(v)))
                .AssertWhere(!string.IsNullOrEmpty(keyword), s => s.Account == keyword ||
                                                                  s.Name.Contains(keyword) ||
                                                                  s.PyCode.Contains(keyword) ||
                                                                  s.Mobile.Contains(keyword))
                .Segment(pager)
                .Join(context.SysDictItems.Where(s => s.SysDictType.Code == "Degree").DefaultIfEmpty(), s => s.DegreeNo, s => s.Value, (s, t) => new { sysUser = s, dictDegree = t })
                .Join(context.SysDictItems.Where(s => s.SysDictType.Code == "Title").DefaultIfEmpty(), s => s.sysUser.TitleNo, s => s.Value, (s, t) => new { s.sysUser, s.dictDegree, dictTitle = t })
                .ExtendSelect(s => new SysUser
                    {
                        OrgName = s.sysUser.SysOrg.FullName,
                        SexName = s.sysUser.Sex.GetDescription(),
                        DegreeName = s.dictDegree.Name,
                        TitleName = s.dictTitle.Name,
                    })
                .OrderBy(sorting, u => u.OrderBy(s => s.SysOrg.Code))
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户列表（用指定的角色ID进行排除）。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="orgCode">机构编码。</param>
        /// <param name="roleId">角色ID。</param>
        /// <param name="keyword">关键字。</param>
        /// <param name="pager"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public virtual async Task<List<SysUser>> GetUsersByRoleExcludeAsync(int userId, string orgCode, int roleId, string keyword, DataPager pager, SortDefinition sorting)
        {
            var orgs = GetPurviewOrgs(userId);

            return await context.SysUsers
                .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                .Where(s => s.State == StateFlags.Enabled && !context.SysUserRoles.Where(t => t.RoleID == roleId).Any(t => t.UserID == s.UserID))
                //如果指定了orgCode则按orgCode左匹配，否则根据用户的数据权限进行逐一的左匹配
                .AssertWhere(!string.IsNullOrEmpty(orgCode) || orgs == null, s => s.SysOrg.Code.StartsWith(orgCode), s => s.BatchOr(orgs, (t, v) => t.SysOrg.Code.StartsWith(v)))
                .AssertWhere(!string.IsNullOrEmpty(keyword), s => s.Account == keyword ||
                                                                  s.Name.Contains(keyword) ||
                                                                  s.PyCode.Contains(keyword) ||
                                                                  s.Mobile.Contains(keyword))
                .Segment(pager)
                .Join(context.SysDictItems.Where(s => s.SysDictType.Code == "Degree").DefaultIfEmpty(), s => s.DegreeNo, s => s.Value, (s, t) => new { sysUser = s, dictDegree = t })
                .Join(context.SysDictItems.Where(s => s.SysDictType.Code == "Title").DefaultIfEmpty(), s => s.sysUser.TitleNo, s => s.Value, (s, t) => new { s.sysUser, s.dictDegree, dictTitle = t })
                .ExtendSelect(s => new SysUser
                    {
                        OrgName = s.sysUser.SysOrg.FullName,
                        SexName = s.sysUser.Sex.GetDescription(),
                        DegreeName = s.dictDegree.Name,
                        TitleName = s.dictTitle.Name,
                    })
                .OrderBy(sorting, u => u.OrderBy(s => s.SysOrg.Code))
                .ToListAsync();
        }

        /// <summary>
        /// 获取机构下的用户列表。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [CacheSupport]
        public virtual async Task<List<SysUser>> GetUsersAsync(int orgId)
        {
            return await context.SysUsers
                .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                .Where(s => s.State == StateFlags.Enabled && s.OrgID == orgId)
                .Select(s => new SysUser { UserID = s.UserID, Name = s.Name })
                .ToListAsync();
        }

        /// <summary>
        /// 获取机构下某角色的用户列表。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [CacheSupport]
        public virtual async Task<List<SysUser>> GetUsersAsync(int orgId, int roleId)
        {
            var userIds = context.SysUserRoles
                .Where(s => s.RoleID == roleId && s.SysUser.OrgID == orgId)
                .Select(s => s.UserID);

            return await context.SysUsers
                .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                .Where(s => s.State == StateFlags.Enabled && userIds.Contains(s.UserID))
                .Select(s => new SysUser { UserID = s.UserID, Name = s.Name })
                .ToListAsync();
        }

        /// <summary>
        /// 获取某角色的用户列表。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="pager"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public virtual async Task<List<SysUser>> GetUsersByRoleAsync(int roleId, DataPager pager, SortDefinition sorting)
        {
            return await context.SysUsers
            .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                .Where(s => s.State == StateFlags.Enabled)
                .Where(s => context.SysUserRoles.Any(t => t.RoleID == roleId && t.UserID == s.UserID))
                .Segment(pager)
                .OrderBy(sorting)
                .ExtendSelect(s => new SysUser
                {
                    OrgName = s.SysOrg.FullName,
                    SexName = s.Sex.GetDescription()
                })
                .ToListAsync();
        }

        /// <summary>
        /// 根据角色代码（拼音码）获取用户列表。
        /// </summary>
        /// <param name="orgCode">机构编码。</param>
        /// <param name="postName">角色名称，如果是多个角色，用竖线分隔。</param>
        /// <param name="keyword"></param>
        /// <param name="orgCodeLength">机构编码的位数。如果指定，则在此编码范围的机构下检索。比如分公司范围，则此值传 4。</param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysUserRole))]
        public virtual async Task<List<SysUser>> GetUsersAsync(string orgCode, string postName, string keyword, int? orgCodeLength)
        {
            IQueryable<int> userIds = null;

            if (!string.IsNullOrEmpty(postName))
            {
                var postNames = postName.Split('|');
                userIds = context.SysUserRoles
                    .Where(s => postNames.Contains(s.SysRole.Name))
                    .AssertWhere(!string.IsNullOrEmpty(orgCode) && orgCodeLength == null, s => s.SysUser.SysOrg.Code.StartsWith(orgCode))
                    .AssertWhere(!string.IsNullOrEmpty(orgCode) && orgCodeLength != null && orgCode.Length > orgCodeLength, s => s.SysUser.SysOrg.Code.StartsWith(orgCode.Substring(0, (int)orgCodeLength)))
                    .Select(s => s.UserID);
            }

            return await context.SysUsers
                .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                .Where(s => s.State == StateFlags.Enabled)
                .AssertWhere(userIds != null, s => userIds.Contains(s.UserID))
                .AssertWhere(!string.IsNullOrEmpty(orgCode) && orgCodeLength == null, s => s.SysOrg.Code.StartsWith(orgCode))
                .AssertWhere(!string.IsNullOrEmpty(orgCode) && orgCodeLength != null && orgCode.Length > orgCodeLength, s => s.SysOrg.Code.StartsWith(orgCode.Substring(0, (int)orgCodeLength)))
                .AssertWhere(!string.IsNullOrEmpty(keyword), s => s.Account == keyword ||
                                                                  s.Name.Contains(keyword) ||
                                                                  s.PyCode.Contains(keyword))
                .Select(s => new SysUser { UserID = s.UserID, Name = s.Name, DeviceNo = s.DeviceNo })
                .ToListAsync();
        }

        /// <summary>
        /// 获取用户人员信息列表 根据机构编码和状态
        /// </summary>
        /// <param name="orgCode">组织机构编码</param>
        /// <param name="state">状态 启用或 停用</param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysOrg))]
        public virtual async Task<List<SysUser>> GetUsersByCodeAsync(string orgCode, StateFlags? state)
        {
            return await context.SysUsers
                .AssertWhere(!string.IsNullOrEmpty(orgCode), s => s.SysOrg.Code.StartsWith(orgCode))
                .AssertWhere(state != null, s => s.State == state)
                .ToListAsync();
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteUserAsync(int userId)
        {
            return await context.SysUsers.DeleteAsync(userId) > 0;
        }

        /// <summary>
        /// 更新用户照片。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="photo">照片路径。</param>
        public virtual async Task<bool> UpdateUserPhotoAsync(int userId, string photo)
        {
            return await context.SysUsers.UpdateAsync(() => new SysUser { Photo = photo }, s => s.UserID == userId) > 0;
        }

        /// <summary>
        /// 在机构列表中从下级往上找匹配 <paramref name="orgCode"/> 的机构。
        /// </summary>
        /// <param name="orgs"></param>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        private string FindOrgCode(IEnumerable<SysOrg> orgs, string orgCode)
        {
            for (var i = orgCode.Length; i >= 2; i -= 2)
            {
                foreach (var org in orgs)
                {
                    if (org.Code.Length < i)
                    {
                        continue;
                    }

                    if (orgCode.Substring(0, i) == org.Code.Substring(0, i))
                    {
                        return org.Code;
                    }
                }
            }

            return string.Empty;
        }
        #endregion

        #region 模块
        /// <summary>
        /// 获取模块信息。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual async Task<SysModule> GetModuleAsync(int moduleId)
        {
            var info = await context.SysModules.GetAsync(moduleId);
            if (info != null)
            {
                var treeOper = context.CreateTreeRepository<SysModule>();

                //找以它的上一个模块
                var parent = await treeOper.RecurrenceParent(info).FirstOrDefaultAsync();
                if (parent != null)
                {
                    info.ParentId = parent.ModuleID;
                }

                return info;
            }

            return null;
        }

        /// <summary>
        /// 获取模块的下一个顺序号。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual async Task<int> GetModuleNextOrderNoAsync(int? moduleId)
        {
            var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysModule)).EntityTree.SignLength;
            var parent = moduleId == null ? null : await context.SysModules.GetAsync(moduleId);
            var parentCode = parent == null ? string.Empty : parent.Code;
            var likeMatch = new string('_', signLength);

            return await context.SysModules.Where(s => s.Code.Like(parentCode + likeMatch)).MaxAsync(s => s.OrderNo) + 1;
        }

        /// <summary>
        /// 保存模块。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task<int> SaveModuleAsync(int? moduleId, SysModule info)
        {
            info.PyCode = info.Name.ToPinyin();

            var treeOper = context.CreateTreeRepository<SysModule>();

            if (moduleId == null)
            {
                if (info.ParentId == null)
                {
                    await treeOper.InsertAsync(info, null);
                }
                else
                {
                    //插入为parent的孩子
                    await treeOper.InsertAsync(info, context.SysModules.Get(info.ParentId), EntityTreePosition.Children);
                }

                moduleId = info.ModuleID;
            }
            else
            {
                //移动到parent下
                await treeOper.MoveAsync(info.Normalize(moduleId), await context.SysModules.GetAsync(info.ParentId), EntityTreePosition.Children);
            }

            return (int)moduleId;
        }

        /// <summary>
        /// 获取模块列表。
        /// </summary>
        /// <param name="parentId">父节点ID。</param>
        /// <param name="targetId">目标节点ID，即要展开并选定的节点。</param>
        /// <param name="currentId">当前节点，在添加节点的时候要把当前节点排除。</param>
        /// <param name="state">状态。</param>
        /// <returns></returns>
        public virtual async Task<List<SysModule>> GetModulesAsync(int? parentId, int? targetId, int? currentId, StateFlags? state)
        {
            SysModule parent = null;
            if (parentId != null)
            {
                parent = await context.SysModules.FirstOrDefaultAsync(s => s.ModuleID == parentId);
            }

            var treeOper = context.CreateTreeRepository<SysModule>();
            var result = await treeOper.QueryChildren(parent)
                //如果指定currentId，则需要排除
                .AssertWhere(currentId != null, s => s.ModuleID != currentId)
                .OrderBy(s => s.OrderNo)
                //把HasChildren属性扩展出来
                .Select(s => s.ExtendAs<SysModule>(() => new SysModule
                {
                    HasChildren = treeOper.HasChildren(s, null),
                }))
                .ToListAsync();

            //如果要定位到指定的节点，则递归处理
            if (targetId != null && !TreeNodeExpandChecker.IsExpanded())
            {
                var target = await context.SysModules.GetAsync(targetId);
                var parents = await treeOper.RecurrenceParent(target).Select(s => s.ModuleID).ToListAsync();

                await result.ExpandAsync(parents, async childId => await GetModulesAsync(childId, targetId, currentId, state),
                    parents.Count - 1);
            }

            return result;
        }

        /// <summary>
        /// 按关键字搜索模块。
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public virtual async Task<List<SysModule>> SearchModulesAsync(string keyword)
        {
            return await context.SysModules.Where(s => (s.Name.Contains(keyword) || s.PyCode.Contains(keyword) || s.Url.Contains(keyword)))
                .ToListAsync();
        }

        /// <summary>
        /// 删除模块。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteModuleAsync(int moduleId)
        {
            return await context.SysModules.DeleteAsync(moduleId) > 0;
        }

        /// <summary>
        /// 将模块向上移。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual async Task<bool> MoveModuleUpAsync(int moduleId)
        {
            var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysModule)).EntityTree.SignLength;
            var info = await context.SysModules.GetAsync(moduleId);
            var parentCode = info.Code.Left(info.Code.Length - signLength);
            var likeMatch = new string('_', signLength);

            var prev = await context.SysModules.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo < info.OrderNo).OrderByDescending(s => s.OrderNo).FirstOrDefaultAsync();
            if (prev != null)
            {
                var orderNo = prev.OrderNo;
                prev.OrderNo = info.OrderNo;
                info.OrderNo = orderNo;

                await context.UseTransactionAsync(async (dc, token) =>
                    {
                        await dc.SysModules.UpdateAsync(info, token);
                        await dc.SysModules.UpdateAsync(prev, token);
                    });

                return true;
            }

            return false;
        }

        /// <summary>
        /// 将模块向下移。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual async Task<bool> MoveModuleDownAsync(int moduleId)
        {
            var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysModule)).EntityTree.SignLength;
            var info = await context.SysModules.GetAsync(moduleId);
            var parentCode = info.Code.Left(info.Code.Length - signLength);
            var likeMatch = new string('_', signLength);

            var next = await context.SysModules.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo > info.OrderNo).OrderBy(s => s.OrderNo).FirstOrDefaultAsync();
            if (next != null)
            {
                var orderNo = next.OrderNo;
                next.OrderNo = info.OrderNo;
                info.OrderNo = orderNo;

                await context.UseTransactionAsync(async (dc, token) =>
                    {
                        await dc.SysModules.UpdateAsync(info, token);
                        await dc.SysModules.UpdateAsync(next, token);
                    });

                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置模块状态。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        public virtual async Task SetModuleStateAsync(int moduleId, StateFlags state)
        {
            await context.SysModules.UpdateAsync(() => new SysModule { State = state }, s => s.ModuleID == moduleId);
        }
        #endregion

        #region 操作
        /// <summary>
        /// 获取模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual async Task<List<SysOperate>> GetOperatesAsync(int moduleId)
        {
            return await context.SysOperates
                .Where(s => s.ModuleID == moduleId)
                .OrderBy(s => s.OrderNo)
                .ToListAsync();
        }

        /// <summary>
        /// 保存模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="rows"></param>
        [TransactionSupport]
        public virtual async Task SaveOperatesAsync(int moduleId, List<SysOperate> rows)
        {
            var index = 1;
            rows.ForEach(s =>
                {
                    s.ModuleID = moduleId;
                    s.State = StateFlags.Enabled;
                    s.OrderNo = index++;
                });

            await context.SysOperates.DeleteAsync(s => s.ModuleID == moduleId);

            await context.SysOperates.BatchAsync(rows, (u, s) => u.Insert(s));
        }

        /// <summary>
        /// 保存模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="added"></param>
        /// <param name="updated"></param>
        /// <param name="deleted"></param>
        [TransactionSupport]
        public virtual async Task SaveOperatesAsync(int moduleId, List<SysOperate> added, List<SysOperate> updated, List<SysOperate> deleted)
        {
            var index = await context.SysOperates.Where(s => s.ModuleID == moduleId).MaxAsync(s => s.OrderNo) + 1;

            added.ForEach(s =>
                {
                    s.ModuleID = moduleId;
                    s.State = StateFlags.Enabled;
                    s.OrderNo = index++;
                });

            await context.SysOperates.BatchAsync(added, (u, s) => u.Insert(s));
            await context.SysOperates.BatchAsync(updated, (u, s) => u.Update(s));
            await context.SysOperates.BatchAsync(deleted, (u, s) => u.Delete(s, true));
        }
        #endregion

        #region 机构
        /// <summary>
        /// 获取机构列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="targetId"></param>
        /// <param name="currentId"></param>
        /// <param name="state"></param>
        /// <param name="keyword"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public virtual async Task<List<SysOrg>> GetOrgsAsync(int? parentId, int? targetId, int? currentId, StateFlags? state, string keyword, OrgAttribute? attribute)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                return await context.SysOrgs
                    .Where(s => (s.Name.Contains(keyword)) && !s.Code.StartsWith("99"))
                    .AssertWhere(attribute != null, s => s.Attribute <= attribute && s.Attribute != 0)
                    .AssertWhere(state != null, s => s.State == state)
                    .ToListAsync();
            }

            SysOrg parent = null;
            if (parentId != null)
            {
                parent = await context.SysOrgs.FirstOrDefaultAsync(s => s.OrgID == parentId);
            }

            var treeOper = context.CreateTreeRepository<SysOrg>();
            var result = await treeOper.QueryChildren(parent)
                .AssertWhere(attribute != null, s => s.Attribute <= attribute && s.Attribute != 0)
                //如果指定currentId，则需要排除
                .AssertWhere(currentId != null, s => s.OrgID != currentId)
                .AssertWhere(state != null, s => s.State == state)
                .Where(s => !s.Code.StartsWith("99"))
                .OrderBy(s => s.OrderNo)
                //把HasChildren属性扩展出来
                .Select(s => s.ExtendAs<SysOrg>(() => new SysOrg
                {
                    HasChildren = treeOper.HasChildren(s, t => attribute == null || (attribute != null && t.Attribute <= attribute && t.Attribute != 0)),
                    AttributeName = s.Attribute.GetDescription()
                }))
                .ToListAsync();

            if (targetId != null && !TreeNodeExpandChecker.IsExpanded())
            {
                var target = await context.SysOrgs.GetAsync(targetId);
                var parents = await treeOper.RecurrenceParent(target).Select(s => s.OrgID).ToListAsync();

                await result.ExpandAsync(parents, async childId => await GetOrgsAsync(childId, targetId, currentId, state, string.Empty, attribute),
                    parents.Count - 1);
            }

            return result;
        }

        /// <summary>
        /// 获取机构列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyword"></param>
        /// <param name="attribute"></param>
        /// <param name="attachType"></param>
        /// <param name="corpType"></param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysRole))]
        [CacheRelation(typeof(SysUser))]
        [CacheRelation(typeof(SysOrgPermission))]
        public virtual async Task<List<SysOrg>> GetOrgsAsync(int userId, string keyword, OrgAttribute? attribute)
        {
            //通过数据权限来限定要显示的机构
            var purOrgs = GetPurviewOrgs(userId);

            var result = new List<SysOrg>();

            var list = await context.SysOrgs
                .Where(s => !s.Code.StartsWith("99") && s.State == StateFlags.Enabled)
                .BatchOr(purOrgs, (o, s) => o.Code.StartsWith(s))
                .ToListAsync();

            Util.MakeChildren(result, list, string.Empty, 2);

            return result;
        }

        /// <summary>
        /// 获取机构信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<SysOrg> GetOrgAsync(int orgId)
        {
            var info = await context.SysOrgs.GetAsync(orgId);
            if (info != null)
            {
                var treeOper = context.CreateTreeRepository<SysOrg>();

                //找以它的上一个机构
                var parent = await treeOper.RecurrenceParent(info).FirstOrDefaultAsync();
                if (parent != null)
                {
                    info.ParentId = parent.OrgID;
                }

                return info;
            }

            return null;
        }

        /// <summary>
        /// 获取机构的下一个顺序号。
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual async Task<int> GetOrgNextOrderNoAsync(int? parentId)
        {
            var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysOrg)).EntityTree.SignLength;
            var parent = parentId == null ? null : await context.SysOrgs.GetAsync(parentId);
            var parentCode = parent == null ? string.Empty : parent.Code;
            var likeMatch = new string('_', signLength);

            return await context.SysOrgs.Where(s => s.Code.Like(parentCode + likeMatch)).MaxAsync(s => s.OrderNo) + 1;
        }

        /// <summary>
        /// 保存机构。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task<int> SaveOrgAsync(int? orgId, SysOrg info)
        {
            info.PyCode = info.Name.ToPinyin();

            var treeOper = context.CreateTreeRepository<SysOrg>();

            if (orgId == null)
            {
                if (info.ParentId == null)
                {
                    await treeOper.InsertAsync(info, null);
                }
                else
                {
                    //插入为parent的孩子
                    await treeOper.InsertAsync(info, await context.SysOrgs.GetAsync(info.ParentId), EntityTreePosition.Children);
                }

                orgId = info.OrgID;
            }
            else
            {
                //移动到parent下
                await treeOper.MoveAsync(info.Normalize(orgId), await context.SysOrgs.GetAsync(info.ParentId), EntityTreePosition.Children);
            }

            return (int)orgId;
        }

        /// <summary>
        /// 保存多个机构。
        /// </summary>
        /// <param name="parentId">父机构ID。</param>
        /// <param name="orgs"></param>
        /// <returns></returns>
        [TransactionSupport]
        public virtual async Task<bool> SaveOrgsAsync(int? parentId, List<SysOrg> orgs)
        {
            var orderNo = await GetOrgNextOrderNoAsync(parentId);

            orgs.ForEach(s =>
                {
                    s.State = StateFlags.Enabled;
                    s.PyCode = s.Name.ToPinyin();
                    s.OrderNo = (++orderNo);
                });

            var treeOper = context.CreateTreeRepository<SysOrg>();
            var parent = parentId == null ? null : await context.SysOrgs.GetAsync(parentId);

            await treeOper.BatchInsertAsync(orgs, parent, EntityTreePosition.Children);

            return true;
        }

        /// <summary>
        /// 按关键字搜索机构。
        /// </summary>
        /// <param name="keyword">关键字。</param>
        /// <returns></returns>
        public virtual async Task<List<SysOrg>> SearchOrgsAsync(string keyword)
        {
            return await context.SysOrgs.Where(s => s.PyCode.Contains(keyword) || s.Name.Contains(keyword))
                .ToListAsync();
        }

        /// <summary>
        /// 将机构向上移。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<bool> MoveOrgUpAsync(int orgId)
        {
            var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysOrg)).EntityTree.SignLength;
            var info = await context.SysOrgs.GetAsync(orgId);
            var parentCode = info.Code.Left(info.Code.Length - signLength);
            var likeMatch = new string('_', signLength);

            var prev = await context.SysOrgs.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo < info.OrderNo).OrderByDescending(s => s.OrderNo).FirstOrDefaultAsync();
            if (prev != null)
            {
                var orderNo = prev.OrderNo;
                prev.OrderNo = info.OrderNo;
                info.OrderNo = orderNo;

                await context.UseTransactionAsync(async (dc, token) =>
                    {
                        await dc.SysOrgs.UpdateAsync(info, token);
                        await dc.SysOrgs.UpdateAsync(prev, token);
                    });

                return true;
            }

            return false;
        }

        /// <summary>
        /// 将机构向下移。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<bool> MoveOrgDownAsync(int orgId)
        {
            var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysOrg)).EntityTree.SignLength;
            var info = await context.SysOrgs.GetAsync(orgId);
            var parentCode = info.Code.Left(info.Code.Length - signLength);
            var likeMatch = new string('_', signLength);

            var next = await context.SysOrgs.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo > info.OrderNo).OrderBy(s => s.OrderNo).FirstOrDefaultAsync();
            if (next != null)
            {
                var orderNo = next.OrderNo;
                next.OrderNo = info.OrderNo;
                info.OrderNo = orderNo;

                await context.UseTransactionAsync(async (dc, token) =>
                    {
                        await dc.SysOrgs.UpdateAsync(info, token);
                        await dc.SysOrgs.UpdateAsync(next, token);
                    });

                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置机构状态。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        public virtual async Task SetOrgStateAsync(int orgId, StateFlags state)
        {
            await context.SysOrgs.UpdateAsync(() => new SysOrg { State = state }, s => s.OrgID == orgId);
        }

        /// <summary>
        /// 删除机构。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteOrgAsync(int orgId)
        {
            var org = await context.SysOrgs.GetAsync(orgId);
            if (org.Code.Length == 2)
            {
                throw new ClientNotificationException("不能删除顶级机构。");
            }

            return await context.SysOrgs.DeleteAsync(org) > 0;
        }
        #endregion

        #region 角色
        /// <summary>
        /// 获取指定的角色。
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public virtual async Task<SysRole> GetRoleAsync(int roleId)
        {
            return await context.SysRoles.GetAsync(roleId);
        }

        /// <summary>
        /// 保存机构信息。
        /// </summary>
        /// <param name="roleId">主键值。</param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task<int> SaveRoleAsync(int? roleId, SysRole info)
        {
            if (await context.SysRoles.AnyAsync(s => s.Name == info.Name && s.RoleID != roleId))
            {
                throw new ClientNotificationException(string.Format("角色{0}已经存在，名称不能重复。", info.Name));
            }

            if (roleId == null)
            {
                info.PyCode = info.Name.ToPinyin();
                info.Code = GetNextCode();
                await context.SysRoles.InsertAsync(info);
                roleId = info.RoleID;
            }
            else
            {
                await context.SysRoles.UpdateAsync(info, s => s.RoleID == roleId);
            }

            return (int)roleId;
        }

        /// <summary>
        /// 保存多个角色。
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        public virtual async Task<bool> SaveRolesAsync(List<SysRole> posts)
        {
            var names = posts.Select(s => s.Name).ToArray();
            names = await (context.SysRoles.Where(s => names.Contains(s.Name)).Select(s => s.Name)).ToArrayAsync();
            if (names.Length > 0)
            {
                throw new ClientNotificationException(string.Format("角色{0}已经存在，不能重复添加。", string.Join("、", names)));
            }

            var code = GetNextCode();
            foreach (var info in posts)
            {
                info.Code = code;
                info.PyCode = info.Name.ToPinyin();
                code = GetNextCode(code, 4);
            }

            await context.SysRoles.BatchAsync(posts, (u, s) => u.Insert(s));

            return true;
        }

        private string GetNextCode()
        {
            var post = context.SysRoles.OrderByDescending(s => s.Code).FirstOrDefault();
            if (post == null)
            {
                return "0001";
            }

            return GetNextCode(post.Code, 4);
        }

        private string GetNextCode(string code, int length)
        {
            var newCode = (Convert.ToInt32(code) + 1).ToString();
            return new string('0', length - newCode.Length) + newCode;
        }

        /// <summary>
        /// 设置角色状态。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        public virtual async Task SetRoleStateAsync(int roleId, StateFlags state)
        {
            await context.SysRoles.UpdateAsync(() => new SysRole { State = state }, s => s.RoleID == roleId);
        }

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteRoleAsync(int id)
        {
            return await context.SysRoles.DeleteAsync(id) > 0;
        }

        /// <summary>
        /// 获取角色列表。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="keyword">关键字。</param>
        /// <param name="pager"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public virtual async Task<List<SysRole>> GetRolesAsync(StateFlags? state, string keyword, DataPager pager, SortDefinition sorting)
        {
            return await context.SysRoles
                .AssertWhere(state != null, s => s.State == state)
                .AssertWhere(!string.IsNullOrEmpty(keyword), s => s.Name.Contains(keyword) || s.PyCode.Contains(keyword))
                .Select(s => s.ExtendAs<SysRole>(() => new SysRole
                {
                    //AttributeName = s.Attribute.GetDescription()
                }))
                .Segment(pager)
                .OrderBy(sorting)
                .ToListAsync();
        }
        #endregion

        #region 权限
        /// <summary>
        /// 获取用户具有的操作模块。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysUserRole))]
        [CacheRelation(typeof(SysModulePermission))]
        public virtual async Task<List<SysModule>> GetPurviewModulesAsync(int userId)
        {
            var result = new List<SysModule>();

            var user = await context.SysUsers.GetAsync(userId);
            var roleIds = context.SysUserRoles.Where(s => s.UserID == userId).Select(s => s.RoleID).Distinct();

            var list = await context.SysModules.Where(s => s.State == StateFlags.Enabled)
                //超级用户不用判断权限
                .AssertWhere(user.Account != "admin", s => context.SysModulePermissions.Any(t => roleIds.Contains(t.RoleID) && t.ModuleID == s.ModuleID))
                .ToListAsync();

            Util.MakeChildren(result, list, string.Empty);
            return result;
        }

        /// <summary>
        /// 根据架构ID和角色ID获取模块列表。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <returns></returns>
        public virtual async Task<List<SysModule>> GetModulesByRoleAsync(int roleId)
        {
            var result = new List<SysModule>();

            if (roleId == 0)
            {
                return result;
            }

            var operates = await context.SysOperates.ToListAsync();
            var operatePermissions = await context.SysOperatePermissions.Include(s => s.SysOperate).ToListAsync();

            var list = await context.SysModules.Where(s => s.State == StateFlags.Enabled)
                .Select(s => s.ExtendAs<SysModule>(() => new SysModule
                {
                    //判断是否此模块是否给定了角色权限
                    Permissible = context.SysModulePermissions
                                .Any(t => t.RoleID == roleId && t.ModuleID == s.ModuleID),
                    //SysOperates = operates.Where(t => t.ModuleID == s.ModuleID).ToEntitySet() //如果CacheParsing开启则下一次无法获取到数据
                }))
                .ToListAsync();

            list.ForEach(s =>
                {
                    s.SysOperates = operates.Where(t => t.ModuleID == s.ModuleID).ToEntitySet();
                    s.SysOperates.ForEach(t => t.Permissible = operatePermissions
                        .Any(v => v.OperID == t.OperID && v.ModuleID == s.ModuleID && v.RoleID == roleId));
                });

            Util.MakeChildren(result, list, string.Empty);

            operates.Clear();
            operatePermissions.Clear();

            return result;
        }

        /// <summary>
        /// 根据角色ID获取机构列表。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <returns></returns>
        public virtual async Task<List<SysOrg>> GetOrgsByRoleAsync(int roleId)
        {
            var result = new List<SysOrg>();

            if (roleId == 0)
            {
                return result;
            }

            var list = await context.SysOrgs.Where(s => s.State == StateFlags.Enabled && s.Attribute == OrgAttribute.Org)
                .Select(s => s.ExtendAs<SysOrg>(() => new SysOrg
                {
                    //判断是否此机构是否给定了角色权限
                    Permissible = context.SysOrgPermissions.Any(t => t.RoleID == roleId && t.OrgID == s.OrgID)
                }))
                .ToListAsync();

            Util.MakeChildren(result, list, string.Empty, 2);

            return result;
        }

        /// <summary>
        /// 保存功能角色的权限。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="modules"></param>
        /// <param name="opers"></param>
        [TransactionSupport]
        public virtual async Task SaveFuncRolePermissions(int roleId, List<int> modules, Dictionary<int, List<int>> opers)
        {
            //清理数据
            await context.SysModulePermissions.DeleteAsync(s => s.RoleID == roleId);
            await context.SysOperatePermissions.DeleteAsync(s => s.RoleID == roleId);

            var permissions = modules.Select(s => new SysModulePermission
            {
                RoleID = roleId,
                ModuleID = s
            });

            await context.SysModulePermissions.BatchAsync(permissions, (u, s) => u.Insert(s));

            var operatePermissions = opers.SelectMany(s => s.Value.Select(t => new SysOperatePermission { ModuleID = s.Key, OperID = t, RoleID = roleId })).ToList();
            if (operatePermissions.Count > 0)
            {
                await context.SysOperatePermissions.BatchAsync(operatePermissions, (u, s) => u.Insert(s));
            }
        }

        /// <summary>
        /// 保存数据角色的权限。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="orgs"></param>
        [TransactionSupport]
        public virtual async Task SaveOrgRolePermissionsAsync(int roleId, List<int> orgs)
        {
            //清理数据
            await context.SysOrgPermissions.DeleteAsync(s => s.RoleID == roleId);

            var permissions = orgs.Select(s => new SysOrgPermission
            {
                OrgID = s,
                RoleID = roleId
            });

            await context.SysOrgPermissions.BatchAsync(permissions, (u, s) => u.Insert(s));
        }

        /// <summary>
        /// 根据 Url 获取操作按钮。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="moduleUrl">模块的URL。</param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysUserRole))]
        [CacheRelation(typeof(SysOperate))]
        [CacheRelation(typeof(SysOperatePermission))]
        public virtual List<SysOperate> GetPurviewOperates(int userId, string moduleUrl)
        {
            var user = context.SysUsers.Get(userId);
            var posts = context.SysUserRoles.Where(s => s.UserID == userId).Select(s => s.RoleID);

            if (user.Account == "admin")
            {
                return context.SysOperates
                    .Where(s => moduleUrl.EndsWith(s.SysModule.Url))
                    .OrderBy(s => s.OrderNo)
                    .ToList();
            }

            return context.SysOperatePermissions
                .Include(s => s.SysOperate)
                .Where(s => moduleUrl.EndsWith(s.SysModule.Url) && posts.Contains(s.RoleID))
                .OrderBy(s => s.SysOperate.OrderNo)
                .Select(s => s.SysOperate)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// 获取用户具有的数据权限（机构编码）。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysUser))]
        [CacheRelation(typeof(SysUserRole))]
        [CacheRelation(typeof(SysOrgPermission))]
        public virtual List<string> GetPurviewOrgs(int userId)
        {
            var user = context.SysUsers.Include(s => s.SysOrg).FirstOrDefault(s => s.UserID == userId);
            if (user.Account == "admin")
            {
                return null;
            }

            SqlCommand sql = $@"
SELECT
	o.Code
FROM
	SysOrgPermission m
JOIN (
	SELECT
		p.RoleID,
		u.OrgID
	FROM
		SysUserRole p
	JOIN SysUser u ON u.UserID = p.UserID
	WHERE
		p.UserID = {userId}
) t ON m.RoleID = t.RoleID
JOIN SysOrg o ON o.OrgID = m.OrgID
UNION ALL
	SELECT
		o.code
	FROM
		SysUser u
	JOIN SysOrg o ON o.OrgID = u.OrgID
	WHERE
		u.UserID = {userId}
";

            var orgs = context.Database.ExecuteEnumerable<string>(sql).ToList();

            //如果没有分配数据权限，且所属机构是部门，找到分公司的节点
            if (orgs.Count == 1 && orgs[0] == user.SysOrg.Code && user.SysOrg.Attribute == OrgAttribute.Dept)
            {
                var tree = context.CreateTreeRepository<SysOrg>();
                var org = tree.RecurrenceParent(user.SysOrg, s => s.Attribute != OrgAttribute.Dept).FirstOrDefault();
                if (org != null)
                {
                    orgs[0] = org.Code;
                }
            }

            return orgs;
        }

        /// <summary>
        /// 为角色添加多个用户。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <param name="users">用户ID列表。</param>
        /// <returns></returns>
        public virtual async Task AddRoleUsers(int roleId, List<int> users)
        {
            //已经存在的要排除掉
            var exists = context.SysUserRoles.Where(s => s.RoleID == roleId).Select(s => s.UserID).ToArray();
            var userRoles = users.Where(s => !exists.Contains(s)).Select(s => new SysUserRole { RoleID = roleId, UserID = s });
            await context.SysUserRoles.BatchAsync(userRoles, (u, s) => u.Insert(s));
        }

        /// <summary>
        /// 移除角色中的指定的用户。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <param name="users">用户ID列表。</param>
        /// <returns></returns>
        public virtual async Task DeleteRoleUsers(int roleId, List<int> users)
        {
            await context.SysUserRoles.DeleteAsync(s => s.RoleID == roleId && users.Contains(s.UserID));
        }
        #endregion

        #region 字典
        /// <summary>
        /// 根据类别编码获取字典项。
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public virtual List<SysDictItem> GetDictItems(string typeCode)
        {
            return context.SysDictItems
                .Where(s => s.SysDictType.Code == typeCode)
                .OrderBy(s => s.OrderNo)
                .ToList();
        }
        #endregion
    }
}
