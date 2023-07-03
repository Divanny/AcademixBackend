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
using Microsoft.AspNetCore.Mvc.Internal;
using Data.Common;

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
        /// Valida que el usuario colocado existe y retorna el correo electrónico parcialmente oculto
        /// </summary>
        /// <param name="NombreUsuario"></param>
        /// <returns></returns>
        [Route("ValidateUser")]
        [HttpGet]
        public OperationResult ValidateUser(string NombreUsuario)
        {
            using (UsuariosRepo ur = new UsuariosRepo())
            {
                if (NombreUsuario != null)
                {
                    var usuario = ur.GetFirst(u => u.NombreUsuario == NombreUsuario);

                    if (usuario != null)
                    {
                        return new OperationResult(true, "Se ha encontrado el usuario", new { MaskedMail = maskMail(usuario.CorreoElectronico) });
                    }
                }
                return new OperationResult(false, "Este usuario no existes");
            };
        }

        /// <summary>
        /// Cambia la contraseña del usuario automáticamente
        /// </summary>
        /// <param name="NombreUsuario"></param>
        /// <param name="CorreoElectronico"></param>
        /// <returns></returns>
        [Route("ChangePassword")]
        [HttpPut]
        public OperationResult ChangePassword(string NombreUsuario, string CorreoElectronico)
        {
            Utilities utilities = new Utilities();
            using (UsuariosRepo ur = new UsuariosRepo())
            {
                if (NombreUsuario != null)
                {
                    var usuario = ur.GetFirst(u => u.NombreUsuario == NombreUsuario);


                    if (CorreoElectronico.Trim() == usuario.CorreoElectronico.Trim())
                    {
                        var newPassword = utilities.GenerateRandomPassword(8);
                        usuario.Password = newPassword;
                        
                        Mailing mailing = new Mailing();
                        string body = $"Estimado <b>{usuario.Nombres + " " + usuario.Apellidos}</b>,<br/><br/>Hemos recibido tu solicitud para restablecer tu contraseña. Entendemos lo importante que es acceder a tu cuenta y estamos aquí para ayudarte.<br/><br/>Hemos generado una nueva contraseña para tu cuenta. A continuación, te proporcionamos tus nuevos datos de inicio de sesión:<br/><br/><b>Nombre de usuario:</b> {usuario.NombreUsuario}<br/><b>Contraseña:</b> {newPassword}<br/><br/>Te recomendamos cambiar tu contraseña tan pronto como accedas a tu cuenta para garantizar la seguridad de tus datos personales.<br/><br/>Si no has solicitado esta recuperación de contraseña, te sugerimos tomar medidas inmediatas para proteger tu cuenta. Ponte en contacto con nuestro equipo de soporte a través de la solicitud de soporte por medio de la web para que podamos asistirte en este proceso.<br/>";
                        mailing.SendEmail(usuario.CorreoElectronico, "Recuperar contraseña", body, "Recuperar contraseña");
                        
                        usuario.PasswordEncrypted = Cipher.Encrypt(newPassword, Properties.Settings.Default.JwtSecret);
                        ur.Edit(usuario, usuario.idUsuario);

                        return new OperationResult(true, "Se ha enviado un correo electrónico con su nueva contraseña");
                    }
                }
                return new OperationResult(false, "El correo electrónico no coincide");
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

        /// <summary>
        /// Retorna el correo electrónico parcialmente oculto. Ej: d*****@gmail.com
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private string maskMail(string email)
        {
            string result = "";
            bool isAfterArroba = false;
            if (email != null)
            {
                result += email[0];

                for (int i = 1; i < email.Length; i++)
                {
                    if (email[i] != '@' && !isAfterArroba)
                    {
                        result += '*';
                    }
                    else
                    {
                        if (!isAfterArroba) isAfterArroba = true;

                        result += email[i];
                    }
                }
            }
            return result;
        }
    }
}