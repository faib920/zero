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
    /// 操作权限 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysOperatePermission", Description = "操作权限")]
    [MetadataType(typeof(SysOperatePermissionMetadata))]
    public partial class SysOperatePermission : LightEntity<SysOperatePermission>
    {
        /// <summary>
        /// 获取或设置模块ID。
        /// </summary>

        [PropertyMapping(ColumnName = "ModuleID", Description = "模块ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int ModuleID { get; set; }

        /// <summary>
        /// 获取或设置操作ID。
        /// </summary>

        [PropertyMapping(ColumnName = "OperID", Description = "操作ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int OperID { get; set; }

        /// <summary>
        /// 获取或设置角色ID。
        /// </summary>

        [PropertyMapping(ColumnName = "RoleID", Description = "角色ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int RoleID { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysOperate"/> 对象。
        /// </summary>
        public virtual SysOperate SysOperate { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysModule"/> 对象。
        /// </summary>
        public virtual SysModule SysModule { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysRole"/> 对象。
        /// </summary>
        public virtual SysRole SysRole { get; set; }

    }
	
    public class SysOperatePermissionMetadata
    {
        /// <summary>
        /// 属性 ModuleID 的验证特性。
        /// </summary>
        [Required]
        public object ModuleID { get; set; }

        /// <summary>
        /// 属性 OperID 的验证特性。
        /// </summary>
        [Required]
        public object OperID { get; set; }

        /// <summary>
        /// 属性 RoleID 的验证特性。
        /// </summary>
        [Required]
        public object RoleID { get; set; }

    }
}

