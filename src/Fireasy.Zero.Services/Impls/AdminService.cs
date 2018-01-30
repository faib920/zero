// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;
using Fireasy.Common.Extensions;
using Fireasy.Common.Serialization;
using Fireasy.Data;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Linq;
using Fireasy.Data.Entity.Metadata;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fireasy.Zero.Services.Impls
{
    public class AdminService : BaseService, IAdminService
    {
        #region 用户
        /// <summary>
        /// 验证用户帐号和密码。
        /// </summary>
        /// <param name="account">用户帐号。</param>
        /// <param name="validator">密码验证器。</param>
        /// <param name="tokenCreator">token生成器。</param>
        /// <returns></returns>
        public virtual SessionContext CheckLogin(string account, Func<string, bool> validator, Func<SysUser, string> tokenCreator)
        {
            using (var context = new DbContext())
            {
                var user = context.SysUsers
                    .Include(s => s.SysOrg)
                    .FirstOrDefault(s => s.Account == account || s.Mobile == account);

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
                context.SysUsers.Update(user);

                return new SessionContext { UserID = user.UserID, UserName = user.Name, OrgID = user.OrgID };
            }
        }

        /// <summary>
        /// 更新用户设备号。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public virtual bool UpdateDeviceNo(int userId, string deviceNo)
        {
            using (var context = new DbContext())
            {
                return context.SysUsers.Update(() => new SysUser { DeviceNo = deviceNo }, s => s.UserID == userId) > 0;
            }
        }

        /// <summary>
        /// 获取指定的用户。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual SysUser GetUser(int userId)
        {
            using (var context = new DbContext())
            {
                var user = context.SysUsers.Get(userId);
                if (user != null)
                {
                    user.Role = string.Join(",", context.SysUserRoles.Where(s => s.UserID == userId).Select(s => s.RoleID));
                }

                return user;
            }
        }

        /// <summary>
        /// 获取指定的用户名称。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysUser))]
        public virtual string GetUserName(int userId)
        {
            using (var context = new DbContext())
            {
                var user = context.SysUsers.Get(userId);
                return user == null ? string.Empty : user.Name;
            }
        }

        /// <summary>
        /// 根据帐号获取一个用户。
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public virtual SysUser GetUserByAccount(string account)
        {
            using (var context = new DbContext())
            {
                return context.SysUsers.FirstOrDefault(s => s.Account == account);
            }
        }

        /// <summary>
        /// 保存用户信息。
        /// </summary>
        /// <param name="userId">主键值。</param>
        /// <param name="info"></param>
        /// <param name="pwdCreator"></param>
        /// <returns></returns>
        [TransactionSupport]
        public virtual int SaveUser(int? userId, SysUser info, Func<string> pwdCreator)
        {
            using (var context = new DbContext())
            {
                if (context.SysUsers.Any(s => s.Account == info.Account && userId != s.UserID))
                {
                    throw new ClientNotificationException("帐号重复，不能重复添加。");
                }

                if (context.SysUsers.Any(s => s.Mobile == info.Mobile && s.UserID != userId))
                {
                    throw new ClientNotificationException(string.Format("手机号为{0}的用户已经存在。", info.Name));
                }

                var userRoles = new List<SysUserRole>();
                IEnumerable<int> roleIds = null;
                var roleNames = string.Empty;

                if (!string.IsNullOrEmpty(info.Role))
                {
                    var posts = context.SysRoles.ToList();
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
                    context.SysUsers.Insert(info);
                    userId = info.UserID;
                }
                else
                {
                    context.SysUsers.Update(info, s => s.UserID == userId);
                    context.SysUserRoles.Delete(s => s.UserID == userId);
                }

                if (roleIds != null)
                {
                    userRoles.AddRange(roleIds.Select(s => new SysUserRole { RoleID = s, UserID = (int)userId }));
                    context.SysUserRoles.Batch(userRoles, (u, s) => u.Insert(s));
                }

                return (int)userId;
            }
        }

        /// <summary>
        /// 保存用户信息。
        /// </summary>
        /// <param name="userId">主键值。</param>
        /// <param name="info"></param>
        public virtual bool SaveUser(int userId, SysUser info)
        {
            using (var context = new DbContext())
            {
                context.SysUsers.Update(info, s => s.UserID == userId);
                return true;
            }
        }

        /// <summary>
        /// 保存多个用户。
        /// </summary>
        /// <param name="orgId">机构ID。</param>
        /// <param name="users"></param>
        /// <param name="pwdCreator"></param>
        /// <returns></returns>
        [TransactionSupport]
        public virtual bool SaveUsers(int orgId, List<SysUser> users, Func<string> pwdCreator)
        {
            using (var context = new DbContext())
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

                return context.SysUsers.Batch(users, (u, s) => u.Insert(s)) > 0;
            }
        }

        /// <summary>
        /// 设置用户状态。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        public virtual void SetUserState(int userId, StateFlags state)
        {
            using (var context = new DbContext())
            {
                context.SysUsers.Update(() => new SysUser { State = state }, s => s.UserID == userId);
            }
        }

        /// <summary>
        /// 重设用户的密码。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="pwdCreator">密码生成器。</param>
        public virtual void ResetUserPassword(int userId, string password, Func<string> pwdCreator)
        {
            using (var context = new DbContext())
            {
                context.SysUsers.Update(() => new SysUser { Password = pwdCreator() }, s => s.UserID == userId);
            }
        }

        /// <summary>
        /// 修改用户的密码。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="validator">密码比较器。</param>
        /// <param name="pwdCreator">密码生成器。</param>
        public virtual bool ModifyUserPassword(int userId, Func<string, bool> validator, Func<string> pwdCreator)
        {
            using (var context = new DbContext())
            {
                var user = context.SysUsers.Get(userId);
                if (user == null || !validator(user.Password))
                {
                    throw new ClientNotificationException("输入的密码有误。");
                }

                user.Password = pwdCreator();
                context.SysUsers.Update(user);
                return true;
            }
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
        public virtual List<SysUser> GetUsers(int userId, string orgCode, StateFlags? state, string keyword, DataPager pager, SortDefinition sorting)
        {
            using (var context = new DbContext())
            {
                return context.SysUsers
                    .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                    //.AssertRight(userId, orgCode)
                    .AssertWhere(state != null, s => s.State == state)
                    .AssertWhere(!string.IsNullOrEmpty(orgCode), s => s.SysOrg.Code.StartsWith(orgCode))
                    .AssertWhere(!string.IsNullOrEmpty(keyword), s => s.Account == keyword ||
                                                                      s.Name.Contains(keyword) ||
                                                                      s.PyCode.Contains(keyword) ||
                                                                      s.Mobile.Contains(keyword))
                    .Segment(pager)
                    .ExtendSelect(s => new SysUser
                        {
                            OrgName = s.SysOrg.FullName,
                            SexName = s.Sex.GetDescription()
                        })
                    .OrderBy(sorting)
                    .ToList();
            }
        }

        /// <summary>
        /// 获取机构下的用户列表。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        [CacheSupport]
        public virtual List<SysUser> GetUsers(int orgId)
        {
            using (var context = new DbContext())
            {
                return context.SysUsers
                    .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                    .Where(s => s.State == StateFlags.Enabled && s.OrgID == orgId)
                    .Select(s => new SysUser { UserID = s.UserID, Name = s.Name })
                    .ToList();
            }
        }

        /// <summary>
        /// 获取机构下某角色的用户列表。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [CacheSupport]
        public virtual List<SysUser> GetUsers(int orgId, int roleId)
        {
            using (var context = new DbContext())
            {
                var userIds = context.SysUserRoles
                    .Where(s => s.RoleID == roleId && s.SysUser.OrgID == orgId)
                    .Select(s => s.UserID);

                return context.SysUsers
                    .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                    .Where(s => s.State == StateFlags.Enabled && userIds.Contains(s.UserID))
                    .Select(s => new SysUser { UserID = s.UserID, Name = s.Name })
                    .ToList();
            }
        }

        /// <summary>
        /// 获取机构下某角色的用户列表。
        /// </summary>
        /// <param name="orgCode"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [CacheSupport]
        public virtual List<SysUser> GetUsers(string orgCode, int roleId)
        {
            using (var context = new DbContext())
            {
                while (orgCode.Length > 0)
                {
                    var list = context.SysUsers
                    .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                        .Where(s => s.State == StateFlags.Enabled && s.SysOrg.Code == orgCode)
                        .Select(s => new SysUser { UserID = s.UserID, Name = s.Name })
                        .ToList();

                    if (list.Count > 0)
                    {
                        return list;
                    }

                    orgCode = orgCode.Substring(0, orgCode.Length - 2);
                }

                return new List<SysUser>();
            }
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
        public virtual List<SysUser> GetUsers(string orgCode, string postName, string keyword, int? orgCodeLength)
        {
            using (var context = new DbContext())
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

                return context.SysUsers
                    .Where(s => s.Account != "admin" || string.IsNullOrEmpty(s.Account))
                    .Where(s => s.State == StateFlags.Enabled)
                    .AssertWhere(userIds != null, s => userIds.Contains(s.UserID))
                    .AssertWhere(!string.IsNullOrEmpty(orgCode) && orgCodeLength == null, s => s.SysOrg.Code.StartsWith(orgCode))
                    .AssertWhere(!string.IsNullOrEmpty(orgCode) && orgCodeLength != null && orgCode.Length > orgCodeLength, s => s.SysOrg.Code.StartsWith(orgCode.Substring(0, (int)orgCodeLength)))
                    .AssertWhere(!string.IsNullOrEmpty(keyword), s => s.Account == keyword ||
                                                                      s.Name.Contains(keyword) ||
                                                                      s.PyCode.Contains(keyword))
                    .Select(s => new SysUser { UserID = s.UserID, Name = s.Name, DeviceNo = s.DeviceNo })
                    .ToList();
            }
        }

        /// <summary>
        /// 获取用户人员信息列表 根据机构编码和状态
        /// </summary>
        /// <param name="orgCode">组织机构编码</param>
        /// <param name="state">状态 启用或 停用</param>
        /// <returns></returns>
        [CacheSupport]
        [CacheRelation(typeof(SysOrg))]
        public virtual List<SysUser> GetUsersByCode(string orgCode, StateFlags? state)
        {
            using (var context = new DbContext())
            {
                return context.SysUsers
                    .AssertWhere(!string.IsNullOrEmpty(orgCode), s => s.SysOrg.Code.StartsWith(orgCode))
                    .AssertWhere(state != null, s => s.State == state)
                    .ToList();
            }
        }

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual bool DeleteUser(int userId)
        {
            using (var context = new DbContext())
            {
                return context.SysUsers.Delete(userId) > 0;
            }
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
        public virtual SysModule GetModule(int moduleId)
        {
            using (var context = new DbContext())
            {
                var info = context.SysModules.Get(moduleId);
                if (info != null)
                {
                    var treeOper = context.CreateTreeRepository<SysModule>();

                    //找以它的上一个模块
                    var parent = treeOper.RecurrenceParent(info).FirstOrDefault();
                    if (parent != null)
                    {
                        info.ParentId = parent.ModuleID;
                    }

                    return info;
                }

                return null;
            }
        }

        /// <summary>
        /// 获取模块的下一个顺序号。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual int GetModuleNextOrderNo(int? moduleId)
        {
            using (var context = new DbContext())
            {
                var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysModule)).EntityTree.SignLength;
                var parent = moduleId == null ? null : context.SysModules.Get(moduleId);
                var parentCode = parent == null ? string.Empty : parent.Code;
                var likeMatch = new string('_', signLength);

                return context.SysModules.Where(s => s.Code.Like(parentCode + likeMatch)).Max(s => s.OrderNo) + 1;
            }
        }

        /// <summary>
        /// 保存模块。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int SaveModule(int? moduleId, SysModule info)
        {
            using (var context = new DbContext())
            {
                info.PyCode = info.Name.ToPinyin();

                var treeOper = context.CreateTreeRepository<SysModule>();

                if (moduleId == null)
                {
                    if (info.ParentId == null)
                    {
                        treeOper.Insert(info, null);
                    }
                    else
                    {
                        //插入为parent的孩子
                        treeOper.Insert(info, context.SysModules.Get(info.ParentId), EntityTreePosition.Children);
                    }

                    moduleId = info.ModuleID;
                }
                else
                {
                    //移动到parent下
                    treeOper.Move(info.Normalize(moduleId), context.SysModules.Get(info.ParentId), EntityTreePosition.Children);
                }

                return (int)moduleId;
            }
        }

        /// <summary>
        /// 获取模块列表。
        /// </summary>
        /// <param name="parentId">父节点ID。</param>
        /// <param name="targetId">目标节点ID，即要展开并选定的节点。</param>
        /// <param name="currentId">当前节点，在添加节点的时候要把当前节点排除。</param>
        /// <param name="state">状态。</param>
        /// <returns></returns>
        public virtual List<SysModule> GetModules(int? parentId, int? targetId, int? currentId, StateFlags? state)
        {
            using (var context = new DbContext())
            {
                SysModule parent = null;
                if (parentId != null)
                {
                    parent = context.SysModules.FirstOrDefault(s => s.ModuleID == parentId);
                }

                var treeOper = context.CreateTreeRepository<SysModule>();
                var result = treeOper.QueryChildren(parent)
                    //如果指定currentId，则需要排除
                    .AssertWhere(currentId != null, s => s.ModuleID != currentId)
                    .OrderBy(s => s.OrderNo)
                    //把HasChildren属性扩展出来
                    .Select(s => s.ExtendAs<SysModule>(() => new SysModule
                    {
                        HasChildren = treeOper.HasChildren(s, null),
                    }))
                    .ToList();

                //如果要定位到指定的节点，则递归处理
                if (targetId != null && !TreeNodeExpandChecker.IsExpanded())
                {
                    var target = context.SysModules.Get(targetId);
                    var parents = treeOper.RecurrenceParent(target).Select(s => s.ModuleID).ToList();

                    result.Expand(parents, childId => GetModules(childId, targetId, currentId, state),
                        parents.Count - 1);
                }

                return result;
            }
        }

        /// <summary>
        /// 按关键字搜索模块。
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public virtual List<SysModule> SearchModules(string keyword)
        {
            using (var context = new DbContext())
            {
                return context.SysModules.Where(s => (s.Name.Contains(keyword) || s.PyCode.Contains(keyword) || s.Url.Contains(keyword)))
                    .ToList();
            }
        }

        /// <summary>
        /// 删除模块。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual bool DeleteModule(int moduleId)
        {
            using (var context = new DbContext())
            {
                return context.SysModules.Delete(moduleId) > 0;
            }
        }

        /// <summary>
        /// 将模块向上移。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual bool MoveModuleUp(int moduleId)
        {
            using (var context = new DbContext())
            {
                var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysModule)).EntityTree.SignLength;
                var info = context.SysModules.Get(moduleId);
                var parentCode = info.Code.Left(info.Code.Length - signLength);
                var likeMatch = new string('_', signLength);

                var prev = context.SysModules.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo < info.OrderNo).OrderByDescending(s => s.OrderNo).FirstOrDefault();
                if (prev != null)
                {
                    var orderNo = prev.OrderNo;
                    prev.OrderNo = info.OrderNo;
                    info.OrderNo = orderNo;

                    context.SysModules.Update(info);
                    context.SysModules.Update(prev);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 将模块向下移。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual bool MoveModuleDown(int moduleId)
        {
            using (var context = new DbContext())
            {
                var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysModule)).EntityTree.SignLength;
                var info = context.SysModules.Get(moduleId);
                var parentCode = info.Code.Left(info.Code.Length - signLength);
                var likeMatch = new string('_', signLength);

                var next = context.SysModules.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo > info.OrderNo).OrderBy(s => s.OrderNo).FirstOrDefault();
                if (next != null)
                {
                    var orderNo = next.OrderNo;
                    next.OrderNo = info.OrderNo;
                    info.OrderNo = orderNo;

                    context.SysModules.Update(info);
                    context.SysModules.Update(next);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 设置模块状态。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        public virtual void SetModuleState(int moduleId, StateFlags state)
        {
            using (var context = new DbContext())
            {
                context.SysModules.Update(() => new SysModule { State = state }, s => s.ModuleID == moduleId);
            }
        }
        #endregion

        #region 操作
        /// <summary>
        /// 获取模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public virtual List<SysOperate> GetOperates(int moduleId)
        {
            using (var context = new DbContext())
            {
                return context.SysOperates
                    .Where(s => s.ModuleID == moduleId)
                    .OrderBy(s => s.OrderNo)
                    .ToList();
            }
        }

        /// <summary>
        /// 保存模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="rows"></param>
        [TransactionSupport]
        public virtual void SaveOperates(int moduleId, List<SysOperate> rows)
        {
            var index = 1;
            rows.ForEach(s =>
            {
                s.ModuleID = moduleId;
                s.State = StateFlags.Enabled;
                s.OrderNo = index++;
            });

            using (var context = new DbContext())
            {
                context.SysOperates.Delete(s => s.ModuleID == moduleId);

                context.SysOperates.Batch(rows, (u, s) => u.Insert(s));
            }
        }

        /// <summary>
        /// 保存模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="added"></param>
        /// <param name="updated"></param>
        /// <param name="deleted"></param>
        [TransactionSupport]
        public virtual void SaveOperates(int moduleId, List<SysOperate> added, List<SysOperate> updated, List<SysOperate> deleted)
        {

            using (var context = new DbContext())
            {
                var index = context.SysOperates.Where(s => s.ModuleID == moduleId).Max(s => s.OrderNo) + 1;

                added.ForEach(s =>
                {
                    s.ModuleID = moduleId;
                    s.State = StateFlags.Enabled;
                    s.OrderNo = index++;
                });

                context.SysOperates.Batch(added, (u, s) => u.Insert(s));
                context.SysOperates.Batch(updated, (u, s) => u.Update(s));
                context.SysOperates.Batch(deleted, (u, s) => u.Delete(s, true));
            }
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
        public virtual List<SysOrg> GetOrgs(int? parentId, int? targetId, int? currentId, StateFlags? state, string keyword, OrgAttribute? attribute)
        {
            using (var context = new DbContext())
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    return context.SysOrgs
                        .Where(s => (s.Name.Contains(keyword)) && !s.Code.StartsWith("99"))
                        .AssertWhere(attribute != null, s => s.Attribute <= attribute && s.Attribute != 0)
                        .AssertWhere(state != null, s => s.State == state)
                        .ToList();
                }

                SysOrg parent = null;
                if (parentId != null)
                {
                    parent = context.SysOrgs.FirstOrDefault(s => s.OrgID == parentId);
                }

                var treeOper = context.CreateTreeRepository<SysOrg>();
                var result = treeOper.QueryChildren(parent)
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
                    .ToList();

                if (targetId != null && !TreeNodeExpandChecker.IsExpanded())
                {
                    var target = context.SysOrgs.Get(targetId);
                    var parents = treeOper.RecurrenceParent(target).Select(s => s.OrgID).ToList();

                    result.Expand(parents, childId => GetOrgs(childId, targetId, currentId, state, string.Empty, attribute),
                        parents.Count - 1);
                }

                return result;
            }
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
        public virtual List<SysOrg> GetOrgs(int userId, string keyword, OrgAttribute? attribute)
        {
            using (var context = new DbContext())
            {
                //通过数据权限来限定要显示的机构
                var purOrgs = GetPurviewOrgs(userId);

                var result = new List<SysOrg>();

                var list = context.SysOrgs
                    .Where(s => !s.Code.StartsWith("99") && s.State == StateFlags.Enabled)
                    .BatchOr(purOrgs, (o, s) => o.Code.StartsWith(s))
                    .ToList();

                Util.MakeChildren(result, list, string.Empty, 2);

                return result;
            }
        }

        /// <summary>
        /// 获取机构信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual SysOrg GetOrg(int orgId)
        {
            using (var context = new DbContext())
            {
                var info = context.SysOrgs.Get(orgId);
                if (info != null)
                {
                    var treeOper = context.CreateTreeRepository<SysOrg>();

                    //找以它的上一个机构
                    var parent = treeOper.RecurrenceParent(info).FirstOrDefault();
                    if (parent != null)
                    {
                        info.ParentId = parent.OrgID;
                    }

                    return info;
                }

                return null;
            }
        }

        /// <summary>
        /// 获取机构的下一个顺序号。
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public virtual int GetOrgNextOrderNo(int? parentId)
        {
            using (var context = new DbContext())
            {
                var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysOrg)).EntityTree.SignLength;
                var parent = parentId == null ? null : context.SysOrgs.Get(parentId);
                var parentCode = parent == null ? string.Empty : parent.Code;
                var likeMatch = new string('_', signLength);

                return context.SysOrgs.Where(s => s.Code.Like(parentCode + likeMatch)).Max(s => s.OrderNo) + 1;
            }
        }

        /// <summary>
        /// 保存机构。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int SaveOrg(int? orgId, SysOrg info)
        {
            using (var context = new DbContext())
            {
                info.PyCode = info.Name.ToPinyin();

                var treeOper = context.CreateTreeRepository<SysOrg>();

                if (orgId == null)
                {
                    if (info.ParentId == null)
                    {
                        treeOper.Insert(info, null);
                    }
                    else
                    {
                        //插入为parent的孩子
                        treeOper.Insert(info, context.SysOrgs.Get(info.ParentId), EntityTreePosition.Children);
                    }

                    orgId = info.OrgID;
                }
                else
                {
                    //移动到parent下
                    treeOper.Move(info.Normalize(orgId), context.SysOrgs.Get(info.ParentId), EntityTreePosition.Children);
                }

                return (int)orgId;
            }
        }

        /// <summary>
        /// 保存多个机构。
        /// </summary>
        /// <param name="parentId">父机构ID。</param>
        /// <param name="orgs"></param>
        /// <returns></returns>
        [TransactionSupport]
        public virtual bool SaveOrgs(int? parentId, List<SysOrg> orgs)
        {

            using (var context = new DbContext())
            {
                var orderNo = GetOrgNextOrderNo(parentId);

                orgs.ForEach(s =>
                {
                    s.State = StateFlags.Enabled;
                    s.PyCode = s.Name.ToPinyin();
                    s.OrderNo = (++orderNo);
                });

                var treeOper = context.CreateTreeRepository<SysOrg>();
                var parent = parentId == null ? null : context.SysOrgs.Get(parentId);

                treeOper.BatchInsert(orgs, parent, EntityTreePosition.Children);

                return true;
            }
        }

        /// <summary>
        /// 按关键字搜索机构。
        /// </summary>
        /// <param name="keyword">关键字。</param>
        /// <returns></returns>
        public virtual List<SysOrg> SearchOrgs(string keyword)
        {
            using (var context = new DbContext())
            {
                return context.SysOrgs.Where(s => (s.Name.Contains(keyword)))
                    .ToList();
            }
        }

        /// <summary>
        /// 将机构向上移。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual bool MoveOrgUp(int orgId)
        {
            using (var context = new DbContext())
            {
                var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysOrg)).EntityTree.SignLength;
                var info = context.SysOrgs.Get(orgId);
                var parentCode = info.Code.Left(info.Code.Length - signLength);
                var likeMatch = new string('_', signLength);

                var prev = context.SysOrgs.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo < info.OrderNo).OrderByDescending(s => s.OrderNo).FirstOrDefault();
                if (prev != null)
                {
                    var orderNo = prev.OrderNo;
                    prev.OrderNo = info.OrderNo;
                    info.OrderNo = orderNo;

                    context.SysOrgs.Update(info);
                    context.SysOrgs.Update(prev);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 将机构向下移。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual bool MoveOrgDown(int orgId)
        {
            using (var context = new DbContext())
            {
                var signLength = EntityMetadataUnity.GetEntityMetadata(typeof(SysOrg)).EntityTree.SignLength;
                var info = context.SysOrgs.Get(orgId);
                var parentCode = info.Code.Left(info.Code.Length - signLength);
                var likeMatch = new string('_', signLength);

                var next = context.SysOrgs.Where(s => s.Code.Like(parentCode + likeMatch) && s.OrderNo > info.OrderNo).OrderBy(s => s.OrderNo).FirstOrDefault();
                if (next != null)
                {
                    var orderNo = next.OrderNo;
                    next.OrderNo = info.OrderNo;
                    info.OrderNo = orderNo;

                    context.SysOrgs.Update(info);
                    context.SysOrgs.Update(next);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 设置机构状态。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        public virtual void SetOrgState(int orgId, StateFlags state)
        {
            using (var context = new DbContext())
            {
                context.SysOrgs.Update(() => new SysOrg { State = state }, s => s.OrgID == orgId);
            }
        }

        /// <summary>
        /// 删除机构。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual bool DeleteOrg(int orgId)
        {
            using (var context = new DbContext())
            {
                var org = context.SysOrgs.Get(orgId);
                if (org.Code.Length == 2)
                {
                    throw new ClientNotificationException("不能删除顶级机构。");
                }

                return context.SysOrgs.Delete(org) > 0;
            }
        }
        #endregion

        #region 角色
        /// <summary>
        /// 获取指定的角色。
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public virtual SysRole GetRole(int roleId)
        {
            using (var context = new DbContext())
            {
                return context.SysRoles.Get(roleId);
            }
        }

        /// <summary>
        /// 保存机构信息。
        /// </summary>
        /// <param name="roleId">主键值。</param>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual int SaveRole(int? roleId, SysRole info)
        {
            using (var context = new DbContext())
            {
                if (context.SysRoles.Any(s => s.Name == info.Name && s.RoleID != roleId))
                {
                    throw new ClientNotificationException(string.Format("角色{0}已经存在，名称不能重复。", info.Name));
                }

                if (roleId == null)
                {
                    info.PyCode = info.Name.ToPinyin();
                    info.Code = GetNextCode();
                    context.SysRoles.Insert(info);
                    roleId = info.RoleID;
                }
                else
                {
                    context.SysRoles.Update(info, s => s.RoleID == roleId);
                }

                return (int)roleId;
            }
        }

        /// <summary>
        /// 保存多个角色。
        /// </summary>
        /// <param name="posts"></param>
        /// <returns></returns>
        public virtual bool SaveRoles(List<SysRole> posts)
        {
            using (var context = new DbContext())
            {
                var names = posts.Select(s => s.Name).ToArray();
                names = (context.SysRoles.Where(s => names.Contains(s.Name)).Select(s => s.Name)).ToArray();
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

                context.SysRoles.Batch(posts, (u, s) => u.Insert(s));

                return true;
            }
        }

        private string GetNextCode()
        {
            using (var context = new DbContext())
            {
                var post = context.SysRoles.OrderByDescending(s => s.Code).FirstOrDefault();
                if (post == null)
                {
                    return "0001";
                }

                return GetNextCode(post.Code, 4);
            }
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
        public virtual void SetRoleState(int roleId, StateFlags state)
        {
            using (var context = new DbContext())
            {
                context.SysRoles.Update(() => new SysRole { State = state }, s => s.RoleID == roleId);
            }
        }

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool DeleteRole(int id)
        {
            using (var context = new DbContext())
            {
                return context.SysRoles.Delete(id) > 0;
            }
        }

        /// <summary>
        /// 获取角色列表。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="keyword">关键字。</param>
        /// <param name="pager"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        public virtual List<SysRole> GetRoles(StateFlags? state, string keyword, DataPager pager, SortDefinition sorting)
        {
            using (var context = new DbContext())
            {
                return context.SysRoles
                    .AssertWhere(state != null, s => s.State == state)
                    .AssertWhere(!string.IsNullOrEmpty(keyword), s => s.Name.Contains(keyword) || s.PyCode.Contains(keyword))
                    .Select(s => s.ExtendAs<SysRole>(() => new SysRole
                    {
                        //AttributeName = s.Attribute.GetDescription()
                    }))
                    .Segment(pager)
                    .OrderBy(sorting)
                    .ToList();
            }
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
        public virtual List<SysModule> GetPurviewModules(int userId)
        {
            var result = new List<SysModule>();

            using (var context = new DbContext())
            {
                var user = context.SysUsers.Get(userId);
                var roleIds = context.SysUserRoles.Where(s => s.UserID == userId).Select(s => s.RoleID).Distinct();

                var list = context.SysModules.Where(s => s.State == StateFlags.Enabled)
                    //超级用户不用判断权限
                    .AssertWhere(user.Account != "admin", s => context.SysModulePermissions.Any(t => roleIds.Contains(t.RoleID) && t.ModuleID == s.ModuleID))
                    .ToList();

                Util.MakeChildren(result, list, string.Empty);
                return result;
            }
        }

        /// <summary>
        /// 根据架构ID和角色ID获取模块列表。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <returns></returns>
        public virtual List<SysModule> GetModulesByRole(int roleId)
        {
            var result = new List<SysModule>();

            if (roleId == 0)
            {
                return result;
            }

            using (var context = new DbContext())
            {
                var operates = context.SysOperates.ToList();
                var operatePermissions = context.SysOperatePermissions.Include(s => s.SysOperate).ToList();

                var list = context.SysModules.Where(s => s.State == StateFlags.Enabled)
                    .Select(s => s.ExtendAs<SysModule>(() => new SysModule
                    {
                        //判断是否此模块是否给定了角色权限
                        Permissible = context.SysModulePermissions
                                .Any(t => t.RoleID == roleId && t.ModuleID == s.ModuleID),
                        SysOperates = operates.Where(t => t.ModuleID == s.ModuleID).ToEntitySet()
                    }))
                    .ToList();

                list.ForEach(s =>
                {
                    s.SysOperates.ForEach(t => t.Permissible = operatePermissions
                        .Any(v => v.OperID == t.OperID && v.ModuleID == s.ModuleID && v.RoleID == roleId));
                });

                //CommonService.MakeChildren(result, list, string.Empty);

                operates.Clear();
                operatePermissions.Clear();

                return result;
            }
        }

        /// <summary>
        /// 根据角色ID获取机构列表。
        /// </summary>
        /// <param name="orgId">机构ID。</param>
        /// <param name="roleId">角色ID。</param>
        /// <returns></returns>
        public virtual List<SysOrg> GetOrgsByRole(int orgId, int roleId)
        {
            var result = new List<SysOrg>();

            if (orgId == 0 || roleId == 0)
            {
                return result;
            }

            using (var context = new DbContext())
            {
                var list = context.SysOrgs.Where(s => s.State == StateFlags.Enabled)
                    .Select(s => s.ExtendAs<SysOrg>(() => new SysOrg
                    {
                        //判断是否此机构是否给定了角色权限
                        Permissible = context.SysOrgPermissions.Any(t => t.RoleID == roleId && t.OrgID == orgId)
                    }))
                    .ToList();

                //CommonService.MakeChildren(result, list, string.Empty, 2);
                return result;
            }
        }

        /// <summary>
        /// 保存功能角色的权限，包括分配的用户。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="modules"></param>
        /// <param name="opers"></param>
        [TransactionSupport]
        public virtual void SaveFuncRolePermissions(int roleId, List<int> modules, Dictionary<int, List<int>> opers)
        {
            using (var context = new DbContext())
            {
                //清理数据
                context.SysModulePermissions.Delete(s => s.RoleID == roleId);
                context.SysOperatePermissions.Delete(s => s.RoleID == roleId);

                var permissions = modules.Select(s => new SysModulePermission
                {
                    RoleID = roleId,
                    ModuleID = s
                });

                context.SysModulePermissions.Batch(permissions, (u, s) => u.Insert(s));

                var operatePermissions = opers.SelectMany(s => s.Value.Select(t => new SysOperatePermission { ModuleID = s.Key, OperID = t, RoleID = roleId })).ToList();
                if (operatePermissions.Count > 0)
                {
                    context.SysOperatePermissions.Batch(operatePermissions, (u, s) => u.Insert(s));
                }
            }
        }

        /// <summary>
        /// 保存数据角色的权限，包括分配的用户。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="roleId"></param>
        /// <param name="orgs"></param>
        [TransactionSupport]
        public virtual void SaveOrgRolePermissions(int orgId, int roleId, List<int> orgs)
        {
            using (var context = new DbContext())
            {
                //清理数据
                context.SysOrgPermissions.Delete(s => s.OrgID == orgId && s.RoleID == roleId);

                var permissions = orgs.Select(s => new SysOrgPermission
                {
                    OrgID = orgId,
                    RoleID = roleId
                });

                context.SysOrgPermissions.Batch(permissions, (u, s) => u.Insert(s));
            }
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
            using (var context = new DbContext())
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
                    .Select(s => s.SysOperate)
                    .OrderBy(s => s.OrderNo)
                    .ToList();
            }
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
            using (var context = new DbContext())
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
) t ON m.OrgID = t.OrgID
AND m.RoleID = t.RoleID
JOIN SysOrg o ON o.OrgID = m.DataID
UNION ALL
	(
		SELECT
			o.code
		FROM
			SysUser u
		JOIN SysOrg o ON o.OrgID = u.OrgID
		WHERE
			u.UserID = {userId}
	)";

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
        }
        #endregion
    }
}
