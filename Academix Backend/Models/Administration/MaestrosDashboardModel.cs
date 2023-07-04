using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class MaestrosDashboardModel
    {
        public MaestroModel InfoMaestro { get; set; }
        public UsuariosModel InfoUsuario { get; set; }
        public List<SeccionAsignaturaModel> Secciones { get; set; }
    }
}
