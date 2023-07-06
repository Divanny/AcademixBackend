using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class SeccionAsignaturaModel
    {
        public int idSeccion { get; set; }
        public int codigoSeccion { get; set; }
        public string descripcion { get; set; }
        public int idModalidad { get; set; }
        public string Modalidad { get; set; }
        public int capacidadMax { get; set; }
        public int idAsignatura { get; set; }
        public string Asignatura { get; set; }
        public string codigoAsignatura { get; set; }
        public int idMaestro { get; set; }
        public string Maestro { get; set; }
        public int creditos { get; set; } 
        public bool esActivo { get; set; }
        public IEnumerable <SeccionHorarioDetalleModel> detalleSeccion { get; set; }
        public int cantidadEstudiantes { get; set; }
        public int calificacion { get; set; }
    }
}
