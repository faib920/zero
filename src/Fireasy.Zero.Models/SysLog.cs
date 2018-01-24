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
    /// 系统日志 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysLog", Description = "系统日志")]
    [MetadataType(typeof(SysLogMetadata))]
    public partial class SysLog : LightEntity<SysLog>
    {
        /// <summary>
        /// 获取或设置日志ID。
        /// </summary>

        [PropertyMapping(ColumnName = "LogID", Description = "日志ID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int LogID { get; set; }

        /// <summary>
        /// 获取或设置日志类型。
        /// </summary>

        [PropertyMapping(ColumnName = "LogType", Description = "日志类型", IsNullable = false)]
        public virtual int LogType { get; set; }

        /// <summary>
        /// 获取或设置标题。
        /// </summary>

        [PropertyMapping(ColumnName = "Title", Description = "标题", Length = 200, IsNullable = true)]
        public virtual string Title { get; set; }

        /// <summary>
        /// 获取或设置内容。
        /// </summary>

        [PropertyMapping(ColumnName = "Content", Description = "内容", IsNullable = true)]
        public virtual string Content { get; set; }

        /// <summary>
        /// 获取或设置机构ID。
        /// </summary>

        [PropertyMapping(ColumnName = "OrgID", Description = "机构ID", IsNullable = false)]
        public virtual int OrgID { get; set; }

        /// <summary>
        /// 获取或设置操作人ID。
        /// </summary>

        [PropertyMapping(ColumnName = "OperateUserID", Description = "操作人ID", IsNullable = false)]
        public virtual int OperateUserID { get; set; }

        /// <summary>
        /// 获取或设置操作人姓名。
        /// </summary>

        [PropertyMapping(ColumnName = "OperateUserName", Description = "操作人姓名", Length = 20, IsNullable = true)]
        public virtual string OperateUserName { get; set; }

        /// <summary>
        /// 获取或设置记录日期。
        /// </summary>

        [PropertyMapping(ColumnName = "LogTime", Description = "记录日期", IsNullable = true)]
        public virtual DateTime? LogTime { get; set; }

        /// <summary>
        /// 获取或设置关联 <see cref="SysOrg"/> 对象。
        /// </summary>
        public virtual SysOrg SysOrg { get; set; }

    }
	
    public class SysLogMetadata
    {
        /// <summary>
        /// 属性 LogID 的验证特性。
        /// </summary>
        [Required]
        public object LogID { get; set; }

        /// <summary>
        /// 属性 LogType 的验证特性。
        /// </summary>
        [Required]
        public object LogType { get; set; }

        /// <summary>
        /// 属性 Title 的验证特性。
        /// </summary>
        [StringLength(200)]
        public object Title { get; set; }

        /// <summary>
        /// 属性 Content 的验证特性。
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 属性 OrgID 的验证特性。
        /// </summary>
        [Required]
        public object OrgID { get; set; }

        /// <summary>
        /// 属性 OperateUserID 的验证特性。
        /// </summary>
        [Required]
        public object OperateUserID { get; set; }

        /// <summary>
        /// 属性 OperateUserName 的验证特性。
        /// </summary>
        [StringLength(20)]
        public object OperateUserName { get; set; }

        /// <summary>
        /// 属性 LogTime 的验证特性。
        /// </summary>
        public object LogTime { get; set; }

    }
}

