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

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar el dashboard del Estudiante
    /// </summary>
    [RoutePrefix("api/EstudiantesDashboard")]
    public class EstudiantesDashboardController : ApiBaseController
    {
        public AcadmixEntities academixEntities { get; set; }
        public UsuariosRepo usuariosRepo { get; set; }
        public EstudiantesRepo estudiantesRepo { get; set; }
        public EstudiantesDashboardController()
        {
            academixEntities = new AcadmixEntities();
            usuariosRepo = new UsuariosRepo(academixEntities);
            estudiantesRepo = new EstudiantesRepo(academixEntities);
        }
        /// <summary>
        /// Obtiene toda la información principal del home de Estudiantes.
        /// </summary>
        /// <returns></returns>
        [Autorizar(AllowAnyProfile = true)]
        public EstudiantesDashboardModel Get()
        {
            int idUsuarioOnline = OnlineUser.GetUserId();

            EstudiantesDashboardModel estudiantesDashboard = new EstudiantesDashboardModel();
            estudiantesDashboard.InfoEstudiante = estudiantesRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            estudiantesDashboard.InfoUsuario = usuariosRepo.Get(x => x.idUsuario == idUsuarioOnline).FirstOrDefault();
            estudiantesDashboard.AsignaturasSeleccionadas = 

        }
    }
}