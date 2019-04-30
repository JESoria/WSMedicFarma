using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class UsuarioModel
    {
        public int id_usuario { get; set; }
        public string correo { get; set; }
        public string password { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string genero { get; set; }
        public string fecha_nacimiento { get; set; }
        public string facebook_id { get; set; }
        public string estado { get; set; }
        public string usuario { get; set; }
    }
}