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
        /// <summary>
        /// AcademixEntities
        /// </summary>
        public AcadmixEntities academixEntities { get; set; }
        /// <summary>
        /// UsuariosRepo
        /// </summary>
        public UsuariosRepo usuariosRepo { get; set; }
        /// <summary>
        /// EstudiantesRepo
        /// </summary>
        public EstudiantesRepo estudiantesRepo { get; set; }
        /// <summary>
        /// MaestrosRepo
        /// </summary>
        public MaestrosRepo maestrosRepo { get; set; }
        /// <summary>
        /// AsignaturasRepo
        /// </summary>
        public AsignaturasRepo asignaturasRepo { get; set; }
        /// <summary>
        /// CarrerasRepo
        /// </summary>
        public CarrerasRepo carrerasRepo { get; set; }
        /// <summary>
        /// SolicitudesSoporteRepo
        /// </summary>
        public SolicitudesSoporteRepo solicitudesSoporteRepo { get; set; }
        /// <summary>
        /// SeccionAsignaturaRepo
        /// </summary>
        public SeccionAsignaturaRepo seccionAsignaturaRepo { get; set; }
        /// <summary>
        /// API Dashboard
        /// </summary>
        public DashboardController()
        {
            academixEntities = new AcadmixEntities();
            usuariosRepo = new UsuariosRepo(academixEntities);
            estudiantesRepo = new EstudiantesRepo(academixEntities);
            maestrosRepo = new MaestrosRepo(academixEntities);
            asignaturasRepo = new AsignaturasRepo(academixEntities);
            carrerasRepo = new CarrerasRepo(academixEntities);
            solicitudesSoporteRepo = new SolicitudesSoporteRepo(academixEntities);
            seccionAsignaturaRepo = new SeccionAsignaturaRepo(academixEntities);
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

            List<int> ids = academixEntities.Listado_Estudiantes
                           .Where(z => z.idEstudiante == estudiantesDashboard.InfoEstudiante.idEstudiante)
                           .Select(x => x.idSeccion).ToList();

            estudiantesDashboard.AsignaturasSeleccionadas = seccionAsignaturaRepo.Get(x => ids.Contains(x.idSeccion)).ToList();

            return estudiantesDashboard;
        }

        /// <summary>
        /// Obtiene toda la información principal del home de Maestros.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Maestros")]
        [Autorizar(VistasEnum.Inicio)]
        public MaestrosDashboardModel Maestros()
        {
            int idUsuarioOnline = OnlineUser.GetUserId();

            MaestrosDashboardModel maestrosDashboard = new MaestrosDashboardModel();
            maestrosDashboard.InfoMaestro = maestrosRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            maestrosDashboard.InfoUsuario = usuariosRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            maestrosDashboard.Secciones = seccionAsignaturaRepo.Get(x => x.idMaestro == maestrosDashboard.InfoMaestro.idMaestro).ToList();

            return maestrosDashboard;
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

