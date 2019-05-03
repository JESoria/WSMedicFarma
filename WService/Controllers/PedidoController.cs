using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Net.Mail;
using DotLiquid;

namespace WService.Controllers
{
    [Authorize]
    public class PedidoController : ApiController
    {
        private DataAccess da = new DataAccess();

        [HttpPost]
        public async Task<JObject> addPedidos([FromBody]JObject jsonString) {

            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            T_OAUTHTOKEN token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null) {
                return null;
            }
            if (jsonString == null)
            {
                return null;
            }
            else
            {
                try
                {
                    var j = jsonString["data"].ToString();
                    var model = JsonConvert.DeserializeObject<OrdenCompra>(j);
                    if (model.pedidos.estadoPago.Equals("approved")) {
                        model.pedidos.estadoPago = "Pagado";                                                 
                    }

                        using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                    {
                        PEDIDO data = new PEDIDO();
                        data.CODIGO_PEDIDO = model.pedidos.codigoPedido;
                        data.ID_USUARIO = model.pedidos.idusuario;
                        data.ID_SUCURSAL = model.pedidos.idsucursal;
                        data.DIRECCION = model.pedidos.direccion;
                        data.TELEFONO = model.pedidos.telefono;
                        data.MONTO_COMPRA = Convert.ToDecimal(model.pedidos.montoCompra);
                        data.ESTADO_PAGO = model.pedidos.estadoPago;
                        data.ESTADO_PEDIDO = "SIN ENTREGAR";
                        data.FECHA_RECIBIDO = DateTime.Now;
                        db.PEDIDO.Add(data);
                        await db.SaveChangesAsync();  

                        foreach (var x in model.detallePedido)
                        {
                            DETALLE_PEDIDO det = new DETALLE_PEDIDO();
                            det.ID_PEDIDO = data.ID_PEDIDO;
                            det.CANTIDAD = x.cantidad;
                            det.PRODUCTO = x.producto;
                            det.PRECIO_VENTA = Convert.ToDecimal(x.precio);
                            db.DETALLE_PEDIDO.Add(det);
                            await db.SaveChangesAsync();
                        }

                        if (model.pedidos.estadoPago.Equals("Pagado"))
                        {
                            sendEmail(new TicketModel()
                            {
                                usuario = data.USUARIO.NOMBRES,
                                email = data.USUARIO.CORREO,
                                sucursal = data.SUCURSAL.SUCURSAL1,
                                idPedido = data.ID_PEDIDO,
                                codigoPaypal = data.CODIGO_PEDIDO,
                                nombresUsuario = data.USUARIO.NOMBRES + " " + data.USUARIO.APELLIDOS,
                                total = Convert.ToDecimal(data.MONTO_COMPRA)
                            });
                        }
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return null;
        }


        public void sendEmail(TicketModel data) {

            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new MailAddress("medicfarma.comprasonline@gmail.com");

                // The important part -- configuring the SMTP client
                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;   // [1] You can try with 465 also, I always used 587 and got success
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // [2] Added this
                smtp.UseDefaultCredentials = false; // [3] Changed this
                smtp.Credentials = new NetworkCredential("medicfarma.comprasonline@gmail.com", "utec2019$");  // [4] Added this. Note, first parameter is NOT string.
                smtp.Host = "smtp.gmail.com";

                //recipient address
                mail.To.Add(new MailAddress(data.email));

                //Formatted mail body
                mail.IsBodyHtml = true;
                mail.Subject = "Datos de compra";
                string cid = "image001@gembox.com";
                mail.Attachments.Add(new Attachment("C:/logo.png") { ContentId = cid });
                //
                Template template = Template.Parse(
                    " <p><img src='cid:" + cid + "' width='100' height='100' /></p>" +
                    " <p><strong>¡GRACIAS!</strong></p>" +
                    " <p>Hola  {{ user.usuario }} </p>" +
                    " <p>Gracias por tu compra en {{ user.sucursal }} </p>" +
                    " <p>Información del pedido:</p>" +
                    "<hr>" +
                    " <p>CODIGO DE PEDIDO:<mark> {{user.id_pedido}}</mark> </p>" +
                    " <p>CODIGO PAYPAL: <mark>{{user.codigo_paypal}}</mark></p>" +
                    " <p>FACTURADO A: {{user.nombres_usuario}}</p>" +
                    " <p>TOTAL COMPRA $ {{user.total}}</p>"
                    );
                string result = template.Render(Hash.FromAnonymousObject(new
                {
                    user = new TicketDrop(new TicketModel
                    {
                        usuario = data.usuario,
                        email = data.email,
                        sucursal = data.sucursal,
                        idPedido = data.idPedido,
                        codigoPaypal = data.codigoPaypal,
                        nombresUsuario = data.nombresUsuario,
                        total = data.total
                    })
                }));
                //

                mail.Body = result;
                smtp.Send(mail);               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> pedidosList(PEDIDO data) {

            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            T_OAUTHTOKEN token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null)
            {
                return BadRequest();
            }
            else {
                try
                {
                    using (MEDICFARMAEntities db = new MEDICFARMAEntities()) {
                        List<DataPedidoModel> pedidos = new List<DataPedidoModel>();
                        int IdUsuario = Convert.ToInt32(data.ID_USUARIO);
                        
                        db.PEDIDO.Where(x => x.ID_USUARIO == IdUsuario).ToList().ForEach(x =>
                        {
                            if (db.INCIDENCIA.Any(i => i.ID_PEDIDO == x.ID_PEDIDO))
                            {
                                
                            }
                            else {
                                pedidos.Add(new DataPedidoModel()
                                {
                                    idPedido = x.ID_PEDIDO,
                                    codigoPedido = x.CODIGO_PEDIDO,
                                    sucursal = x.SUCURSAL.SUCURSAL1,
                                    direccion = x.SUCURSAL.DIRECCION,
                                    latitud = x.SUCURSAL.LATITUD,
                                    longitud = x.SUCURSAL.LONGITUD,
                                    telefono = x.SUCURSAL.TELEFONO,
                                    montoCompra = Convert.ToDecimal(x.MONTO_COMPRA),
                                });
                            }                           
                        });
                        if (pedidos.Count() == 0)
                        {
                            return NotFound();
                        }
                        else
                        {
                            return Ok(pedidos);

                        }
                    }
                }
                catch (Exception e) {
                     return Ok(e);
                }
            }
        }
    }
}
