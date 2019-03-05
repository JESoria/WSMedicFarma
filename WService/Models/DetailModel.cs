using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class DetailModel
    {
        public string producto { get; set; }
        public string presentacion { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public string laboratorio { get; set; }
        public string principiosActivos { get; set; }
        public string categoria { get; set; }
        public double precio { get; set; }
        public int existencia { get; set; }
    }
}