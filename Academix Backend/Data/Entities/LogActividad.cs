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
    
    public partial class LogActividad
    {
        public int idLog { get; set; }
        public string URL { get; set; }
        public int idUsuario { get; set; }
        public string Metodo { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Data { get; set; }
    }
}
