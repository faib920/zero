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
    /// 公共字典类别 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysDictType", Description = "公共字典类别")]
    [MetadataType(typeof(SysDictTypeMetadata))]
    public partial class SysDictType : LightEntity<SysDictType>
    {
        /// <summary>
        /// 获取或设置类别ID。
        /// </summary>

        [PropertyMapping(ColumnName = "TypeID", Description = "类别ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
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
        /// 获取或设置是否是树结构。
        /// </summary>

        [PropertyMapping(ColumnName = "IsTree", Description = "是否是树结构", IsNullable = false)]
        public virtual bool IsTree { get; set; }

        /// <summary>
        /// 获取或设置编码长度。
        /// </summary>

        [PropertyMapping(ColumnName = "CodeLength", Description = "编码长度", IsNullable = true)]
        public virtual int? CodeLength { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "状态", IsNullable = false)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// 获取或设置OrderNo字典类别分类表主键。
        /// </summary>

        [PropertyMapping(ColumnName = "OrderNo", Description = "排序字段", IsNullable = true)]
        public virtual decimal OrderNo { get; set; }

        /// <summary>
        /// 获取或设置 <see cref="SysDictItem"/> 的子实体集。
        /// </summary>
        public virtual EntitySet<SysDictItem> SysDictItems { get; set; }

    }
	
    public class SysDictTypeMetadata
    {

        /// <summary>
        /// 属性 TypeID 的验证特性。
        /// </summary>
        [Required]
        public object TypeID { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public object OrderNo { get; set; }

        /// <summary>
        /// 属性 Code 的验证特性。
        /// </summary>
        [StringLength(50)]
        [Required]
        public object Code { get; set; }

        /// <summary>
        /// 属性 Name 的验证特性。
        /// </summary>
        [Required]
        [StringLength(100)]
        public object Name { get; set; }

        /// <summary>
        /// 属性 IsTree 的验证特性。
        /// </summary>
        [Required]
        public object IsTree { get; set; }

        /// <summary>
        /// 属性 CodeLength 的验证特性。
        /// </summary>
        public object CodeLength { get; set; }

        /// <summary>
        /// 属性 State 的验证特性。
        /// </summary>
        [Required]
        public object State { get; set; }

    }
}

