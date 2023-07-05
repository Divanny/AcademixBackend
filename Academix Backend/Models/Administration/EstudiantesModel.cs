using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class EstudiantesModel
    {
        public int idEstudiante { get; set; }
        public int idUsuario { get; set; }
        public int idCarrera { get; set; }
        public string Carrera { get; set; }
        public int idArea { get; set; }
        public string Area { get; set; }
        public string Identificacion { get; set; }
        public int trimestresCursados { get; set; }
        public int asignaturasAprobadas { get; set; }
        public int idPensum { get; set; }
        public string Pensum { get; set; }
        public double IndiceTrimestral { get; set; }
        public double IndiceGeneral { get; set; }
        public int CreditosAprobados { get; set; }
        public int CreditosTotales { get; set; }
        public string HonorAcademico { get; set; }
    }
}
