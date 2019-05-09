using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class TicketDrop : Drop
    {
        private readonly TicketModel  _ticket;

        public string usuario 
        {
            get { return _ticket.usuario; }
        }

        public string sucursal 
        {
            get { return _ticket.sucursal; }
        }

        public int idPedido 
        {
            get { return _ticket.idPedido; }
        }

        public string codigoPaypal 
        {
            get { return _ticket.codigoPaypal; }
        }

        public string nombresUsuario
        {
            get { return _ticket.nombresUsuario; }
        }

        public decimal total 
        {
            get { return _ticket.total; }
        }

        public string email
        {
            get { return _ticket.email; }
        }

        public string det
        {
            get { return _ticket.det; }
        }

        public string titulos 
        {
            get { return _ticket.titulos; }
        }

        public ICollection<DetallePedido> detalle {
            get { return _ticket.detalle; }
        }

        public TicketDrop (TicketModel ticket )
        {
            _ticket = ticket;
        }
    }

   
}