using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using WService.Code;
using Ganss.XSS;
using WService.Models;

namespace WService.Controllers
{
    [Authorize]
    public class RegisterEmployeeController : ApiController
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
                model.estado = bool.Parse(sanitizer.Sanitize(model.estado.ToString()));

                 using (MedicFarmaEntities db = new MedicFarmaEntities())
                 {
                     try
                     {
                         var exist = db.EMPLEADO.FirstOrDefault(x => x.USUARIO == model.usuario);

                         if (exist == null)
                         {
                             model.password = Encoder.Encodig(model.password);

                             EMPLEADO emp  = new EMPLEADO();
                             emp.NOMBRES = model.nombres;
                             emp.APELLIDOS = model.apellidos;
                             emp.USUARIO = model.usuario;
                             emp.PASSWORD = model.password;

                             db.EMPLEADO.Add(emp);
                             await db.SaveChangesAsync();

                             return Ok(new RespuestaGenerica() { mensaje = 1 }); //Registro exitoso
                         }
                         else
                         {
                             return Ok(new RespuestaGenerica() { mensaje = 2 }); //El usuario ya existe
                         }



                     }
                     catch (Exception ex)
                     {
                         return BadRequest("no se inserto el usuario" + ex);
                         throw;
                     }


                 }
            }

        }
    }
}
