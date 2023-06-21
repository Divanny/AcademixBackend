using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class AreasModel
    {
        public int idArea { get; set; }
        public string nombre { get; set; }
        public string descripcion { get; set; }
        public int idEncargado { get; set; }
        public string Encargado { get; set; }
        public int CantCarreras { get; set; }
        public bool esActivo { get; set; }
    }
}
