using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogEngineTK.WebUI.Infrastructure.HtmlHelpers;

namespace BlogEngineTK.WebUI.Infrastructure.Filters
{
    /// <summary>
    /// Перенаправляет авторизованного пользователя на главную страницу
    /// </summary>
    public class RedirectAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                UrlHelper helper = new UrlHelper(filterContext.RequestContext);          
                filterContext.Result = new RedirectResult(helper.Home());
            }
        }
    }
}