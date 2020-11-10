// **************************************
// ���ɣ�CodeBuilder (http://www.fireasy.cn/codebuilder)
// ��Ŀ��Fireasy Zero
// ��Ȩ��Copyright Fireasy
// ���ߣ�Huangxd
// ʱ�䣺10/12/2017 21:26:11
// **************************************
using System;
using Fireasy.Data.Entity;
using System.ComponentModel.DataAnnotations;
using Fireasy.Data.Entity.Validation;
using Fireasy.Data;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// ��Ա ʵ���ࡣ
    /// </summary>
    [Serializable]
    [EntityMapping("SysUser", Description = "��Ա")]
    [MetadataType(typeof(SysUserMetadata))]
    public partial class SysUser : LightEntity<SysUser>
    {
        /// <summary>
        /// ��ȡ��������ԱID��
        /// </summary>

        [PropertyMapping(ColumnName = "UserID", Description = "��ԱID", GenerateType = IdentityGenerateType.AutoIncrement, IsPrimaryKey = true, IsNullable = false)]
        [Key]
        public virtual int UserID { get; set; }

        /// <summary>
        /// ��ȡ�����û���ID��
        /// </summary>

        [PropertyMapping(ColumnName = "OrgID", Description = "����ID", IsNullable = false)]
        public virtual int OrgID { get; set; }

        /// <summary>
        /// ��ȡ������������
        /// </summary>

        [PropertyMapping(ColumnName = "Name", Description = "����", Length = 20, IsNullable = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// ��ȡ�������˺š�
        /// </summary>

        [PropertyMapping(ColumnName = "Account", Description = "�˺�", Length = 50, IsNullable = true)]
        public virtual string Account { get; set; }

        /// <summary>
        /// ��ȡ�����ý�ɫ���ơ�
        /// </summary>

        [PropertyMapping(ColumnName = "RoleNames", Description = "��ɫ����", Length = 100, IsNullable = true)]
        public virtual string RoleNames { get; set; }

        /// <summary>
        /// ��ȡ���������롣
        /// </summary>

        [PropertyMapping(ColumnName = "Password", Description = "����", Length = 50, IsNullable = true)]
        public virtual string Password { get; set; }

        /// <summary>
        /// ��ȡ���������֤�š�
        /// </summary>

        [PropertyMapping(ColumnName = "IDCard", Description = "���֤��", Length = 150, IsNullable = true, DataType = System.Data.DbType.String)]
        public virtual CodedData IDCard { get; set; }

        /// <summary>
        /// ��ȡ�������ֻ��š�
        /// </summary>

        [PropertyMapping(ColumnName = "Mobile", Description = "�ֻ���", Length = 20, IsNullable = true)]
        public virtual string Mobile { get; set; }

        /// <summary>
        /// ��ȡ���������䡣
        /// </summary>

        [PropertyMapping(ColumnName = "Email", Description = "����", Length = 50, IsNullable = true)]
        public virtual string Email { get; set; }

        /// <summary>
        /// ��ȡ�������Ա�
        /// </summary>

        [PropertyMapping(ColumnName = "Sex", Description = "�Ա�", IsNullable = true)]
        public virtual Sex Sex { get; set; }

        /// <summary>
        /// ��ȡ������ƴ���롣
        /// </summary>

        [PropertyMapping(ColumnName = "PyCode", Description = "ƴ����", Length = 20, IsNullable = true)]
        public virtual string PyCode { get; set; }

        /// <summary>
        /// ��ȡ������״̬��
        /// </summary>

        [PropertyMapping(ColumnName = "State", Description = "״̬", DefaultValue = StateFlags.Enabled, IsNullable = true)]
        public virtual StateFlags State { get; set; }

        /// <summary>
        /// ��ȡ�����������¼ʱ�䡣
        /// </summary>

        [PropertyMapping(ColumnName = "LastLoginTime", Description = "�����¼ʱ��", IsNullable = true)]
        public virtual DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// ��ȡ���������ơ�
        /// </summary>

        [PropertyMapping(ColumnName = "Token", Description = "����", Length = 100, IsNullable = true)]
        public virtual string Token { get; set; }

        /// <summary>
        /// ��ȡ�������豸�š�
        /// </summary>

        [PropertyMapping(ColumnName = "DeviceNo", Description = "�豸��", Length = 100, IsNullable = true)]
        public virtual string DeviceNo { get; set; }

        /// <summary>
        /// ��ȡ������ѧ����
        /// </summary>

        [PropertyMapping(ColumnName = "DegreeNo", Description = "ѧ��", IsNullable = true)]
        public virtual int? DegreeNo { get; set; }

        /// <summary>
        /// ��ȡ������ְ�ơ�
        /// </summary>

        [PropertyMapping(ColumnName = "TitleNo", Description = "ְ��", IsNullable = true)]
        public virtual int? TitleNo { get; set; }

        /// <summary>
        /// ��ȡ��������Ƭ��
        /// </summary>

        [PropertyMapping(ColumnName = "Photo", Description = "��Ƭ", IsNullable = true)]
        public virtual string Photo { get; set; }

        /// <summary>
        /// ��ȡ�����ù��� <see cref="SysOrg"/> ����
        /// </summary>
        public virtual SysOrg SysOrg { get; set; }

        /// <summary>
        /// ��ȡ�����ù��� <see cref="SysOrg"/> ����
        /// </summary>
        public virtual EntitySet<SysUserRole> SysUserRoles { get; set; }
    }

    public class SysUserMetadata
    {
        /// <summary>
        /// ���� UserID ����֤���ԡ�
        /// </summary>
        [Required]
        public object UserID { get; set; }

        /// <summary>
        /// ���� OrgID ����֤���ԡ�
        /// </summary>
        [Required]
        public object OrgID { get; set; }

        /// <summary>
        /// ���� Name ����֤���ԡ�
        /// </summary>
        [StringLength(20)]
        [Required]
        public object Name { get; set; }

        /// <summary>
        /// ���� Account ����֤���ԡ�
        /// </summary>
        [StringLength(50)]
        public object Account { get; set; }

        /// <summary>
        /// ���� RoleNames ����֤���ԡ�
        /// </summary>
        [StringLength(100)]
        public object RoleNames { get; set; }

        /// <summary>
        /// ���� Password ����֤���ԡ�
        /// </summary>
        [StringLength(50)]
        public object Password { get; set; }

        /// <summary>
        /// ���� IDCard ����֤���ԡ�
        /// </summary>
        [StringLength(18)]
        [IDCard]
        public object IDCard { get; set; }

        /// <summary>
        /// ���� Mobile ����֤���ԡ�
        /// </summary>
        [StringLength(20)]
        [Required]
        [Mobile]
        public object Mobile { get; set; }

        /// <summary>
        /// ���� Email ����֤���ԡ�
        /// </summary>
        [StringLength(50)]
        [Email]
        public object Email { get; set; }

        /// <summary>
        /// ���� Sex ����֤���ԡ�
        /// </summary>
        [Required]
        public object Sex { get; set; }

        /// <summary>
        /// ���� PyCode ����֤���ԡ�
        /// </summary>
        [StringLength(20)]
        public object PyCode { get; set; }

        /// <summary>
        /// ���� State ����֤���ԡ�
        /// </summary>
        public object State { get; set; }

        /// <summary>
        /// ���� LastLoginTime ����֤���ԡ�
        /// </summary>
        public object LastLoginTime { get; set; }

        /// <summary>
        /// ���� Token ����֤���ԡ�
        /// </summary>
        [StringLength(200)]
        public object Token { get; set; }

        /// <summary>
        /// ���� DeviceNo ����֤���ԡ�
        /// </summary>
        [StringLength(100)]
        public object DeviceNo { get; set; }

    }
}

