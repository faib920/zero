using Fireasy.Common.Ioc;
using Fireasy.Common.Serialization;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;

namespace Fireasy.Zero.AspNetCore
{
    public static class Extensions
    {
        /// <summary>
        /// 包装 Json 数据。
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="value"></param>
        /// <param name="converters"></param>
        /// <returns></returns>
        public static JsonResult Json(this Controller controller, object value, params JsonConverter[] converters)
        {
            var option = new JsonSerializeOption();
            if (converters != null)
            {
                option.Converters.AddRange(converters);
            }

            return new JsonResultWrapper(value, option);
        }

        public static IHtmlContent Toolbar(this IHtmlHelper html)
        {
            var session = html.ViewContext.HttpContext.GetSession();
            var adminSvr = ContainerUnity.GetContainer().Resolve<IAdminService>();
            var operates = adminSvr.GetPurviewOperates(session.UserID, html.ViewContext.HttpContext.Request.Path);
            var sb = new StringBuilder();

            foreach (var oper in operates)
            {
                if (oper != null)
                {
                    sb.AppendFormat("<a key=\"{0}\" class=\"easyui-linkbutton\" onclick=\"toolbarClick('{0}')\" data-options=\"iconCls: '{1}', plain: true\">{2}</a>", oper.Code, oper.Icon, oper.Name);
                }
            }

            return html.Raw(sb.ToString());
        }
    }
}