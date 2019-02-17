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
        public async Task<IHttpActionResult> Cerca(SearchModel data)
        {
            string producto = data.producto;
            int idFarmacia = data.idFarmacia;
            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null)
            {
                return BadRequest();
            }
            else
            {/*
                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    var coord = new GeoCoordinate(data.latitud, data.longitud);
                    List<ProductSearchModel> lista = new List<ProductSearchModel>();
                    List<ProductSearchModel> lista2 = new List<ProductSearchModel>();

                    db.SUCURSAL_PRODUCTO.OrderBy(x => x.ID_SUCURSAL_PRODUCTO).ToList().ForEach(x =>
                    {
                        db.SUCURSAL.Where(s => s.ID_SUCURSAL == x.ID_SUCURSAL).ToList().ForEach(y =>
                        {
                            db.FARMACIA.Where(f => f.ID_FARMACIA == y.ID_FARMACIA).ToList().ForEach(z =>
                            {
                                db.PRODUCTO.Where(p => p.ID_PRODUCTO == x.ID_PRODUCTO && p.PRODUCTO1.Contains(producto) && y.ID_FARMACIA == idFarmacia).ToList().ForEach(w =>
                                {

                                    lista.Add(new ProductSearchModel() { sucursal = y.SUCURSAL1, idSucursal = y.ID_SUCURSAL, latitud = y.LATITUD, longitud = y.LONGITUD, direccion = y.DIRECCION, idSucursalProducto = x.ID_SUCURSAL_PRODUCTO, producto = w.PRODUCTO1 });

                                });
                            });
                        });
                    });


                    foreach (var x in lista)
                    {
                        double lat = Convert.ToDouble(x.latitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double lon = Convert.ToDouble(x.longitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double distance = SearchModel.Distance(data.latitud, data.longitud, lat, lon);
                        if (distance < 2)
                        {
                            ProductSearchModel products = new ProductSearchModel();
                            products.producto = x.producto;
                            products.idSucursalProducto = x.idSucursalProducto;
                            products.idSucursal = x.idSucursal;
                            products.sucursal = x.sucursal;
                            products.latitud = x.latitud;
                            products.longitud = x.longitud;
                            products.direccion = x.direccion;
                            products.distancia = distance;
                            lista2.Add(products);
                        }
                    }
                    return Ok(lista2.OrderBy(x => x.distancia));
                }

                */
                switch (idFarmacia) {
                    case 2:
                            
                        return Ok();
                        
                    case 1013:
                        return Ok();
                       
                    case 1014:
                        return Ok();
                        
                }

            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> ProductSearch(ProductSearchModel data)
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

                List<ProductSearchModel> productos = new List<ProductSearchModel>();

                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {

                    db.SUCURSAL_PRODUCTO.OrderBy(x => x.ID_SUCURSAL_PRODUCTO).ToList().ForEach(x =>
                    {
                        db.PRODUCTO.Where(y => y.ID_PRODUCTO == x.ID_PRODUCTO && y.PRODUCTO1.Contains(data.producto) && x.ID_SUCURSAL == data.idSucursal).ToList().ForEach(z =>
                        {
                            productos.Add(new ProductSearchModel
                            {
                                idSucursalProducto = x.ID_SUCURSAL_PRODUCTO,
                                idSucursal = x.ID_SUCURSAL,
                                producto = z.PRODUCTO1,
                                precio = z.PRECIO
                            });
                        });
                    });
                    return Ok(productos.OrderBy(x => x.precio));
                }
            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> ProductDetail(ProductSearchModel data)
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