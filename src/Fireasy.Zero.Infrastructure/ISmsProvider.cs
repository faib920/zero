// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Fireasy.Zero.Infrastructure
{
    public interface ISmsProvider
    {
        /// <summary>
        /// 发送信息提供者。
        /// </summary>
        /// <param name="template">模板编码。</param>
        /// <param name="param">模板参数。</param>
        /// <param name="mobiles">手机号。</param>
        /// <returns>如果发送失败，则为失败信息。</returns>
        string SendMessage(string template, object param, params string[] mobiles);
    }
}
