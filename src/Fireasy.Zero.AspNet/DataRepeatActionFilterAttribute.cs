using Fireasy.Common.ComponentModel;
using Fireasy.Web.EasyUI;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Infrastructure;
using System.Web.Mvc;

namespace Fireasy.Zero.AspNet
{
    public class DataRepeatActionFilterAttribute : ValidationActionFilterAttribute
    {
        protected override void HandleExceptionForJson(ExceptionContext filterContext)
        {
            if (filterContext.Exception is DataRepeatException)
            {
                filterContext.Result = GetValidResult((filterContext.Exception as DataRepeatException));
                filterContext.ExceptionHandled = true;
            }
            else
            {
                base.HandleExceptionForJson(filterContext);
            }
        }

        private ActionResult GetValidResult(DataRepeatException exception)
        {
            var result = new JsonResult
            {
                Data = (Result.Info("Repeat", new { exception.Title, exception.Rows }))
            };
            return new JsonResultWrapper(result, 0);
        }

    }
}