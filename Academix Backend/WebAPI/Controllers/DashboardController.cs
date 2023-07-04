using Data.Administration;
using Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Infraestructure;
using System.Web.Http;
using Data.Entities;
using System.Security.Cryptography.X509Certificates;
using Swashbuckle.Swagger;
using Models.Common;
using System.Globalization;
using Models.Enums;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los diferentes Dashboard
    /// </summary>
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiBaseController
    {
        public AcadmixEntities academixEntities { get; set; }
        public UsuariosRepo usuariosRepo { get; set; }
        public EstudiantesRepo estudiantesRepo { get; set; }
        public MaestrosRepo maestrosRepo { get; set; }
        public AsignaturasRepo asignaturasRepo { get; set; }
        public CarrerasRepo carrerasRepo { get; set; }
        public SolicitudesSoporteRepo solicitudesSoporteRepo { get; set; }
        public DashboardController()
        {
            academixEntities = new AcadmixEntities();
            usuariosRepo = new UsuariosRepo(academixEntities);
            estudiantesRepo = new EstudiantesRepo(academixEntities);
            maestrosRepo = new MaestrosRepo(academixEntities);
            asignaturasRepo = new AsignaturasRepo(academixEntities);
            carrerasRepo = new CarrerasRepo(academixEntities);
            solicitudesSoporteRepo = new SolicitudesSoporteRepo(academixEntities);
        }

        /// <summary>
        /// Obtiene toda la información principal del home de Estudiantes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Estudiantes")]
        [Autorizar(VistasEnum.Inicio)]
        public EstudiantesDashboardModel Estudiantes()
        {
            int idUsuarioOnline = OnlineUser.GetUserId();

            EstudiantesDashboardModel estudiantesDashboard = new EstudiantesDashboardModel();
            estudiantesDashboard.InfoEstudiante = estudiantesRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            estudiantesDashboard.InfoUsuario = usuariosRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            estudiantesDashboard.AsignaturasSeleccionadas = new List<AsignaturasModel>();

            return estudiantesDashboard;
        }

        [HttpGet]
        [Route("Maestro")]
        [Autorizar(VistasEnum.Inicio)]
        public EstudiantesDashboardModel Maestro()
        {
            int idUsuarioOnline = OnlineUser.GetUserId();

            EstudiantesDashboardModel estudiantesDashboard = new EstudiantesDashboardModel();
            estudiantesDashboard.InfoEstudiante = estudiantesRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            estudiantesDashboard.InfoUsuario = usuariosRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            estudiantesDashboard.AsignaturasSeleccionadas = new List<AsignaturasModel>();

            return estudiantesDashboard;
        }

        /// <summary>
        /// Obtiene toda la información principal del home de Administradores.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Administrador")]
        [Autorizar(VistasEnum.Inicio)]
        public AdministradorDashboardModel Administrador()
        {
            AdministradorDashboardModel administradorDashboard = new AdministradorDashboardModel();
            administradorDashboard.CantidadMaestros = maestrosRepo.Get().Count();
            administradorDashboard.CantidadEstudiantes = estudiantesRepo.Get().Count();
            administradorDashboard.CantidadAsignaturas = asignaturasRepo.Get().Count();
            administradorDashboard.CantidadSolicitudSoporte = solicitudesSoporteRepo.Get(x => x.idEstatus == (int)EstatusSolicitudSoporteEnum.PendienteaRevisión).Count();


            List<CarreraEstudiante> carreraEstudiantes = new List<CarreraEstudiante>();

            carrerasRepo.Get().ToList().ForEach(x =>
                carreraEstudiantes.Add(new CarreraEstudiante()
                {
                    idCarrera = x.idCarrera,
                    Carrera = x.nombre,
                    CantidadEstudiantes = x.CantEstudiantes,
                })
            );

            administradorDashboard.GraficaNumeroEstudiantes = carreraEstudiantes.OrderByDescending(x => x.CantidadEstudiantes).ToList();

            List<SolicitudesPorMes> solicitudesPorMes = new List<SolicitudesPorMes>();
            List<SolicitudesSoporteModel> solicitudesSoporte = solicitudesSoporteRepo.Get().ToList();

            Dictionary<int, int> cantidadPorMes = new Dictionary<int, int>();

            foreach (var solicitud in solicitudesSoporte)
            {
                int mes = solicitud.FechaSolicitud.Month;

                if (cantidadPorMes.ContainsKey(mes))
                {
                    cantidadPorMes[mes]++;
                }
                else
                {
                    cantidadPorMes[mes] = 1;
                }
            }

            foreach (var item in cantidadPorMes)
            {
                string nombreMes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Key);
                solicitudesPorMes.Add(new SolicitudesPorMes()
                {
                    Mes = nombreMes,
                    CantidadSolicitudes = item.Value
                });
            }

            administradorDashboard.GraficaSolicitudesSoporte = solicitudesPorMes;

            return administradorDashboard;
        }
    }
}

