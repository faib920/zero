using Fireasy.Common.ComponentModel;
using Fireasy.Data.Entity;
using Fireasy.Zero.Infrastructure;

namespace Fireasy.Zero.Models
{
    [EntityTreeMapping(InnerSign = "Code", Name = "Name", FullName = "FullName", SignLength = 2)]
    public partial class SysOrg : ITreeNode<SysOrg>, ITreeNodeRecursion<SysOrg>
    {
        public System.Collections.Generic.List<SysOrg> Children { get; set; }

        System.Collections.IList ITreeNode.Children
        {
            get
            {
                return Children;
            }
            set
            {
                Children = (System.Collections.Generic.List<SysOrg>)value;
            }
        }

        public bool HasChildren { get; set; }

        object ITreeNode.Id => OrgID;

        public bool IsLoaded { get; set; }

        public int? ParentId { get; set; }

        /// <summary>
        /// 机构的权限。
        /// </summary>
        public bool Permissible { get; set; }

        /// <summary>
        /// 属性名称。
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// 附加类别。
        /// </summary>
        public int? AttachType { get; set; }

        /// <summary>
        /// 附加的数据ID。
        /// </summary>
        public int? AttachID { get; set; }

        /// <summary>
        /// 附加数据的所属机构ID。
        /// </summary>
        public int? AttachOrgID { get; set; }

        /// <summary>
        /// 附加数据的所属机构编码。
        /// </summary>
        public string AttachOrgCode { get; set; }

    }
}
