using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace HW01_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("http://localhost:4200 , http://localhost:8100", headers: "*", methods: "*");
            //var ion = new EnableCorsAttribute("http://localhost:8100", headers: "*", methods: "*");
            config.EnableCors(cors);

            //var ionic = new EnableCorsAttribute("http://localhost:8100", headers: "*", methods: "*");
            //config.EnableCors(ionic);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
