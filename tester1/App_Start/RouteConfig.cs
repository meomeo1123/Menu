using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace tester1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
               routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Admin",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { controller = "ListUser", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "tester1.Areas.Admin.Controllers" }
            );
            routes.MapRoute(
               name: "Admin1",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "AddCart", action = "GetSalesForUser", id = UrlParameter.Optional }
            );
        }
    }
}
