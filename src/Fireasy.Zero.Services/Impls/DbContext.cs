// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Entity;
using Fireasy.Zero.Models;

namespace Fireasy.Zero.Services.Impls
{
    internal class DbContext : EntityContext
    {
        /// <summary>
        /// 获取或设置 数据权限 实体仓储。
        /// </summary> 
        public EntityRepository<SysOrgPermission> SysOrgPermissions { get; set; }
        /// <summary>
        /// 获取或设置 公共字典项 实体仓储。
        /// </summary> 
        public EntityRepository<SysDictItem> SysDictItems { get; set; }
        /// <summary>
        /// 获取或设置 公共字典类别 实体仓储。
        /// </summary> 
        public EntityRepository<SysDictType> SysDictTypes { get; set; }
        /// <summary>
        /// 获取或设置 系统日志 实体仓储。
        /// </summary> 
        public EntityRepository<SysLog> SysLogs { get; set; }
        /// <summary>
        /// 获取或设置 功能模块 实体仓储。
        /// </summary> 
        public EntityRepository<SysModule> SysModules { get; set; }
        /// <summary>
        /// 获取或设置 功能权限 实体仓储。
        /// </summary> 
        public EntityRepository<SysModulePermission> SysModulePermissions { get; set; }
        /// <summary>
        /// 获取或设置 操作 实体仓储。
        /// </summary> 
        public EntityRepository<SysOperate> SysOperates { get; set; }
        /// <summary>
        /// 获取或设置 操作权限 实体仓储。
        /// </summary> 
        public EntityRepository<SysOperatePermission> SysOperatePermissions { get; set; }
        /// <summary>
        /// 获取或设置 机构 实体仓储。
        /// </summary> 
        public EntityRepository<SysOrg> SysOrgs { get; set; }
        /// <summary>
        /// 获取或设置 角色 实体仓储。
        /// </summary> 
        public EntityRepository<SysRole> SysRoles { get; set; }
        /// <summary>
        /// 获取或设置 人员 实体仓储。
        /// </summary> 
        public EntityRepository<SysUser> SysUsers { get; set; }
        /// <summary>
        /// 获取或设置 人员角色 实体仓储。
        /// </summary> 
        public EntityRepository<SysUserRole> SysUserRoles { get; set; }
    }
}
