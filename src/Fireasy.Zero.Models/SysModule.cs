// **************************************
// 生成：CodeBuilder (http://www.fireasy.cn/codebuilder)
// 项目：Fireasy Zero
// 版权：Copyright Fireasy
// 作者：Huangxd
// 时间：10/12/2017 21:26:05
// **************************************
using System;
using Fireasy.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// 功能模块 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysModule", Description = "功能模块")]
    [MetadataType(typeof(SysModuleMetadata))]
    public partial class SysModule : LightEntity<SysModule>
    {
        /// <summary>
        /// 获取或设置模块ID。
        /// </summary>

        [PropertyMapping(ColumnName = "ModuleID", Description = "模块ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int ModuleID { get; set; }

        /// <summary>
        /// 获取或设置名称。
        /// </summary>

        [PropertyMapping(ColumnName = "Name", Description = "名称", Length = 100, IsNullable = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 获取或设置编码。
        /// </summary>

        [PropertyMapping(ColumnName = "Code", Description = "编码", Length = 50, IsNullable = true)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 获取或设置地址。
        /// </summary>

        [PropertyMapping(ColumnName = "Url", Description = "地址", Length = 100, IsNullable = true)]
        public virtual string Url { get; set; }

        /// <summary>
        /// 获取或设置参数。
        /// </summary>

        [PropertyMapping(ColumnName = "Arguments", Description = "参数", Length = 50, IsNullable = true)]
        public virtual string Arguments { get; set; }

        /// <summary>
        /// 获取或设置图标1。
        /// </summary>

        [PropertyMapping(ColumnName = "Icon1", Description = "图标1", Length = 50, IsNullable = true)]
        public virtual string Icon1 { get; set; }

        /// <summary>
        /// 获取或设置图标2。
        /// </summary>

        [PropertyMapping(ColumnName = "Icon2", Description = "图标2", Length = 50, IsNullable = true)]
        public virtual string Icon2 { get; set; }

        /// <summary>
        /// 获取或设置图标2。
        /// </summary>

        [PropertyMapping(ColumnName = "Icon3", Description = "图标2", Length = 50, IsNullable = true)]
        public virtual string Icon3 { get; set; }

        /// <summary>
        /// 获取或设置拼音码。
        /// </summary>

        [PropertyMapping(ColumnName = "PyCode", Description = "拼音码", Length = 50, IsNullable = true)]
        public virtual string PyCode { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "状态", IsNullable = false)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// 获取或设置排序。
        /// </summary>

        [PropertyMapping(ColumnName = "OrderNo", Description = "排序", IsNullable = true)]
        public virtual int OrderNo { get; set; }

        /// <summary>
        /// 获取或设置备注。
        /// </summary>

        [PropertyMapping(ColumnName = "Remark", Description = "备注", Length = 500, IsNullable = true)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysOperate"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysOperate> SysOperates { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysModulePermission"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysModulePermission> SysModulePermissions { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysOperatePermission"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysOperatePermission> SysOperatePermissions { get; set; }

    }
	
    public class SysModuleMetadata
    {
        /// <summary>
        /// 属性 ModuleID 的验证特性。
        /// </summary>
        [Required]
        public object ModuleID { get; set; }

        /// <summary>
        /// 属性 Name 的验证特性。
        /// </summary>
        [StringLength(100)]
        [Required]
        public object Name { get; set; }

        /// <summary>
        /// 属性 Code 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Code { get; set; }

        /// <summary>
        /// 属性 Url 的验证特性。
        /// </summary>
        [StringLength(100)]
        public object Url { get; set; }

        /// <summary>
        /// 属性 Arguments 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Arguments { get; set; }

        /// <summary>
        /// 属性 Icon1 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Icon1 { get; set; }

        /// <summary>
        /// 属性 Icon2 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Icon2 { get; set; }

        /// <summary>
        /// 属性 Icon3 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Icon3 { get; set; }

        /// <summary>
        /// 属性 PyCode 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object PyCode { get; set; }

        /// <summary>
        /// 属性 State 的验证特性。
        /// </summary>
        [Required]
        public object State { get; set; }

        /// <summary>
        /// 属性 OrderNo 的验证特性。
        /// </summary>
        [Required]
        public object OrderNo { get; set; }

        /// <summary>
        /// 属性 Remark 的验证特性。
        /// </summary>
        [StringLength(500)]
        public object Remark { get; set; }

    }
}

