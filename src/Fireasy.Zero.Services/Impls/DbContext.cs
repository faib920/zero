﻿// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Data.Entity;
using Fireasy.Zero.Models;
using System;

namespace Fireasy.Zero.Services.Impls
{
    public class DbContext : EntityContext
    {
        public DbContext(IServiceProvider serviceProvider)
            : base (serviceProvider)
        {
        }

        /// <summary>
        /// 获取或设置 数据权限 实体仓储。
        /// </summary> 
        public IRepository<SysOrgPermission> SysOrgPermissions { get; set; }
        /// <summary>
        /// 获取或设置 公共字典项 实体仓储。
        /// </summary> 
        public IRepository<SysDictItem> SysDictItems { get; set; }
        /// <summary>
        /// 获取或设置 公共字典类别 实体仓储。
        /// </summary> 
        public IRepository<SysDictType> SysDictTypes { get; set; }
        /// <summary>
        /// 获取或设置 系统日志 实体仓储。
        /// </summary> 
        public IRepository<SysLog> SysLogs { get; set; }
        /// <summary>
        /// 获取或设置 功能模块 实体仓储。
        /// </summary> 
        public IRepository<SysModule> SysModules { get; set; }
        /// <summary>
        /// 获取或设置 功能权限 实体仓储。
        /// </summary> 
        public IRepository<SysModulePermission> SysModulePermissions { get; set; }
        /// <summary>
        /// 获取或设置 操作 实体仓储。
        /// </summary> 
        public IRepository<SysOperate> SysOperates { get; set; }
        /// <summary>
        /// 获取或设置 操作权限 实体仓储。
        /// </summary> 
        public IRepository<SysOperatePermission> SysOperatePermissions { get; set; }
        /// <summary>
        /// 获取或设置 机构 实体仓储。
        /// </summary> 
        public IRepository<SysOrg> SysOrgs { get; set; }
        /// <summary>
        /// 获取或设置 角色 实体仓储。
        /// </summary> 
        public IRepository<SysRole> SysRoles { get; set; }
        /// <summary>
        /// 获取或设置 人员 实体仓储。
        /// </summary> 
        public IRepository<SysUser> SysUsers { get; set; }
        /// <summary>
        /// 获取或设置 人员角色 实体仓储。
        /// </summary> 
        public IRepository<SysUserRole> SysUserRoles { get; set; }
    }
}
