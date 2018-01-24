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
using Fireasy.Data.Entity.Validation;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// 人员 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysUser", Description = "人员")]
    [MetadataType(typeof(SysUserMetadata))]
    public partial class SysUser : LightEntity<SysUser>
    {
        /// <summary>
        /// 获取或设置人员ID。
        /// </summary>

        [PropertyMapping(ColumnName = "UserID", Description = "人员ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int UserID { get; set; }

        /// <summary>
        /// 获取或设置机构ID。
        /// </summary>

        [PropertyMapping(ColumnName = "OrgID", Description = "机构ID", IsNullable = false)]
        public virtual int OrgID { get; set; }

        /// <summary>
        /// 获取或设置姓名。
        /// </summary>

        [PropertyMapping(ColumnName = "Name", Description = "姓名", Length = 20, IsNullable = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// 获取或设置账号。
        /// </summary>

        [PropertyMapping(ColumnName = "Account", Description = "账号", Length = 50, IsNullable = true)]
        public virtual string Account { get; set; }

        /// <summary>
        /// 获取或设置角色名称。
        /// </summary>

        [PropertyMapping(ColumnName = "RoleNames", Description = "角色名称", Length = 100, IsNullable = true)]
        public virtual string RoleNames { get; set; }

        /// <summary>
        /// 获取或设置密码。
        /// </summary>

        [PropertyMapping(ColumnName = "Password", Description = "密码", Length = 50, IsNullable = true)]
        public virtual string Password { get; set; }

        /// <summary>
        /// 获取或设置手机号。
        /// </summary>

        [PropertyMapping(ColumnName = "Mobile", Description = "手机号", Length = 20, IsNullable = true)]
        public virtual string Mobile { get; set; }

        /// <summary>
        /// 获取或设置邮箱。
        /// </summary>

        [PropertyMapping(ColumnName = "Email", Description = "邮箱", Length = 50, IsNullable = true)]
        public virtual string Email { get; set; }

        /// <summary>
        /// 获取或设置性别。
        /// </summary>

        [PropertyMapping(ColumnName = "Sex", Description = "性别", IsNullable = true)]
        public virtual Sex Sex { get; set; }

        /// <summary>
        /// 获取或设置拼音码。
        /// </summary>

        [PropertyMapping(ColumnName = "PyCode", Description = "拼音码", Length = 20, IsNullable = true)]
        public virtual string PyCode { get; set; }

        /// <summary>
        /// 获取或设置状态。
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "状态", IsNullable = true)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// 获取或设置最近登录时间。
        /// </summary>

        [PropertyMapping(ColumnName = "LastLoginTime", Description = "最近登录时间", IsNullable = true)]
        public virtual DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 获取或设置令牌。
        /// </summary>

        [PropertyMapping(ColumnName = "Token", Description = "令牌", Length = 100, IsNullable = true)]
        public virtual string Token { get; set; }

        /// <summary>
        /// 获取或设置设备号。
        /// </summary>

        [PropertyMapping(ColumnName = "DeviceNo", Description = "设备号", Length = 100, IsNullable = true)]
        public virtual string DeviceNo { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysOrg"/> 对象。
        /// </summary>
        public virtual SysOrg SysOrg { get; set; }
    }
	
    public class SysUserMetadata
    {
        /// <summary>
        /// 属性 UserID 的验证特性。
        /// </summary>
        [Required]
        public object UserID { get; set; }

        /// <summary>
        /// 属性 OrgID 的验证特性。
        /// </summary>
        [Required]
        public object OrgID { get; set; }

        /// <summary>
        /// 属性 Name 的验证特性。
        /// </summary>
        [StringLength(20)]
        [Required]
        public object Name { get; set; }

        /// <summary>
        /// 属性 Account 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Account { get; set; }

        /// <summary>
        /// 属性 RoleNames 的验证特性。
        /// </summary>
        [StringLength(100)]
        public object RoleNames { get; set; }

        /// <summary>
        /// 属性 Password 的验证特性。
        /// </summary>
        [StringLength(50)]
        public object Password { get; set; }

        /// <summary>
        /// 属性 Mobile 的验证特性。
        /// </summary>
        [StringLength(20)]
        [Required]
        [Mobile]
        public object Mobile { get; set; }

        /// <summary>
        /// 属性 Email 的验证特性。
        /// </summary>
        [StringLength(50)]
        [Email]
        public object Email { get; set; }

        /// <summary>
        /// 属性 Sex 的验证特性。
        /// </summary>
        [Required]
        public object Sex { get; set; }

        /// <summary>
        /// 属性 PyCode 的验证特性。
        /// </summary>
        [StringLength(20)]
        public object PyCode { get; set; }

        /// <summary>
        /// 属性 State 的验证特性。
        /// </summary>
        public object State { get; set; }

        /// <summary>
        /// 属性 LastLoginTime 的验证特性。
        /// </summary>
        public object LastLoginTime { get; set; }

        /// <summary>
        /// 属性 Token 的验证特性。
        /// </summary>
        [StringLength(200)]
        public object Token { get; set; }

        /// <summary>
        /// 属性 DeviceNo 的验证特性。
        /// </summary>
        [StringLength(100)]
        public object DeviceNo { get; set; }

    }
}

