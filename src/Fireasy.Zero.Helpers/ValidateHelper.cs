// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.Caching;
using System;
#if !NETSTANDARD2_0
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
#else
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.DrawingCore.Imaging;
using System.DrawingCore.Text;
#endif
using System.IO;
using System.Text;

namespace Fireasy.Zero.Helpers
{
    /// <summary>
    /// 验证码辅助类。
    /// </summary>
    public class ValidateHelper
    {
        /// <summary>
        /// 产生随机字符。
        /// </summary>
        /// <param name="length">字符的个数。</param>
        /// <returns></returns>
        public static string GenerateCode(int length = 4)
        {
            var sb = new StringBuilder();
            var ran = new Random();
            for (var i = 0; i < length; i++)
            {
                sb.Append((char)ran.Next(65, 90));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成指定字符的验证码图片。
        /// </summary>
        /// <param name="code">验证码。</param>
        /// <param name="width">图片的宽度。</param>
        /// <param name="height">图片的高度。</param>
        /// <returns></returns>
        public static byte[] GenerateImage(string code, int width, int height)
        {
            var ran = new Random();
            using (var bmp = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var x = 2;
                foreach (var c in code)
                {
                    graphics.RotateTransform(ran.Next(-5, 5));
                    //var color = Color.FromArgb(ran.Next(0, 128), ran.Next(0, 128), ran.Next(0, 128));
                    var color = Color.FromArgb(255, 255, 255);
                    var bursh = new SolidBrush(color);
                    graphics.DrawString(c.ToString(), new Font("Consolas", 18), bursh, x, 0);
                    x += 18;
                    graphics.ResetTransform();
                    bursh.Dispose();
                }

                using (var memory = new MemoryStream())
                {
                    bmp.Save(memory, ImageFormat.Png);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// 将验证码进行缓存。
        /// </summary>
        /// <param name="key">验证码的key。</param>
        /// <param name="code">验证码。</param>
        public static void Cache(string key, string code)
        {
            var cacheMgr = CacheManagerFactory.CreateManager();
            cacheMgr.Add("Validate_" + key, code, new RelativeTime(TimeSpan.FromMinutes(5)));
        }

        /// <summary>
        /// 判断 <paramref name="code"/> 是否和缓存里一致。
        /// </summary>
        /// <param name="key">验证码的key。</param>
        /// <param name="code">验证码。</param>
        /// <returns></returns>
        public static bool Validate(string key, string code)
        {
            var cacheMgr = CacheManagerFactory.CreateManager();
            var vcode = string.Empty;
            if (cacheMgr.TryGet("Validate_" + key, out vcode))
            {
                return code.Equals(vcode, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
