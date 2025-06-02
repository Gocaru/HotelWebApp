using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HotelDB_API_Framework
{
    public static class WebApiConfig
    {

        public static void Register(HttpConfiguration config)
        {

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",     //caminho dos routes
                defaults: new { id = RouteParameter.Optional }  //o id é opcional
            );
        }
    }
}
