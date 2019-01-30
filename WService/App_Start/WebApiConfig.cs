using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace WService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web

            // Rutas de API web
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.Routes.MapHttpRoute(
               name: "LoginApp",
               routeTemplate: "v1/Login",
               defaults: new { Controller = "Login", Action = "Index" }
               );

            config.Routes.MapHttpRoute(
               name: "Register",
               routeTemplate: "v1/Register",
               defaults: new { Controller = "Register", Action = "Index" }
               );

            config.Routes.MapHttpRoute(
              name: "DrugstoresList",
              routeTemplate: "v1/DrugstoresList",
              defaults: new { Controller = "Search", Action = "DrugstoresList" }
              );

            config.Routes.MapHttpRoute(
              name: "NearbyBranchOffices",
              routeTemplate: "v1/NearbyBranchOffices",
              defaults: new { Controller = "Search", Action = "NearbyBranchOffices" }
              );
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
