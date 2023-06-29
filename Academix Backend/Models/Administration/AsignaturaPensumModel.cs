using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class AsignaturaPensumModel
    {
        public int idAsignaturaPensum { get; set; }
        [Required(ErrorMessage = "Debe especificar las asignaturas")]
        public int idPensum { get; set; }
        public int idTrimestre { get; set; }
        public string Trimestre { get; set; }
        public List<AsignaturasModel> Asignaturas { get; set; }
    }
}
