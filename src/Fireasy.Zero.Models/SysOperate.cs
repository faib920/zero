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
    /// 操作 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysOperate", Description = "操作")]
    [MetadataType(typeof(SysOperateMetadata))]
    public partial class SysOperate : LightEntity<SysOperate>
    {
        /// <summary>
        /// 获取或设置操作ID。
        /// </summary>

        [PropertyMapping(ColumnName = "OperID", Description = "操作ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int OperID { get; set; }

        /// <summary>
        /// 获取或设置模块ID。
        /// </summary>

        [PropertyMapping(ColumnName = "ModuleID", Description = "模块ID", IsNullable = false)]
        public virtual int ModuleID { get; set; }

        /// <summary>
        /// 获取或设置编码。
        /// </summary>

        [PropertyMapping(ColumnName = "Code", Description = "编码", Length = 20, IsNullable = true)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 获取或设置名称。
        /// </summary>

        [PropertyMapping(ColumnName = "Name", Description = "名称", Length = 20, IsNullable = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 获取或设置图标。
        /// </summary>

        [PropertyMapping(ColumnName = "Icon", Description = "图标", Length = 50, IsNullable = true)]
        public virtual string Icon { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "状态", IsNullable = false)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// 获取或设置排序。
        /// </summary>

        [PropertyMapping(ColumnName = "OrderNo", Description = "排序", IsNullable = false)]
        public virtual int OrderNo { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysModule"/> 对象。
        /// </summary>
        public virtual SysModule SysModule { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysOperatePermission"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysOperatePermission> SysOperatePermissions { get; set; }

    }
	
    public class SysOperateMetadata
    {
        /// <summary>
        /// 属性 OperID 的验证特性。
        /// </summary>
        [Required]
        public object OperID { get; set; }

        /// <summary>
        /// 属性 ModuleID 的验证特性。
        /// </summary>
        [Required]
        public object ModuleID { get; set; }

        /// <summary>
        /// 属性 Code 的验证特性。
        /// </summary>
        [StringLength(20)]
        [Required]
        public object Code { get; set; }

        /// <summary>
        /// 属性 Name 的验证特性。
        /// </summary>
        [StringLength(20)]
        [Required]
        public object Name { get; set; }

        /// <summary>
        /// 属性 Icon 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Icon { get; set; }

        /// <summary>
        /// 属性 State 的验证特性。
        /// </summary>
        [Required]
        public object State { get; set; }

    }
}

