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
using System.Web.Http.Results;
using WService.Code;
using WService.Models;


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
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

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
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return null;
        }

        [HttpPost]
        public async Task<IHttpActionResult> pedidosList(PEDIDO data) {

            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

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
                        //Que la fecha sea el dia de ahora
                        db.PEDIDO.Where(x => x.ID_USUARIO == IdUsuario).ToList().ForEach(x =>
                        {
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
