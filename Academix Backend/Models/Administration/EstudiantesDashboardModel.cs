using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class EstudiantesDashboardModel
    {
        public int CantidadEstudiantes { get; set; }
        public int CantidadMaestros { get; set; }
        public int CantidadSolicitudSoporte { get; set; }
        public object GraficaNumeroEstudiantes { get; set; }
        public object GraficaSolicitudesSoporte { get; set; }
    }
}
