using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class SolicitudesSoporteModel
    {
        public int idSolicitud { get; set; }
        public int idUsuario { get; set; }
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Debe especificar un mensaje para la solicitud")]
        public string Mensaje { get; set; }
        public System.DateTime FechaSolicitud { get; set; }
        public int idEstatus { get; set; }
        public string Estatus { get; set; }
        public System.DateTime FechaUltimoEstatus { get; set; }
        public int idAsignadoA { get; set; }
        public string AsignadoA { get; set; }
        public string Respuesta { get; set; }
    }
}
