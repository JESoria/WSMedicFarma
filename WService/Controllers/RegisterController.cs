using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using WService.Code;
using Ganss.XSS;
using WService.Models;
using WService.Models.Response;

namespace WService.Controllers
{
    [Authorize]
    public class RegisterController : ApiController
    {
        private DataAccess da = new DataAccess();

        [HttpPost]
        public async Task<IHttpActionResult> Index(UsuarioModel model)
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null)
            {
                return BadRequest();
            }
            if (model == null)
            {
                return BadRequest();
            }
            else
            {
                var sanitizer = new HtmlSanitizer();
                model.correo = sanitizer.Sanitize(model.correo);
                model.password = sanitizer.Sanitize(model.password);
                model.nombres = sanitizer.Sanitize(model.nombres);
                model.apellidos = sanitizer.Sanitize(model.apellidos);
                model.genero = sanitizer.Sanitize(model.genero);
                model.fecha_nacimiento = sanitizer.Sanitize(model.fecha_nacimiento);
                model.facebook_id = sanitizer.Sanitize(model.facebook_id);

                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    USUARIO usuario = new USUARIO();
                    usuario.CORREO = model.apellidos;
                    usuario.NOMBRES = model.nombres;
                    usuario.APELLIDOS = model.apellidos;
                    usuario.GENERO = model.genero;
                    usuario.FECHA_NACIMIENTO = DateTime.Parse(model.fecha_nacimiento);
                    usuario.FACEBOOK_ID = int.Parse(model.fecha_nacimiento);

                    db.USUARIO.Add(usuario);
                    db.SaveChanges();

                    CREDENCIAL_USUARIO credencial_usuario = new CREDENCIAL_USUARIO();

                    ////var sqlUser = db.USUARIO.FirstOrDefault(x => x.CORREO == model.correo
                    //        && x.NOMBRES == model.nombres);

                    credencial_usuario.ID_USUARIO = usuario.ID_USUARIO;
                    credencial_usuario.PASSWORD = model.password;
                    db.CREDENCIAL_USUARIO.Add(credencial_usuario);
                    db.SaveChanges();

                    var sqlCompleted = db.CREDENCIAL_USUARIO.FirstOrDefault(x => x.ID_USUARIO == credencial_usuario.ID_USUARIO);

                    if (sqlCompleted != null) {
                        return Ok("Usuario Agregado Satisfactoriamente");
                    }
                    else {
                        return BadRequest("Ocurrio un error");
                    }
                }
            }

        }
    }
}
