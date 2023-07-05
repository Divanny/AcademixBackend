using Data.Administration;
using Models.Administration;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Infraestructure;
using System.Web.Http;
using Data.Entities;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar el ranking de estudiante
    /// </summary>
    [RoutePrefix("api/Ranking")]
    public class RankingController : ApiBaseController
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
        /// API Dashboard
        /// </summary>
        public RankingController()
        {
            academixEntities = new AcadmixEntities();
            usuariosRepo = new UsuariosRepo(academixEntities);
        }

        /// <summary>
        /// Get ranking de estudiantes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<UsuariosModel> Get()
        {
            List<UsuariosModel> usuarios = usuariosRepo.Get().Where(x => x.InfoEstudiante != null).OrderByDescending(x => x.InfoEstudiante.IndiceGeneral).Take(100).ToList();
            return usuarios;
        }
    }
}

