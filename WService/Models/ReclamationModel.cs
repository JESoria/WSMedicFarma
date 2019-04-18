using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class ReclamationModel
    {
        public int idPedido  { get; set; }
        public string incidencia  { get; set; }
        public DateTime FechaIncidencia  { get; set; }
        public DateTime FechaResuelto  { get; set; }
        public string observacion  { get; set; }
    }
}