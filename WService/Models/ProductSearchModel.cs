using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class ProductSearchModel
    {
        public string producto { get; set; }
        public decimal precio { get; set; }
        public int idSucursalProducto { get; set; }
        public int idSucursal { get; set; }
        public string sucursal  { get; set; }
        public string latitud  { get; set; }
        public string longitud  { get; set; }
        public string direccion { get; set; }
        public double distancia  { get; set; }
    }
}