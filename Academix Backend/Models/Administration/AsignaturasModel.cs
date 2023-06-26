using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class AsignaturasModel
    {
        public int idAsignatura { get; set; }
        public string NombreAsignatura { get; set; }
        public string CodigoAsignatura { get; set; }
        public int idArea { get; set; }
        public string Area { get; set; }
        public int Creditos { get; set; }
        public int idCarrera { get; set; }
    }
}
