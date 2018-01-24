using Fireasy.Common;

namespace Fireasy.Zero.Models
{
    public enum StateFlags
    {
        [EnumDescription("禁用")]
        Disabled = 0,
        [EnumDescription("启用")]
        Enabled = 1
    }

    public enum Sex
    {
        [EnumDescription("男")]
        Male = 1,
        [EnumDescription("女")]
        Female = 2
    }

    public enum OrgAttribute
    {
        Org,
        Dept
    }
}
