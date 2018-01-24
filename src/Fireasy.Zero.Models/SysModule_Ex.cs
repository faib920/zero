using Fireasy.Common.ComponentModel;
using Fireasy.Data.Entity;
using Fireasy.Zero.Infrastructure;

namespace Fireasy.Zero.Models
{
    [EntityTreeMapping(InnerSign = "Code")]
    public partial class SysModule : ITreeNode<SysModule>, ITreeNodeRecursion<SysModule>
    {
        public System.Collections.Generic.List<SysModule> Children { get; set; }

        System.Collections.IList ITreeNode.Children
        {
            get
            {
                return Children;
            }
            set
            {
                Children = (System.Collections.Generic.List<SysModule>)value;
            }
        }

        public bool HasChildren { get; set; }

        object ITreeNode.Id => ModuleID;

        public bool IsLoaded { get; set; }

        public int? ParentId { get; set; }

        /// <summary>
        /// 模块的权限。
        /// </summary>
        public bool Permissible { get; set; }
    }
}
