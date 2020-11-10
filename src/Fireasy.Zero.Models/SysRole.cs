// **************************************
// ���ɣ�CodeBuilder (http://www.fireasy.cn/codebuilder)
// ��Ŀ��Fireasy Zero
// ��Ȩ��Copyright Fireasy
// ���ߣ�Huangxd
// ʱ�䣺10/12/2017 21:26:08
// **************************************
using System;
using Fireasy.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// ��ɫ ʵ���ࡣ
    /// </summary>
    [Serializable]
    [EntityMapping("SysRole", Description = "��ɫ")]
    [MetadataType(typeof(SysRoleMetadata))]
    public partial class SysRole : LightEntity<SysRole>
    {
        /// <summary>
        /// ��ȡ�����ý�ɫID��
        /// </summary>

        [PropertyMapping(ColumnName = "RoleID", Description = "��ɫID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        public virtual int RoleID { get; set; }

        /// <summary>
        /// ��ȡ�����ñ��롣
        /// </summary>

        [PropertyMapping(ColumnName = "Code", Description = "����", Length = 20, IsNullable = true)]
        public virtual string Code { get; set; }

        /// <summary>
        /// ��ȡ���������ơ�
        /// </summary>

        [PropertyMapping(ColumnName = "Name", Description = "����", Length = 50, IsNullable = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// ��ȡ������״̬��
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "״̬", IsNullable = false)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// ��ȡ���������ԡ�
        /// </summary>

        [PropertyMapping(ColumnName = "Attribute", Description = "����", IsNullable = false)]
        public virtual int Attribute { get; set; }

        /// <summary>
        /// ��ȡ������ƴ���롣
        /// </summary>

        [PropertyMapping(ColumnName = "PyCode", Description = "ƴ����", Length = 50, IsNullable = true)]
        public virtual string PyCode { get; set; }

        /// <summary>
        /// ��ȡ�����ñ�ע��
        /// </summary>

        [PropertyMapping(ColumnName = "Remark", Description = "��ע", Length = 500, IsNullable = true)]
        public virtual string Remark { get; set; }

        /// <summary>
        /// ��ȡ������ <see cref="SysUserRole"/> ����ʵ�弯��
        /// </summary>
        public virtual EntitySet<SysUserRole> SysUserRoles { get; set; }

        /// <summary>
        /// ��ȡ������ <see cref="SysModulePermission"/> ����ʵ�弯��
        /// </summary>
        public virtual EntitySet<SysModulePermission> SysModulePermissions { get; set; }

        /// <summary>
        /// ��ȡ������ <see cref="SysOperatePermission"/> ����ʵ�弯��
        /// </summary>
        public virtual EntitySet<SysOperatePermission> SysOperatePermissions { get; set; }

        /// <summary>
        /// ��ȡ������ <see cref="SysOrgPermission"/> ����ʵ�弯��
        /// </summary>
        public virtual EntitySet<SysOrgPermission> SysDataPermissions { get; set; }

    }
	
    public class SysRoleMetadata
    {
        /// <summary>
        /// ���� RoleID ����֤���ԡ�
        /// </summary>
        [Required]
        public object RoleID { get; set; }

        /// <summary>
        /// ���� Code ����֤���ԡ�
        /// </summary>
        [StringLength(20)]
        public object Code { get; set; }

        /// <summary>
        /// ���� Name ����֤���ԡ�
        /// </summary>
        [StringLength(50)]
        [Required]
        public object Name { get; set; }

        /// <summary>
        /// ���� State ����֤���ԡ�
        /// </summary>
        [Required]
        public object State { get; set; }

        /// <summary>
        /// ���� Attribute ����֤���ԡ�
        /// </summary>
        public object Attribute { get; set; }

        /// <summary>
        /// ���� PyCode ����֤���ԡ�
        /// </summary>
        [StringLength(50)]
        public object PyCode { get; set; }

        /// <summary>
        /// ���� Remark ����֤���ԡ�
        /// </summary>
        [StringLength(500)]
        public object Remark { get; set; }

    }
}

