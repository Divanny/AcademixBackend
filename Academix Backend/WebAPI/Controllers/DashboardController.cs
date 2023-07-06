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
using Data.Common;
using OnlineUser = WebAPI.Infraestructure.OnlineUser;

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
            var idEstudiante = academixEntities.Estudiante
                           .Where(z => z.idUsuario == idUsuarioOnline)
                           .Select(x => x.idEstudiante)
                           .FirstOrDefault();
            Utilities utilities1 = new Utilities();
            var periodo = utilities1.ObtenerTrimestreActual();

            EstudiantesDashboardModel estudiantesDashboard = new EstudiantesDashboardModel();
            estudiantesDashboard.InfoEstudiante = estudiantesRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            estudiantesDashboard.InfoEstudiante.IndiceTrimestral = GetMiIndiceTrimestral();
            estudiantesDashboard.InfoEstudiante.IndiceGeneral = GetMiIndiceGeneral();
            estudiantesDashboard.InfoEstudiante.CreditosAprobados = GetCreditosAprobados();
            estudiantesDashboard.InfoEstudiante.CreditosTotales = GetCreditosTotales(estudiantesDashboard.InfoEstudiante.idPensum);
            estudiantesDashboard.InfoEstudiante.HonorAcademico = GetMiHonorAcademico();
            estudiantesDashboard.InfoUsuario = usuariosRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();

            List<int> ids = academixEntities.Listado_Estudiantes
                           .Where(z => z.idEstudiante == estudiantesDashboard.InfoEstudiante.idEstudiante)
                           .Select(x => x.idSeccion).ToList();

            estudiantesDashboard.AsignaturasSeleccionadas = seccionAsignaturaRepo.Get(x => ids.Contains(x.idSeccion)).ToList();
            Utilities utilities = new Utilities();

            foreach(var asignaturaSeleccionada in estudiantesDashboard.AsignaturasSeleccionadas)
            {
                var idListadoEstudiante = academixEntities.Listado_Estudiantes
                                .Where(x => x.idEstudiante == idEstudiante && x.idPeriodo == periodo && x.anioPeriodo == DateTime.Now.Year && x.idSeccion == asignaturaSeleccionada.idSeccion).Select(x => x.idListadoEstudiante).FirstOrDefault();

                var calificacion = academixEntities.Publicacion
                                    .Where(x => x.idListadoEstudiante == idListadoEstudiante)
                                    .Select(z => z.idCalificacion).FirstOrDefault();

                int actualPosicion = estudiantesDashboard.AsignaturasSeleccionadas.IndexOf(asignaturaSeleccionada);
                estudiantesDashboard.AsignaturasSeleccionadas[actualPosicion].calificacion = calificacion;
                estudiantesDashboard.AsignaturasSeleccionadas[actualPosicion].calificacionLiteral = utilities.getCalificacionLiteral(calificacion);
            }

            var trimestreActual = utilities.ObtenerTrimestreActual();
            string PeriodoDesde = "", PeriodoHasta = "", PeriodoAño = "";
            if (trimestreActual == (int)PeriodosEnum.febero_abril)
            {
                PeriodoDesde = "Febrero";
                PeriodoHasta = "Abril";
                PeriodoAño =  DateTime.Now.Year.ToString();
            }
            else if (trimestreActual == (int)PeriodosEnum.mayo_julio)
            {
                PeriodoDesde = "Mayo";
                PeriodoHasta = "Julio";
                PeriodoAño = DateTime.Now.Year.ToString();
            }
            else if (trimestreActual == (int)PeriodosEnum.agosto_octubre)
            {
                PeriodoDesde = "Agosto";
                PeriodoHasta = "Octubre";
                PeriodoAño =  DateTime.Now.Year.ToString();
            }
            else if (trimestreActual == (int)PeriodosEnum.noviembre_enero)
            {
                PeriodoDesde = "Noviembre";
                PeriodoHasta = "Enero";
                PeriodoAño = DateTime.Now.Year.ToString();
            }

            estudiantesDashboard.PeriodoDesde = PeriodoDesde;
            estudiantesDashboard.PeriodoHasta = PeriodoHasta;
            estudiantesDashboard.PeriodoAño = PeriodoAño;

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

        /// <summary>
        /// Obtiene el indice trimestral del Estudiante
        /// </summary>
        /// <returns></returns>
        private double GetMiIndiceTrimestral()
        {
            using (AcadmixEntities dbc = new AcadmixEntities())
            {
                ListadoEstudiantesRepo listadoEstudiantesRepo = new ListadoEstudiantesRepo(dbc);
                Utilities utilities = new Utilities();

                List<Listado_Estudiantes> ids = listadoEstudiantesRepo.getListadoByEstudiante();
                double indiceTrimestal = 0, sumaPuntos = 0;
                int sumaCreditos = 0, creditos = 0;

                foreach (var item in ids)
                {
                    int calificacion = dbc.Publicacion
                                    .Where(x => x.idListadoEstudiante == item.idListadoEstudiante)
                                    .Select(x => x.idCalificacion).FirstOrDefault();

                    if (calificacion != 0)
                    {
                        string literal = utilities.getCalificacionLiteral(calificacion);

                        double valorLiteral = utilities.ValorDelLiteral(literal);

                        int idAsignatura = dbc.Seccion_Asignatura
                                            .Where(x => x.idSeccion == item.idSeccion)
                                            .Select(x => x.idAsignatura).FirstOrDefault();

                        creditos = dbc.Asignatura
                                           .Where(x => x.idAsignatura == idAsignatura)
                                           .Select(x => x.creditos).FirstOrDefault();

                        double puntos = creditos * valorLiteral;

                        sumaCreditos += creditos;
                        sumaPuntos += puntos;
                    }
                }
                indiceTrimestal = sumaPuntos / sumaCreditos;
                return indiceTrimestal;
            }
        }
        /// <summary>
        /// Obtiene el indice general del Estudiante
        /// </summary>
        /// <returns></returns>
        private double GetMiIndiceGeneral()
        {
            using (AcadmixEntities dbc = new AcadmixEntities())
            {
                ListadoEstudiantesRepo listadoEstudiantesRepo = new ListadoEstudiantesRepo(dbc);
                Utilities utilities = new Utilities();

                List<Listado_Estudiantes> ids = listadoEstudiantesRepo.getListadoByEstudianteGeneral();
                double indiceTrimestal, sumaPuntos = 0;
                int sumaCreditos = 0, creditos;

                foreach (var item in ids)
                {
                    int calificacion = dbc.Publicacion
                                    .Where(x => x.idListadoEstudiante == item.idListadoEstudiante)
                                    .Select(x => x.idCalificacion).FirstOrDefault();

                    if (calificacion != 0)
                    {
                        string literal = utilities.getCalificacionLiteral(calificacion);

                        double valorLiteral = utilities.ValorDelLiteral(literal);

                        int idAsignatura = dbc.Seccion_Asignatura
                                            .Where(x => x.idSeccion == item.idSeccion)
                                            .Select(x => x.idAsignatura).FirstOrDefault();

                        creditos = dbc.Asignatura
                                           .Where(x => x.idAsignatura == idAsignatura)
                                           .Select(x => x.creditos).FirstOrDefault();

                        double puntos = creditos * valorLiteral;

                        sumaCreditos += creditos;
                        sumaPuntos += puntos;
                    }
                }
                indiceTrimestal = sumaPuntos / sumaCreditos;
                return indiceTrimestal;
            }
        }

        /// <summary>
        /// Get creditos aprobados del Estudiante
        /// </summary>
        /// <returns></returns>
        private int GetCreditosAprobados()
        {
            using (AcadmixEntities dbc = new AcadmixEntities())
            {
                ListadoEstudiantesRepo listadoEstudiantesRepo = new ListadoEstudiantesRepo(dbc);
                Utilities utilities = new Utilities();

                List<Listado_Estudiantes> ids = listadoEstudiantesRepo.getListadoByEstudianteGeneral();
                int sumaCreditos = 0, creditos;

                foreach (var item in ids)
                {
                    int calificacion = dbc.Publicacion
                                    .Where(x => x.idListadoEstudiante == item.idListadoEstudiante)
                                    .Select(x => x.idCalificacion).FirstOrDefault();

                    if (calificacion != 0)
                    {
                        int idAsignatura = dbc.Seccion_Asignatura
                                            .Where(x => x.idSeccion == item.idSeccion)
                                            .Select(x => x.idAsignatura).FirstOrDefault();

                        creditos = dbc.Asignatura
                                           .Where(x => x.idAsignatura == idAsignatura)
                                           .Select(x => x.creditos).FirstOrDefault();

                        sumaCreditos += creditos;
                    }
                }
                return sumaCreditos;
            }
        }

        /// <summary>
        /// Get creditos aprobados del Estudiante
        /// </summary>
        /// <returns></returns>
        private int GetCreditosTotales(int idPensum)
        {
            using (AcadmixEntities dbc = new AcadmixEntities())
            {
                int sumaCreditosTotales = 0;

                List<int> idsAsignaturas = academixEntities.Asignatura_Pensum
                           .Where(z => z.idPensum == idPensum)
                           .Select(x => x.idAsignatura).ToList();

                sumaCreditosTotales = asignaturasRepo.Get(x => idsAsignaturas.Contains(x.idAsignatura)).Sum(x => x.Creditos);

                return sumaCreditosTotales;
            }
        }

        /// <summary>
        /// Get el honor académico del Estudiante
        /// </summary>
        /// <returns></returns>
        private string GetMiHonorAcademico()
        {
            using (AcadmixEntities dbc = new AcadmixEntities())
            {
                ListadoEstudiantesRepo listadoEstudiantesRepo = new ListadoEstudiantesRepo(dbc);
                double indiceGeneral = GetMiIndiceGeneral();
                string HonorAcademico = "";

                if (indiceGeneral >= 3.8)
                {
                    HonorAcademico = "Summa Cum Laude";
                }
                else if (indiceGeneral >= 3.6)
                {
                    HonorAcademico = "Magna Cum Laude";
                }
                else if (indiceGeneral >= 3.4)
                {
                    HonorAcademico = "Cum Laude";
                }
                else
                {
                    HonorAcademico = "N/A";
                }
                return HonorAcademico;
            }
        }
    }
}

