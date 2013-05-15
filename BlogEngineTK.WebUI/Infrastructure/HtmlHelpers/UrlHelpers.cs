using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlogEngineTK.WebUI.Infrastructure.HtmlHelpers
{
    public static class UrlHelpers
    {
        public static string Home(this UrlHelper helper)
        {
            return helper.Content("~/");
        }

        public static string CurrentUrl(this UrlHelper helper)
        {
            return HttpContext.Current.Request.Url.PathAndQuery;
        }

        public static string StyleSheetFolder(this UrlHelper helper)
        {
            return helper.Content("~/Content/");
        }

        public static string ScriptsFolder(this UrlHelper helper)
        {
            return helper.Content("~/Scripts/");
        }

        public static string CustomScriptsFolder(this UrlHelper helper)
        {
            return helper.Content("~/Scripts/Custom/");
        }

        public static string BaseLayout(this UrlHelper helper)
        {
            return helper.Content("~/Views/Shared/_Layout.cshtml");
        }

        public static string BlogLayout(this UrlHelper helper)
        {
            return helper.Content("~/Views/Shared/_BlogLayout.cshtml");
        }

        public static string AdminLayout(this UrlHelper helper)
        {
            return helper.Content("~/Areas/Admin/Views/Shared/_AdminLayout.cshtml");
        }        
    }
}