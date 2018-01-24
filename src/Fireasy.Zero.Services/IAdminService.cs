// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Models;
using System;
using System.Collections.Generic;

namespace Fireasy.Zero.Services
{
    /// <summary>
    /// 系统管理服务。
    /// </summary>
    public interface IAdminService
    {
        #region 用户
        /// <summary>
        /// 验证用户帐号和密码。
        /// </summary>
        /// <param name="account">用户帐号。</param>
        /// <param name="validator">密码验证器。</param>
        /// <param name="tokenCreator">token生成器。</param>
        /// <returns></returns>
        SessionContext CheckLogin(string account, Func<string, bool> validator, Func<SysUser, string> tokenCreator);

        /// <summary>
        /// 获取指定的用户。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        SysUser GetUser(int userId);

        /// <summary>
        /// 获取指定的用户名称。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserName(int userId);

        /// <summary>
        /// 根据帐号获取一个用户。
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        SysUser GetUserByAccount(string account);

        /// <summary>
        /// 保存用户信息。
        /// </summary>
        /// <param name="userId">主键值。</param>
        /// <param name="info"></param>
        /// <param name="pwdCreator"></param>
        /// <returns></returns>
        int SaveUser(int? userId, SysUser info, Func<string> pwdCreator);

        /// <summary>
        /// 保存用户信息。
        /// </summary>
        /// <param name="userId">主键值。</param>
        /// <param name="info"></param>
        /// <returns></returns>
        bool SaveUser(int userId, SysUser info);

        /// <summary>
        /// 保存多个用户。
        /// </summary>
        /// <param name="orgId">机构ID。</param>
        /// <param name="users"></param>
        /// <param name="pwdCreator"></param>
        /// <returns></returns>
        bool SaveUsers(int orgId, List<SysUser> users, Func<string> pwdCreator);

        /// <summary>
        /// 设置用户状态。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        void SetUserState(int userId, StateFlags state);

        /// <summary>
        /// 重设用户的密码。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="pwdCreator">密码生成器。</param>
        void ResetUserPassword(int userId, string password, Func<string> pwdCreator);

        /// <summary>
        /// 修改用户的密码。
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="validator">密码比较器。</param>
        /// <param name="pwdCreator">密码生成器。</param>
        bool ModifyUserPassword(int userId, Func<string, bool> validator, Func<string> pwdCreator);

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
        List<SysUser> GetUsers(int userId, string orgCode, StateFlags? state, string keyword, DataPager pager, SortDefinition sorting);

        /// <summary>
        /// 根据角色代码（拼音码）获取用户列表。
        /// </summary>
        /// <param name="orgCode">机构编码。</param>
        /// <param name="RoleName">角色名称，如果是多个角色，用竖线分隔。</param>
        /// <param name="keyword"></param>
        /// <param name="orgCodeLength">机构编码的位数。如果指定，则在此编码范围的机构下检索。比如分公司范围，则此值传 4。</param>
        /// <returns></returns>
        List<SysUser> GetUsers(string orgCode, string RoleName, string keyword, int? orgCodeLength);

        /// <summary>
        /// 获取机构下的用户列表。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        List<SysUser> GetUsers(int orgId);

        /// <summary>
        /// 获取机构下某角色的用户列表。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<SysUser> GetUsers(int orgId, int roleId);

        /// <summary>
        /// 获取机构下某角色的用户列表。
        /// </summary>
        /// <param name="orgCode"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<SysUser> GetUsers(string orgCode, int roleId);

        /// <summary>
        /// 获取用户人员信息列表 根据机构编码和状态
        /// </summary>
        /// <param name="orgCode">组织机构编码</param>
        /// <param name="state">状态 启用或 停用</param>
        /// <returns></returns>
        List<SysUser> GetUsersByCode(string orgCode, StateFlags? state);

        /// <summary>
        /// 删除用户。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool DeleteUser(int userId);

        #endregion

        #region 模块
        /// <summary>
        /// 获取模块信息。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        SysModule GetModule(int moduleId);

        /// <summary>
        /// 获取模块的下一个顺序号。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        int GetModuleNextOrderNo(int? moduleId);

        /// <summary>
        /// 保存模块。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        int SaveModule(int? moduleId, SysModule info);

        /// <summary>
        /// 获取模块列表。
        /// </summary>
        /// <param name="parentId">父节点ID。</param>
        /// <param name="targetId">目标节点ID，即要展开并选定的节点。</param>
        /// <param name="currentId">当前节点，在添加节点的时候要把当前节点排除。</param>
        /// <param name="state">状态。</param>
        /// <returns></returns>
        List<SysModule> GetModules(int? parentId, int? targetId, int? currentId, StateFlags? state);

        /// <summary>
        /// 按关键字搜索模块。
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        List<SysModule> SearchModules(string keyword);

        /// <summary>
        /// 删除模块。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        bool DeleteModule(int moduleId);

        /// <summary>
        /// 将模块向上移。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        bool MoveModuleUp(int moduleId);

        /// <summary>
        /// 将模块向下移。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        bool MoveModuleDown(int moduleId);

        /// <summary>
        /// 设置模块状态。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        void SetModuleState(int moduleId, StateFlags state);

        #endregion

        #region 操作
        /// <summary>
        /// 获取模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        List<SysOperate> GetOperates(int moduleId);

        /// <summary>
        /// 保存模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="rows"></param>
        void SaveOperates(int moduleId, List<SysOperate> rows);

        /// <summary>
        /// 保存模块的操作。
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="added"></param>
        /// <param name="updated"></param>
        /// <param name="deleted"></param>
        void SaveOperates(int moduleId, List<SysOperate> added, List<SysOperate> updated, List<SysOperate> deleted);

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
        List<SysOrg> GetOrgs(int? parentId, int? targetId, int? currentId, StateFlags? state, string keyword, OrgAttribute? attribute);

        /// <summary>
        /// 获取机构列表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyword"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        List<SysOrg> GetOrgs(int userId, string keyword, OrgAttribute? attribute);

        /// <summary>
        /// 获取机构信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        SysOrg GetOrg(int orgId);

        /// <summary>
        /// 获取机构的下一个顺序号。
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        int GetOrgNextOrderNo(int? parentId);

        /// <summary>
        /// 保存机构。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        int SaveOrg(int? orgId, SysOrg info);

        /// <summary>
        /// 保存多个机构。
        /// </summary>
        /// <param name="parentId">父机构ID。</param>
        /// <param name="orgs"></param>
        /// <returns></returns>
        bool SaveOrgs(int? parentId, List<SysOrg> orgs);

        /// <summary>
        /// 按关键字搜索机构。
        /// </summary>
        /// <param name="keyword">关键字。</param>
        /// <returns></returns>
        List<SysOrg> SearchOrgs(string keyword);

        /// <summary>
        /// 将机构向上移。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        bool MoveOrgUp(int orgId);

        /// <summary>
        /// 将机构向下移。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        bool MoveOrgDown(int orgId);

        /// <summary>
        /// 设置机构状态。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        void SetOrgState(int orgId, StateFlags state);

        /// <summary>
        /// 删除机构。
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        bool DeleteOrg(int orgId);

        #endregion

        #region 角色

        /// <summary>
        /// 获取指定的角色。
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        SysRole GetRole(int roleId);

        /// <summary>
        /// 保存机构信息。
        /// </summary>
        /// <param name="roleId">主键值。</param>
        /// <param name="info"></param>
        /// <returns></returns>
        int SaveRole(int? roleId, SysRole info);

        /// <summary>
        /// 保存多个角色。
        /// </summary>
        /// <param name="Roles"></param>
        /// <returns></returns>
        bool SaveRoles(List<SysRole> Roles);

        /// <summary>
        /// 设置角色状态。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="state">是否启用，反之禁用。</param>
        void SetRoleState(int roleId, StateFlags state);

        /// <summary>
        /// 删除角色。
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        bool DeleteRole(int roleId);

        /// <summary>
        /// 获取角色列表。
        /// </summary>
        /// <param name="state">状态。</param>
        /// <param name="keyword">关键字。</param>
        /// <param name="pager"></param>
        /// <param name="sorting"></param>
        /// <returns></returns>
        List<SysRole> GetRoles(StateFlags? state, string keyword, DataPager pager, SortDefinition sorting);
        #endregion

        #region 权限
        /// <summary>
        /// 获取用户具有的操作模块。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <returns></returns>
        List<SysModule> GetPurviewModules(int userId);

        /// <summary>
        /// 根据角色ID获取模块列表。
        /// </summary>
        /// <param name="roleId">角色ID。</param>
        /// <returns></returns>
        List<SysModule> GetModulesByRole(int roleId);

        /// <summary>
        /// 根据机构ID和角色ID获取机构列表。
        /// </summary>
        /// <param name="orgId">机构ID。</param>
        /// <param name="roleId">角色ID。</param>
        /// <returns></returns>
        List<SysOrg> GetOrgsByRole(int orgId, int roleId);

        /// <summary>
        /// 保存功能角色的权限，包括分配的用户。
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="modules"></param>
        /// <param name="opers"></param>
        void SaveFuncRolePermissions(int roleId, List<int> modules, Dictionary<int, List<int>> opers);

        /// <summary>
        /// 保存数据角色的权限，包括分配的用户。
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="roleId"></param>
        /// <param name="orgs"></param>
        void SaveOrgRolePermissions(int orgId, int roleId, List<int> orgs);

        /// <summary>
        /// 根据 Url 获取操作按钮。
        /// </summary>
        /// <param name="userId">用户ID。</param>
        /// <param name="moduleUrl">模块的URL。</param>
        /// <returns></returns>
        List<SysOperate> GetPurviewOperates(int usrId, string moduleUrl);

        /// <summary>
        /// 获取用户具有的数据权限（机构编码）。
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<string> GetPurviewOrgs(int userId);
        #endregion
    }
}
