
namespace Fireasy.Zero.Models
{
    public partial class SysUser
    {
        /// <summary>
        /// 机构名称。
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 性别名称
        /// </summary>
        public string SexName { get; set; }

        /// <summary>
        /// 逗号分隔的多个角色。
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 照片。
        /// </summary>
        public string Photo { get; set; }
    }
}
