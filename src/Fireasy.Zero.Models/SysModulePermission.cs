// **************************************
// 生成：CodeBuilder (http://www.fireasy.cn/codebuilder)
// 项目：Fireasy Zero
// 版权：Copyright Fireasy
// 作者：Huangxd
// 时间：10/12/2017 21:26:06
// **************************************
using System;
using Fireasy.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// 功能权限 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysModulePermission", Description = "功能权限")]
    [MetadataType(typeof(SysModulePermissionMetadata))]
    public partial class SysModulePermission : LightEntity<SysModulePermission>
    {
        /// <summary>
        /// 获取或设置角色ID。
        /// </summary>

        [PropertyMapping(ColumnName = "RoleID", Description = "角色ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int RoleID { get; set; }

        /// <summary>
        /// 获取或设置模块ID。
        /// </summary>

        [PropertyMapping(ColumnName = "ModuleID", Description = "模块ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int ModuleID { get; set; }

    }
	
    public class SysModulePermissionMetadata
    {
        /// <summary>
        /// 属性 RoleID 的验证特性。
        /// </summary>
        [Required]
        public object RoleID { get; set; }

        /// <summary>
        /// 属性 ModuleID 的验证特性。
        /// </summary>
        [Required]
        public object ModuleID { get; set; }

    }
}

