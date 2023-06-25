using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class SeccionHorarioDetalleModel
    {
        public int idSeccionHorario { get; set; }
        public int idSecciom { get; set; }
        public int idAula { get; set; }
        public string Aula { get; set; }
        public int idDia { get; set; }
        public string Dia { get; set; }
        public System.TimeSpan horaDesde { get; set; }
        public System.TimeSpan horaHasta { get; set; }
    }
}
