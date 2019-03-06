using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using WService.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using WService.Code;

namespace WService.Controllers
{
    [Authorize]
    public class DetailController : ApiController
    {
        private DataAccess da = new DataAccess();

        [HttpPost]
        public async Task<IHttpActionResult> ProductDetail(SearchModel data)
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null){
                return BadRequest();
            }
            else {
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

                    return Farma;
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
            DetailModel Detalle = null;

            try
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Detail", data);
                response.EnsureSuccessStatusCode();

                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
                Detalle = JsonConvert.DeserializeObject<DetailModel>(jsonString.Result);

                if (Detalle != null){

                    return Ok(Detalle);
                }
                else {
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
