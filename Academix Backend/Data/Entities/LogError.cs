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
    
    public partial class LogError
    {
        public int idLogError { get; set; }
        public int idUsuario { get; set; }
        public System.DateTime Fecha { get; set; }
        public string Mensaje { get; set; }
        public string StackTrace { get; set; }
        public string Origen { get; set; }
        public string Tipo { get; set; }
    }
}
