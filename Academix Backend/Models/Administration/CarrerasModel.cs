using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class CarrerasModel
    {
        public int idCarrera { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int duracionTrimestres { get; set; }
        public int creditos { get; set; }
        public int idArea { get; set; }
        public string Area { get; set; }
        public int CantEstudiantes { get; set; }
        public bool esActivo { get; set; }
    }
}
