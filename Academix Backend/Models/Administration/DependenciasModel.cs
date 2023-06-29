using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class DependenciasModel
    {
        public int idDependencia { get; set; }
        public int idAsignatura { get; set; }
        public string Asignatura { get; set; }
        public int idPrerrequisito { get; set; }
        public string Prerrequisito { get; set; }
    }
}
