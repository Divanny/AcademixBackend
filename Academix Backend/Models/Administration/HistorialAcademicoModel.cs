using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class HistorialAcademicoModel
    {
        public int materiaId { get; set; }
        public string descripcion { get; set; }
        public string codigoAsignatura { get; set; }
        public string literal { get; set; }
        public DateTime fechaAprobada { get; set; }
        public List<string> prerrequisito { get; set; }
        public int trimestreID { get; set; }
        public string descripcionTrimestre { get; set; }
        public int maestroID { get; set; }
        public string nombreMaestro { get; set; }
        public string estado { get; set; }
    }
}
