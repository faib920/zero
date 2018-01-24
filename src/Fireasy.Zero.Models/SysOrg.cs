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
using Fireasy.Data.Entity.Validation;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// 机构 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysOrg", Description = "机构")]
    [MetadataType(typeof(SysOrgMetadata))]
    public partial class SysOrg : LightEntity<SysOrg>
    {
        /// <summary>
        /// 获取或设置机构ID。
        /// </summary>

        [PropertyMapping(ColumnName = "OrgID", Description = "机构ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int OrgID { get; set; }

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
        /// 获取或设置全称。
        /// </summary>

        [PropertyMapping(ColumnName = "FullName", Description = "全称", Length = 500, IsNullable = true)]
        public virtual string FullName { get; set; }

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
        /// 获取或设置拼音码。
        /// </summary>

        [PropertyMapping(ColumnName = "PyCode", Description = "拼音码", Length = 100, IsNullable = true)]
        public virtual string PyCode { get; set; }

        /// <summary>
        /// 获取或设置属性(0:机构,1:部门)。
        /// </summary>

        [PropertyMapping(ColumnName = "Attribute", Description = "属性(0:机构,1:部门)", IsNullable = false)]
        public virtual OrgAttribute Attribute { get; set; }

    }
	
    public class SysOrgMetadata
    {
        /// <summary>
        /// 属性 OrgID 的验证特性。
        /// </summary>
        [Required]
        public object OrgID { get; set; }

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
        /// 属性 FullName 的验证特性。
        /// </summary>
        [StringLength(500)]
        public object FullName { get; set; }

        /// <summary>
        /// 属性 State 的验证特性。
        /// </summary>
        [Required]
        public object State { get; set; }

        /// <summary>
        /// 属性 OrderNo 的验证特性。
        /// </summary>
        public object OrderNo { get; set; }

        /// <summary>
        /// 属性 PyCode 的验证特性。
        /// </summary>
        [StringLength(100)]
        public object PyCode { get; set; }

        /// <summary>
        /// 属性 Attribute 的验证特性。
        /// </summary>
        [Required]
        public object Attribute { get; set; }

    }
}

