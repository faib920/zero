// **************************************
// 生成：CodeBuilder (http://www.fireasy.cn/codebuilder)
// 项目：Fireasy Zero
// 版权：Copyright Fireasy
// 作者：Huangxd
// 时间：10/12/2017 21:26:11
// **************************************
using System;
using Fireasy.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// 人员角色 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysUserRole", Description = "人员角色")]
    [MetadataType(typeof(SysUserRoleMetadata))]
    public partial class SysUserRole : LightEntity<SysUserRole>
    {
        /// <summary>
        /// 获取或设置人员ID。
        /// </summary>

        [PropertyMapping(ColumnName = "UserID", Description = "人员ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int UserID { get; set; }

        /// <summary>
        /// 获取或设置角色ID。
        /// </summary>

        [PropertyMapping(ColumnName = "RoleID", Description = "角色ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int RoleID { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysUser"/> 对象。
        /// </summary>
        public virtual SysUser SysUser { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysRole"/> 对象。
        /// </summary>
        public virtual SysRole SysRole { get; set; }

    }
	
    public class SysUserRoleMetadata
    {
        /// <summary>
        /// 属性 UserID 的验证特性。
        /// </summary>
        [Required]
        public object UserID { get; set; }

        /// <summary>
        /// 属性 RoleID 的验证特性。
        /// </summary>
        [Required]
        public object RoleID { get; set; }

    }
}

