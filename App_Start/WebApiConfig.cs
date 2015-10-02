using LongTermCare_Xml_.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LongTermCare_Xml_
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //xml序列話
            config.Formatters.XmlFormatter.UseXmlSerializer = true;
            // Web API routes
            config.MapHttpAttributeRoutes();
           // config.Filters.Add(new AuthenticationFilter());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
