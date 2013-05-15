using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlogEngineTK.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            #region Ignore

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ignoring that route, because many browsers request favicon.ico and ASP.NET MVC will try get that controller
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.([iI][cC][oO]|[gG][iI][fF])(/.*)?" });

            routes.IgnoreRoute("elfinder.connector"); // ignore route to http-handler "elFinder.Connector"

            #endregion            

            routes.MapRoute(
                name: "",
                url: "Post{postId}/{routeSegment}",
                defaults: new { controller = "Home", action = "Post", routeSegment = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "",
                url: "Page{page}",
                defaults: new { controller = "Home", action = "Index" },
                constraints: new { page = @"\d+" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}