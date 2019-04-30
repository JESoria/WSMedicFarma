using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    public class SearchByDrugstoreController : ApiController
    {
        private DataAccess da = new DataAccess(); 

        [HttpPost]
        public async Task<IHttpActionResult> SearchDrugstore (SearchModel data)
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            T_OAUTHTOKEN token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

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

                    Task<int> Farmacia = null;


                    switch (data.idFarmacia)
                    {
                        case 2:
                            Farmacia =
                                ProcessURLAsync(Clients.URLClientA, clientA, data);
                            break;
                        case 1014:
                            Farmacia =
                                 ProcessURLAsync(Clients.URLClientB, clientB, data);
                            break;
                        case 1013:
                            Farmacia =
                               ProcessURLAsync(Clients.URLClientC, clientC, data);
                            break;
                        default:
                            return Ok("3");//no existe la farmacia, la consulta no genero resultado

                    }

                    // Esperando tarea.  
                    int Farma = await Farmacia;


                    List<ProductSearchModel> listaBusqueda = new List<ProductSearchModel>();
                    using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                    {

                        db.CONSULTAS.OrderBy(x => x.PRECIO).ToList().ForEach(x =>
                        {
                            listaBusqueda.Add(new ProductSearchModel()
                            {
                                producto = x.PRODUCTO,
                                precio = Convert.ToDecimal(x.PRECIO),
                                idSucursalProducto = Convert.ToInt32(x.ID_SUCURSAL_PRODUCTO),
                                idSucursal = Convert.ToInt32(x.ID_SUCURSAL),
                                sucursal = x.SUCURSAL,
                                latitud = x.LATITUD,
                                longitud = x.LONGITUD,
                                direccion = x.DIRECCION,
                                idFarmacia = Convert.ToInt32(x.ID_FARMACIA)
                            });
                        });

                        if (listaBusqueda.Count != 0)
                        {
                            db.CONSULTAS.OrderBy(x => x.ID).ToList().ForEach(x =>
                            {
                                db.CONSULTAS.Remove(x);
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

        async Task<int> ProcessURLAsync(string url, HttpClient client, SearchModel data)
        {
            List<ProductSearchModel> lista = null;
            try
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Search", data);
                response.EnsureSuccessStatusCode();

                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
                lista = JsonConvert.DeserializeObject<List<ProductSearchModel>>(jsonString.Result);

                if (lista.Count() != 0)
                {

                    foreach (var y in lista)
                    {
                        using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                        {
                            CONSULTAS product = new CONSULTAS();
                            product.PRODUCTO = y.producto;
                            product.PRECIO = y.precio;
                            product.ID_SUCURSAL_PRODUCTO = y.idSucursalProducto;
                            product.ID_SUCURSAL = y.idSucursal;
                            product.LATITUD = y.latitud;
                            product.LONGITUD = y.longitud;
                            product.DIRECCION = y.direccion;
                            product.DISTANCIA = Convert.ToDecimal(y.distancia);
                            product.SUCURSAL = y.sucursal;
                            product.ID_FARMACIA = y.idFarmacia;

                            db.CONSULTAS.Add(product);
                            await db.SaveChangesAsync();
                        }
                    }
                    lista.Clear();
                    return 1;//Cuando la consulta se realizó correctamente
                }
                else
                {
                    return 3; //Cuando la consulta no generó resultado
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 2;   //Cuando se generó una excepcion
            }
        }
    }
}
