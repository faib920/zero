// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Extensions;
using Fireasy.Common.Security;
using System.Text;

namespace Fireasy.Zero.Infrastructure
{
    /// <summary>
    /// MD5加密算法。
    /// </summary>
    public class Md5Encryptor : IEncryptProvider
    {
        //加盐格式
        private const string MD5_SALT = "&^*73{0}MuIDr";

        /// <summary>
        /// 创建密文。
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Create(string password)
        {
            var md5 = CryptographyFactory.Create("MD5");

            return md5.Encrypt(string.Format(MD5_SALT, password), Encoding.UTF8).ToHex();
        }

        /// <summary>
        /// 验证密文是否正确。
        /// </summary>
        /// <param name="password"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public bool Validate(string password, string actual)
        {
            return Create(password) == actual;
        }
    }
}
