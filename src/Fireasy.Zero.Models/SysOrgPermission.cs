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
    /// 数据权限 实体类。
    /// </summary>
    [Serializable]
    [EntityMapping("SysOrgPermission", Description = "数据权限")]
    [MetadataType(typeof(SysOrgPermissionMetadata))]
    public partial class SysOrgPermission : LightEntity<SysOrgPermission>
    {
        /// <summary>
        /// 获取或设置角色ID。
        /// </summary>

        [PropertyMapping(ColumnName = "RoleID", Description = "角色ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int RoleID { get; set; }

        /// <summary>
        /// 获取或设置机构ID。
        /// </summary>

        [PropertyMapping(ColumnName = "OrgID", Description = "机构ID", IsPrimaryKey = true, IsNullable = false)]
        public virtual int OrgID { get; set; }

    }
	
    public class SysOrgPermissionMetadata
    {
        /// <summary>
        /// 属性 RoleID 的验证特性。
        /// </summary>
        [Required]
        public object RoleID { get; set; }

        /// <summary>
        /// 属性 OrgID 的验证特性。
        /// </summary>
        [Required]
        public object OrgID { get; set; }

    }
}

