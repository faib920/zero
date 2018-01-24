using System.Web.Mvc;

namespace Fireasy.Zero.AspNet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
            filters.Add(new Fireasy.Web.Mvc.HandleErrorAttribute());
        }
    }
}
