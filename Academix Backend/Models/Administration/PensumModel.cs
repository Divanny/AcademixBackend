using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class PensumModel
    {
        public int idPensum { get; set; }
        public string nombrePensum { get; set; }
        public string anioPensum { get; set; }
        public string descripcion { get; set; }
        public int idCarrera { get; set; }
        public string Carrera { get; set; }
        public int CantAsignaturas { get; set; }
        public int limiteCreditoTrimestral { get; set; }
        public bool esActivo { get; set; }
    }
}
