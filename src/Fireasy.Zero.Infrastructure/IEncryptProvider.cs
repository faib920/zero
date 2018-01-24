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
    /// 密码学验证提供者。
    /// </summary>
    public interface IEncryptProvider
    {
        /// <summary>
        /// 创建密文。
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string Create(string password);

        /// <summary>
        /// 验证密文是否正确。
        /// </summary>
        /// <param name="password"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        bool Validate(string password, string actual);
    }
}
