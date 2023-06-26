using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Administration
{
    public class AdministradorDashboardModel
    {
        public int CantidadEstudiantes { get; set; }
        public int CantidadMaestros { get; set; }
        public int CantidadAsignaturas { get; set; }
        public int CantidadSolicitudSoporte { get; set; }
        public List<CarreraEstudiante> GraficaNumeroEstudiantes { get; set; }
        public List<SolicitudesPorMes> GraficaSolicitudesSoporte { get; set; }
    }
    public class CarreraEstudiante
    {
        public int idCarrera { get; set; }
        public string Carrera { get; set; }
        public int CantidadEstudiantes { get; set; }
    }
    public class SolicitudesPorMes
    {
        public string Mes { get; set; }
        public int CantidadSolicitudes { get; set; }
    }
}
