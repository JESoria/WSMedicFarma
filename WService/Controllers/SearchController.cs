using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.IO;
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
        public async Task<IHttpActionResult> DrugstoresList()
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
                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    List<DrugstoresModel> list = new List<DrugstoresModel>();
                    db.FARMACIA.OrderBy(x => x.ID_FARMACIA).ToList().ForEach(x => {
                        //MemoryStream ms = new MemoryStream(x.IMAGEN);
                        //Image image = Image.FromStream(ms);

                        list.Add(new DrugstoresModel()
                        {
                            idFarmacia = x.ID_FARMACIA,
                            NombreFarmacia = x.FARMACIA1,
                            //imagenFarmacia = image
                        });
                    });
                    return Ok(list);
                }
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> NearbyBranchOffices(NearbyDrugstoreModel data)
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
                int idFarmacia = data.idFarmacia;
                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    var coord = new GeoCoordinate(data.latitude, data.longitude);
                    List<BranchOfficesModel> allBranchOffices = new List<BranchOfficesModel>();
                    List<BranchOfficesModel> nearby  = new List<BranchOfficesModel>();

                    db.SUCURSAL.OrderBy(x => x.ID_FARMACIA).Where(x => x.ID_FARMACIA == idFarmacia).ToList().ForEach(x =>
                    {
                        allBranchOffices.Add(new BranchOfficesModel() {
                            idSucursal = x.ID_SUCURSAL,
                            sucursal = x.SUCURSAL1,
                            direccion = x.DIRECCION,
                            longitud = x.LONGITUD,
                            latitud = x.LATITUD
                        });
                    });


                    foreach (var x in allBranchOffices)
                    {
                        double lat = Convert.ToDouble(x.latitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double lon = Convert.ToDouble(x.longitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double distance = SearchModel.Distance(data.latitude, data.longitude, lat, lon);
                        if (distance < 2)          //nearbyplaces which are within 2 kms 
                        {
                            BranchOfficesModel offices = new BranchOfficesModel();
                            offices.idSucursal = x.idSucursal;
                            offices.sucursal = x.sucursal;
                            offices.direccion = x.direccion;
                            offices.latitud = x.latitud;
                            offices.longitud = x.longitud;
                            offices.distancia = distance;
                            nearby.Add(offices);
                        }
                    }
                    return Ok(nearby.OrderBy(x => x.distancia));

                }

            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> ProductSearch(ProductSearchModel data) {

            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null)
            {
                return BadRequest();
            }
            else {

                List<ProductSearchModel> productos = new List<ProductSearchModel>();

                using (MedicFarmaEntities db = new MedicFarmaEntities()) {

                    db.SUCURSAL_PRODUCTO.OrderBy(x => x.ID_SUCURSAL_PRODUCTO).ToList().ForEach(x =>
                    {
                        db.PRODUCTO.Where(y => y.ID_PRODUCTO == x.ID_PRODUCTO && y.PRODUCTO1.Contains(data.producto) && x.ID_SUCURSAL == data.idSucursal).ToList().ForEach(z =>
                        {
                            productos.Add(new ProductSearchModel {
                                idSucursalProducto = x.ID_SUCURSAL_PRODUCTO,
                                idSucursal = x.ID_SUCURSAL,
                                producto = z.PRODUCTO1,
                                precio = z.PRECIO
                            });
                        });
                    });
                    return Ok(productos);
                }
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ProductDetail(SearchModel data)
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
                List<DetailModel> detalle = new List<DetailModel>();

                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    db.SUCURSAL_PRODUCTO.Where(x => x.ID_SUCURSAL_PRODUCTO == data.idSucursalProducto).ToList().ForEach(x => {
                        db.PRODUCTO.Where(y => y.ID_PRODUCTO == x.ID_PRODUCTO).ToList().ForEach(y => {
                            db.PRESENTACION.Where(z => z.ID_PRESENTACION == y.ID_PRESENTACION).ToList().ForEach(z => {
                                db.CATEGORIA.Where(c => c.ID_CATEGORIA == y.ID_CATEGORIA).ToList().ForEach(c => {
                                    db.LABORATORIO.Where(l => l.ID_LABORATORIO == y.ID_LABORATORIO).ToList().ForEach(l =>
                                    {
                                        detalle.Add(new DetailModel()
                                        {
                                            producto = y.PRODUCTO1,
                                            presentacion = z.PRESENTACION1,
                                            fechaVencimiento = x.FECHA_VENCIMIENTO,
                                            laboratorio = l.LABORATORIO1,
                                            principiosActivos = y.DESCRIPCION,
                                            categoria = c.CATEGORIA1,
                                            //precio = Convert.ToDouble(x.PRECIO)
                                        });
                                    });
                                });
                            });
                        });
                    });

                    return Ok(detalle);
                }

            }
        }
    }
}