//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WService.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ADMINISTRADOR_FARMACIA
    {
        public int ID_ADMINISTRADOR { get; set; }
        public string CODIGO_ADMINISTRADOR { get; set; }
        public int ID_FARMACIA { get; set; }
        public string NOMBRES { get; set; }
        public string APELLIDOS { get; set; }
        public string USUARIO { get; set; }
        public string PASSWORD { get; set; }
        public string DUI { get; set; }
        public string NIT { get; set; }
        public string TELEFONO { get; set; }
        public string DIRECCION { get; set; }
    
        public virtual FARMACIA FARMACIA { get; set; }
    }
}
