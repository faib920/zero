using System;

namespace Fireasy.Zero.Dtos
{
    public class UserDto
    {
        /// <summary>
        /// 人员ID。
        /// </summary>

        public int UserID { get; set; }

        /// <summary>
        /// 机构ID。
        /// </summary>

        public int OrgID { get; set; }

        /// <summary>
        /// 姓名。
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// 账号。
        /// </summary>

        public string Account { get; set; }

        /// <summary>
        /// 角色名称。
        /// </summary>

        public string RoleNames { get; set; }

        /// <summary>
        /// 密码。
        /// </summary>

        public string Password { get; set; }

        /// <summary>
        /// 身份证号。
        /// </summary>

        public string IDCard { get; set; }

        /// <summary>
        /// 手机号。
        /// </summary>

        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱。
        /// </summary>

        public string Email { get; set; }

        /// <summary>
        /// 性别。
        /// </summary>

        public int Sex { get; set; }

        /// <summary>
        /// 拼音码。
        /// </summary>

        public string PyCode { get; set; }

        /// <summary>
        /// 状态。
        /// </summary>

        public int State { get; set; }

        /// <summary>
        /// 最近登录时间。
        /// </summary>

        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 令牌。
        /// </summary>

        public string Token { get; set; }

        /// <summary>
        /// 设备号。
        /// </summary>

        public string DeviceNo { get; set; }

        /// <summary>
        /// 学历。
        /// </summary>

        public int? DegreeNo { get; set; }

        /// <summary>
        /// 职称。
        /// </summary>

        public int? TitleNo { get; set; }

        /// <summary>
        /// 照片。
        /// </summary>

        public string Photo { get; set; }

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
        /// 学历。
        /// </summary>
        public string DegreeName { get; set; }

        /// <summary>
        /// 职称。
        /// </summary>
        public string TitleName { get; set; }
    }
}
