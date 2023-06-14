using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
            name: "Swagger",
            routeTemplate: "swagger/docs/{apiVersion}",
            defaults: null,
            constraints: new { apiVersion = @"^[a-zA-Z0-9]+$" },
            handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger/ui/index"));
        }
    }
}
