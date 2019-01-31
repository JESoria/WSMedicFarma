using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class ProductSearchModel
    {
        public int idSucursalProducto { get; set; }
        public int idSucursal { get; set; } 
        public string producto { get; set; }
        public decimal precio { get; set; }
    }
}