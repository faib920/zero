// **************************************
// 生成：CodeBuilder (http://www.fireasy.cn/codebuilder)
// 项目：Fireasy Zero
// 版权：Copyright Fireasy
// 作者：Huangxd
// 时间：10/12/2017 21:26:08
// **************************************
using System;
using Fireasy.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// 角色 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysRole", Description = "角色")]
    [MetadataType(typeof(SysRoleMetadata))]
    public partial class SysRole : LightEntity<SysRole>
    {
        /// <summary>
        /// 获取或设置角色ID。
        /// </summary>

        [PropertyMapping(ColumnName = "RoleID", Description = "角色ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int RoleID { get; set; }

        /// <summary>
        /// 获取或设置编码。
        /// </summary>

        [PropertyMapping(ColumnName = "Code", Description = "编码", Length = 20, IsNullable = true)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 获取或设置名称。
        /// </summary>

        [PropertyMapping(ColumnName = "Name", Description = "名称", Length = 50, IsNullable = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "状态", IsNullable = false)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// 获取或设置属性。
        /// </summary>

        [PropertyMapping(ColumnName = "Attribute", Description = "属性", IsNullable = false)]
        public virtual int Attribute { get; set; }

        /// <summary>
        /// 获取或设置拼音码。
        /// </summary>

        [PropertyMapping(ColumnName = "PyCode", Description = "拼音码", Length = 50, IsNullable = true)]
        public virtual string PyCode { get; set; }

        /// <summary>
        /// 获取或设置备注。
        /// </summary>

        [PropertyMapping(ColumnName = "Remark", Description = "备注", Length = 500, IsNullable = true)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysUserRole"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysUserRole> SysUserRoles { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysModulePermission"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysModulePermission> SysModulePermissions { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysOperatePermission"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysOperatePermission> SysOperatePermissions { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysOrgPermission"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysOrgPermission> SysDataPermissions { get; set; }

    }
	
    public class SysRoleMetadata
    {
        /// <summary>
        /// 属性 RoleID 的验证特性。
        /// </summary>
        [Required]
        public object RoleID { get; set; }

        /// <summary>
        /// 属性 Code 的验证特性。
        /// </summary>
        [StringLength(20)]
        public object Code { get; set; }

        /// <summary>
        /// 属性 Name 的验证特性。
        /// </summary>
        [StringLength(50)]
        [Required]
        public object Name { get; set; }

        /// <summary>
        /// 属性 State 的验证特性。
        /// </summary>
        [Required]
        public object State { get; set; }

        /// <summary>
        /// 属性 Attribute 的验证特性。
        /// </summary>
        [Required]
        public object Attribute { get; set; }

        /// <summary>
        /// 属性 PyCode 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object PyCode { get; set; }

        /// <summary>
        /// 属性 Remark 的验证特性。
        /// </summary>
        [StringLength(500)]
        public object Remark { get; set; }

    }
}

