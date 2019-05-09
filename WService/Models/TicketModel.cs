using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class TicketModel
    {
        public string usuario { get; set; }
        public string sucursal { get; set; }
        public int idPedido { get; set; }
        public string codigoPaypal { get; set; }
        public string nombresUsuario { get; set; }
        public ICollection<DetallePedido> detalle { get; set; }
        public decimal total { get; set; }
        public string email { get; set; }
        public string det { get; set; }
        public string titulos { get; set; }
    }
}