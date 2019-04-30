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
            T_OAUTHTOKEN token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

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
                model.estado = sanitizer.Sanitize(model.estado);

                using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                {
                    try
                    {
                        var exist = db.USUARIO.FirstOrDefault(x => x.CORREO == model.correo);

                        if (exist == null)
                        {
                            USUARIO usuario = new USUARIO();
                            usuario.CORREO = model.correo;
                            usuario.NOMBRES = model.nombres;
                            usuario.APELLIDOS = model.apellidos;
                            usuario.GENERO = model.genero;
                            usuario.FECHA_NACIMIENTO = DateTime.ParseExact(model.fecha_nacimiento, "yyyy-MM-dd", System.Globalization.CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
                            usuario.FACEBOOK_ID = model.facebook_id;

                            db.USUARIO.Add(usuario);
                            await db.SaveChangesAsync();

                            CREDENCIAL_USUARIO credencial_usuario = new CREDENCIAL_USUARIO();

                            model.password = Encoder.Encodig(model.password);

                            credencial_usuario.ID_USUARIO = usuario.ID_USUARIO;
                            credencial_usuario.PASSWORD = model.password;
                            credencial_usuario.ESTADO = model.estado;

                            db.CREDENCIAL_USUARIO.Add(credencial_usuario);
                            await db.SaveChangesAsync();

                            return Ok(new RespuestaGenerica() { mensaje = 1 }); //Registro exitoso
                        }
                        else {
                            return Ok(new RespuestaGenerica() { mensaje = 2 }); //El usuario ya existe
                        }

                       

                    }
                    catch (Exception ex) {
                        return BadRequest("no se inserto el usuario" + ex);
                        throw;
                    }
                
                    
                }
            }

        }
    }
}
