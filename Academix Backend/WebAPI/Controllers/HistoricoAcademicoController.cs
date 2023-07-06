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
using System.Web.UI.WebControls;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para ver el HistoricoAcademico del estudiante
    /// </summary>
    [RoutePrefix("api/HistoricoAcademico")]
    public class HistoricoAcademicoController : ApiBaseController
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
        /// SeccionAsignaturaRepo
        /// </summary>
        public SeccionAsignaturaRepo seccionAsignaturaRepo { get; set; }
        /// <summary>
        /// ListadoEstudianteRepo
        /// </summary>
        public ListadoEstudiantesRepo listadoEstudianteRepo { get; set; }
        /// <summary>
        /// API Dashboard
        /// </summary>
        public HistoricoAcademicoController()
        {
            academixEntities = new AcadmixEntities();
            usuariosRepo = new UsuariosRepo(academixEntities);
            seccionAsignaturaRepo = new SeccionAsignaturaRepo(academixEntities);
            listadoEstudianteRepo = new ListadoEstudiantesRepo(academixEntities);
        }

        /// <summary>
        /// Obtiene el HistoricoAcademico del estudiante
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Autorizar(VistasEnum.HistorialAcademico)]
        public object Get()
        {
            int idUsuario = OnlineUser.GetUserId();
            UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == idUsuario).FirstOrDefault();
            var Listado = listadoEstudianteRepo.Get(x => x.idEstudiante == usuario.InfoEstudiante.idEstudiante);
            Utilities utilities = new Utilities();

            List<HistorialAcademicoModel> HistorialAcademicoModel = new List<HistorialAcademicoModel>();

            foreach (var asignaturaTrimestre in Listado)
            {
                AsignaturasRepo asignaturasRepo = new AsignaturasRepo();
                var infoAsignatura = asignaturasRepo.Get(x => x.idAsignatura == asignaturaTrimestre.idAsignatura).FirstOrDefault();

                SeccionAsignaturaRepo seccionesRepo = new SeccionAsignaturaRepo();
                var infoSeccion = seccionesRepo.Get(x => x.idSeccion == asignaturaTrimestre.idSeccion).FirstOrDefault();

                HistorialAcademicoModel.Add(new HistorialAcademicoModel
                {
                    materiaId = infoAsignatura.idAsignatura,
                    descripcion = asignaturaTrimestre.nombreAsignatura,
                    codigoAsignatura = infoAsignatura.CodigoAsignatura,
                    literal = utilities.getCalificacionLiteral(asignaturaTrimestre.calificacion),
                    fechaAprobada = asignaturaTrimestre.fechaPublicacion ?? DateTime.Now,
                    prerrequisito = infoAsignatura.Dependencias.Select(x => x.Prerrequisito).ToList(),
                    trimestreID = asignaturaTrimestre.idPeriodo,
                    descripcionTrimestre = asignaturaTrimestre.nombrePeriodo + " " + asignaturaTrimestre.anioPeriodo,
                    maestroID = infoSeccion.idMaestro,
                    nombreMaestro = infoSeccion.Maestro,
                    estado = (asignaturaTrimestre.calificacion >= 70) ? "Aprobada" : "Reprobada",
                });
            }

            return HistorialAcademicoModel;
        }
    }
}

