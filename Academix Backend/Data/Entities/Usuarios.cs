//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Usuarios
    {
        public int idUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string CorreoElectronico { get; set; }
        public string Password { get; set; }
        public int idPerfil { get; set; }
        public int idEstado { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public System.DateTime UltimoIngreso { get; set; }
    }
}
