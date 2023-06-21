using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class MaestroModel
    {
        public int idMaestro { get; set; }
        public int idUsuario { get; set; }
        public string credencial { get; set; }
        public int idArea { get; set; }
        public string Area { get; set; }
        
    }
}
