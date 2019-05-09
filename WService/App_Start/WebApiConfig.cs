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
               name: "EmployeeRegister",
               routeTemplate: "v1/RegisterEmployee",
               defaults: new { Controller = "RegisterEmployee", Action = "Index" }
               );

            config.Routes.MapHttpRoute(
              name: "DrugstoresList",
              routeTemplate: "v1/DrugstoresList",
              defaults: new { Controller = "Search", Action = "DrugstoresList" }
              );

            //Buscar el medicamento en todas las farmacias
            config.Routes.MapHttpRoute(
               name: "Nearby",
               routeTemplate: "v1/Nearby",
               defaults: new { Controller = "SearchAllDrugstores", Action = "SearchNearby" }
               );

            //Buscar el medicamento en una farmacia
            config.Routes.MapHttpRoute(
               name: "NearbyDrugstore",
               routeTemplate: "v1/NearbyDrugstore",
               defaults: new { Controller = "SearchByDrugstore", Action = "SearchDrugstore" }
               );

            //Buscar el medicamento en una sucursal especifica
            config.Routes.MapHttpRoute(
              name: "SearchMore",
              routeTemplate: "v1/SearchMore",
              defaults: new { Controller = "Search", Action = "SearchMore" }
              );

            //Detalle del medicamento
            config.Routes.MapHttpRoute(
               name: "Detail",
               routeTemplate: "v1/Detail",
               defaults: new { Controller = "Detail", Action = "ProductDetail" }
               );

            //Insertar un nuevo pedido
            config.Routes.MapHttpRoute(
               name: "Order",
               routeTemplate: "v1/Order",
               defaults: new { Controller = "Pedido", Action = "addPedidos" }
               );


            //Listar pedidos por usuario
            config.Routes.MapHttpRoute(
               name: "OrdersList",
               routeTemplate: "v1/OrdersList",
               defaults: new { Controller = "Pedido", Action = "pedidosList" }
               );


            //Insertar un nuevo reclamo
            config.Routes.MapHttpRoute(
               name: "Reclam",
               routeTemplate: "v1/Reclam",
               defaults: new { Controller = "Reclamations", Action = "addReclamation" }
               );

            //Enviar comprobante
            config.Routes.MapHttpRoute(
               name: "Ticket",
               routeTemplate: "v1/Ticket",
               defaults: new { Controller = "Ticket", Action = "SendEmailM" }
               );

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
