using Fireasy.Common.Ioc;
using Fireasy.Common.Serialization;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Services;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet
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

        public static IHtmlString Toolbar(this HtmlHelper html)
        {
            var session = html.ViewContext.HttpContext.GetSession();
            var adminSvr = ContainerUnity.GetContainer().Resolve<IAdminService>();
            var operates = adminSvr.GetPurviewOperates(session.UserID, HttpContext.Current.Request.RawUrl);
            var sb = new StringBuilder();

            foreach (var oper in operates)
            {
                if (oper != null)
                {
                    sb.AppendFormat("<a key=\"{0}\" class=\"easyui-linkbutton\" onclick=\"toolbarClick('{0}')\" data-options=\"iconCls: '{1}', plain: true\">{2}</a>", oper.Code, oper.Icon, oper.Name);
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}