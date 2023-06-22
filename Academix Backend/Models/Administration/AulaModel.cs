using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class AulaModel
    {
        public int idAula { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int idTipoAula { get; set; }
        public string TipoAula { get; set; }
    }
}
