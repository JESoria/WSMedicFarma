using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models.Response
{
    public class ProfileModel
    {
        public int ID_USUARIO { get; set; }
        public string NOMBRES { get; set; }
        public string APELLIDOS { get; set; }
        public string GENERO { get; set; }
        public System.DateTime FECHA_NACIMIENTO { get; set; }
        public string CORREO { get; set; }
        public Nullable<int> FACEBOOK_ID { get; set; }

    }
}