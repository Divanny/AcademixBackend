using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class PublicacionModel
    {
        public int idPublicacion { get; set; }
        public int idListadoEstudiante { get; set; }
        public int idCalificacion { get; set; }
        public System.DateTime fechaPublicacion { get; set; }
    }
}
