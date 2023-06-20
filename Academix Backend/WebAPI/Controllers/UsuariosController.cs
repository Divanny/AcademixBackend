using Data.Administration;
using Data.Common;
using Models.Administration;
using Models.Common;
using Models.Enums;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebAPI.Infraestructure;
using System.Web.Http;

namespace WebAPI.Controllers
{
    /// <summary>
    /// API para manejar los usuarios del sistema.
    /// </summary>
    [RoutePrefix("api/Usuarios")]
    public class UsuariosController : ApiBaseController
    {
        UsuariosRepo usuariosRepo = new UsuariosRepo();
        PerfilesRepo perfilesRepo = new PerfilesRepo();

        /// <summary>
        /// Obtiene un listado de los usuarios registrados.
        /// </summary>
        /// <returns></returns>
        // GET api/Usuarios
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public List<UsuariosModel> Get()
        {
            return usuariosRepo.Get().ToList();
        }

        /// <summary>
        /// Obtiene un usuario en específico.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Usuarios/5
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public UsuariosModel Get(int id)
        {
            return usuariosRepo.Get(x => x.idUsuario == id).FirstOrDefault();
        }

        /// <summary>
        /// Agrega un nuevo usuario (se necesita permiso de administrador).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Usuarios
        [HttpPost]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Post([FromBody] UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                UsuariosModel usuario = usuariosRepo.GetByUsername(model.NombreUsuario);
                if (usuario != null)
                {
                    return new OperationResult(false, "Este usuario ya está registrado");
                }

                if (model.Password == null || model.Password == "")
                {
                    return new OperationResult(false, "Se debe colocar una contraseña válida", Validation.Errors);
                }


                model.PasswordEncrypted = Cipher.Encrypt(model.Password, Properties.Settings.Default.JwtSecret);

                if (model.idEstado == 0)
                {
                    model.idEstado = (int)EstadoUsuarioEnum.Activo;
                }

                model.FechaRegistro = DateTime.Now;
                model.UltimoIngreso = DateTime.Now;

                if (model.idPerfil == 0)
                {
                    var DefaultProfile = perfilesRepo.GetDefaultProfile();
                    model.idPerfil = DefaultProfile.idPerfil;
                }

                var created = usuariosRepo.Add(model);
                usuariosRepo.SaveChanges();
                return new OperationResult(true, "Se creado este usuario satisfactoriamente", created);
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        /// <summary>
        /// Actualiza la información de un usuario.
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Usuarios/5
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult Put(int idUsuario, [FromBody] UsuariosModel model)
        {
            if (ValidateModel(model))
            {
                UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == idUsuario).FirstOrDefault();

                if (usuario == null)
                {
                    return new OperationResult(false, "Este usuario no existe.");
                }

                var usuarioExists = usuariosRepo.Get(x => x.NombreUsuario == model.NombreUsuario).FirstOrDefault();

                if (usuarioExists != null)
                {
                    if (usuarioExists.idUsuario != idUsuario)
                    {
                        return new OperationResult(false, "Este usuario ya está registrado");
                    }
                }

                model.UltimoIngreso = DateTime.Now;

                usuariosRepo.Edit(model, idUsuario);
                return new OperationResult(true, "Se ha actualizado satisfactoriamente");
            }
            else
            {
                return new OperationResult(false, "Los datos ingresados no son válidos", Validation.Errors);
            }
        }

        [Route("CambiarContraseña")]
        [HttpPut]
        //[Autorizar(AllowAnyProfile = true)]
        public OperationResult CambiarContraseña(int idUsuario, string password)
        {
            UsuariosModel usuario = usuariosRepo.Get(x => x.idUsuario == idUsuario).FirstOrDefault();

            if (usuario == null)
            {
                return new OperationResult(false, "Este usuario no existe.");
            }

            if (password != null && password != "")
            {
                Utilities utilities = new Utilities();
                var PasswordValidation = utilities.ValidarContraseña(password);

                if (!PasswordValidation.Success)
                {
                    return PasswordValidation;
                }

                usuario.PasswordEncrypted = Cipher.Encrypt(password, Properties.Settings.Default.JwtSecret);
                usuariosRepo.Edit(usuario, idUsuario);
                return new OperationResult(true, "La contraseña se ha actualizado satisfactoriamente");
            }

            return new OperationResult(false, "Los datos ingresados no son válidos");
        }

        /// <summary>
        /// Obtiene los diferentes estados que puede estar un usuario.
        /// </summary>
        /// <returns></returns>
        // GET api/Usuarios/GetEstadosUsuarios
        [Route("GetEstadosUsuarios")]
        [HttpGet]
        //[Autorizar(AllowAnyProfile = true)]
        public object GetEstadosUsuarios()
        {
            return usuariosRepo.GetEstadosUsuarios();
        }
    }
}