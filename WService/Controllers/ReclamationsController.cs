using System;
using System.Collections.Generic;
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
    public class ReclamationsController : ApiController
    {
        private DataAccess da = new DataAccess();

        [HttpPost]
        public async Task<IHttpActionResult> addReclamation(ReclamationModel model) {

            try
            {
                IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
                string header = headerValues.FirstOrDefault();
                t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

                if (token == null)
                {
                    return null;
                }
                if (model == null)
                {
                    return null;
                }
                else
                {
                    using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                    {
                        INCIDENCIA i = new INCIDENCIA();
                        i.ID_PEDIDO = model.idPedido;
                        i.INCIDENCIA1 = model.incidencia;
                        i.FECHA_INCIDENCIA = DateTime.Now;
                        i.ESTADO = "SIN RESOLVER";
                        db.INCIDENCIA.Add(i);
                        await db.SaveChangesAsync();
                    }
                    return Ok("1");
                }
            }
            catch (Exception e) {
                return NotFound();
            }          
        }
    }
}
