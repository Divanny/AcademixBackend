using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class EstudiantesDashboardModel
    {
        public EstudiantesModel InfoEstudiante { get; set; }
        public UsuariosModel InfoUsuario { get; set; }
        public List<SeccionAsignaturaModel> AsignaturasSeleccionadas { get; set; }
        public string PeriodoDesde { get; set; }    
        public string PeriodoHasta { get; set; }    
        public string PeriodoAño { get; set; }    

    }
}
