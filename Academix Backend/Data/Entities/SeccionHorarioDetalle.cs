//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class SeccionHorarioDetalle
    {
        public int idSeccionHorario { get; set; }
        public int idSecciom { get; set; }
        public int idAula { get; set; }
        public int idDia { get; set; }
        public System.TimeSpan horaDesde { get; set; }
        public System.TimeSpan horaHasta { get; set; }
    }
}