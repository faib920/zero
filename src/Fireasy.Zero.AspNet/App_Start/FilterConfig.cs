using Fireasy.Web.EasyUI;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new DataRepeatActionFilterAttribute());
            filters.Add(new ValidationActionFilterAttribute());
        }
    }
}
