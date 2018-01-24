// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Fireasy.Zero.Infrastructure
{
    /// <summary>
    /// 用户会话信息。
    /// </summary>
    public class SessionContext
    {
        /// <summary>
        /// 获取或设置用户ID。
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 获取或设置用户名称。
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 获取或设置机构ID。
        /// </summary>
        public int OrgID { get; set; }
    }
}
