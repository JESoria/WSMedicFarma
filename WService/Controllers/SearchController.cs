using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                       // Image image = Image.FromStream(ms);

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
        public async Task<IHttpActionResult> SearchMore (SearchModel data)
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
                try
                {
                    // Declarando los objetos HttpCliente e incrementando el tamaño del buffer  
                    // MaxResponseContentBufferSize, Obtiene o establece el número máximo de bytes que se almacenan en el búfer al leer el contenido de la respuesta.  
                    HttpClient clientA =
                       new HttpClient() { MaxResponseContentBufferSize = 1000000 };
                    HttpClient clientB =
                       new HttpClient() { MaxResponseContentBufferSize = 1000000 };
                    HttpClient clientC =
                       new HttpClient() { MaxResponseContentBufferSize = 1000000 };

                    // Creando e iniciando las tareas. A medida que termina cada tarea, DisplayResults
                    // muestra su longitud. 

                    Task<IHttpActionResult> Farmacia = null;


                    switch (data.idFarmacia)
                    {
                        case 2:
                            Farmacia =
                                ProcessURLAsync(Clients.URLClientA, clientA, data);
                            break;
                        case 1013:
                            Farmacia =
                                 ProcessURLAsync(Clients.URLClientB, clientB, data);
                            break;
                        case 1014:
                            Farmacia =
                                ProcessURLAsync(Clients.URLClientC, clientC, data);
                            break;
                        default:
                            return Ok("3");//no existe la farmacia, la consulta no genero resultado

                    }

                    // Esperando tarea.  
                    IHttpActionResult Farma = await Farmacia;

                    List<ProductSearchModel> listaBusqueda = new List<ProductSearchModel>();
                    using (MedicFarmaEntities db = new MedicFarmaEntities()) {
                        db.consultas.OrderBy(x => x.precio).ToList().ForEach(x =>
                        {
                            listaBusqueda.Add(new ProductSearchModel()
                            {
                                producto = x.producto,
                                precio = Convert.ToDecimal(x.precio),
                                idSucursalProducto = Convert.ToInt32(x.idSucursalProducto),
                                idSucursal = Convert.ToInt32(x.idSucursal),
                                sucursal = x.sucursal,
                                latitud = x.latitud,
                                longitud = x.longitud,
                                direccion = x.direccion,
                                idFarmacia = Convert.ToInt32(x.idFarmacia)
                            });
                        });

                        if (listaBusqueda.Count != 0)
                        {
                            db.consultas.OrderBy(x => x.id).ToList().ForEach(x =>
                            {
                                db.consultas.Remove(x);
                                db.SaveChanges();
                            });

                            return Ok(listaBusqueda);
                        }
                        else
                        {
                            return Ok("1");//Cuando la lista esta vacia y no se encontro el medicamento
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Ok("2");//Cuando se genera una excepcion
                }
            }
        }

        async Task<IHttpActionResult> ProcessURLAsync(string url, HttpClient client, SearchModel data)
        {
            List<ProductSearchModel> lista = null;
            try
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync("api/SearchByDrugstore", data);
                response.EnsureSuccessStatusCode();

                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
                lista = JsonConvert.DeserializeObject<List<ProductSearchModel>>(jsonString.Result);

                if (lista.Count() != 0)
                {

                    foreach (var y in lista)
                    {
                        using (MedicFarmaEntities db = new MedicFarmaEntities())
                        {
                            consultas product = new consultas();
                            product.producto = y.producto;
                            product.precio = y.precio;
                            product.idSucursalProducto = y.idSucursalProducto;
                            product.idSucursal = y.idSucursal;

                            db.consultas.Add(product);
                            await db.SaveChangesAsync();
                        }
                    }
                    lista.Clear();
                    return Ok("1");//Cuando la consulta se realizó correctamente
                }
                else
                {
                    return Ok("3"); //Cuando la consulta no generó resultado
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok("2");   //Cuando se generó una excepcion
            }
        }

    }
}