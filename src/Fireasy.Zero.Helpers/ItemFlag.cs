// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common;

namespace Fireasy.Zero.Helpers
{
    /// <summary>
    /// 下拉列表项的标识。
    /// </summary>
    public enum ItemFlag
    {
        /// <summary>
        /// 无。
        /// </summary>
        [EnumDescription("无")]
        None = 0,
        /// <summary>
        /// 全部。
        /// </summary>
        [EnumDescription("--全部--")]
        All = 1,
        /// <summary>
        /// 请选择。
        /// </summary>
        [EnumDescription("--请选择--")]
        Select = 2,
        /// <summary>
        /// 最顶级。
        /// </summary>
        [EnumDescription("[最顶级]")]
        Root = 3
    }
}
