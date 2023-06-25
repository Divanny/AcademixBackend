using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class EstudiantesDashboard
    {
        public EstudiantesModel InfoEstudiante { get; set; }
        public UsuariosModel InfoUsuario { get; set; }
        public object AsignaturasSeleccionadas { get; set; }
    }
}
