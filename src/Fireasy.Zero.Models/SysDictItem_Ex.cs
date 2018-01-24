using Fireasy.Common.ComponentModel;
using Fireasy.Data.Entity;
using System;

namespace Fireasy.Zero.Models
{
    /// <summary>
    /// 公共字典项 扩展类
    /// </summary>
    [EntityTreeMapping(InnerSign = "Code", SignLength = 3)]
    public partial class SysDictItem : ITreeNode<SysDictItem>
    {
        public System.Collections.Generic.List<SysDictItem> Children { get; set; }
        System.Collections.IList ITreeNode.Children
        {
            get { return Children; }
            set { Children = (System.Collections.Generic.List<SysDictItem>)value; }
        }
        public bool HasChildren { get; set; }

        object ITreeNode.Id => ItemID;
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
        /// 字典编码。
        /// </summary>
        public string TypeCode { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string TypeName { get; set; }
    }
}
