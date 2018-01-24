// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.IO;

namespace Fireasy.Zero.Infrastructure
{
    /// <summary>
    /// 文件存储提供者。
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// 存储文件数据。
        /// </summary>
        /// <param name="bytes">文件数据。</param>
        /// <param name="bucket">存储空间。</param>
        /// <param name="fileName">文件名。</param>
        /// <returns></returns>
        string Save(byte[] bytes, string bucket, string fileName);

        /// <summary>
        /// 存储文件流。
        /// </summary>
        /// <param name="stream">文件流。</param>
        /// <param name="bucket">存储空间。</param>
        /// <param name="fileName">文件名。</param>
        /// <returns></returns>
        string Save(Stream stream, string bucket, string fileName);

        /// <summary>
        /// 加载文件数据。
        /// </summary>
        /// <param name="path">虚拟路径。</param>
        /// <returns></returns>
        byte[] Load(string path);

        /// <summary>
        /// 解析出完整的文件路径。
        /// </summary>
        /// <param name="path">相对路径。</param>
        /// <returns></returns>
        string Reslove(string path);

        /// <summary>
        /// 裁剪文件路径，完整路径变成相对路径。
        /// </summary>
        /// <param name="path">完整。</param>
        /// <returns></returns>
        string Clip(string path);
    }
}
