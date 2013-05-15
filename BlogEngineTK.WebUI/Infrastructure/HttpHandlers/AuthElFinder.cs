using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogEngineTK.WebUI.Infrastructure.HttpHandlers
{
    public class AuthElFinder : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var handler = new elFinder.Connector.Connector();
                handler.ProcessRequest(context);
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.End();   
            }
        }
    }
}