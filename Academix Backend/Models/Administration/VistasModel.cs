using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class VistasModel
    {
        public int idVista { get; set; }
        public string Vista { get; set; }
        public string DescVista { get; set; }
        public string URL { get; set; }
        public bool Principal { get; set; }
        public bool Permiso { get; set; }
        public string Icon { get; set; }
    }
}
