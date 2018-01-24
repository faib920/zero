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
    /// 公共字典项 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysDictItem", Description = "公共字典项")]
    [MetadataType(typeof(SysDictItemMetadata))]
    public partial class SysDictItem : LightEntity<SysDictItem>
    {
        /// <summary>
        /// 获取或设置字典项ID。
        /// </summary>

        [PropertyMapping(ColumnName = "ItemID", Description = "字典项ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int ItemID { get; set; }

        /// <summary>
        /// 获取或设置类别ID。
        /// </summary>

        [PropertyMapping(ColumnName = "TypeID", Description = "类别ID", IsNullable = false)]
        public virtual int TypeID { get; set; }

        /// <summary>
        /// 获取或设置编码。
        /// </summary>

        [PropertyMapping(ColumnName = "Code", Description = "编码", Length = 50, IsNullable = true)]
        public virtual string Code { get; set; }


        /// <summary>
        /// 获取或设置名称。
        /// </summary>

        [PropertyMapping(ColumnName = "Name", Description = "名称", Length = 100, IsNullable = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 获取或设置拼音码。
        /// </summary>

        [PropertyMapping(ColumnName = "PyCode", Description = "拼音码", Length = 100, IsNullable = true)]
        public virtual string PyCode { get; set; }

        /// <summary>
        /// 获取或设置值。
        /// </summary>

        [PropertyMapping(ColumnName = "Value", Description = "值", IsNullable = true)]
        public virtual int? Value { get; set; }

        /// <summary>
        /// 获取或设置排序。
        /// </summary>

        [PropertyMapping(ColumnName = "OrderNo", Description = "排序", IsNullable = true)]
        public virtual int OrderNo { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "状态", IsNullable = false)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysDictType"/> 对象。
        /// </summary>
        public virtual SysDictType SysDictType { get; set; }

    }
	
    public class SysDictItemMetadata
    {
        /// <summary>
        /// 属性 ItemID 的验证特性。
        /// </summary>
        [Required]
        public object ItemID { get; set; }

        /// <summary>
        /// 属性 TypeID 的验证特性。
        /// </summary>
        [Required]
        public object TypeID { get; set; }

        /// <summary>
        /// 属性 Code 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Code { get; set; }

        /// <summary>
        /// 属性 Name 的验证特性。
        /// </summary>
        [StringLength(100)]
        [Required]
        public object Name { get; set; }

        /// <summary>
        /// 属性 PyCode 的拼音码
        /// </summary>
        [StringLength(100)]
        public object PyCode { get; set; }

        /// <summary>
        /// 属性 Value 的验证特性。
        /// </summary>
        [Required]
        public object Value { get; set; }

        /// <summary>
        /// 属性 OrderNo 的验证特性。
        /// </summary>
        public object OrderNo { get; set; }

        /// <summary>
        /// 属性 State 的验证特性。
        /// </summary>
        [Required]
        public object State { get; set; }

    }
}

