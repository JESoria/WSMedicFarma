using Ganss.XSS;
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
using WService.Models.Response;

namespace WService.Controllers
{
    [Authorize]
    public class LoginController : ApiController
    {
        private DataAccess da = new DataAccess();

        [HttpPost]
        public async Task<IHttpActionResult> Index(LoginModel model)
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null)
            {
                return BadRequest();
            }
            else if (model == null)
            {
                return BadRequest();
            }
            else {
                var sanitizer = new HtmlSanitizer();
                model.username = sanitizer.Sanitize(model.username);
                model.passworld = sanitizer.Sanitize(model.passworld);
                using (MedicFarmaEntities db = new MedicFarmaEntities()) {
                    model.passworld = Encoder.Encodig(model.passworld);

                    var userX = db.CREDENCIAL_USUARIO.FirstOrDefault(x => x.PASSWORD == model.passworld
                            && x.USUARIO1.CORREO == model.username);

                    if (userX != null) {
                        return Ok(new ProfileModel() {
                            APELLIDOS = userX.USUARIO1.APELLIDOS,
                            CORREO = userX.USUARIO1.CORREO,
                            FACEBOOK_ID = userX.USUARIO1.FACEBOOK_ID,
                            FECHA_NACIMIENTO = userX.USUARIO1.FECHA_NACIMIENTO,
                            GENERO = userX.USUARIO1.GENERO,
                            ID_USUARIO = userX.USUARIO1.ID_USUARIO,
                            NOMBRES = userX.USUARIO1.NOMBRES
                        });
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
            }
            
        }
    }
}
