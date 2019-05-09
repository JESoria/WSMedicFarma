using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mail;
using WService.Code;
using System.Net.Mail;
using DotLiquid;
using WService.Models;
using System.Security.Claims;

namespace WService.Controllers
{
    public class TicketController : ApiController
    {
        private DataAccess da = new DataAccess();
        
        [HttpPost]
        public async Task<IHttpActionResult> SendEmailM (TicketModel data)
        {
            
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
                return Ok("1");
            }   
        
    }
}
