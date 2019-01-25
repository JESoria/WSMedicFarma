using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class ProductsModel
    {
        public string sucursal { get; set; }
        public string producto { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public double precio { get; set; }
        public int idSucursalProducto { get; set; }
        public double distancia { get; set; }
    }
}