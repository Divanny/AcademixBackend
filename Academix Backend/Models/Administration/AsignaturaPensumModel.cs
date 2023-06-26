using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class AsignaturaPensumModel
    {
        public int idAsignaturaPensum { get; set; }
        public int idAsignatura { get; set; }
        public string Asignatura { get; set; }
        public int idPensum { get; set; }
        public string Pensum { get; set; }
        public int idTrimestre { get; set; }
    }
}
