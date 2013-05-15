using BlogEngineTK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Infrastructure.Filters
{
    /// <summary>
    /// Фильтр для обработки запросов при отключенном блоге
    /// </summary>
    public class BlogOffAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!BlogSettings.Current.IsBlogOn)
            {
                filterContext.Result = new ViewResult()
                {
                    ViewName = "BlogOff",
                };
            }
        }
    }
}