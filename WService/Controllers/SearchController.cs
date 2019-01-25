using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WService.Code;
using WService.Models;

namespace WService.Controllers
{
    [Authorize]
    public class SearchController : ApiController
    {
        private DataAccess da = new DataAccess();

        [HttpPost]
        public async Task<IHttpActionResult> NearbyDrugstore(NearbyDrugstoreModel data)
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null)
            {
                return BadRequest();
            }
            else
            {

                string producto = data.product;
                int idFarmacia = data.idFarmacia;
                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    var coord = new GeoCoordinate(data.latitude, data.longitude);
                    List<ProductsModel> lista = new List<ProductsModel>();
                    List<ProductsModel> lista2 = new List<ProductsModel>();

                    db.SUCURSAL_PRODUCTO.OrderBy(x => x.ID_SUCURSAL_PRODUCTO).ToList().ForEach(x =>
                    {
                        db.SUCURSAL.Where(s => s.ID_SUCURSAL == x.ID_SUCURSAL).ToList().ForEach(y =>
                        {
                            db.FARMACIA.Where(f => f.ID_FARMACIA == y.ID_FARMACIA).ToList().ForEach(z =>
                            {
                                db.PRODUCTO.Where(p => p.ID_PRODUCTO == x.ID_PRODUCTO && p.PRODUCTO1.Contains(producto) && y.ID_FARMACIA == idFarmacia).ToList().ForEach(w =>
                                {
                                    //pendiente: agregar precio ya que no se ha actualizado el modelo de la base de datos
                                    lista.Add(new ProductsModel() { producto = w.PRODUCTO1, sucursal = y.SUCURSAL1, latitud = y.LATITUD, longitud = y.LONGITUD, idSucursalProducto = x.ID_SUCURSAL_PRODUCTO });

                                });
                            });
                        });
                    });


                    foreach (var x in lista)
                    {
                        double lat = Convert.ToDouble(x.latitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double lon = Convert.ToDouble(x.longitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double distance = SearchModel.Distance(data.latitude, data.longitude, lat, lon);
                        if (distance < 2)          //nearbyplaces which are within 2 kms 
                        {
                            ProductsModel products = new ProductsModel();
                            products.sucursal = x.sucursal;
                            products.latitud = x.latitud;
                            products.longitud = x.longitud;
                            products.producto = x.producto;
                            products.precio = x.precio;
                            products.idSucursalProducto = x.idSucursalProducto;
                            products.distancia = distance;
                            lista2.Add(products);
                        }
                    }
                    return Ok(lista2.OrderBy(x => x.distancia));

                }

            }
        }
    }
}