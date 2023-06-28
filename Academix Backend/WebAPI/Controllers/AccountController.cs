using Data.Administration;
using Models.Administration;
using Models.Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebAPI.Infraestructure;
using Data.Entities;
using System.Web.Http;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar la cuenta del usuario.
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : ApiBaseController
    {
        /// <summary>
        /// Obtiene la información del usuario en línea.
        /// </summary>
        /// <returns></returns>
        [Route("GetUserData")]
        [Autorizar(AllowAnyProfile = true)]
        public object Get()
        {
            List<VistasModel> vistas = new List<VistasModel>();
            UsuariosModel usuario = new UsuariosModel();

            var status = SessionData.Get();

            if (status.Estado == EstadoSesion.NoIniciada)
            {
                return new
                {
                    usuario = usuario,
                    vistas = vistas
                };
            }

            using (UsuariosRepo ur = new UsuariosRepo())
            {
                usuario = ur.Get(Convert.ToInt32(status.idUsuario));
            }

            using (PerfilesRepo pr = new PerfilesRepo())
            {
                vistas = pr.GetPermisos(Convert.ToInt32(status.idPerfil)).Where(v => v.Permiso).ToList();
            }

            return new
            {
                usuario = usuario,
                vistas = vistas
            };
        }

        /// <summary>
        /// Iniciar sesión en el sistema.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        [Route("LogIn")]
        [HttpPost]
        public OperationResult Post([FromBody] Credentials credentials)
        {
            try
            {
                if (ValidateModel(credentials))
                {
                    Authentication auth = new Authentication();
                    LogInResult result = auth.LogIn(credentials);

                    if (result.IsSuccessful)
                    {
                        using (var dbc = new AcadmixEntities())
                        {
                            var logger = new Data.Common.Logger(dbc);
                            logger.LogHttpRequest(result.idUsuario, null);
                        }
                    }

                    return new OperationResult(result.IsSuccessful, result.Message, result.UserValidated, result.Token);
                }
                else
                    return new OperationResult(false, "Los datos suministrados no son válidos", Validation.Errors);
            }
            catch (Exception ex) {
                using (var dbc = new AcadmixEntities())
                {
                    var logger = new Data.Common.Logger(dbc);
                    logger.LogError(ex);
                    return new OperationResult(false, "Error al iniciar sesión");
                }
            }
        }

        /// <summary>
        /// Obtiene "Mi perfil" del usuario.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [Route("MyProfile")]
        [Autorizar(AllowAnyProfile = true)]
        [HttpGet]
        public object MyProfile(string userName)
        {
            using (UsuariosRepo ur = new UsuariosRepo())
            {
                var usuario = ur.GetFirst(u => u.NombreUsuario == userName);
                return usuario;
            };
        }

        /// <summary>
        /// Cierra sesión en el sistema.
        /// </summary>
        /// <returns></returns>
        [Route("LogOff")]
        public bool Put()
        {
            SessionData.Clear();
            return true;
        }
    }
}