using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class DataPedidoModel
    {
        public int idPedido { get; set; }
        public string codigoPedido { get; set; }
        public string sucursal { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public decimal montoCompra { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
    }

    public class OrdenCompra {
        public Pedido pedidos  { get; set; }
        public List<DetallePedido> detallePedido { get; set; }
    }

    public class Pedido {
        public string codigoPedido { get; set; }
        public int idusuario { get; set; }
        public int idsucursal  { get; set; }
        public string direccion { get; set; }
        public string telefono { get; set; }
        public double montoCompra { get; set; }
        public string estadoPago { get; set; }
        public string estadoPedido { get; set; }
        public string correo { get; set; }
        public string nombres { get; set; }
        public string apellidos  { get; set; }
        public DateTime fecha { get; set; }
    }

    public class DetallePedido 
    {
        public int idSucursalProducto { get; set; }
        public int idFarmacia { get; set; }
        public int cantidad { get; set; }
        public string producto { get; set; }
        public double precio { get; set; }
    }
}